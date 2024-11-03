using Entities.Exceptions;

namespace Presentation.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces(contentType: "application/json", "application/xml")]
public class CompanyController(IServiceManager serviceManager, IMapper mapper, ILogger<CompanyController> logger)
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

    [HttpGet("Collection/{ids}", Name = "CompanyCollection")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetByIds([ModelBinder(BinderType = typeof(ArrayModelBinder<>))]
        IEnumerable<Guid> ids)
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

        logger.LogInformation("Verify if the company exist.");
        if (company is null)
        {
            logger.LogInformation($"Company with Id: {id} does not exist in the database.");
            throw new CompanyNotFoundException(Guid.Parse(id));
        }
        
        logger.LogInformation("Mapping to the dto.");
        var companyDto = company.Adapt<CompanyDto>();

        logger.LogInformation("Returning the result to the client.");
        return Ok(companyDto);
    }

    [HttpPost(Name = "CreateCompany")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<CompanyDto>> Create([FromBody] CompanyCreateDto? model)
    {
        if (model is null || string.IsNullOrWhiteSpace(model.Name?.Trim()) || string.IsNullOrEmpty(model.Name?.Trim()))
            throw new CompanyBadRequestException();
        
        var dbEntity = mapper.Map<Company>(model);

        await serviceManager.CompanyService.CreateCompany(dbEntity);
        await serviceManager.CompanyService.SaveChanges();

        var dto = mapper.Map<CompanyDto>(dbEntity);
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

        var dbcompanies = mapper.Map<IEnumerable<Company>>(companies);

        foreach (var company in dbcompanies)
        {
            await serviceManager.CompanyService.CreateCompany(company);
        }
        
        await serviceManager.CompanyService.SaveChanges();
        
        var companiesDto = dbcompanies.Adapt<IEnumerable<CompanyDto>>();
        var ids = string.Join(',', companiesDto.Select(c => c.Id));

        return CreatedAtRoute("CompanyCollection", new { ids }, companiesDto);

    }
    [HttpPut]
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
        
        if (companyExist is null)
            throw new CompanyNotFoundException(Guid.Parse(id));
        
        await serviceManager.CompanyService.DeleteCompany(id, trackChanges:true);
        await serviceManager.CompanyService.SaveChanges();  
        return NoContent(); 
    }

    
   
}
