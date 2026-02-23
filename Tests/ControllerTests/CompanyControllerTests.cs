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

public class CompanyControllerTests
{
    private readonly Mock<IServiceManager> _mockServiceManager;
    private readonly Mock<ICompanyService> _mockCompanyService;
    private readonly Mock<ILogger<CompanyController>> _mockLogger;
    private readonly Mock<INexusEngine> _mockNexus;
    private readonly CompanyController _controller;

    public CompanyControllerTests()
    {
        _mockServiceManager = new Mock<IServiceManager>();
        _mockCompanyService = new Mock<ICompanyService>();
        _mockLogger = new Mock<ILogger<CompanyController>>();
        _mockNexus = new Mock<INexusEngine>();

        _mockServiceManager.Setup(s => s.CompanyService).Returns(_mockCompanyService.Object);
        _mockServiceManager.Setup(s => s.Save()).Returns(Task.CompletedTask);

        _controller = new CompanyController(
            _mockServiceManager.Object,
            _mockLogger.Object,
            _mockNexus.Object
        );

        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    #region GetAllWithEither Tests

    [Fact]
    public async Task GetAllWithEither_WhenCacheExists_ReturnsCachedData()
    {
        var pagination = new PaginationParameters();
        var cachedCompanies = new List<CompanyDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Cached Company" }
        };

        _mockNexus
            .Setup(n => n.GetAsync<IEnumerable<CompanyDto>>(It.IsAny<string>()))
            .ReturnsAsync(cachedCompanies);

        var result = await _controller.GetAllWithEither(pagination);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        _mockCompanyService.Verify(s => s.GetAllWithEither(It.IsAny<PaginationParameters>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task GetAllWithEither_WhenNoCacheAndCompaniesExist_ReturnsOk()
    {
        var pagination = new PaginationParameters();
        var companies = new List<Company>
        {
            new() { Id = Guid.NewGuid(), Name = "Company 1" }
        };
        var metaData = new MetaData { CurrentPage = 1, TotalPages = 1, PageSize = 10, TotalCount = 1 };

        _mockNexus
            .Setup(n => n.GetAsync<IEnumerable<CompanyDto>>(It.IsAny<string>()))
            .ReturnsAsync((IEnumerable<CompanyDto>?)null);

        _mockCompanyService
            .Setup(s => s.GetAllWithEither(pagination, false))
            .Returns(EitherAsync<string, (IEnumerable<Company> companies, MetaData metaData)>
                .FromRight((companies, metaData)));

        var result = await _controller.GetAllWithEither(pagination);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetAllWithEither_WhenError_ReturnsNotFound()
    {
        var pagination = new PaginationParameters();

        _mockNexus
            .Setup(n => n.GetAsync<IEnumerable<CompanyDto>>(It.IsAny<string>()))
            .ReturnsAsync((IEnumerable<CompanyDto>?)null);

        _mockCompanyService
            .Setup(s => s.GetAllWithEither(pagination, false))
            .Returns(EitherAsync<string, (IEnumerable<Company> companies, MetaData metaData)>
                .FromLeft("Companies not found"));

        var result = await _controller.GetAllWithEither(pagination);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    #endregion

    #region GetByIdWithEither Tests

    [Fact]
    public async Task GetByIdWithEither_WhenCompanyExists_ReturnsOk()
    {
        var id = Guid.NewGuid().ToString();
        var company = new Company { Id = Guid.Parse(id), Name = "Test Company" };

        _mockCompanyService
            .Setup(s => s.GetByConditionWithEither(id, false))
            .Returns(EitherAsync<string, Company>.FromRight(company));

        var result = await _controller.GetByIdWithEither(id);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetByIdWithEither_WhenNotFound_ReturnsNotFound()
    {
        var id = Guid.NewGuid().ToString();

        _mockCompanyService
            .Setup(s => s.GetByConditionWithEither(id, false))
            .Returns(EitherAsync<string, Company>.FromLeft("Company not found"));

        var result = await _controller.GetByIdWithEither(id);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetByIdWithEither_WhenInvalidId_ReturnsBadRequest()
    {
        var id = "invalid-guid";

        _mockCompanyService
            .Setup(s => s.GetByConditionWithEither(id, false))
            .Returns(EitherAsync<string, Company>.FromLeft("not a valid Guid"));

        var result = await _controller.GetByIdWithEither(id);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    #endregion

    #region GetCompaniesCollectionWithEitherByIds Tests

    [Fact]
    public async Task GetCompaniesCollectionWithEitherByIds_WhenCompaniesExist_ReturnsOk()
    {
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var companies = new List<Company>
        {
            new() { Id = ids[0], Name = "Company 1" },
            new() { Id = ids[1], Name = "Company 2" }
        };

        _mockCompanyService
            .Setup(s => s.GetByIdsWithEither(ids, false))
            .Returns(EitherAsync<string, IEnumerable<Company>>.FromRight(companies));

        var result = await _controller.GetCompaniesCollectionWithEitherByIds(ids);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetCompaniesCollectionWithEitherByIds_WhenNotFound_ReturnsNotFound()
    {
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        _mockCompanyService
            .Setup(s => s.GetByIdsWithEither(ids, false))
            .Returns(EitherAsync<string, IEnumerable<Company>>.FromLeft("Companies not found"));

        var result = await _controller.GetCompaniesCollectionWithEitherByIds(ids);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    #endregion

    #region CreateEither Tests

    [Fact]
    public async Task CreateEither_WhenValid_ReturnsCreatedAtRoute()
    {
        var createDto = new CompanyCreateDto { Name = "New Company", Address = "New Address" };
        var createdCompany = new Company { Id = Guid.NewGuid(), Name = "New Company", Address = "New Address" };

        _mockCompanyService
            .Setup(s => s.CreateCompanyWithEither(It.IsAny<Company>()))
            .Returns(EitherAsync<string, Company>.FromRight(createdCompany));

        var result = await _controller.CreateEither(createDto);

        var createdResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
        Assert.Equal("GetCompanyById", createdResult.RouteName);
    }

    [Fact]
    public async Task CreateEither_WhenInvalid_ReturnsBadRequest()
    {
        var createDto = new CompanyCreateDto { Name = "", Address = "" };

        _mockCompanyService
            .Setup(s => s.CreateCompanyWithEither(It.IsAny<Company>()))
            .Returns(EitherAsync<string, Company>.FromLeft("Name is required"));

        var result = await _controller.CreateEither(createDto);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    #endregion

    #region DeleteEither Tests

    [Fact]
    public async Task DeleteEither_WhenValid_ReturnsNoContent()
    {
        var id = Guid.NewGuid().ToString();

        _mockCompanyService
            .Setup(s => s.DeleteCompanyWithEither(id, false))
            .Returns(EitherAsync<string, Unit>.FromRight(new Unit()));

        var result = await _controller.DeleteEither(id);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteEither_WhenNotFound_ReturnsNotFound()
    {
        var id = Guid.NewGuid().ToString();

        _mockCompanyService
            .Setup(s => s.DeleteCompanyWithEither(id, false))
            .Returns(EitherAsync<string, Unit>.FromLeft("Company not found"));

        var result = await _controller.DeleteEither(id);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    #endregion
}
