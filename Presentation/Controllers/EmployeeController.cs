namespace Presentation.Controllers;

[Route("api/v{version:apiVersion}/company/{CompanyId}/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json", "application/xml")]
public class EmployeeController(IServiceManager serviceManager, ILogger<EmployeeController> logger)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll(string companyId,[FromQuery] PaginationParameters paginationParameters)
    {
        await serviceManager.CompanyService.GetByCondition(companyId, trackChanges: false);
        
        logger.LogInformation($"Getting all the employees for company Id {companyId}.");
        var employees = await serviceManager.EmployeeService.GetAll(companyId, paginationParameters, trackChanges: false);

        logger.LogInformation("Adding the information to request headers.");
        var queryable = employees.Employees.AsQueryable();
        var metaData = employees.metaData;
        
        HttpContext.HeadersPaginationParametersInsert(queryable, metaData);
        
        logger.LogInformation("Mapping to employeesDto.");
        var employeesDto = employees.Employees.Adapt<IEnumerable<EmployeeToShowToClientDto>>();

        return Ok(employeesDto);
    }

    [HttpGet("{id}", Name = "GetEmployeeForCompany")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<EmployeeDto>> Get(string companyId, string id)
    {
        await serviceManager.CompanyService.GetByCondition(companyId, trackChanges: false);
        
        var employee = await serviceManager.EmployeeService.GetByCondition(companyId, id, trackChanges: false);

        var employeeDto = employee.Adapt<EmployeeToShowToClientDto>();

        return Ok(employeeDto);
    }

    [HttpPost(Name = "CreateEmployeeForCompany")]
    [ProducesResponseType(201)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<EmployeeDto>> Create(string companyId, [FromBody] EmployeeCreateDto model)
    {
        if (!ModelState.IsValid)
        {
            logger.LogInformation($"Invalid model state for object {typeof(EmployeeCreateDto)}.");
            return UnprocessableEntity(ModelState);
        }

        await serviceManager.CompanyService.GetByCondition(companyId, trackChanges: false);
        
        var dbEntity = model.Adapt<Employee>();

        await serviceManager.EmployeeService.CreateEmployee(companyId, dbEntity);
        await serviceManager.EmployeeService.SaveChanges();

        var dto = dbEntity.Adapt<EmployeeDto>();

        return CreatedAtRoute("GetEmployeeForCompany", new { companyId = dto.CompanyId, id = dto.Id }, dto);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(string companyId, string id)
    {
         await serviceManager.CompanyService.GetByCondition(companyId, trackChanges: false);
        
         await serviceManager.EmployeeService.GetByCondition(companyId, id, trackChanges: false);
         
         logger.LogInformation($"Deleting the employee for company {companyId}");
         await serviceManager.EmployeeService.DeleteEmployee(companyId, id, trackChanges: true);
         await serviceManager.EmployeeService.SaveChanges();

         return NoContent();
    }

    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Update(string companyId, string id, [FromBody] EmployeeUpdateDto model)
    {
        if (!ModelState.IsValid)
        {
            logger.LogInformation($"Invalid model state for object {typeof(EmployeeCreateDto)}.");
            return UnprocessableEntity(ModelState);
        }

        await serviceManager.CompanyService.GetByCondition(companyId, trackChanges: false);
        
        // Modelo conectado aqui para que ef pueda seguir los cambios que recibe la entidad
        var dbEntity = await serviceManager.EmployeeService.GetByCondition(companyId, id, trackChanges: true);
        
        // Seteo el track para que ef me cambie el estado de la entidad a modified
        // _context.Entry<T>().State = EntityState.Modified
        //_mapper.Map(model, dbEntity);
        model.Adapt(dbEntity);
        await serviceManager.EmployeeService.SaveChanges();

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
            logger.LogInformation("The model can not be null");
            return BadRequest("The model can not be null");
        }

        await serviceManager.CompanyService.GetByCondition(companyId, trackChanges: false);
     
        // Here I have to track the entity to change the state to modified and being able to save the changes.
        var employeeDb = await serviceManager.EmployeeService.GetByCondition(companyId, id, trackChanges: true);
        
        var employeeToPath = employeeDb.Adapt<EmployeeUpdateDto>();

        pacthDoc.ApplyTo(employeeToPath, (IObjectAdapter) ModelState);

        TryValidateModel(employeeToPath);

        if (!ModelState.IsValid)
        {
            logger.LogInformation("Invalid model state for patch document.");
            return UnprocessableEntity(ModelState);
        }
        
        employeeToPath.Adapt(employeeDb);
        
        await serviceManager.EmployeeService.SaveChanges();

        return NoContent();
    }
}
