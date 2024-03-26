using Entities.Models;

namespace Service.Contracts.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<Employee>> GetAll(string CompanyId, bool trackChanges);
    Task<Employee> GetByCondiction(string companyId, string Id, bool trackChanges);
    Task<Employee> CreateEmployee(string companyId, Employee employee);
    Task DeleteEmployee(string companyId, string Id, bool trackChanges);

    Task SaveChanges();
}