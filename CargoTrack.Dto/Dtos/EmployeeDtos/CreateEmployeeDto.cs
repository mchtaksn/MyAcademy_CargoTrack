namespace CargoTrack.Dto.Dtos.EmployeeDtos
{
    public class CreateEmployeeDto
    {
        public Guid AppUserId { get; set; }
        public Guid BranchId { get; set; }
        public string Position { get; set; }
        public bool IsActive { get; set; } = true;
    }
}