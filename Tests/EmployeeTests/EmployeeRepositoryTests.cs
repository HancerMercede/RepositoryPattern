using Moq;
using Xunit;
using Shared.RequestFeatures;
using Shared.Shared; 

namespace Tests.EmployeeTests;

public class EmployeeRepositoryTests
{
    private readonly Mock<IEmployeeRepository> _mockRepo = new();

    [Fact]
    public async Task GetEmployees_WhenCalled_ReturnsPagedListWithCorrectCount()
    {
        // Arrange
        var companyId = Guid.NewGuid().ToString();
        var pagination = new PaginationParameters();
        var expectedEmployees = CreateSampleEmployees(companyId);
        
        _mockRepo.Setup(repo => repo.GetAll(companyId, pagination, false))
                 .ReturnsAsync(expectedEmployees);

        // Act
        var result = await _mockRepo.Object.GetAll(companyId, pagination, false);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<Employee>>(result);
        Assert.Equal(expectedEmployees.Count, result.Count());
        _mockRepo.Verify(x => x.GetAll(companyId, pagination, false), Times.Once);
    }

    [Fact]
    public async Task GetEmployeeById_WhenEmployeeExists_ReturnsEmployeeWithCorrectData()
    {
        // Arrange
        var companyId = Guid.NewGuid().ToString();
        var employeeId = Guid.NewGuid().ToString();
        var expectedEmployee = CreateSampleEmployee(companyId, employeeId);
        
        _mockRepo.Setup(repo => repo.GetByCondition(companyId, employeeId, false))
                 .ReturnsAsync(expectedEmployee);

        // Act
        var result = await _mockRepo.Object.GetByCondition(companyId, employeeId, false);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedEmployee.Name, result.Name);
        Assert.Equal(expectedEmployee.Id, result.Id);
        _mockRepo.Verify(x => x.GetByCondition(companyId, employeeId, false), Times.Once);
    }
    
    [Fact]
    public async Task GetEmployeeById_WhenEmployeeDoesNotExist_ReturnsNull()
    {
        // Arrange
        var companyId = Guid.NewGuid().ToString();
        var employeeId = Guid.NewGuid().ToString();
        
        _mockRepo.Setup(repo => repo.GetByCondition(companyId, employeeId, false))!
            .ReturnsAsync((Employee)null!);

        // Act
        var result = await _mockRepo.Object.GetByCondition(companyId, employeeId, false);

        // Assert
        Assert.Null(result);
    }
 
    [Fact]
    public void CreateEmployee_WhenValid_CallsRepositoryCreate()
    {
        // Arrange
        var companyId = Guid.NewGuid().ToString();
        var employee = new Employee { Name = "New Employee", CompanyId = Guid.Parse(companyId), Position = "Developer" };

        // Act
        _mockRepo.Object.CreateEmployeeForCompany(companyId, employee);

        // Assert
        _mockRepo.Verify(r => r.CreateEmployeeForCompany(It.Is<string>(id => id == companyId), 
            It.Is<Employee>(e => e.Name == "New Employee" && e.Position == "Developer")), Times.Once);
    }

    [Fact]
    public void DeleteEmployee_WhenCalled_CallsRepositoryDelete()
    {
        // Arrange
        var companyId = Guid.NewGuid().ToString();
        var employeeId = Guid.NewGuid();
        var employee = new Employee { Id = employeeId, Name = "To Be Deleted" };

        // Act
        _mockRepo.Object.DeleteEmployee(companyId, employeeId.ToString(), false);

        // Assert
        _mockRepo.Verify(r => r.DeleteEmployee(
            It.Is<string>(c => c == companyId),
            It.Is<string>(e => e == employeeId.ToString()),
            It.Is<bool>(trackChanges => trackChanges == false)), Times.Once);
    }
    private PageList<Employee> CreateSampleEmployees(string companyId)
    {
        var employees = new List<Employee>
        {
            new() {
                Id = Guid.NewGuid(),
                Name = "Hancer Mercedes",
                Age = 36,
                CompanyId = Guid.Parse(companyId),
                Position = ".NET Developer"
            }
        };
      
        return new PageList<Employee>(employees, employees.Count, 1, 10);
    }

    private Employee CreateSampleEmployee(string companyId, string id)
    {
        return new Employee
        {
            Id = Guid.Parse(id),
            Name = "Hancer Mercedes",
            Age = 36,
            CompanyId = Guid.Parse(companyId),
            Position = ".NET Developer"
        };
    }
}