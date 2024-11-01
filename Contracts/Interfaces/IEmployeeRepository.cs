using Entities.Models;
using Shared.RequestFeatures;
using Shared.Shared;

namespace Contracts.Interfaces;

public interface IEmployeeRepository
{
    Task<PageList<Employee>> GetAll(string CompanyId, PaginationParameters pagination, bool trackChanges);
    Task<Employee> GetByCondiction(string CompanyId, string EmployeeId, bool trackChanges);
    Task<Employee> CreateEmployeeForCompany(string CompanyId, Employee employee);
    Task DeleteEmployee(string CompanyId,string Id, bool trackChanges);

   // Task UpdateEmployee(string CompanyId, string Id, bool trackChanges);

}
