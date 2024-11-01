namespace Service;

internal sealed class CompanyService:ICompanyService
{
    private readonly IRepositoryManager _repositoryManager;
    
    public CompanyService(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;
    }

    public async Task<(IEnumerable<Company> companies, MetaData metaData)> GetAll(PaginationParameters pagination, bool trackChanges)
    {
        var companies = await _repositoryManager.Company.GetAll(pagination, trackChanges);
        var metaData = companies.MetaData;

        return (companies, metaData);
    }

    public async Task<Company> GetByCondiction(string Id, bool trackChanges)
    {
       return await _repositoryManager.Company.GetByCondiction(Id, trackChanges);
    }

    public async Task<Company> CreateCompany(Company company)
    {
        return await _repositoryManager.Company.CreateCompany(company);
    }

    public async Task<IEnumerable<Company>> GetByIds(IEnumerable<Guid> Ids, bool trackChanges)
    {
        return await _repositoryManager.Company.GetByIds(Ids, trackChanges);
    }

    public async Task DeleteCompany(string Id, bool trackChanges)
    {
        await _repositoryManager.Company.DeleteCompany(Id, trackChanges);
    }

    public async Task SaveChanges()
    {
        await _repositoryManager.Save();
    }
}