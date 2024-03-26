

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
    

    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAll()
    {
        _logger.LogInformation("Getting all the companies.");
        var companies = await _serviceManager.CompanyService.GetAll(trackChanges: false);

        if (companies is null)
        {
            return NotFound();
        }
        
        _logger.LogInformation("Mapping to companiesDtos.");
        //var companiesDtos = _mapper.Map<IEnumerable<CompanyDto>>(companies);

        var companiesDtos = companies.Adapt<IEnumerable<CompanyDto>>();
      
        return Ok(companiesDtos);
    }

    [HttpGet("Collection/{Ids}", Name = "CompanyCollection")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetByIds([ModelBinder(BinderType = typeof(ArrayModelBinder<>))]
        IEnumerable<Guid> Ids)
    {
        var dbcompanies = await _serviceManager.CompanyService.GetByIds(Ids, trackChanges: false);
        var companyDto = dbcompanies.Adapt<CompanyDto>();
        return Ok(companyDto);
    }

    [HttpGet("{Id}", Name = "GetById")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<CompanyDto>> Get(string Id)
    {
        var company = await _serviceManager.CompanyService.GetByCondiction(Id, trackChanges: false);

        if (company is null)
        {
            _logger.LogInformation($"Company with Id: {Id} does not exist in the database.");
            return NotFound($"Company with Id: {Id} does not exist in the database.");
        }

        //var companyDto = _mapper.Map<CompanyDto>(company);
        var companyDto = company.Adapt<CompanyDto>();

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
            _logger.LogInformation("Companies must not be null");
            return BadRequest();
        }

        var dbcompanies = _mapper.Map<IEnumerable<Company>>(companies);

        foreach (var company in dbcompanies)
        {
            await _serviceManager.CompanyService.CreateCompany(company);
        }

        //await _repository.Save();
        await _serviceManager.CompanyService.SaveChanges();

        var dtos = _mapper.Map<IEnumerable<CompanyDto>>(dbcompanies);
        var ids = string.Join(',', dtos.Select(c => c.Id));

        return CreatedAtRoute("CompanyCollection", new { ids }, dtos);

    }
    [HttpPut]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Update(string CompanyId, [FromBody] CompanyUpdateDto model)
    {

        if (model is null) { _logger.LogError("Error: the model can be null"); return BadRequest("Error: the model can be null"); }
       

        var dbEntity = await _serviceManager.CompanyService.GetByCondiction(CompanyId, trackChanges: true);

        if (dbEntity is null) return NotFound();


        // _mapper.Map(model, dbEntity);
        model.Adapt(dbEntity);
        await _serviceManager.CompanyService.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{Id}")]
    public async Task<IActionResult> Delete(string Id)
    {
        await _serviceManager.CompanyService.DeleteCompany(Id, trackChanges:true);
        await _serviceManager.CompanyService.SaveChanges();  
        return NoContent(); 
    }

    
   
}
