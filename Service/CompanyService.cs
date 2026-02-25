using Entities.Exceptions;

namespace Service;

internal sealed class CompanyService(IRepositoryManager repositoryManager) : ICompanyService
{
    #region Conventional Implementation without Either  
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
    #endregion
    public EitherAsync<string, (IEnumerable<Company> companies, MetaData metaData)> GetAllWithEither(PaginationParameters pagination, bool trackChanges)
    {
        return EitherAsync<string, (IEnumerable<Company> companies, MetaData metaData)>.Try(async () =>
            {
                var companies = await repositoryManager.Company.GetAll(pagination, trackChanges);
                return (companies, companies.MetaData);
            }, exception => exception.Message
        ).Ensure(db => db.companies.Any(), "No companies found");
    }

    public EitherAsync<string, IEnumerable<Company>> GetByIdsWithEither(IEnumerable<Guid> ids, bool trackChanges)
    {
        return EitherAsync<string, IEnumerable<Company>>.Try(async () =>
            {
                var companies = await repositoryManager.Company.GetByIds(ids, trackChanges);
                return companies;
            }, exception=> exception.Message)
            .Ensure(companies => companies.Count() == ids.Count(), 
                new CollectionByIdsBadRequestException().Message);
    }

    public EitherAsync<string, Company> CreateCompanyWithEither(Company company)
    {
        return EitherAsync<string, Company>.FromRight(company)
            .Ensure(c => !string.IsNullOrEmpty(c.Name), 
                "company name can not be null or empty.")
            .Ensure(c=> !string.IsNullOrEmpty(c.Address),
                "company address can not be null or empty.")
            .FlatMap<Company>(c => EitherAsync<string, Company>.Try(async () =>
            {
                var dbCompany = await repositoryManager.Company.CreateRecord(company);
                await repositoryManager.Save();
                return dbCompany;
            }, exception => exception.Message));
    }
    
    public EitherAsync<string, Company> GetByConditionWithEither(string id, bool trackChanges)
    {
        return EitherAsync<string, string>.FromRight(id)
            .Ensure(s=>!string.IsNullOrWhiteSpace(s), "The id can not be null or empty.")
            .Ensure(s=>Guid.TryParse(s, out _), $"{id} is not a valid Guid.")
            .FlatMap<Company>(c=>EitherAsync<string, Company>.Try(async () =>
            {
                var company = await repositoryManager.Company.GetByCondition(c, trackChanges);
                return company;
            }, exception => exception.Message))
            .Ensure(company => company is not null, "Company not found.");

    }
    
    public EitherAsync<string, Unit> DeleteCompanyWithEither(string id, bool trackChanges)
    {
        return EitherAsync<string, string>.FromRight(id)
            .Ensure(s =>!string.IsNullOrWhiteSpace(s) , $"The id can not be null or empty.")
            .Ensure(s=>Guid.TryParse(s,out _),$"{id} is not a valid Guid.")
            .FlatMap<Unit>(idParam=> EitherAsync<string, Unit>.Try(async () =>
            {
                await repositoryManager.Company.DeleteRecord(idParam, trackChanges);
                await repositoryManager.Save();

                return new Unit();
            }, _ => "Company not found."));
    }
}