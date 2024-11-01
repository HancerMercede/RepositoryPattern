
using Entities.Models;
using Shared.RequestFeatures;
using Shared.Shared;

namespace Contracts.Interfaces;

public interface ICompanyRepository:IBaseInterface<Company, PaginationParameters>
{
    //Task<PageList<Company>> GetAll(PaginationParameters pagination, bool trackChanges);
    //Task<Company> GetByCondiction(string Id, bool trackChanges);
    //Task<Company> CreateCompany(Company company);
    //Task<IEnumerable<Company>> GetByIds(IEnumerable<Guid> Ids, bool trackChanges);

    //Task DeleteCompany(string Id, bool trackChanges);
}
