namespace Presentation.Controllers;

[Route("api/v{version:apiVersion}/company/{CompanyId}/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json", "application/xml")]
public class EmployeeController(IServiceManager serviceManager, ILogger<EmployeeController> logger)
    : ControllerBase
{
    #region  Conventional Implementation without Either
    // [HttpGet]
    // [ProducesResponseType(200)]
    // [ProducesResponseType(404)]
    // [ProducesResponseType(403)]
    // public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll(string companyId,[FromQuery] PaginationParameters paginationParameters)
    // {
    //     await serviceManager.CompanyService.GetByCondition(companyId, trackChanges: false);
    //     
    //     logger.LogInformation($"Getting all the employees for company Id {companyId}.");
    //     var employees = await serviceManager.EmployeeService.GetAll(companyId, paginationParameters, trackChanges: false);
    //
    //     logger.LogInformation("Adding the information to request headers.");
    //     var queryable = employees.Employees.AsQueryable();
    //     var metaData = employees.metaData;
    //     
    //     HttpContext.HeadersPaginationParametersInsert(queryable, metaData);
    //     
    //     logger.LogInformation("Mapping to employeesDto.");
    //     var employeesDto = employees.Employees.Adapt<IEnumerable<EmployeeToShowToClientDto>>();
    //
    //     return Ok(employeesDto);
    // }

    // [HttpGet("{id}", Name = "GetEmployeeForCompany")]
    // [ProducesResponseType(200)]
    // [ProducesResponseType(404)]
    // [ProducesResponseType(400)]
    // public async Task<ActionResult<EmployeeDto>> Get(string companyId, string id)
    // {
    //     await serviceManager.CompanyService.GetByCondition(companyId, trackChanges: false);
    //     
    //     var employee = await serviceManager.EmployeeService.GetByCondition(companyId, id, trackChanges: false);
    //
    //     var employeeDto = employee.Adapt<EmployeeToShowToClientDto>();
    //
    //     return Ok(employeeDto);
    // }

    // [HttpPost(Name = "CreateEmployeeForCompany")]
    // [ProducesResponseType(201)]
    // [ProducesResponseType(404)]
    // public async Task<ActionResult<EmployeeDto>> Create(string companyId, [FromBody] EmployeeCreateDto? model)
    // {
    //     if(model is null) 
    //         BadRequest("The model cannot be null.");
    //     
    //     if (!ModelState.IsValid)
    //     {
    //         logger.LogInformation($"Invalid model state for object {typeof(EmployeeCreateDto)}.");
    //         return UnprocessableEntity(ModelState);
    //     }
    //
    //     await serviceManager.CompanyService.GetByCondition(companyId, trackChanges: false);
    //     
    //     var dbEntity = model?.Adapt<Employee>();
    //
    //     await serviceManager.EmployeeService.CreateEmployee(companyId, dbEntity!);
    //     await serviceManager.Save();
    //
    //     var dto = dbEntity?.Adapt<EmployeeDto>();
    //
    //     return CreatedAtRoute("GetEmployeeForCompany", new { companyId = dto?.CompanyId, id = dto?.Id }, dto);
    // }

    // [HttpDelete("{id}")]
    // [ProducesResponseType(204)]
    // [ProducesResponseType(404)]
    // public async Task<IActionResult> Delete(string companyId, string id)
    // {
    //      logger.LogInformation($"Deleting the employee for company {companyId}");
    //      await serviceManager.EmployeeService.DeleteEmployee(companyId, id, trackChanges: true);
    //      await serviceManager.Save();
    //
    //      return NoContent();
    // }

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
        await serviceManager.Save();

        return NoContent();
    }
    //
    // [HttpPatch("{id}")]
    // [ProducesResponseType(204)]
    // [ProducesResponseType(404)]
    // [ProducesResponseType(400)]
    // public async Task<IActionResult> Path(string companyId, string id, [FromBody] JsonPatchDocument<EmployeeUpdateDto>? PacthDoc)
    // {
    //     await serviceManager.CompanyService.GetByCondition(companyId, trackChanges: false);
    //  
    //     // Here I have to track the entity to change the state to modified and being able to save the changes.
    //     var employeeDb = await serviceManager.EmployeeService.GetByCondition(companyId, id, trackChanges: true);
    //     
    //     var employeeToPath = employeeDb.Adapt<EmployeeUpdateDto>();
    //
      // PacthDoc?.ApplyTo(employeeToPath, (IObjectAdapter) ModelState);
    //
    //     TryValidateModel(employeeToPath);
    //
    //     if (!ModelState.IsValid)
    //     {
    //         logger.LogInformation("Invalid model state for patch document.");
    //         return UnprocessableEntity(ModelState);
    //     }
    //     
    //     employeeToPath.Adapt(employeeDb);
    //     
    //     await serviceManager.EmployeeService.SaveChanges();
    //
    //     return NoContent();
    // }
    
    [HttpPatch("{employeeId}", Name = "PatchEmployee")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(string companyId, string employeeId,
        [FromBody] JsonPatchDocument<EmployeeUpdateDto>? patchDoc)
    {
    
        if (!ModelState.IsValid)
        {
            logger.LogError($"Invalid model state for object {typeof(EmployeeUpdateDto)}");
            return  UnprocessableEntity(patchDoc);
        }
    
        if(patchDoc is null) 
            return BadRequest("Invalid patch document, can't be null");
        var result = await serviceManager.EmployeeService.GetEmployeeForPatch(companyId,employeeId, compTrackChanges: false, empTrackChanges:true);
    
        patchDoc.ApplyTo(result.employeeToPath, ModelState);
        
        TryValidateModel(result.employeeToPath);
        
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);
        
        await serviceManager.EmployeeService.SaveChangesForPatch(result.employeeToPath, result.employee);
        return NoContent();
    }
    #endregion 

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAllEmployeesWithEither(string companyId, [FromQuery] PaginationParameters pagination)
    {  
        var result = await serviceManager.EmployeeService.GetAllEmployees(companyId, pagination, trackChanges: false).Run();
        return result.Match<ActionResult<IEnumerable<EmployeeDto>>>(error=>NotFound(error),
        success =>
        {
            var employeesDtos = success.employees.Adapt<List<EmployeeDto>>().AsQueryable();
            var metadata = success.metaData;
            HttpContext.HeadersPaginationParametersInsert(employeesDtos, metadata);
            return Ok(employeesDtos);
        });
    }

    [HttpGet("{Id}", Name = "GetEmployeeForCompany")]
    public async Task<ActionResult<EmployeeDto>> GetByConditionEither(string companyId, string id)
        => (await serviceManager.EmployeeService.GetByConditionWithEither(companyId, id, trackChanges: false)
                .Map(e=>e.Adapt<EmployeeDto>()).Run())
            .HandleResult();

    [HttpPost(Name = "CreateEmployeeForCompany")]
    public async Task<ActionResult<EmployeeDto>> CreateEmployeeForCompany(string companyId, [FromBody] EmployeeCreateDto model)
    {
        var employeeDb = model.Adapt<Employee>();
        var result =   await serviceManager.EmployeeService.CreateEmployeeWithEither(companyId, employeeDb)
            .Map(e=>e.Adapt<EmployeeDto>()).Run();
        
         return result.HandleCreated("GetEmployeeForCompany", e => new {companyId = e.CompanyId, id = e.Id});
    }

    [HttpDelete("{id}", Name = "DeleteEmployee")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteEmployee(string companyId, string id) =>
        (await serviceManager.EmployeeService.DeleteEmployeeWithEither(companyId, id, false).Run()).HandleResult();
}
