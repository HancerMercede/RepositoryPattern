using Entities.Models;

namespace Service.Contracts.Interfaces;

public interface ICompanyService
{
    Task<IEnumerable<Company>> GetAll(bool trackChanges);
    Task<Company> GetByCondiction(string Id, bool trackChanges);
    Task<Company> CreateCompany(Company company);
    Task<IEnumerable<Company>> GetByIds(IEnumerable<Guid> Ids, bool trackChanges);

    Task DeleteCompany(string Id, bool trackChanges);
}