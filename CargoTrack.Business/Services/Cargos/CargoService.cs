using CargoTrack.Business.Services.CargoPricing;
using CargoTrack.DataAccess.Repositories.AuditLogs;
using CargoTrack.DataAccess.Repositories.Branches;
using CargoTrack.DataAccess.Repositories.CargoMovements;
using CargoTrack.DataAccess.Repositories.Cargos;
using CargoTrack.DataAccess.Repositories.Deliveries;
using CargoTrack.DataAccess.Repositories.DeliveryExceptions;
using CargoTrack.Dto.Dtos.CargoDtos;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using Mapster;
using System.ComponentModel.DataAnnotations;

namespace CargoTrack.Business.Services.Cargos
{
    public class CargoService(
        ICargoPricingService _cargoPricingService,
        ICargoRepository _cargoRepository,
        ICargoMovementRepository _cargoMovementRepository,
        IBranchRepository _branchRepository,
        IDeliveryRepository _deliveryRepository,
        IDeliveryExceptionRepository _deliveryExceptionRepository,
        IAuditLogRepository _auditLogRepository) : ICargoService
    {
        private static readonly Dictionary<CargoStatus, List<CargoStatus>> _allowedTransitions = new()
        {
            { CargoStatus.Received, new List<CargoStatus> { CargoStatus.InTransferCenter, CargoStatus.ArrivedAtDeliveryBranch } },
            { CargoStatus.InTransferCenter, new List<CargoStatus> { CargoStatus.DispatchedFromTransferCenter } },
            { CargoStatus.DispatchedFromTransferCenter, new List<CargoStatus> { CargoStatus.ArrivedAtDeliveryBranch, CargoStatus.InTransferCenter } },
            { CargoStatus.ArrivedAtDeliveryBranch, new List<CargoStatus> { CargoStatus.OutForDelivery } },
            { CargoStatus.OutForDelivery, new List<CargoStatus> { CargoStatus.Delivered, CargoStatus.FailedDelivery } },
            { CargoStatus.FailedDelivery, new List<CargoStatus> { CargoStatus.OutForDelivery, CargoStatus.InReturnProcess } },
            { CargoStatus.InReturnProcess, new List<CargoStatus> { CargoStatus.ReturnedToSender } },
        };

        public List<CargoStatus> GetAllowedNextStatuses(CargoStatus currentStatus)
        {
            return _allowedTransitions.TryGetValue(currentStatus, out var next) ? next : new List<CargoStatus>();
        }

        public async Task<List<ResultCargoDto>> GetAllAsync()
        {
            var cargos = await _cargoRepository.GetAllAsync();
            return cargos.Adapt<List<ResultCargoDto>>();
        }

        public async Task<UpdateCargoDto> GetByIdAsync(Guid id)
        {
            var cargo = await _cargoRepository.GetByIdAsync(id);
            if (cargo is null)
            {
                throw new ValidationException("Kargo bulunamadı.");
            }
            return cargo.Adapt<UpdateCargoDto>();
        }

        public async Task<ResultCargoDto> GetDetailAsync(Guid id)
        {
            var cargo = await _cargoRepository.GetByIdAsync(id);
            if (cargo is null)
            {
                throw new ValidationException("Kargo bulunamadı.");
            }

            var dto = cargo.Adapt<ResultCargoDto>();

            var movements = await _cargoMovementRepository.GetByCargoIdAsync(cargo.Id);
            dto.Movements = movements.Select(m => new CargoMovementTimelineDto
            {
                NewStatus = m.NewStatus,
                MovementDate = m.MovementDate,
                LocationName = m.Branch?.Name ?? m.TransferCenter?.Name ?? "—",
                Description = m.Description
            }).ToList();

            return dto;
        }

        public async Task<ResultCargoDto> GetByTrackCodeAsync(string trackCode)
        {
            var cargo = await _cargoRepository.GetByTrackCodeAsync(trackCode);
            if (cargo is null)
            {
                throw new ValidationException("Bu takip numarasına ait kargo bulunamadı.");
            }
            return cargo.Adapt<ResultCargoDto>();
        }

        public async Task<TrackResultDto> GetPublicTrackResultAsync(string trackCode)
        {
            var cargo = await _cargoRepository.GetByTrackCodeAsync(trackCode);
            if (cargo is null)
            {
                throw new ValidationException("Bu takip numarasına ait kargo bulunamadı.");
            }

            var movements = await _cargoMovementRepository.GetByCargoIdAsync(cargo.Id);

            return new TrackResultDto
            {
                TrackCode = cargo.TrackCode,
                CargoStatus = cargo.CargoStatus,
                OriginCityName = cargo.OriginBranch.City?.Name,
                DestinationCityName = cargo.DestinationBranch.City?.Name,
                OriginLat = cargo.OriginBranch.City?.Latitude,
                OriginLng = cargo.OriginBranch.City?.Longitude,
                DestinationLat = cargo.DestinationBranch.City?.Latitude,
                DestinationLng = cargo.DestinationBranch.City?.Longitude,
                EstimatedArrivalDate = cargo.EstimatedArrivalDate,
                Movements = movements.Select(m => new CargoMovementTimelineDto
                {
                    NewStatus = m.NewStatus,
                    MovementDate = m.MovementDate,
                    LocationName = m.Branch?.Name ?? m.TransferCenter?.Name ?? "—",
                    Description = m.Description
                }).ToList()
            };
        }

        public async Task CreateAsync(CreateCargoDto createCargoDto, Guid createdByEmployeeId, Guid createdByAppUserId)
        {
            var cargo = createCargoDto.Adapt<Cargo>();
            cargo.TrackCode = await GenerateUniqueTrackCodeAsync();
            cargo.ShipmentDate = DateTime.UtcNow;
            cargo.EstimatedArrivalDate = await CalculateEstimatedArrivalDateAsync(
                createCargoDto.CargoType,
                createCargoDto.OriginBranchId,
                createCargoDto.DestinationBranchId,
                cargo.ShipmentDate);
            cargo.CargoStatus = CargoStatus.Received;

            await _cargoRepository.CreateAsync(cargo);
            await _cargoPricingService.CalculateAndSaveAsync(cargo);

            var initialMovement = new CargoMovement
            {
                CargoId = cargo.Id,
                PreviousStatus = null,
                NewStatus = CargoStatus.Received,
                MovementDate = DateTime.UtcNow,
                Description = "Kargo oluşturuldu ve gönderici şubede teslim alındı.",
                BranchId = cargo.OriginBranchId,
                TransferCenterId = null,
                EmployeeId = createdByEmployeeId
            };

            await _cargoMovementRepository.CreateAsync(initialMovement);

            await WriteAuditLogAsync(createdByAppUserId, AuditActionType.Create, cargo.Id,
                oldValue: null, newValue: cargo.CargoStatus.ToString(),
                description: $"Kargo oluşturuldu: {cargo.TrackCode}");
        }

        public async Task UpdateAsync(UpdateCargoDto updateCargoDto, Guid updatedByAppUserId)
        {
            var cargo = await _cargoRepository.GetByIdAsync(updateCargoDto.Id);
            if (cargo is null)
            {
                throw new ValidationException("Güncellenecek kargo bulunamadı.");
            }

            var oldSnapshot = $"Ağırlık:{cargo.Weight}, Tip:{cargo.CargoType}, Ölçüler:{cargo.Width}x{cargo.Length}x{cargo.Height}";

            cargo.Weight = updateCargoDto.Weight;
            cargo.Width = updateCargoDto.Width;
            cargo.Length = updateCargoDto.Length;
            cargo.Height = updateCargoDto.Height;
            cargo.CargoType = updateCargoDto.CargoType;

            await _cargoRepository.UpdateAsync(cargo);

            var newSnapshot = $"Ağırlık:{cargo.Weight}, Tip:{cargo.CargoType}, Ölçüler:{cargo.Width}x{cargo.Length}x{cargo.Height}";

            await WriteAuditLogAsync(updatedByAppUserId, AuditActionType.Update, cargo.Id,
                oldValue: oldSnapshot, newValue: newSnapshot,
                description: $"Kargo bilgileri güncellendi: {cargo.TrackCode}");
        }

        public async Task ChangeStatusAsync(Guid cargoId, CargoStatus newStatus, Guid performedByEmployeeId, Guid performedByAppUserId, Guid? branchId, Guid? transferCenterId, string description)
        {
            var cargo = await _cargoRepository.GetByIdAsync(cargoId);
            if (cargo is null)
            {
                throw new ValidationException("Kargo bulunamadı.");
            }

            var currentStatus = cargo.CargoStatus;

            if (!_allowedTransitions.TryGetValue(currentStatus, out var allowedNextStatuses)
                || !allowedNextStatuses.Contains(newStatus))
            {
                throw new ValidationException($"'{currentStatus}' durumundan '{newStatus}' durumuna geçiş yapılamaz.");
            }

            cargo.CargoStatus = newStatus;
            await _cargoRepository.UpdateAsync(cargo);

            var movement = new CargoMovement
            {
                CargoId = cargo.Id,
                PreviousStatus = currentStatus,
                NewStatus = newStatus,
                MovementDate = DateTime.UtcNow,
                Description = description,
                BranchId = branchId,
                TransferCenterId = transferCenterId,
                EmployeeId = performedByEmployeeId
            };

            await _cargoMovementRepository.CreateAsync(movement);

            await WriteAuditLogAsync(performedByAppUserId, AuditActionType.StatusChange, cargo.Id,
                oldValue: currentStatus.ToString(), newValue: newStatus.ToString(),
                description: description);
        }

        public async Task<string> DispatchForDeliveryAsync(Guid cargoId, Guid employeeId, Guid appUserId, Guid branchId)
        {
            var code = Random.Shared.Next(100000, 999999).ToString();

            await ChangeStatusAsync(cargoId, CargoStatus.OutForDelivery, employeeId, appUserId, branchId, null,
                "Kargo dağıtıma çıktı. Teslimat kodu oluşturuldu.");

            var existingDelivery = await _deliveryRepository.GetLatestByCargoIdAsync(cargoId);

            if (existingDelivery is not null)
            {
                existingDelivery.DeliveryCode = code;
                existingDelivery.IsCodeVerified = false;
                existingDelivery.ReceivedByName = null;
                existingDelivery.DeliveredDate = null;
                await _deliveryRepository.UpdateAsync(existingDelivery);
            }
            else
            {
                var delivery = new Delivery
                {
                    CargoId = cargoId,
                    DeliveryCode = code,
                    IsCodeVerified = false,
                    EmployeeId = employeeId
                };
                await _deliveryRepository.CreateAsync(delivery);
            }

            return code;
        }

        public async Task DeliverAsync(Guid cargoId, string enteredCode, string receivedByName, Guid employeeId, Guid appUserId, Guid branchId)
        {
            var delivery = await _deliveryRepository.GetLatestByCargoIdAsync(cargoId);
            if (delivery is null || delivery.DeliveryCode != enteredCode.Trim())
            {
                throw new ValidationException("Teslimat kodu hatalı. Lütfen kontrol edip tekrar deneyin.");
            }

            delivery.IsCodeVerified = true;
            delivery.ReceivedByName = receivedByName;
            delivery.DeliveredDate = DateTime.UtcNow;
            await _deliveryRepository.UpdateAsync(delivery);

            await ChangeStatusAsync(cargoId, CargoStatus.Delivered, employeeId, appUserId, branchId, null,
                $"{receivedByName} tarafından teslim alındı.");
        }

        public async Task RecordFailedDeliveryAsync(Guid cargoId, DeliveryExceptionReason reason, Guid employeeId, Guid appUserId, Guid branchId)
        {
            var previousAttempts = await _deliveryExceptionRepository.CountByCargoIdAsync(cargoId);
            var attemptNumber = previousAttempts + 1;

            var exception = new DeliveryException
            {
                CargoId = cargoId,
                Reason = reason,
                Description = $"Teslimat denemesi {attemptNumber}. kez başarısız oldu: {reason}.",
                AttemptNumber = attemptNumber,
                ExceptionDate = DateTime.UtcNow,
                EmployeeId = employeeId
            };
            await _deliveryExceptionRepository.CreateAsync(exception);

            await ChangeStatusAsync(cargoId, CargoStatus.FailedDelivery, employeeId, appUserId, branchId, null,
                $"Teslimat denemesi başarısız ({attemptNumber}/3): {reason}.");

            if (attemptNumber >= 3)
            {
                await ChangeStatusAsync(cargoId, CargoStatus.InReturnProcess, employeeId, appUserId, branchId, null,
                    "3 başarısız denemenin ardından iade sürecine alındı.");
            }
        }

        private async Task WriteAuditLogAsync(Guid appUserId, AuditActionType actionType, Guid entityId, string? oldValue, string? newValue, string description)
        {
            var log = new AuditLog
            {
                AppUserId = appUserId,
                ActionType = actionType,
                EntityName = nameof(Cargo),
                EntityId = entityId,
                ActionDate = DateTime.UtcNow,
                OldValue = oldValue,
                NewValue = newValue,
                Description = description
            };
            await _auditLogRepository.CreateAsync(log);
        }

        private async Task<string> GenerateUniqueTrackCodeAsync()
        {
            string trackCode;
            Cargo? existing;

            do
            {
                var randomDigits = Random.Shared.Next(100000, 999999);
                trackCode = $"MYC-{DateTime.UtcNow.Year}-{randomDigits}";
                existing = await _cargoRepository.GetByTrackCodeAsync(trackCode);
            }
            while (existing is not null);

            return trackCode;
        }

        private async Task<DateTime> CalculateEstimatedArrivalDateAsync(
            CargoType cargoType, Guid originBranchId, Guid destinationBranchId, DateTime shipmentDate)
        {
            if (cargoType == CargoType.Urgent)
            {
                return shipmentDate.AddDays(1);
            }

            var originBranch = await _branchRepository.GetByIdAsync(originBranchId);
            var destinationBranch = await _branchRepository.GetByIdAsync(destinationBranchId);

            if (originBranch is null || destinationBranch is null)
            {
                throw new ValidationException("Geçersiz şube bilgisi.");
            }

            var baseDays = originBranch.CityId == destinationBranch.CityId ? 1 : 3;
            return shipmentDate.AddDays(baseDays);
        }

        public async Task<List<ResultCargoDto>> GetByBranchAsync(Guid branchId)
        {
            var cargos = await _cargoRepository.GetByBranchAsync(branchId);
            return cargos.Adapt<List<ResultCargoDto>>();
        }

        public async Task<List<ResultCargoDto>> GetByCustomerAsync(Guid appUserId)
        {
            var cargos = await _cargoRepository.GetByCustomerAsync(appUserId);
            return cargos.Adapt<List<ResultCargoDto>>();
        }

        public async Task<PagedCargoResultDto> GetPagedAsync(int page, int pageSize, string? statusGroup, string? search)
        {
            var (items, totalCount) = await _cargoRepository.GetPagedAsync(page, pageSize, statusGroup, search);
            var counts = await _cargoRepository.GetStatusCountsAsync(search);

            return new PagedCargoResultDto
            {
                Items = items.Adapt<List<ResultCargoDto>>(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                AllCount = counts.All,
                TransitCount = counts.Transit,
                DeliveredCount = counts.Delivered,
                ProblemCount = counts.Problem
            };
        }
    }
}