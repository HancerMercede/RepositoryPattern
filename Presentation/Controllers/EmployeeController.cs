using Entities.Exceptions;
using Microsoft.AspNetCore.JsonPatch;

namespace Presentation.Controllers;

[Route("api/v{version:apiVersion}/company/{CompanyId}/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json", "application/xml")]
public class EmployeeController(IServiceManager serviceManager, IMapper mapper, ILogger<EmployeeController> logger)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll(string companyId,[FromQuery] PaginationParameters paginationParameters)
    {

        var existCompany = await serviceManager.CompanyService.GetByCondition(companyId, trackChanges: false);
        
        if (existCompany is null)
        {
            logger.LogInformation($"The company with Id: {companyId} does not exist in the database.");
            throw new CompanyNotFoundException(Guid.Parse(companyId));
        }
        logger.LogInformation($"Getting all the employees for company Id {companyId}.");
        var employees = await serviceManager.EmployeeService.GetAll(companyId, paginationParameters, trackChanges: false);

        logger.LogInformation("Adding the information to request headers.");
        var queryable = employees.Employees.AsQueryable();
        var metaData = employees.metaData;
        HttpContext.HeadersPaginationParametersInsert(queryable, metaData);

        if (employees.Employees is null)
        {
            logger.LogInformation("There is no employees for this company.");
            return NotFound();
        }


        logger.LogInformation("Mapping to employeesDtos.");
        var employeesDtos = employees.Employees.Adapt<IEnumerable<EmployeeToShowToClientDto>>();

        return Ok(employeesDtos);
    }

    [HttpGet("{id}", Name = "GetEmployeeForCompany")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<EmployeeDto>> Get(string companyId, string id)
    {
        var existCompany = await serviceManager.CompanyService.GetByCondition(companyId, trackChanges: false);

        if (existCompany is null)
        {
            logger.LogInformation($"The company with Id: {companyId} does not exist in the database.");
            throw new CompanyNotFoundException(Guid.Parse(companyId));
        }

        var employee = await serviceManager.EmployeeService.GetByCondition(companyId, id, trackChanges: false);

        if (employee is null)
        {
            logger.LogInformation($"The employee with the Id:{id} does not exist in the database.");
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
            logger.LogInformation($"The object {typeof(EmployeeCreateDto)} is null.");
            throw new EmployeeBadRequestException();
        }

        if (!ModelState.IsValid)
        {
            logger.LogInformation($"Invalid model state for object {typeof(EmployeeCreateDto)}.");
            return UnprocessableEntity(ModelState);
        }

        var existCompany = await serviceManager.CompanyService.GetByCondition(companyId, trackChanges: false);

        if (existCompany is null)
        {
            logger.LogInformation($"The company with Id: {companyId} does not exist in the database.");
            throw new CompanyNotFoundException(Guid.Parse(companyId));
        }

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
        var existcompany = await serviceManager.CompanyService.GetByCondition(companyId, trackChanges: false);

        if (existcompany is null)
        {
            logger.LogInformation($"The company with Id: {companyId} does not exist in the database.");
            throw new CompanyNotFoundException(Guid.Parse(companyId));
        }
         var employeeExist = await serviceManager.EmployeeService.GetByCondition(companyId, id, trackChanges: false);

         if (employeeExist is null)
         {
             logger.LogInformation($"The employee with Id: {companyId} does not exist in the database.");
             throw new EmployeeNotFoundException(Guid.Parse(id));
         }
         
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
        if (model is null)
        {
            logger.LogInformation("The model can not be null");
            return BadRequest("The model can not be null");
        }

        if (!ModelState.IsValid)
        {
            logger.LogInformation($"Invalid model state for object {typeof(EmployeeCreateDto)}.");
            return UnprocessableEntity(ModelState);
        }

        var existcompany = await serviceManager.CompanyService.GetByCondition(companyId, trackChanges: false);
        if (existcompany is null)
        {
            logger.LogInformation($"The company with Id: {companyId} does not exist in the database.");
            throw new CompanyNotFoundException(Guid.Parse(companyId));
        }

        // Modelo conectado aqui para que ef pueda seguir los cambios que recibe la entidad
        var dbEntity = await serviceManager.EmployeeService.GetByCondition(companyId, id, trackChanges: true);
        if (dbEntity is null)
        {
            logger.LogInformation($"The employee with Id: {id} does not exist in the database.");
            return BadRequest($"The employee with Id: {id} does not exist in the database.");
        }
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

        var existcompany = await serviceManager.CompanyService.GetByCondition(companyId, trackChanges: false);
        if (existcompany is null)
        {
            logger.LogInformation($"The company with Id:{companyId} does not exist.");
            throw new CompanyNotFoundException(Guid.Parse(companyId));
        }
        // Here I have to track the entity to change the state to modified and being able to save the changes.
        var employeeDb = await serviceManager.EmployeeService.GetByCondition(companyId, id, trackChanges: true);
        if (employeeDb is null)
        {
            logger.LogInformation($"The employee with Id:{id} does not exist in the database.");
            return NotFound($"The employee with Id:{id} does not exist in the database.");
        }

        var employeeToPath = mapper.Map<EmployeeUpdateDto>(employeeDb);

        pacthDoc.ApplyTo(employeeToPath, (Microsoft.AspNetCore.JsonPatch.Adapters.IObjectAdapter) ModelState);

        TryValidateModel(employeeToPath);

        if (!ModelState.IsValid)
        {
            logger.LogInformation("Invalid model state for patch document.");
            return UnprocessableEntity(ModelState);
        }

        mapper.Map(employeeToPath, employeeDb);

        await serviceManager.EmployeeService.SaveChanges();

        return NoContent();
    }
}
