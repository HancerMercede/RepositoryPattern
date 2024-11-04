using Contracts.Interfaces;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Shared.RequestFeatures;
using Shared.Shared;

namespace Repository;

public class EmployeeRepository(RepositoryContext repositoryContext)
    : RepositoryBase<Employee>(repositoryContext), IEmployeeRepository
{
    public async Task<PageList<Employee>> GetAll(string companyId, PaginationParameters paginationParameters, bool trackChanges)
    
    {
        var employees = await FindByCondition(c => c.CompanyId == Guid.Parse(companyId), trackChanges)
            .OrderBy(e => e.Name)
            .ToListAsync();


        var pagination = PageList<Employee>.ToPageList(employees, paginationParameters.PageNumber, paginationParameters.PageSize);
       
        return pagination;
    }
    
    public async Task<Employee> GetByCondition(string companyId, string employeeId, bool trackChanges)
    {
        var employee = await FindByCondition(c => c.CompanyId == Guid.Parse(companyId) 
                                               && c.Id == Guid.Parse(employeeId), trackChanges)
                                              .SingleOrDefaultAsync();
        return employee!;
    }

    public async Task<Employee> CreateEmployeeForCompany(string companyId, Employee employee)
    {
        employee.CompanyId = Guid.Parse(companyId);
        await Create(employee);

        return employee;
    }

    public async Task DeleteEmployee(string companyId, string id, bool trackChanges)
    {
        var dbEntity = await FindByCondition(c => c.CompanyId == Guid.Parse(companyId) && c.Id == Guid.Parse(id), trackChanges)
            .SingleOrDefaultAsync();

        await Delete(dbEntity!);
    }
}
