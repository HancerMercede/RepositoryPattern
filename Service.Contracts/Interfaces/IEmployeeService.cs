using Dtos.DtoModels;
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
    Task<(EmployeeUpdateDto employeeToPath, Employee employee)> GetEmployeeForPatch(string companyId, string id, bool compTrackChanges, bool empTrackChanges);
    Task SaveChangesForPatch(EmployeeUpdateDto employee, Employee employeeEntity);
    
    // Either Methods
    EitherAsync<string,  (IEnumerable<Employee> employees, MetaData metaData)> GetAllEmployees(string companyId, PaginationParameters pagination, bool trackChanges);
    EitherAsync<string, Employee> GetByConditionWithEither(string companyId, string id, bool trackChanges);
    
    EitherAsync<string , Employee> CreateEmployeeWithEither(string companyId, Employee employee);
    
    EitherAsync<string, Unit> DeleteEmployeeWithEither(string companyId, string id, bool trackChanges);
}