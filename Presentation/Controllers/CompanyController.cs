namespace Presentation.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces(contentType: "application/json", "application/xml")]
public class CompanyController(IServiceManager serviceManager, ILogger<CompanyController> logger)
    : ControllerBase
{
    [HttpGet(Name = "GetCompanies")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAll([FromQuery] PaginationParameters pagination)
    {
        logger.LogInformation("Getting all the companies.");
        var dbEntities = await serviceManager.CompanyService.GetAll(pagination, false);

        logger.LogInformation("Sending the request headers information.");
        var queryable = dbEntities.companies.AsQueryable();
        var metaData = dbEntities.metaData;
       
        HttpContext.HeadersPaginationParametersInsert(queryable, metaData);

        if (!dbEntities.companies.Any()) return NotFound("No companies found.");
        
        logger.LogInformation("Mapping to companiesDto.");
        var companiesDto = dbEntities.companies.Adapt<IEnumerable<CompanyDto>>();

        logger.LogInformation("Returning the result to the client.");
        return Ok(companiesDto);
    }

    [HttpGet("collection/({ids})", Name = "CompanyCollection")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetByIds([ModelBinder(BinderType = typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
    {
        logger.LogInformation("Getting all the companies by ids.");
        var companies = await serviceManager.CompanyService.GetByIds(ids, trackChanges: false);
       
        logger.LogInformation("Mapping to companies Dto.");
        var companiesDto = companies.Adapt<IEnumerable<CompanyDto>>();

        logger.LogInformation("Returning the result to the client.");
        return Ok(companiesDto);
    }
    
    [HttpGet("{id}", Name = "GetById")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<CompanyDto>> Get(string id)
    {
        logger.LogInformation("Getting the company by id.");
        var company = await serviceManager.CompanyService.GetByCondition(id, trackChanges:false);

        logger.LogInformation("Mapping to the dto.");
        var companyDto = company.Adapt<CompanyDto>();

        logger.LogInformation("Returning the result to the client.");
        return Ok(companyDto);
    }

    [HttpPost(Name = "CreateCompany")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<CompanyDto>> Create([FromBody] CompanyCreateDto model)
    {
        if (!ModelState.IsValid)
        {
            logger.LogInformation($"Invalid model state for object {typeof(CompanyCreateDto)}");
            return UnprocessableEntity(ModelState);
        }

        var dbEntity = model.Adapt<Company>();

        await serviceManager.CompanyService.CreateCompany(dbEntity);
        await serviceManager.CompanyService.SaveChanges();

        var dto = dbEntity.Adapt<CompanyDto>();
        return CreatedAtRoute("GetById", new { id = dto.Id }, dto);
    }
    
    [HttpPost("Collection")]
    public async Task<ActionResult<CompanyDto>> CreateCompanyCollection([FromBody] IEnumerable<CompanyCreateDto>? companies)
    {
        if (companies is null)
        {
            logger.LogError("Companies must not be null");
            throw new CompanyBadRequestException();
        }

        var dbCompanies = companies.Adapt<IEnumerable<Company>>();

        foreach (var company in dbCompanies)
        {
            await serviceManager.CompanyService.CreateCompany(company);
        }
        
        await serviceManager.CompanyService.SaveChanges();
        
        var companiesDto = dbCompanies.Adapt<IEnumerable<CompanyDto>>();
        var ids = string.Join(',', companiesDto.Select(c => c.Id));

        return CreatedAtRoute("CompanyCollection", new { ids }, companiesDto);

    }
    [HttpPut("{companyId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Update(string companyId, [FromBody] CompanyUpdateDto? model)
    {
        if (model is null) { logger.LogError("Error: the model can be null"); throw new CompanyBadRequestException(); }
        
        var dbEntity = await serviceManager.CompanyService.GetByCondition(companyId, trackChanges: true);

        if (dbEntity is null) return NotFound();
        
        model.Adapt(dbEntity);
        await serviceManager.CompanyService.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{Id}")]
    public async Task<IActionResult> Delete(string id)
    {   var companyExist = await serviceManager.CompanyService.GetByCondition(id, trackChanges: false);
        
        await serviceManager.CompanyService.DeleteCompany(id, trackChanges:true);
        await serviceManager.CompanyService.SaveChanges();  
        return NoContent(); 
    }

    
   
}


