using CargoTrack.Dto.Dtos.CargoDtos;
using CargoTrack.Dto.Dtos.EmployeeDtos;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using Mapster;

namespace CargoTrack.Business.Mappings
{
    public static class MapsterConfig
    {
        public static void Configure()
        {
            TypeAdapterConfig<Cargo, ResultCargoDto>.NewConfig()
                .Map(d => d.SenderFullName, s => s.Sender != null ? s.Sender.FirstName + " " + s.Sender.LastName : "")
                .Map(d => d.ReceiverFullName, s => s.Receiver != null ? s.Receiver.FirstName + " " + s.Receiver.LastName : "")
                .Map(d => d.OriginBranchName, s => s.OriginBranch != null ? s.OriginBranch.Name : "")
                .Map(d => d.DestinationBranchName, s => s.DestinationBranch != null ? s.DestinationBranch.Name : "")
                .Map(d => d.OriginCityName, s => s.OriginBranch != null && s.OriginBranch.City != null ? s.OriginBranch.City.Name : "")
                .Map(d => d.DestinationCityName, s => s.DestinationBranch != null && s.DestinationBranch.City != null ? s.DestinationBranch.City.Name : "")
                .Map(d => d.Desi, s => Math.Round(s.Width * s.Length * s.Height / 3000, 1))
                .Map(d => d.IsDelayed, s => s.CargoStatus != CargoStatus.Delivered
                                         && s.CargoStatus != CargoStatus.ReturnedToSender
                                         && s.EstimatedArrivalDate < DateTime.UtcNow)
                .Map(d => d.OriginLat, s => s.OriginBranch != null && s.OriginBranch.City != null ? s.OriginBranch.City.Latitude : null)
                .Map(d => d.OriginLng, s => s.OriginBranch != null && s.OriginBranch.City != null ? s.OriginBranch.City.Longitude : null)
                .Map(d => d.DestinationLat, s => s.DestinationBranch != null && s.DestinationBranch.City != null ? s.DestinationBranch.City.Latitude : null)
                .Map(d => d.DestinationLng, s => s.DestinationBranch != null && s.DestinationBranch.City != null ? s.DestinationBranch.City.Longitude : null)
                .Ignore(d => d.Movements);

            TypeAdapterConfig<Employee, ResultEmployeeDto>.NewConfig()
                .Map(d => d.FullName, s => s.AppUser != null ? s.AppUser.FirstName + " " + s.AppUser.LastName : "")
                .Map(d => d.Email, s => s.AppUser != null ? s.AppUser.Email : "")
                .Map(d => d.BranchName, s => s.Branch != null ? s.Branch.Name : "")
                .Map(d => d.CityName, s => s.Branch != null && s.Branch.City != null ? s.Branch.City.Name : "");
        }
    }
}