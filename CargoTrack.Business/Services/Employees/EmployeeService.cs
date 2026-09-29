using CargoTrack.DataAccess.Repositories.Employees;
using CargoTrack.Dto.Dtos.EmployeeDtos;
using CargoTrack.Entity.Entities;
using Mapster;
using System.ComponentModel.DataAnnotations;

namespace CargoTrack.Business.Services.Employees
{
    public class EmployeeService(IEmployeeRepository _employeeRepository) : IEmployeeService
    {
        public async Task<ResultEmployeeDto> GetByAppUserIdAsync(Guid appUserId)
        {
            var employee = await _employeeRepository.GetByAppUserIdAsync(appUserId);
            if (employee is null)
            {
                throw new ValidationException("Bu kullanıcıya ait personel kaydı bulunamadı.");
            }
            return employee.Adapt<ResultEmployeeDto>();
        }

        public async Task<List<ResultEmployeeDto>> GetAllAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();
            return employees.Adapt<List<ResultEmployeeDto>>();
        }

        public async Task<UpdateEmployeeDto> GetByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee is null)
            {
                throw new ValidationException("Personel bulunamadı.");
            }
            return employee.Adapt<UpdateEmployeeDto>();
        }

        public async Task CreateAsync(CreateEmployeeDto createEmployeeDto)
        {
            var employee = createEmployeeDto.Adapt<Employee>();
            await _employeeRepository.CreateAsync(employee);
        }

        public async Task UpdateAsync(UpdateEmployeeDto updateEmployeeDto)
        {
            var employee = await _employeeRepository.GetByIdAsync(updateEmployeeDto.Id);
            if (employee is null)
            {
                throw new ValidationException("Personel bulunamadı.");
            }

            employee.Position = updateEmployeeDto.Position;
            employee.BranchId = updateEmployeeDto.BranchId;
            employee.IsActive = updateEmployeeDto.IsActive;

            await _employeeRepository.UpdateAsync(employee);
        }

        public async Task DeleteAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee is null)
            {
                throw new ValidationException("Personel bulunamadı.");
            }
            await _employeeRepository.DeleteAsync(employee);
        }
    }
}