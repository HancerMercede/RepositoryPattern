
using Entities.Models;
using System.Linq.Expressions;

namespace Contracts.Interfaces;

public interface ICompanyRepository
{
    Task<IEnumerable<Company>> GetAll(bool trackChanges);
    Task<Company> GetByCondiction(string Id, bool trackChanges);
    Task<Company> CreateCompany(Company company);
    Task<IEnumerable<Company>> GetByIds(IEnumerable<Guid> Ids, bool trackChanges);

    Task DeleteCompany(string Id, bool trackChanges);
}
