namespace CargoTrack.Dto.Dtos.EmployeeDtos
{
    public class UpdateEmployeeDto
    {
        public Guid Id { get; set; }
        public string Position { get; set; }
        public Guid BranchId { get; set; }
        public bool IsActive { get; set; }
    }
}