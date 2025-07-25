namespace Service;

public class ServiceManager(IRepositoryManager repositoryManager) : IServiceManager
{
    private readonly Lazy<ICompanyService> _companyService = new(() => new CompanyService(repositoryManager));
    private readonly Lazy<IEmployeeService> _employeeService = new(() => new EmployeeService(repositoryManager));

    public ICompanyService CompanyService => _companyService.Value;
    public IEmployeeService EmployeeService => _employeeService.Value;
    public async Task Save()=> await repositoryManager.Save();
}