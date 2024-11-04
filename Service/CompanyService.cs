using Entities.Exceptions;

namespace Service;

internal sealed class CompanyService(IRepositoryManager repositoryManager) : ICompanyService
{
    public async Task<(IEnumerable<Company> companies, MetaData metaData)> GetAll(PaginationParameters pagination, bool trackChanges)
    {
        var companies = await repositoryManager.Company.GetAll(pagination, trackChanges);
        var metaData = companies.MetaData;

        return (companies, metaData);
    }

    public async Task<Company> GetByCondition(string id, bool trackChanges)
    {
        if (id is null)
            throw new IdParametersBadRequestException();
        
        var company = await repositoryManager.Company.GetByCondition(id, trackChanges);
       
        if (company is null)
            throw new CompanyNotFoundException(Guid.Parse(id));

        return company;
    }
    
    public async Task<Company> CreateCompany(Company company)
    {
        if (company is null)
            throw new CompanyBadRequestException();
        
        return await repositoryManager.Company.CreateRecord(company);
    }

    public async Task<IEnumerable<Company>> GetByIds(IEnumerable<Guid> ids, bool trackChanges)
    {
        var companiesEntities =  await repositoryManager.Company.GetByIds(ids, trackChanges);
        if (ids.Count() != companiesEntities.Count())
        {
            throw new CollectionByIdsBadRequestException();
        }
        return companiesEntities;
    }

    

    public async Task DeleteCompany(string id, bool trackChanges) => await repositoryManager.Company.DeleteRecord(id, trackChanges);
    
    public async Task SaveChanges()
    {
        await repositoryManager.Save();
    }
}