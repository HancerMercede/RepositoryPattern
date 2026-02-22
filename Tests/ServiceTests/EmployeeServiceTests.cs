using Dtos.DtoModels;
using Entities.Exceptions;
using Service;
using Shared.RequestFeatures;
using Shared.Shared;

namespace Tests.ServiceTests;

public class EmployeeServiceTests
{
    private readonly Mock<IRepositoryManager> _mockRepoManager;
    private readonly Mock<IEmployeeRepository> _mockEmployeeRepo;
    private readonly Mock<ICompanyRepository> _mockCompanyRepo;
    private readonly EmployeeService _service;

    public EmployeeServiceTests()
    {
        _mockRepoManager = new Mock<IRepositoryManager>();
        _mockEmployeeRepo = new Mock<IEmployeeRepository>();
        _mockCompanyRepo = new Mock<ICompanyRepository>();
        _mockRepoManager.Setup(r => r.Employee).Returns(_mockEmployeeRepo.Object);
        _mockRepoManager.Setup(r => r.Company).Returns(_mockCompanyRepo.Object);
        _service = new EmployeeService(_mockRepoManager.Object);
    }

    #region Conventional Tests

    [Fact]
    public async Task CreateEmployee_WhenValid_ReturnsCreatedEmployee()
    {
        var companyId = Guid.NewGuid().ToString();
        var employee = new Employee { Name = "John Doe", Position = "Developer" };
        _mockEmployeeRepo.Setup(r => r.CreateEmployeeForCompany(companyId, employee)).ReturnsAsync(employee);

        var result = await _service.CreateEmployee(companyId, employee);

        Assert.NotNull(result);
        Assert.Equal("John Doe", result.Name);
    }

    [Fact]
    public async Task CreateEmployee_WhenNull_ThrowsException()
    {
        await Assert.ThrowsAsync<EmployeeBadRequestException>(() => _service.CreateEmployee("companyId", null!));
    }

    [Fact]
    public async Task DeleteEmployee_WhenValid_CallsRepository()
    {
        var companyId = Guid.NewGuid().ToString();
        var employeeId = Guid.NewGuid().ToString();
        var company = new Company { Id = Guid.Parse(companyId) };
        var employee = new Employee { Id = Guid.Parse(employeeId) };

        _mockCompanyRepo.Setup(r => r.GetByCondition(companyId, false)).ReturnsAsync(company);
        _mockEmployeeRepo.Setup(r => r.GetByCondition(companyId, employeeId, false)).ReturnsAsync(employee);

        await _service.DeleteEmployee(companyId, employeeId, false);

        _mockEmployeeRepo.Verify(r => r.DeleteEmployee(companyId, employeeId, false), Times.Once);
    }

    [Fact]
    public async Task DeleteEmployee_WhenCompanyNotFound_ThrowsException()
    {
        var companyId = Guid.NewGuid().ToString();
        var employeeId = Guid.NewGuid().ToString();

        _mockCompanyRepo.Setup(r => r.GetByCondition(companyId, false)).ReturnsAsync((Company?)null);

        await Assert.ThrowsAsync<CompanyNotFoundException>(() => _service.DeleteEmployee(companyId, employeeId, false));
    }

    [Fact]
    public async Task DeleteEmployee_WhenEmployeeNotFound_ThrowsException()
    {
        var companyId = Guid.NewGuid().ToString();
        var employeeId = Guid.NewGuid().ToString();
        var company = new Company { Id = Guid.Parse(companyId) };

        _mockCompanyRepo.Setup(r => r.GetByCondition(companyId, false)).ReturnsAsync(company);
        _mockEmployeeRepo.Setup(r => r.GetByCondition(companyId, employeeId, false)).ReturnsAsync((Employee?)null);

        await Assert.ThrowsAsync<EmployeeNotFoundException>(() => _service.DeleteEmployee(companyId, employeeId, false));
    }

    [Fact]
    public async Task GetEmployeeForPatch_WhenValid_ReturnsEmployeeAndDto()
    {
        var companyId = Guid.NewGuid().ToString();
        var employeeId = Guid.NewGuid().ToString();
        var company = new Company { Id = Guid.Parse(companyId) };
        var employee = new Employee { Id = Guid.Parse(employeeId), Name = "John" };

        _mockCompanyRepo.Setup(r => r.GetByCondition(companyId, false)).ReturnsAsync(company);
        _mockEmployeeRepo.Setup(r => r.GetByCondition(companyId, employeeId, false)).ReturnsAsync(employee);

        var result = await _service.GetEmployeeForPatch(companyId, employeeId, false, false);

        Assert.NotNull(result.employeeToPath);
        Assert.NotNull(result.employee);
    }

    [Fact]
    public async Task GetEmployeeForPatch_WhenCompanyNotFound_ThrowsException()
    {
        var companyId = Guid.NewGuid().ToString();
        var employeeId = Guid.NewGuid().ToString();

        _mockCompanyRepo.Setup(r => r.GetByCondition(companyId, false)).ReturnsAsync((Company?)null);

        await Assert.ThrowsAsync<CompanyNotFoundException>(() => _service.GetEmployeeForPatch(companyId, employeeId, false, false));
    }

    [Fact]
    public async Task GetEmployeeForPatch_WhenEmployeeNotFound_ThrowsException()
    {
        var companyId = Guid.NewGuid().ToString();
        var employeeId = Guid.NewGuid().ToString();
        var company = new Company { Id = Guid.Parse(companyId) };

        _mockCompanyRepo.Setup(r => r.GetByCondition(companyId, false)).ReturnsAsync(company);
        _mockEmployeeRepo.Setup(r => r.GetByCondition(companyId, employeeId, false)).ReturnsAsync((Employee?)null);

        await Assert.ThrowsAsync<EmployeeNotFoundException>(() => _service.GetEmployeeForPatch(companyId, employeeId, false, false));
    }

    [Fact]
    public async Task SaveChangesForPatch_CallsSave()
    {
        var employeeDto = new EmployeeUpdateDto { Name = "Updated" };
        var employee = new Employee { Name = "Original" };

        await _service.SaveChangesForPatch(employeeDto, employee);

        _mockRepoManager.Verify(r => r.Save(), Times.Once);
    }

    [Fact]
    public async Task GetAll_ReturnsEmployeesWithMetaData()
    {
        var companyId = Guid.NewGuid().ToString();
        var pagination = new PaginationParameters();
        var employees = new PageList<Employee>(new List<Employee>(), 0, 1, 10);

        _mockEmployeeRepo.Setup(r => r.GetAll(companyId, pagination, false)).ReturnsAsync(employees);

        var result = await _service.GetAll(companyId, pagination, false);

        Assert.NotNull(result.Employees);
        Assert.Equal(employees.MetaData, result.metaData);
    }

    [Fact]
    public async Task GetByCondition_WhenEmployeeExists_ReturnsEmployee()
    {
        var companyId = Guid.NewGuid().ToString();
        var employeeId = Guid.NewGuid().ToString();
        var employee = new Employee { Id = Guid.Parse(employeeId), Name = "John" };

        _mockEmployeeRepo.Setup(r => r.GetByCondition(companyId, employeeId, false)).ReturnsAsync(employee);

        var result = await _service.GetByCondition(companyId, employeeId, false);

        Assert.NotNull(result);
        Assert.Equal("John", result.Name);
    }

    [Fact]
    public async Task GetByCondition_WhenIdIsNull_ThrowsException()
    {
        await Assert.ThrowsAsync<IdParametersBadRequestException>(() => _service.GetByCondition("companyId", null!, false));
    }

    [Fact]
    public async Task GetByCondition_WhenEmployeeNotFound_ThrowsException()
    {
        var companyId = Guid.NewGuid().ToString();
        var employeeId = Guid.NewGuid().ToString();

        _mockEmployeeRepo.Setup(r => r.GetByCondition(companyId, employeeId, false)).ReturnsAsync((Employee?)null);

        await Assert.ThrowsAsync<EmployeeNotFoundException>(() => _service.GetByCondition(companyId, employeeId, false));
    }

    #endregion

    #region Either Tests

    [Fact]
    public async Task GetAllEmployees_WhenEmployeesExist_ReturnsEmployees()
    {
        var companyId = Guid.NewGuid().ToString();
        var pagination = new PaginationParameters();
        var employees = new PageList<Employee>(new List<Employee> { new() { Name = "John" } }, 1, 1, 10);

        _mockEmployeeRepo.Setup(r => r.GetAll(companyId, pagination, false)).ReturnsAsync(employees);

        var result = await _service.GetAllEmployees(companyId, pagination, false).Run();

        Assert.True(result.Match(_ => false, _ => true));
    }

    [Fact]
    public async Task GetAllEmployees_WhenNoEmployees_ReturnsLeft()
    {
        var companyId = Guid.NewGuid().ToString();
        var pagination = new PaginationParameters();
        var employees = new PageList<Employee>(new List<Employee>(), 0, 1, 10);

        _mockEmployeeRepo.Setup(r => r.GetAll(companyId, pagination, false)).ReturnsAsync(employees);

        var result = await _service.GetAllEmployees(companyId, pagination, false).Run();

        Assert.True(result.Match(_ => true, _ => false));
    }

    [Fact]
    public async Task GetAllEmployees_WhenCompanyIdEmpty_ReturnsLeft()
    {
        var result = await _service.GetAllEmployees("", new PaginationParameters(), false).Run();

        Assert.True(result.Match(l => l.Contains("company id can not be null or empty"), _ => false));
    }

    [Fact]
    public async Task GetByConditionWithEither_WhenValid_ReturnsEmployee()
    {
        var companyId = Guid.NewGuid().ToString();
        var employeeId = Guid.NewGuid().ToString();
        var employee = new Employee { Id = Guid.Parse(employeeId), Name = "John" };

        _mockEmployeeRepo.Setup(r => r.GetByCondition(companyId, employeeId, false)).ReturnsAsync(employee);

        var result = await _service.GetByConditionWithEither(companyId, employeeId, false).Run();

        Assert.True(result.Match(_ => false, _ => true));
    }

    [Fact]
    public async Task GetByConditionWithEither_WhenCompanyIdEmpty_ReturnsLeft()
    {
        var result = await _service.GetByConditionWithEither("", "employeeId", false).Run();

        Assert.True(result.Match(l => l.Contains("company id can not be null or empty"), _ => false));
    }

    [Fact]
    public async Task GetByConditionWithEither_WhenEmployeeIdEmpty_ReturnsLeft()
    {
        var result = await _service.GetByConditionWithEither("companyId", "", false).Run();

        Assert.True(result.Match(l => l.Contains("employee id can not be null or empty"), _ => false));
    }

    [Fact]
    public async Task GetByConditionWithEither_WhenEmployeeNotFound_ReturnsLeft()
    {
        var companyId = Guid.NewGuid().ToString();
        var employeeId = Guid.NewGuid().ToString();

        _mockEmployeeRepo.Setup(r => r.GetByCondition(companyId, employeeId, false)).ReturnsAsync((Employee?)null);

        var result = await _service.GetByConditionWithEither(companyId, employeeId, false).Run();

        Assert.True(result.Match(_ => true, _ => false));
    }

    [Fact]
    public async Task CreateEmployeeWithEither_WhenValid_ReturnsEmployee()
    {
        var companyId = Guid.NewGuid().ToString();
        var employee = new Employee { Name = "John", Age = 25, Position = "Developer" };
        _mockEmployeeRepo.Setup(r => r.CreateEmployeeForCompany(companyId, employee)).ReturnsAsync(employee);
        _mockRepoManager.Setup(r => r.Save()).Returns(Task.CompletedTask);

        var result = await _service.CreateEmployeeWithEither(companyId, employee).Run();

        Assert.True(result.Match(_ => false, e => e.Name == "John"));
    }

    [Fact]
    public async Task CreateEmployeeWithEither_WhenCompanyIdEmpty_ReturnsLeft()
    {
        var employee = new Employee { Name = "John", Age = 25, Position = "Developer" };

        var result = await _service.CreateEmployeeWithEither("", employee).Run();

        Assert.True(result.Match(l => l.Contains("company id can not be null or empty"), _ => false));
    }

    [Fact]
    public async Task CreateEmployeeWithEither_WhenNameEmpty_ReturnsLeft()
    {
        var companyId = Guid.NewGuid().ToString();
        var employee = new Employee { Name = "", Age = 25, Position = "Developer" };

        var result = await _service.CreateEmployeeWithEither(companyId, employee).Run();

        Assert.True(result.Match(l => l.Contains("name can not be null or empty"), _ => false));
    }

    [Fact]
    public async Task CreateEmployeeWithEither_WhenAgeBelow18_ReturnsLeft()
    {
        var companyId = Guid.NewGuid().ToString();
        var employee = new Employee { Name = "John", Age = 15, Position = "Developer" };

        var result = await _service.CreateEmployeeWithEither(companyId, employee).Run();

        Assert.True(result.Match(l => l.Contains("age can not be less than 18"), _ => false));
    }

    [Fact]
    public async Task CreateEmployeeWithEither_WhenAgeAbove80_ReturnsLeft()
    {
        var companyId = Guid.NewGuid().ToString();
        var employee = new Employee { Name = "John", Age = 85, Position = "Developer" };

        var result = await _service.CreateEmployeeWithEither(companyId, employee).Run();

        Assert.True(result.Match(l => l.Contains("age can not be less than 18 or more than 80"), _ => false));
    }

    [Fact]
    public async Task CreateEmployeeWithEither_WhenPositionEmpty_ReturnsLeft()
    {
        var companyId = Guid.NewGuid().ToString();
        var employee = new Employee { Name = "John", Age = 25, Position = "" };

        var result = await _service.CreateEmployeeWithEither(companyId, employee).Run();

        Assert.True(result.Match(l => l.Contains("position can not be null or empty"), _ => false));
    }

    [Fact]
    public async Task DeleteEmployeeWithEither_WhenValid_ReturnsUnit()
    {
        var companyId = Guid.NewGuid().ToString();
        var employeeId = Guid.NewGuid().ToString();

        _mockEmployeeRepo.Setup(r => r.DeleteEmployee(companyId, employeeId, false)).Returns(Task.CompletedTask);
        _mockRepoManager.Setup(r => r.Save()).Returns(Task.CompletedTask);

        var result = await _service.DeleteEmployeeWithEither(companyId, employeeId, false).Run();

        Assert.True(result.Match(_ => false, _ => true));
    }

    [Fact]
    public async Task DeleteEmployeeWithEither_WhenCompanyIdEmpty_ReturnsLeft()
    {
        var result = await _service.DeleteEmployeeWithEither("", "employeeId", false).Run();

        Assert.True(result.Match(l => l.Contains("company id can not be null or empty"), _ => false));
    }

    [Fact]
    public async Task DeleteEmployeeWithEither_WhenEmployeeIdEmpty_ReturnsLeft()
    {
        var result = await _service.DeleteEmployeeWithEither("companyId", "", false).Run();

        Assert.True(result.Match(l => l.Contains("user id can not be null or empty"), _ => false));
    }

    #endregion
}
