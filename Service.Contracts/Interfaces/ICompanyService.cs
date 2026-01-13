using Entities.Models;
using Shared.RequestFeatures;
using Shared.Shared;

namespace Service.Contracts.Interfaces;

public interface ICompanyService
{
    Task<(IEnumerable<Company> companies, MetaData metaData)> GetAll(PaginationParameters pagination,bool trackChanges);
    Task<Company> GetByCondition(string id, bool trackChanges);
    Task<Company> CreateCompany(Company company);
    Task<IEnumerable<Company>> GetByIds(IEnumerable<Guid> ids, bool trackChanges);
    Task DeleteCompany(string id, bool trackChanges);
    EitherAsync<string, (IEnumerable<Company> companies, MetaData metaData)> GetAllWithEither(
        PaginationParameters pagination, 
        bool trackChanges);

    EitherAsync<string, IEnumerable<Company>> GetByIdsWithEither(IEnumerable<Guid> ids, bool trackChanges);
    
    EitherAsync<string, Company> CreateCompanyWithEither(Company company);
    EitherAsync<string, Company> GetByConditionWithEither(string id, bool trackChanges);
    
    EitherAsync<string, Unit> DeleteCompanyWithEither(string id,bool trackChanges);
}