using Contracts.Interfaces;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Shared.RequestFeatures;
using Shared.Shared;

namespace Repository;

public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(RepositoryContext repositoryContext) 
        : base(repositoryContext)
    {

    }

    public async Task<PageList<Employee>> GetAll(string CompanyId, PaginationParameters paginationParameters, bool trackChanges)
    
    {
        var employees = await FindByCondiction(c => c.CompanyId == Guid.Parse(CompanyId), trackChanges)
            .OrderBy(e => e.Name)
            .ToListAsync();


        var pagination = PageList<Employee>.ToPageList(employees, paginationParameters.PageNumber, paginationParameters.PageSize);
       
        return pagination;
    }
    
    public async Task<Employee> GetByCondiction(string CompanyId, string EmployeeId, bool trackChanges)
    {
        var employee = await FindByCondiction(c => c.CompanyId == Guid.Parse(CompanyId) 
                                               && c.Id == Guid.Parse(EmployeeId), trackChanges)
                                              .SingleOrDefaultAsync();
        return employee!;
    }

    public async Task<Employee> CreateEmployeeForCompany(string CompanyId, Employee employee)
    {
        employee.CompanyId = Guid.Parse(CompanyId);
        await Create(employee);

        return employee;
    }

    public async Task DeleteEmployee(string CompanyId, string Id, bool trackChanges)
    {
        var dbEntity = await FindByCondiction(c => c.CompanyId == Guid.Parse(CompanyId) && c.Id == Guid.Parse(Id), trackChanges)
            .SingleOrDefaultAsync();

        await Delete(dbEntity!);
    }
    // I'm not using this Method Now
    /* public async Task UpdateEmployee(string CompanyId, string Id, bool trackChanges)
     {

         var dbEntity = await FindByCondiction(c => c.CompanyId == Guid.Parse(CompanyId) && c.Id == Guid.Parse(Id), trackChanges)
             .SingleOrDefaultAsync();

         await Update(dbEntity!);
     }*/
}
