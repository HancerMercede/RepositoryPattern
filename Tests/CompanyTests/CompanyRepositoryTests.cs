using Moq;
using Xunit;
using Shared.RequestFeatures;
using Shared.Shared;


namespace Tests;

public class CompanyRepositoryTests
{
    private readonly Mock<ICompanyRepository> _mockRepo = new();

    [Fact]
    public async Task GetAllCompaniesAsync_WhenOneCompanyExists_ReturnsListWithSingleElement()
    {
        // Arrange
        var pagination = new PaginationParameters();
        var expectedCompanies = CreateSampleCompanies(count: 1);
        
        _mockRepo.Setup(repo => repo.GetAll(pagination, false))
                 .ReturnsAsync(expectedCompanies);

        // Act
        var result = (await _mockRepo.Object.GetAll(pagination, false)).ToList();

        // Assert
        Assert.IsType<List<Company>>(result);
        Assert.Single(result);
        Assert.Equal("Test Company 0", result[0].Name);
        
        _mockRepo.Verify(r => r.GetAll(pagination, false), Times.Once);
    }

    [Fact]
    public async Task GetAllCompaniesAsync_WhenMultipleCompaniesExist_ReturnsCorrectCountryFilter()
    {
        // Arrange
        var pagination = new PaginationParameters();
        var expectedCompanies = CreateSampleCompanies(count: 2);

        _mockRepo.Setup(repo => repo.GetAll(pagination, false))
                 .ReturnsAsync(expectedCompanies);

        // Act
        var result = (await _mockRepo.Object.GetAll(pagination, false)).ToList();

        // Assert
        Assert.IsType<List<Company>>(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, c => Assert.Contains("United States", c.Country));
        
        _mockRepo.Verify(r => r.GetAll(pagination, false), Times.Once);
    }
    [Fact]
    public async Task GetCompanyById_WhenExists_ReturnsCompany()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expected = new Company { Id = id, Name = "Apple", Country = "United States", Address = "palo alto"};
    
        _mockRepo.Setup(repo => repo.GetByCondition(id.ToString(), false))
            .ReturnsAsync(expected);

        // Act
        var result = await _mockRepo.Object.GetByCondition(id.ToString(), false);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Apple", result.Name);
        Assert.Equal("United States", result.Country);
        Assert.Equal("palo alto", result.Address);
    }

    [Fact]
    public async Task GetByIds_WhenCalled_ReturnsRequestedCompanies()
    {
        // Arrange
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var companies = new List<Company> 
        { 
            new() { Id = ids[0], Name = "Co 1" }, 
            new() { Id = ids[1], Name = "Co 2" } 
        };

        _mockRepo.Setup(repo => repo.GetByIds(ids, false))
            .ReturnsAsync(companies);

        // Act
        var result = await _mockRepo.Object.GetByIds(ids, false);

        // Assert
        Assert.Equal(2, result.Count());
        _mockRepo.Verify(r => r.GetByIds(ids, false), Times.Once);
    }

    [Fact]
    public void CreateCompany_WhenValid_CallsCreate()
    {
        // Arrange
        var company = new Company { Name = "New Tech" };

        // Act
        _mockRepo.Object.CreateRecord(company);

        // Assert
        _mockRepo.Verify(r => r.CreateRecord(company), Times.Once);
    }
    private PageList<Company> CreateSampleCompanies(int count)
    {
        var companiesList = new List<Company>();

        for (int i = 0; i < count; i++)
        {
            companiesList.Add(new Company
            {
                Id = Guid.NewGuid(),
                Name = $"Test Company {i}",
                Country = "United States",
                Address = $"{908 + i} Woodrow Way"
            });
        }

        
        return new PageList<Company>(companiesList, companiesList.Count, 1, 10);
    }
}