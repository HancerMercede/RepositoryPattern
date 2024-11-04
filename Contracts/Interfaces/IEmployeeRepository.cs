using Entities.Models;
using Shared.RequestFeatures;
using Shared.Shared;

namespace Contracts.Interfaces;

public interface IEmployeeRepository
{
    Task<PageList<Employee>> GetAll(string companyId, PaginationParameters pagination, bool trackChanges);
    Task<Employee> GetByCondition(string companyId, string employeeId, bool trackChanges);
    Task<Employee> CreateEmployeeForCompany(string companyId, Employee employee);
    Task DeleteEmployee(string companyId,string id, bool trackChanges);
}
