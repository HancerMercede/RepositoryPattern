using Dtos.DtoModels;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Nexus.Store.Contract;
using Presentation.Controllers;
using Service.Contracts.Interfaces;
using Shared.RequestFeatures;
using Shared.Shared;

namespace Tests.ControllerTests;

public class EmployeeControllerTests
{
    private readonly Mock<IServiceManager> _mockServiceManager;
    private readonly Mock<IEmployeeService> _mockEmployeeService;
    private readonly Mock<ILogger<EmployeeController>> _mockLogger;
    private readonly Mock<INexusEngine> _mockNexus;
    private readonly EmployeeController _controller;

    public EmployeeControllerTests()
    {
        _mockServiceManager = new Mock<IServiceManager>();
        _mockEmployeeService = new Mock<IEmployeeService>();
        _mockLogger = new Mock<ILogger<EmployeeController>>();
        _mockNexus = new Mock<INexusEngine>();

        _mockServiceManager.Setup(s => s.EmployeeService).Returns(_mockEmployeeService.Object);
        _mockServiceManager.Setup(s => s.Save()).Returns(Task.CompletedTask);

        _controller = new EmployeeController(
            _mockServiceManager.Object,
            _mockLogger.Object,
            _mockNexus.Object
        );

        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    #region GetAllEmployeesWithEither Tests

    [Fact]
    public async Task GetAllEmployeesWithEither_WhenCacheExists_ReturnsCachedData()
    {
        var companyId = Guid.NewGuid().ToString();
        var pagination = new PaginationParameters();
        var cachedEmployees = new List<EmployeeDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Cached Employee", CompanyId = Guid.Parse(companyId) }
        };

        _mockNexus
            .Setup(n => n.GetAsync<List<EmployeeDto>>(It.IsAny<string>()))
            .ReturnsAsync(cachedEmployees);

        var result = await _controller.GetAllEmployeesWithEither(companyId, pagination);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        _mockEmployeeService.Verify(
            s => s.GetAllEmployees(It.IsAny<string>(), It.IsAny<PaginationParameters>(), It.IsAny<bool>()),
            Times.Never);
    }

    [Fact]
    public async Task GetAllEmployeesWithEither_WhenNoCacheAndEmployeesExist_ReturnsOk()
    {
        var companyId = Guid.NewGuid().ToString();
        var pagination = new PaginationParameters();
        var employees = new List<Employee>
        {
            new() { Id = Guid.NewGuid(), Name = "Employee 1", Age = 30, Position = "Dev", CompanyId = Guid.Parse(companyId) }
        };
        var metaData = new MetaData { CurrentPage = 1, TotalPages = 1, PageSize = 10, TotalCount = 1 };

        _mockNexus
            .Setup(n => n.GetAsync<List<EmployeeDto>>(It.IsAny<string>()))
            .ReturnsAsync((List<EmployeeDto>?)null);

        _mockEmployeeService
            .Setup(s => s.GetAllEmployees(companyId, pagination, false))
            .Returns(EitherAsync<string, (IEnumerable<Employee> employees, MetaData metaData)>
                .FromRight((employees, metaData)));

        var result = await _controller.GetAllEmployeesWithEither(companyId, pagination);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetAllEmployeesWithEither_WhenCompanyNotFound_ReturnsNotFound()
    {
        var companyId = Guid.NewGuid().ToString();
        var pagination = new PaginationParameters();

        _mockNexus
            .Setup(n => n.GetAsync<List<EmployeeDto>>(It.IsAny<string>()))
            .ReturnsAsync((List<EmployeeDto>?)null);

        _mockEmployeeService
            .Setup(s => s.GetAllEmployees(companyId, pagination, false))
            .Returns(EitherAsync<string, (IEnumerable<Employee> employees, MetaData metaData)>
                .FromLeft("Company not found"));

        var result = await _controller.GetAllEmployeesWithEither(companyId, pagination);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    #endregion

    #region GetByConditionEither Tests

    [Fact]
    public async Task GetByConditionEither_WhenEmployeeExists_ReturnsOk()
    {
        var companyId = Guid.NewGuid().ToString();
        var employeeId = Guid.NewGuid().ToString();
        var employee = new Employee
        {
            Id = Guid.Parse(employeeId),
            Name = "Test Employee",
            Age = 30,
            Position = "Developer",
            CompanyId = Guid.Parse(companyId)
        };

        _mockEmployeeService
            .Setup(s => s.GetByConditionWithEither(companyId, employeeId, false))
            .Returns(EitherAsync<string, Employee>.FromRight(employee));

        var result = await _controller.GetByConditionEither(companyId, employeeId);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetByConditionEither_WhenCompanyNotFound_ReturnsNotFound()
    {
        var companyId = Guid.NewGuid().ToString();
        var employeeId = Guid.NewGuid().ToString();

        _mockEmployeeService
            .Setup(s => s.GetByConditionWithEither(companyId, employeeId, false))
            .Returns(EitherAsync<string, Employee>.FromLeft("Company not found"));

        var result = await _controller.GetByConditionEither(companyId, employeeId);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetByConditionEither_WhenEmployeeNotFound_ReturnsNotFound()
    {
        var companyId = Guid.NewGuid().ToString();
        var employeeId = Guid.NewGuid().ToString();

        _mockEmployeeService
            .Setup(s => s.GetByConditionWithEither(companyId, employeeId, false))
            .Returns(EitherAsync<string, Employee>.FromLeft("Employee not found"));

        var result = await _controller.GetByConditionEither(companyId, employeeId);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    #endregion

    #region CreateEmployeeForCompany Tests

    [Fact]
    public async Task CreateEmployeeForCompany_WhenValid_ReturnsCreatedAtRoute()
    {
        var companyId = Guid.NewGuid().ToString();
        var createDto = new EmployeeCreateDto { Name = "New Employee", Age = 25, Position = "Junior Dev" };
        var createdEmployee = new Employee
        {
            Id = Guid.NewGuid(),
            Name = "New Employee",
            Age = 25,
            Position = "Junior Dev",
            CompanyId = Guid.Parse(companyId)
        };

        _mockEmployeeService
            .Setup(s => s.CreateEmployeeWithEither(companyId, It.IsAny<Employee>()))
            .Returns(EitherAsync<string, Employee>.FromRight(createdEmployee));

        var result = await _controller.CreateEmployeeForCompany(companyId, createDto);

        var createdResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
        Assert.Equal("GetEmployeeForCompany", createdResult.RouteName);
    }

    [Fact]
    public async Task CreateEmployeeForCompany_WhenCompanyNotFound_ReturnsNotFound()
    {
        var companyId = Guid.NewGuid().ToString();
        var createDto = new EmployeeCreateDto { Name = "New Employee", Age = 25, Position = "Junior Dev" };

        _mockEmployeeService
            .Setup(s => s.CreateEmployeeWithEither(companyId, It.IsAny<Employee>()))
            .Returns(EitherAsync<string, Employee>.FromLeft("Company not found"));

        var result = await _controller.CreateEmployeeForCompany(companyId, createDto);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateEmployeeForCompany_WhenInvalidData_ReturnsBadRequest()
    {
        var companyId = Guid.NewGuid().ToString();
        var createDto = new EmployeeCreateDto { Name = "", Age = 0, Position = "" };

        _mockEmployeeService
            .Setup(s => s.CreateEmployeeWithEither(companyId, It.IsAny<Employee>()))
            .Returns(EitherAsync<string, Employee>.FromLeft("Name is required"));

        var result = await _controller.CreateEmployeeForCompany(companyId, createDto);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    #endregion

    #region DeleteEmployee Tests

    [Fact]
    public async Task DeleteEmployee_WhenValid_ReturnsNoContent()
    {
        var companyId = Guid.NewGuid().ToString();
        var employeeId = Guid.NewGuid().ToString();

        _mockEmployeeService
            .Setup(s => s.DeleteEmployeeWithEither(companyId, employeeId, false))
            .Returns(EitherAsync<string, Unit>.FromRight(new Unit()));

        var result = await _controller.DeleteEmployee(companyId, employeeId);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteEmployee_WhenCompanyNotFound_ReturnsNotFound()
    {
        var companyId = Guid.NewGuid().ToString();
        var employeeId = Guid.NewGuid().ToString();

        _mockEmployeeService
            .Setup(s => s.DeleteEmployeeWithEither(companyId, employeeId, false))
            .Returns(EitherAsync<string, Unit>.FromLeft("Company not found"));

        var result = await _controller.DeleteEmployee(companyId, employeeId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteEmployee_WhenEmployeeNotFound_ReturnsNotFound()
    {
        var companyId = Guid.NewGuid().ToString();
        var employeeId = Guid.NewGuid().ToString();

        _mockEmployeeService
            .Setup(s => s.DeleteEmployeeWithEither(companyId, employeeId, false))
            .Returns(EitherAsync<string, Unit>.FromLeft("Employee not found"));

        var result = await _controller.DeleteEmployee(companyId, employeeId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    #endregion
}
