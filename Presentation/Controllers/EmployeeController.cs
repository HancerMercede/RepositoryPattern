using Entities.Exceptions;
using Microsoft.AspNetCore.JsonPatch;

namespace Presentation.Controllers;

[Route("api/v{version:apiVersion}/company/{CompanyId}/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json", "application/xml")]
public class EmployeeController : ControllerBase
{
    private readonly IServiceManager _service;
    private readonly IMapper _mapper;
    private readonly ILogger<EmployeeController> _logger;
    
    public EmployeeController(IServiceManager service, IMapper mapper, ILogger<EmployeeController> logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll(string companyId,[FromQuery] PaginationParameters paginationParameters)
    {

        var existCompany = await _service.CompanyService.GetByCondition(companyId, trackChanges: false);
        
        if (existCompany is null)
        {
            _logger.LogInformation($"The company with Id: {companyId} does not exist in the database.");
            throw new CompanyNotFoundException(Guid.Parse(companyId));
        }
        _logger.LogInformation($"Getting all the employees for company Id {companyId}.");
        var employees = await _service.EmployeeService.GetAll(companyId, paginationParameters, trackChanges: false);

        _logger.LogInformation("Adding the information to request headers.");
        var queryable = employees.Employees.AsQueryable();
        var metaData = employees.metaData;
        HttpContext.HeadersPaginationParametersInsert(queryable, metaData);

        if (employees.Employees is null)
        {
            _logger.LogInformation("There is no employees for this company.");
            return NotFound();
        }


        _logger.LogInformation("Mapping to employeesDtos.");
        var employeesDtos = employees.Employees.Adapt<IEnumerable<EmployeeToShowToClientDto>>();

        return Ok(employeesDtos);
    }

    [HttpGet("{id}", Name = "GetEmployeeForCompany")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<EmployeeDto>> Get(string companyId, string id)
    {
        var existCompany = await _service.CompanyService.GetByCondition(companyId, trackChanges: false);

        if (existCompany is null)
        {
            _logger.LogInformation($"The company with Id: {companyId} does not exist in the database.");
            throw new CompanyNotFoundException(Guid.Parse(companyId));
        }

        var employee = await _service.EmployeeService.GetByCondition(companyId, id, trackChanges: false);

        if (employee is null)
        {
            _logger.LogInformation($"The employee with the Id:{id} does not exist in the database.");
            throw new EmployeeNotFoundException(Guid.Parse(id));
        }

        var employeeDto = employee.Adapt<EmployeeToShowToClientDto>();

        return Ok(employeeDto);
    }

    [HttpPost(Name = "CreateEmployeeForCompany")]
    [ProducesResponseType(201)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<EmployeeDto>> Create(string companyId, [FromBody] EmployeeCreateDto model)
    {
        if (model is null)
        {
            _logger.LogInformation($"The object {typeof(EmployeeCreateDto)} is null.");
            return BadRequest("EmployeeCreateDto object is null.");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogInformation($"Invalid model state for object {typeof(EmployeeCreateDto)}.");
            return UnprocessableEntity(ModelState);
        }

        var existcompany = await _service.CompanyService.GetByCondition(companyId, trackChanges: false);

        if (existcompany is null)
        {
            _logger.LogInformation($"The company with Id: {companyId} does not exist in the database.");
            throw new CompanyNotFoundException(Guid.Parse(companyId));
        }

        var dbEntity = _mapper.Map<Employee>(model);

        await _service.EmployeeService.CreateEmployee(companyId, dbEntity);
        await _service.EmployeeService.SaveChanges();

        var dto = dbEntity.Adapt<EmployeeDto>();

        return CreatedAtRoute("GetEmployeeForCompany", new { companyId = dto.CompanyId, id = dto.Id }, dto);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(string companyId, string id)
    {
        var existcompany = await _service.CompanyService.GetByCondition(companyId, trackChanges: false);

        if (existcompany is null)
        {
            _logger.LogInformation($"The company with Id: {companyId} does not exist in the database.");
            throw new CompanyNotFoundException(Guid.Parse(companyId));
        }
         var employeeExist = await _service.EmployeeService.GetByCondition(companyId, id, trackChanges: false);

         if (employeeExist is null)
         {
             _logger.LogInformation($"The employee with Id: {companyId} does not exist in the database.");
             throw new EmployeeNotFoundException(Guid.Parse(id));
         }
         
         _logger.LogInformation($"Deleting the employee for company {companyId}");
        await _service.EmployeeService.DeleteEmployee(companyId, id, trackChanges: true);
        await _service.EmployeeService.SaveChanges();

        return NoContent();
    }

    [HttpPut("{Id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Update(string companyId, string id, [FromBody] EmployeeUpdateDto model)
    {
        if (model is null)
        {
            _logger.LogInformation("The model can not be null");
            return BadRequest("The model can not be null");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogInformation($"Invalid model state for object {typeof(EmployeeCreateDto)}.");
            return UnprocessableEntity(ModelState);
        }

        var existcompany = await _service.CompanyService.GetByCondition(companyId, trackChanges: false);
        if (existcompany is null)
        {
            _logger.LogInformation($"The company with Id: {companyId} does not exist in the database.");
            throw new CompanyNotFoundException(Guid.Parse(companyId));
        }

        // Modelo conectado aqui para que ef pueda seguir los cambios que recibe la entidad
        var dbEntity = await _service.EmployeeService.GetByCondition(companyId, id, trackChanges: true);
        if (dbEntity is null)
        {
            _logger.LogInformation($"The employee with Id: {id} does not exist in the database.");
            return BadRequest($"The employee with Id: {id} does not exist in the database.");
        }
        // Seteo el track para que ef me cambie el estado de la entidad a modified
        // _context.Entry<T>().State = EntityState.Modified
        //_mapper.Map(model, dbEntity);
        model.Adapt(dbEntity);
        await _service.EmployeeService.SaveChanges();

        return NoContent();
    }

    [HttpPatch("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Path(string companyId, string id, [FromBody] JsonPatchDocument<EmployeeUpdateDto> pacthDoc)
    {

        if (pacthDoc is null)
        {
            _logger.LogInformation("The model can not be null");
            return BadRequest("The model can not be null");
        }

        var existcompany = await _service.CompanyService.GetByCondition(companyId, trackChanges: false);
        if (existcompany is null)
        {
            _logger.LogInformation($"The company with Id:{companyId} does not exist.");
            throw new CompanyNotFoundException(Guid.Parse(companyId));
        }
        // Here I have to track the entity to change the state to modified and being able to save the changes.
        var employeeDb = await _service.EmployeeService.GetByCondition(companyId, id, trackChanges: true);
        if (employeeDb is null)
        {
            _logger.LogInformation($"The employee with Id:{id} does not exist in the database.");
            return NotFound($"The employee with Id:{id} does not exist in the database.");
        }

        var employeeToPath = _mapper.Map<EmployeeUpdateDto>(employeeDb);

        pacthDoc.ApplyTo(employeeToPath, (Microsoft.AspNetCore.JsonPatch.Adapters.IObjectAdapter) ModelState);

        TryValidateModel(employeeToPath);

        if (!ModelState.IsValid)
        {
            _logger.LogInformation("Invalid model state for patch document.");
            return UnprocessableEntity(ModelState);
        }

        _mapper.Map(employeeToPath, employeeDb);

        await _service.EmployeeService.SaveChanges();

        return NoContent();
    }
}
