using Contracts.Interfaces;
using Entities.Models;
using Service.Contracts.Interfaces;
using System.ComponentModel.Design;

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

    public async Task<IEnumerable<Employee>> GetAll(string CompanyId, bool trackChanges)
    {
        return await _repositoryManager.Employee.GetAll(CompanyId, trackChanges);
       
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