using CargoTrack.Dto.Dtos.EmployeeDtos;

namespace CargoTrack.Business.Services.Employees
{
    public interface IEmployeeService
    {
        Task<List<ResultEmployeeDto>> GetAllAsync();
        Task<UpdateEmployeeDto> GetByIdAsync(Guid id);
        Task<ResultEmployeeDto> GetByAppUserIdAsync(Guid appUserId);
        Task CreateAsync(CreateEmployeeDto createEmployeeDto);
        Task UpdateAsync(UpdateEmployeeDto updateEmployeeDto);
        Task DeleteAsync(Guid id);
    }
}