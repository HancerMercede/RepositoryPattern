using Contracts.Interfaces;
using Service.Contracts.Interfaces;

namespace Service;

internal sealed class CompanyService:ICompanyService
{
    private readonly IRepositoryManager _repositoryManager;
    
    public CompanyService(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;
    }
}