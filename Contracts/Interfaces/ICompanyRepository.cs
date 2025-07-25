
using Entities.Models;
using Shared.RequestFeatures;
using Shared.Shared;

namespace Contracts.Interfaces;

public interface ICompanyRepository:IBaseInterface<Company, PaginationParameters>;