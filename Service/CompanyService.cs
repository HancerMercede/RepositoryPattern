namespace Service;

internal sealed class CompanyService(IRepositoryManager repositoryManager) : ICompanyService
{
    public async Task<(IEnumerable<Company> companies, MetaData metaData)> GetAll(PaginationParameters pagination, bool trackChanges)
    {
        var companies = await repositoryManager.Company.GetAll(pagination, trackChanges);
        var metaData = companies.MetaData;

        return (companies, metaData);
    }
    public async Task<Company> GetByCondition(string id, bool trackChanges) => await repositoryManager.Company.GetByCondition(id, trackChanges);

    public async Task<Company> CreateCompany(Company company) => await repositoryManager.Company.CreateRecord(company);

    public async Task<IEnumerable<Company>> GetByIds(IEnumerable<Guid> ids, bool trackChanges) => await repositoryManager.Company.GetByIds(ids, trackChanges);

    public async Task DeleteCompany(string id, bool trackChanges) => await repositoryManager.Company.DeleteRecord(id, trackChanges);
    
    public async Task SaveChanges()
    {
        await repositoryManager.Save();
    }
}