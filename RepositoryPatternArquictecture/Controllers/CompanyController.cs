using AutoMapper;
using Contracts.Interfaces;
using Dtos.DtoModels;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using RepositoryPatternArquitecture.ModelBinders;

namespace RepositoryPatternArquitecture.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Produces("application/json")]
public class CompanyController : ControllerBase
{
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<CompanyController> _logger;
    public CompanyController(IRepositoryManager repository, IMapper mapper, ILogger<CompanyController> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet(Name = "GetCompanies")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]

    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAll()
    {
        _logger.LogInformation("Getting all the companies.");
        var companies = await _repository.Company.GetAll(trackChanges: false);

        if (companies is null)
            return NotFound();

        _logger.LogInformation("Mapping to companiesDtos.");
        var companiesDtos = _mapper.Map<IEnumerable<CompanyDto>>(companies);
        //Proyection with select 
        /* var companiesDtos = companies.Select(c => new CompanyDto
         {
             Id = c.Id,
             Name = c.Name,
             FullAddress = string.Join(' ',c.Address, c.Country)
         }).ToList();*/

        return Ok(companiesDtos);
    }

    [HttpGet("Collection/{Ids}", Name = "CompanyCollection")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetByIds([ModelBinder(BinderType = typeof(ArrayModelBinder))]IEnumerable<Guid> Ids)
    {
        var dbcompanies = await _repository.Company.GetByIds(Ids, trackChanges: false);

        var dtos = _mapper.Map<IEnumerable<CompanyDto>>(dbcompanies);

        return Ok(dtos);
    }

    [HttpGet("{Id}", Name = "GetById")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<CompanyDto>> Get(string Id)
    {
        var company = await _repository.Company.GetByCondiction(Id, trackChanges: false);

        if (company is null)
        {
            _logger.LogInformation($"Company with Id: {Id} does not exist in the database.");
            return NotFound($"Company with Id: {Id} does not exist in the database.");
        }


        var companyDto = _mapper.Map<CompanyDto>(company);

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

        await _repository.Company.CreateCompany(dbEntity);
        await _repository.Save();

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
            await _repository.Company.CreateCompany(company);
        }

        await _repository.Save();

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
        if (model is null)
        {
            return BadRequest();
        }

        var dbEntity = await _repository.Company.GetByCondiction(CompanyId, trackChanges: true);
        if (dbEntity is null)
        { 
            return NotFound();
        }

        _mapper.Map(model, dbEntity);
        await _repository.Save();

        return NoContent();
    }

    [HttpDelete("{Id}")]
    public async Task<IActionResult> Delete(string Id)
    {
        await _repository.Company.DeleteCompany(Id, trackChanges:true);
        await _repository.Save();

        return NoContent(); 
    }

    
   
}
