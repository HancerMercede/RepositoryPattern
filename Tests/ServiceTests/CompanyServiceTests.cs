using Entities.Exceptions;
using Service;
using Shared.RequestFeatures;
using Shared.Shared;

namespace Tests.ServiceTests;

public class CompanyServiceTests
{
    private readonly Mock<IRepositoryManager> _mockRepoManager;
    private readonly Mock<ICompanyRepository> _mockCompanyRepo;
    private readonly CompanyService _service;

    public CompanyServiceTests()
    {
        _mockRepoManager = new Mock<IRepositoryManager>();
        _mockCompanyRepo = new Mock<ICompanyRepository>();
        _mockRepoManager.Setup(r => r.Company).Returns(_mockCompanyRepo.Object);
        _service = new CompanyService(_mockRepoManager.Object);
    }

    #region Conventional Tests

    [Fact]
    public async Task GetAll_ReturnsCompaniesWithMetaData()
    {
        var pagination = new PaginationParameters();
        var companies = new PageList<Company>(new List<Company>(), 0, 1, 10);

        _mockCompanyRepo.Setup(r => r.GetAll(pagination, false)).ReturnsAsync(companies);

        var result = await _service.GetAll(pagination, false);

        Assert.NotNull(result.companies);
        Assert.Equal(companies.MetaData, result.metaData);
        _mockCompanyRepo.Verify(r => r.GetAll(pagination, false), Times.Once);
    }

    [Fact]
    public async Task GetByCondition_WhenCompanyExists_ReturnsCompany()
    {
        var id = Guid.NewGuid().ToString();
        var company = new Company { Id = Guid.Parse(id), Name = "Test Company" };

        _mockCompanyRepo.Setup(r => r.GetByCondition(id, false)).ReturnsAsync(company);

        var result = await _service.GetByCondition(id, false);

        Assert.NotNull(result);
        Assert.Equal("Test Company", result.Name);
    }

    [Fact]
    public async Task GetByCondition_WhenIdIsNull_ThrowsException()
    {
        await Assert.ThrowsAsync<IdParametersBadRequestException>(() => _service.GetByCondition(null!, false));
    }

    [Fact]
    public async Task GetByCondition_WhenCompanyNotFound_ThrowsException()
    {
        var id = Guid.NewGuid().ToString();
        _mockCompanyRepo.Setup(r => r.GetByCondition(id, false)).Returns(Task.FromResult<Company?>(null));

        await Assert.ThrowsAsync<CompanyNotFoundException>(() => _service.GetByCondition(id, false));
    }

    [Fact]
    public async Task CreateCompany_WhenValid_ReturnsCreatedCompany()
    {
        var company = new Company { Name = "New Company" };
        _mockCompanyRepo.Setup(r => r.CreateRecord(company)).ReturnsAsync(company);

        var result = await _service.CreateCompany(company);

        Assert.NotNull(result);
        Assert.Equal("New Company", result.Name);
        _mockCompanyRepo.Verify(r => r.CreateRecord(company), Times.Once);
    }

    [Fact]
    public async Task CreateCompany_WhenNull_ThrowsException()
    {
        await Assert.ThrowsAsync<CompanyBadRequestException>(() => _service.CreateCompany(null!));
    }

    [Fact]
    public async Task GetByIds_WhenValid_ReturnsCompanies()
    {
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var companies = new List<Company>
        {
            new() { Id = ids[0], Name = "Company 1" },
            new() { Id = ids[1], Name = "Company 2" }
        };

        _mockCompanyRepo.Setup(r => r.GetByIds(ids, false)).ReturnsAsync(companies);

        var result = await _service.GetByIds(ids, false);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIds_WhenCountMismatch_ThrowsException()
    {
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var companies = new List<Company> { new() { Id = ids[0] } };

        _mockCompanyRepo.Setup(r => r.GetByIds(ids, false)).ReturnsAsync(companies);

        await Assert.ThrowsAsync<CollectionByIdsBadRequestException>(() => _service.GetByIds(ids, false));
    }

    [Fact]
    public async Task DeleteCompany_CallsRepositoryDelete()
    {
        var id = Guid.NewGuid().ToString();

        await _service.DeleteCompany(id, false);

        _mockCompanyRepo.Verify(r => r.DeleteRecord(id, false), Times.Once);
    }

    #endregion

    #region Either Tests

    [Fact]
    public async Task GetAllWithEither_WhenCompaniesExist_ReturnsCompanies()
    {
        var pagination = new PaginationParameters();
        var companies = new PageList<Company>(new List<Company> { new() { Name = "Test" } }, 1, 1, 10);

        _mockCompanyRepo.Setup(r => r.GetAll(pagination, false)).ReturnsAsync(companies);

        var result = await _service.GetAllWithEither(pagination, false).Run();

        Assert.True(result.Match(_ => false, _ => true));
    }

    [Fact]
    public async Task GetAllWithEither_WhenNoCompanies_ThrowsException()
    {
        var pagination = new PaginationParameters();
        var companies = new PageList<Company>(new List<Company>(), 0, 1, 10);

        _mockCompanyRepo.Setup(r => r.GetAll(pagination, false)).ReturnsAsync(companies);

        var result = await _service.GetAllWithEither(pagination, false).Run();

        Assert.True(result.Match(_ => true, _ => false));
    }

    [Fact]
    public async Task CreateCompanyWithEither_WhenValid_ReturnsCompany()
    {
        var company = new Company { Name = "Valid Company", Address = "123 Main St" };
        _mockCompanyRepo.Setup(r => r.CreateRecord(company)).ReturnsAsync(company);
        _mockRepoManager.Setup(r => r.Save()).Returns(Task.CompletedTask);

        var result = await _service.CreateCompanyWithEither(company).Run();

        Assert.True(result.Match(_ => false, c => c.Name == "Valid Company"));
    }

    [Fact]
    public async Task CreateCompanyWithEither_WhenNameEmpty_ReturnsLeft()
    {
        var company = new Company { Name = "", Address = "123 Main St" };

        var result = await _service.CreateCompanyWithEither(company).Run();

        Assert.True(result.Match(_ => true, _ => false));
    }

    [Fact]
    public async Task CreateCompanyWithEither_WhenAddressEmpty_ReturnsLeft()
    {
        var company = new Company { Name = "Valid Name", Address = "" };

        var result = await _service.CreateCompanyWithEither(company).Run();

        Assert.True(result.Match(_ => true, _ => false));
    }

    [Fact]
    public async Task GetByConditionWithEither_WhenValid_ReturnsCompany()
    {
        var id = Guid.NewGuid().ToString();
        var company = new Company { Id = Guid.Parse(id), Name = "Test" };

        _mockCompanyRepo.Setup(r => r.GetByCondition(id, false)).ReturnsAsync(company);

        var result = await _service.GetByConditionWithEither(id, false).Run();

        Assert.True(result.Match(_ => false, _ => true));
    }

    [Fact]
    public async Task GetByConditionWithEither_WhenIdEmpty_ReturnsLeft()
    {
        var result = await _service.GetByConditionWithEither("", false).Run();

        Assert.True(result.Match(l => l.Contains("id can not be null"), _ => false));
    }

    [Fact]
    public async Task GetByConditionWithEither_WhenIdInvalidGuid_ReturnsLeft()
    {
        var result = await _service.GetByConditionWithEither("invalid-guid", false).Run();

        Assert.True(result.Match(l => l.Contains("not a valid Guid"), _ => false));
    }

    [Fact]
    public async Task GetByConditionWithEither_WhenCompanyNotFound_ReturnsLeft()
    {
        var id = Guid.NewGuid().ToString();
        _mockCompanyRepo.Setup(r => r.GetByCondition(id, false)).Returns(Task.FromResult<Company?>(null));

        var result = await _service.GetByConditionWithEither(id, false).Run();

        Assert.True(result.Match(_ => true, _ => false));
    }

    [Fact]
    public async Task DeleteCompanyWithEither_WhenValid_ReturnsUnit()
    {
        var id = Guid.NewGuid().ToString();
        _mockCompanyRepo.Setup(r => r.DeleteRecord(id, false)).Returns(Task.CompletedTask);
        _mockRepoManager.Setup(r => r.Save()).Returns(Task.CompletedTask);

        var result = await _service.DeleteCompanyWithEither(id, false).Run();

        Assert.True(result.Match(_ => false, _ => true));
    }

    [Fact]
    public async Task DeleteCompanyWithEither_WhenIdInvalid_ReturnsLeft()
    {
        var result = await _service.DeleteCompanyWithEither("", false).Run();

        Assert.True(result.Match(_ => true, _ => false));
    }

    #endregion
}
