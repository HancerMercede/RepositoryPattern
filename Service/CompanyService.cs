using Contracts.Interfaces;
using Entities.Models;
using Service.Contracts.Interfaces;

namespace Service;

internal sealed class CompanyService:ICompanyService
{
    private readonly IRepositoryManager _repositoryManager;
    
    public CompanyService(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;
    }

    public async Task<IEnumerable<Company>> GetAll(bool trackChanges)
    {
        return await _repositoryManager.Company.GetAll(trackChanges);
    }

    public async Task<Company> GetByCondiction(string Id, bool trackChanges)
    {
        throw new NotImplementedException();
    }

    public async Task<Company> CreateCompany(Company company)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Company>> GetByIds(IEnumerable<Guid> Ids, bool trackChanges)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteCompany(string Id, bool trackChanges)
    {
        throw new NotImplementedException();
    }
}