using Contracts.Interfaces;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
namespace Repository;

public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(RepositoryContext repositoryContext) 
        : base(repositoryContext)
    {

    }

    public async Task<IEnumerable<Employee>> GetAll(string CompanyId,bool trackChanges)
    
    {
        var employees = await FindByCondiction(c => c.CompanyId == Guid.Parse(CompanyId), trackChanges)
            .OrderBy(e => e.Name)
            .ToListAsync();
           

        return employees;
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
        var dbEntity = await FindByCondiction(c => c.CompanyId ==Guid.Parse(CompanyId) && c.Id == Guid.Parse(Id), trackChanges)
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
