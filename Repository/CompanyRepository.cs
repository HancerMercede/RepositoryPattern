using Contracts.Interfaces;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Shared.RequestFeatures;
using Shared.Shared;

namespace Repository;

public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
{
    public 
        CompanyRepository(RepositoryContext repositoryContext) 
        : base(repositoryContext)
    {

    }

    public async Task<PageList<Company>> GetAll(PaginationParameters paginationParameters, bool trackChanges)
    {
        var companies = await FindAll(trackChanges)
           .OrderBy(c => c.Name)
           .ToListAsync();

        var pagination = PageList<Company>.ToPageList(companies, paginationParameters.PageNumber, paginationParameters.PageSize);


        return pagination;
    }

    public async Task<Company> GetByCondition(string id, bool trackChanges)
    {
        var company = await FindByCondition(c => c.Id == Guid.Parse(id),
            trackChanges)
            .Include(e => e.Employees)
            .SingleOrDefaultAsync();

        return company!;
    }

    public async Task<Company> CreateRecord(Company company)
    {
        await Create(company);
        return company;
    }

    public async Task<IEnumerable<Company>> GetByIds(IEnumerable<Guid> ids, bool trackChanges)
    {
        var companies = await FindByCondition(x => ids.Contains(x.Id), trackChanges)
            .OrderBy(x=>x.Name)
            .ToListAsync();

        return companies;
    }

    public async Task DeleteCompany(string id, bool trackChanges)
    {
        var company = await FindByCondition(c => c.Id == Guid.Parse(id), trackChanges)
            .FirstOrDefaultAsync();

        await Delete(company!);
    }
}
