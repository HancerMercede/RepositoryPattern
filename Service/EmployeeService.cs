namespace Service;

public class EmployeeService:IEmployeeService
{
    private readonly IRepositoryManager _repositoryManager;

    public EmployeeService(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;
    }

    public async Task<Employee> CreateEmployee(string companyId, Employee employee)
    {
        return await _repositoryManager.Employee.CreateEmployeeForCompany(companyId, employee);
    }

    public async Task DeleteEmployee(string companyId, string Id, bool trackChanges)
    {
        await _repositoryManager.Employee.DeleteEmployee(companyId, Id, trackChanges);
    }

    public async Task<(IEnumerable<Employee> Employees, MetaData metaData)> GetAll(string CompanyId,PaginationParameters PaginationParameters, bool trackChanges)
    {
        var employees = await _repositoryManager.Employee.GetAll(CompanyId, PaginationParameters, trackChanges);
        var metadata = employees.MetaData;
       
        return (Employees: employees, metaData: metadata); 
    }

    public async Task<Employee> GetByCondiction(string companyId, string Id, bool trackChanges)
    {
        return await _repositoryManager.Employee.GetByCondiction(companyId, Id,trackChanges);
    }

    public async Task SaveChanges()
    {
        await _repositoryManager.Save();
    }
}