using CargoTrack.Business.Services.Branches;
using CargoTrack.Business.Services.Cargos;
using CargoTrack.Business.Services.Employees;
using CargoTrack.Dto.Dtos.DashboardDtos;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Business.Services.Dashboard
{
    public class DashboardService(
        ICargoService _cargoService,
        IBranchService _branchService,
        IEmployeeService _employeeService) : IDashboardService
    {
        private static readonly CargoStatus[] FinalStatuses = { CargoStatus.Delivered, CargoStatus.ReturnedToSender };

        public async Task<DashboardDto> GetDashboardAsync()
        {
            var cargos = await _cargoService.GetAllAsync();
            var branches = await _branchService.GetAllAsync();
            var employees = await _employeeService.GetAllAsync();

            var total = cargos.Count;
            var delivered = cargos.Count(c => c.CargoStatus == CargoStatus.Delivered);
            var active = cargos.Count(c => !FinalStatuses.Contains(c.CargoStatus));
            var delayed = cargos.Count(c => c.IsDelayed);

            var today = DateTime.UtcNow.Date;
            var last7Days = Enumerable.Range(0, 7)
                .Select(offset => today.AddDays(-6 + offset))
                .Select(date => new DailyTrafficDto
                {
                    Date = date,
                    Count = cargos.Count(c => c.ShipmentDate.Date == date)
                })
                .ToList();

            var statusDistribution = Enum.GetValues<CargoStatus>()
                .Select(status => new StatusDistributionDto
                {
                    Status = status,
                    Count = cargos.Count(c => c.CargoStatus == status)
                })
                .ToList();

            var topBranches = cargos
                .SelectMany(c => new[] { c.OriginBranchName, c.DestinationBranchName })
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .GroupBy(name => name)
                .Select(g => new TopBranchDto { BranchName = g.Key, CargoCount = g.Count() })
                .OrderByDescending(b => b.CargoCount)
                .Take(5)
                .ToList();

            var busiestDayEntry = last7Days.OrderByDescending(d => d.Count).First();

            return new DashboardDto
            {
                TotalCargoCount = total,
                ActiveCargoCount = active,
                DelayedCargoCount = delayed,
                DeliveredCargoCount = delivered,
                DeliveryRatePercent = total == 0 ? 0 : Math.Round(delivered * 100.0 / total, 1),
                TotalBranchCount = branches.Count,
                DistinctCityCount = branches.Select(b => b.City?.Name).Where(n => n is not null).Distinct().Count(),
                TotalEmployeeCount = employees.Count,
                ActiveEmployeeCount = employees.Count(e => e.IsActive),
                Last7DaysTraffic = last7Days,
                StatusDistribution = statusDistribution,
                TopBranches = topBranches,
                BusiestDay = busiestDayEntry.Count > 0 ? busiestDayEntry.Date : null,
                BusiestDayCount = busiestDayEntry.Count,
                BusiestBranchName = topBranches.FirstOrDefault()?.BranchName,
                DailyAverage = Math.Round(last7Days.Average(d => d.Count), 1)
            };
        }
    }
}