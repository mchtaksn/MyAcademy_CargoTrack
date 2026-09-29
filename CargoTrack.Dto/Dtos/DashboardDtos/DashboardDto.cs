using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Dto.Dtos.DashboardDtos
{
    public class DashboardDto
    {
        public int TotalCargoCount { get; set; }
        public int ActiveCargoCount { get; set; }
        public int DelayedCargoCount { get; set; }
        public int DeliveredCargoCount { get; set; }
        public double DeliveryRatePercent { get; set; }
        public int TotalBranchCount { get; set; }
        public int DistinctCityCount { get; set; }
        public int TotalEmployeeCount { get; set; }
        public int ActiveEmployeeCount { get; set; }

        public List<DailyTrafficDto> Last7DaysTraffic { get; set; } = new();
        public List<StatusDistributionDto> StatusDistribution { get; set; } = new();
        public List<TopBranchDto> TopBranches { get; set; } = new();

        // Öne çıkanlar — hero bandındaki üç kısa öngörü için
        public DateTime? BusiestDay { get; set; }
        public int BusiestDayCount { get; set; }
        public string? BusiestBranchName { get; set; }
        public double DailyAverage { get; set; }
    }

    public class DailyTrafficDto
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }

    public class StatusDistributionDto
    {
        public CargoStatus Status { get; set; }
        public int Count { get; set; }
    }

    public class TopBranchDto
    {
        public string BranchName { get; set; }
        public int CargoCount { get; set; }
    }
}