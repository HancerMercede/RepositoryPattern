using Entities.Models;

namespace Contracts.Interfaces;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAll(string CompanyId,bool trackChanges);
    Task<Employee> GetByCondiction(string CompanyId, string EmployeeId, bool trackChanges);
    Task<Employee> CreateEmployeeForCompany(string CompanyId, Employee employee);
    Task DeleteEmployee(string CompanyId,string Id, bool trackChanges);

   // Task UpdateEmployee(string CompanyId, string Id, bool trackChanges);

}
