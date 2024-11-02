using Entities.Models;
using Shared.RequestFeatures;
using Shared.Shared;

namespace Service.Contracts.Interfaces;

public interface IEmployeeService
{
    Task<(IEnumerable<Employee> Employees, MetaData metaData)> GetAll(string companyId, PaginationParameters pagination, bool trackChanges);
    Task<Employee> GetByCondition(string companyId, string id, bool trackChanges);
    Task<Employee> CreateEmployee(string companyId, Employee employee);
    Task DeleteEmployee(string companyId, string id, bool trackChanges);
    Task SaveChanges();
}