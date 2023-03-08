using Contracts.Interfaces;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Repository;

public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
{
    public CompanyRepository(RepositoryContext repositoryContext) 
        : base(repositoryContext)
    {

    }

    public async Task<IEnumerable<Company>> GetAll(bool trackChanges)
    {
        var companies = await FindAll(trackChanges)
           .OrderBy(c => c.Name)
          // .Include(e=>e.Employees)
           .ToListAsync();
        
        return companies;
    }

    public async Task<Company> GetByCondiction(string Id, bool trackChanges)
    {
        var company = await FindByCondiction(c => c.Id == Guid.Parse(Id),
            trackChanges)
            .Include(e => e.Employees)
            .SingleOrDefaultAsync();

        return company!;
    }

    public async Task<Company> CreateCompany(Company company)
    {
        await Create(company);
        return company;
    }

    public async Task<IEnumerable<Company>> GetByIds(IEnumerable<Guid> Ids, bool trackChanges)
    {
        var companies = await FindByCondiction(x => Ids.Contains(x.Id), trackChanges)
            .OrderBy(x=>x.Name)
            .ToListAsync();

        return companies;
    }

    public async Task DeleteCompany(string Id, bool trackChanges)
    {
        var dbcompany = await FindByCondiction(c => c.Id == Guid.Parse(Id), trackChanges)
            .SingleOrDefaultAsync();

        await Delete(dbcompany!);
    }
}
