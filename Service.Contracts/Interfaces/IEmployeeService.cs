using Entities.Models;
using Shared.RequestFeatures;
using Shared.Shared;

namespace Service.Contracts.Interfaces;

public interface IEmployeeService
{
    Task<(IEnumerable<Employee> Employees, MetaData metaData)> GetAll(string CompanyId, PaginationParameters pagination, bool trackChanges);
    Task<Employee> GetByCondiction(string companyId, string Id, bool trackChanges);
    Task<Employee> CreateEmployee(string companyId, Employee employee);
    Task DeleteEmployee(string companyId, string Id, bool trackChanges);

    Task SaveChanges();
}