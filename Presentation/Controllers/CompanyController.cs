using System.Diagnostics.CodeAnalysis;

namespace Presentation.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces(contentType: "application/json", "application/xml")]
public class CompanyController : ControllerBase
{
    private readonly IServiceManager _serviceManager;
    private readonly IMapper _mapper;
    private readonly ILogger<CompanyController> _logger;
  
  
    public CompanyController(IServiceManager serviceManager, IMapper mapper, ILogger<CompanyController> logger)
    {
        _serviceManager = serviceManager;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet(Name = "GetCompanies")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAll([FromQuery] PaginationParameters pagination)
    {
        _logger.LogInformation("Getting all the companies.");
        var dbEntities = await _serviceManager.CompanyService.GetAll(pagination, false);

        _logger.LogInformation("Sending the request headers information.");
        var queryable = dbEntities.companies.AsQueryable();
        var metaData = dbEntities.metaData;
       
        HttpContext.HeadersPaginationParametersInsert(queryable, metaData);

        if (!dbEntities.companies.Any()) return NotFound();


        _logger.LogInformation("Mapping to companiesDtos.");
        var companiesDto = dbEntities.companies.Adapt<IEnumerable<CompanyDto>>();

        _logger.LogInformation("Returning the result to the client.");
        return Ok(companiesDto);
    }

    [HttpGet("Collection/{Ids}", Name = "CompanyCollection")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetByIds([ModelBinder(BinderType = typeof(ArrayModelBinder<>))]
        IEnumerable<Guid> ids)
    {
        _logger.LogInformation("Getting all the companies by ids.");
        var companies = await _serviceManager.CompanyService.GetByIds(ids, trackChanges: false);
       
        _logger.LogInformation("Mapping to companies dtos.");
        var companiesDto = companies.Adapt<IEnumerable<CompanyDto>>();

        _logger.LogInformation("Returning the result to the client.");
        return Ok(companiesDto);
    }
    
    

    [HttpGet("{Id}", Name = "GetById")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<CompanyDto>> Get(string Id)
    {
        _logger.LogInformation("Getting all the companies by id.");
        var company = await _serviceManager.CompanyService.GetByCondiction(Id, false);

        _logger.LogInformation("Verify if the company exist.");
        if (company is null)
        {
            _logger.LogInformation($"Company with Id: {Id} does not exist in the database.");
            return NotFound($"Company with Id: {Id} does not exist in the database.");
        }

        //var companyDto = _mapper.Map<CompanyDto>(company);
        _logger.LogInformation("Mapping to the dto.");
        var companyDto = company.Adapt<CompanyDto>();

        _logger.LogInformation("Returning the result to the client.");
        return Ok(companyDto);
    }

    [HttpPost(Name = "CreateCompany")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<CompanyDto>> Create([FromBody] CompanyCreateDto model)
    {
        if (model is null)
            return BadRequest();

        var dbEntity = _mapper.Map<Company>(model);

        await _serviceManager.CompanyService.CreateCompany(dbEntity);
        await _serviceManager.CompanyService.SaveChanges();

        var dto = _mapper.Map<CompanyDto>(dbEntity);
        return CreatedAtRoute("GetById", new { Id = dto.Id }, dto);
    }
    [HttpPost("Collection")]
    public async Task<ActionResult<CompanyDto>> CreateCompanyCollection([FromBody] IEnumerable<CompanyCreateDto> companies)
    {
        if (companies is null)
        {
            _logger.LogError("Companies must not be null");
            return BadRequest();
        }

        var dbcompanies = _mapper.Map<IEnumerable<Company>>(companies);

        foreach (var company in dbcompanies)
        {
            await _serviceManager.CompanyService.CreateCompany(company);
        }
        
        await _serviceManager.CompanyService.SaveChanges();
        
        var dtos = dbcompanies.Adapt<IEnumerable<CompanyDto>>();
        var ids = string.Join(',', dtos.Select(c => c.Id));

        return CreatedAtRoute("CompanyCollection", new { ids }, dtos);

    }
    [HttpPut]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Update(string companyId, [FromBody] CompanyUpdateDto model)
    {

        if (model is null) { _logger.LogError("Error: the model can be null"); return BadRequest("Error: the model can be null"); }
       

        var dbEntity = await _serviceManager.CompanyService.GetByCondiction(companyId, trackChanges: true);

        if (dbEntity is null) return NotFound();


        // _mapper.Map(model, dbEntity);
        model.Adapt(dbEntity);
        await _serviceManager.CompanyService.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{Id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _serviceManager.CompanyService.DeleteCompany(id, trackChanges:true);
        await _serviceManager.CompanyService.SaveChanges();  
        return NoContent(); 
    }

    
   
}
