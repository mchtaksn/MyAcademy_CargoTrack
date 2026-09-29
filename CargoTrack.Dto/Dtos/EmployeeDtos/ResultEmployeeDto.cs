namespace CargoTrack.Dto.Dtos.EmployeeDtos
{
    public class ResultEmployeeDto
    {
        public Guid Id { get; set; }
        public string Position { get; set; }
        public Guid BranchId { get; set; }
        public string BranchName { get; set; }
        public string CityName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
    }
}