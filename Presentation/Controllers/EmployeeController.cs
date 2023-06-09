﻿// using AutoMapper;
// using Contracts.Interfaces;
// using Dtos.DtoModels;
// using Entities.Models;
// using Microsoft.AspNetCore.JsonPatch;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
//
// namespace Presentation.Controllers
// {
//     [Microsoft.AspNetCore.Components.Route("api/v{version:apiVersion}/company/{CompanyId}/[controller]")]
//     [ApiVersion("1.0")]
//     [ApiController]
//     [Produces("application/json", "application/xml")]
//     public class EmployeeController : ControllerBase
//     {
//
//         private readonly IRepositoryManager _repository;
//         private readonly IMapper _mapper;
//         private readonly ILogger<EmployeeController> _logger;
//         public EmployeeController(IRepositoryManager repository, IMapper mapper, ILogger<EmployeeController> logger)
//         {
//             _repository = repository;
//             _mapper = mapper;
//             _logger = logger;
//
//         }
//
//         [HttpGet]
//         [ProducesResponseType(200)]
//         [ProducesResponseType(404)]
//         [ProducesResponseType(403)]
//         public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll(string CompanyId)
//         {
//
//             var existCompany = await _repository.Company.GetByCondiction(CompanyId, trackChanges: false);
//             if (existCompany is null)
//             {
//                 _logger.LogInformation($"The company with Id: {CompanyId} does not exist in the database.");
//                 return NotFound($"The company with Id: {CompanyId} does not exist in the database.");
//             }
//
//
//             _logger.LogInformation($"Getting all the employees for company Id {CompanyId}.");
//             var employees = await _repository.Employee.GetAll(CompanyId, trackChanges: false);
//
//             if (employees is null)
//             {
//                 _logger.LogInformation("There is no employees for this company.");
//                 return NotFound();
//             }
//
//
//             _logger.LogInformation("Mapping to employeesDtos.");
//             var employeesDtos = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
//
//             return Ok(employeesDtos);
//         }
//
//         [HttpGet("{Id}", Name = "GetEmployeeForCompany")]
//         [ProducesResponseType(200)]
//         [ProducesResponseType(404)]
//         [ProducesResponseType(400)]
//         public async Task<ActionResult<EmployeeDto>> Get(string CompanyId, string Id)
//         {
//             var existCompany = await _repository.Company.GetByCondiction(CompanyId, trackChanges: false);
//
//             if (existCompany is null)
//             {
//                 _logger.LogInformation($"The company with Id: {CompanyId} does not exist in the database.");
//                 return NotFound($"The company with Id: {CompanyId} does not exist in the database.");
//             }
//
//             var employee = await _repository.Employee.GetByCondiction(CompanyId, Id, trackChanges: false);
//
//             if (employee is null)
//             {
//                 _logger.LogInformation($"The employee with the Id:{Id} does not exist in the database.");
//                 return NotFound($"The employee with the Id:{Id} does not exist in the database.");
//             }
//                 
//             var employeeDto = _mapper.Map<EmployeeDto>(employee);
//
//             return Ok(employeeDto);
//         }
//
//         [HttpPost(Name = "CreateEmployeeForCompany")]
//         [ProducesResponseType(201)]
//         [ProducesResponseType(404)]
//         public async Task<ActionResult<EmployeeDto>> Create(string CompanyId, [FromBody] EmployeeCreateDto model)
//         {
//             if (model is null)
//             {
//                 _logger.LogInformation($"The object {typeof(EmployeeCreateDto)} is null.");
//                 return BadRequest("EmployeeCreateDto object is null.");
//             }
//
//             if (!ModelState.IsValid)
//             {
//                 _logger.LogInformation($"Invalid model state for object {typeof(EmployeeCreateDto)}.");
//                 return UnprocessableEntity(ModelState);
//             }
//             
//             var Existcompany = await _repository.Company.GetByCondiction(CompanyId, trackChanges: false);
//
//             if (Existcompany is null)
//             {
//                 _logger.LogInformation($"The company with Id: {CompanyId} does not exist in the database.");
//                 return NotFound($"The company with Id: {CompanyId} does not exist in the database.");
//             }
//
//             var dbEntity = _mapper.Map<Employee>(model);
//
//             await _repository.Employee.CreateEmployeeForCompany(CompanyId, dbEntity);
//             await _repository.Save();
//
//             var dto = _mapper.Map<EmployeeDto>(dbEntity);
//
//             return CreatedAtRoute("GetEmployeeForCompany", new { companyId= dto.CompanyId, Id = dto.Id }, dto);
//         }
//
//         [HttpDelete("{Id}")]
//         [ProducesResponseType(204)]
//         [ProducesResponseType(404)]
//         public async Task<IActionResult> Delete(string CompanyId, string Id)
//         {
//             var Existcompany = await _repository.Company.GetByCondiction(CompanyId, trackChanges: false);
//             
//             if (Existcompany is null)
//             {
//                 _logger.LogInformation($"The company with Id: {CompanyId} does not exist in the database.");
//                 return NotFound($"The company with Id: {CompanyId} does not exist in the database.");
//             }
//
//             await _repository.Employee.DeleteEmployee(CompanyId, Id, trackChanges: true);
//             await _repository.Save();
//
//             return NoContent();
//         }
//
//         [HttpPut("{Id}")]
//         [ProducesResponseType(204)]
//         [ProducesResponseType(404)]
//         [ProducesResponseType(400)]
//         public async Task<IActionResult> Update(string CompanyId, string Id, [FromBody] EmployeeUpdateDto model)
//         {
//             if (model is null) 
//             {
//                 _logger.LogInformation("The model can not be null");
//                 return BadRequest("The model can not be null");
//             }
//
//             if (!ModelState.IsValid)
//             {
//                 _logger.LogInformation($"Invalid model state for object {typeof(EmployeeCreateDto)}.");
//                 return UnprocessableEntity(ModelState);
//             }
//
//             var company = await _repository.Company.GetByCondiction(CompanyId, trackChanges: false);
//             if (company is null)
//             {
//                 _logger.LogInformation($"The company with Id: {CompanyId} does not exist in the database.");
//                 return BadRequest($"The company with Id: {CompanyId} does not exist in the database.");
//             }
//
//             // Modelo conectado aqui para que ef pueda seguir los cambios que recibe la entidad
//             var dbEntity = await _repository.Employee.GetByCondiction(CompanyId,Id, trackChanges: true);
//             if (dbEntity is null)
//             {
//                 _logger.LogInformation($"The employee with Id: {Id} does not exist in the database.");
//                 return BadRequest($"The employee with Id: {Id} does not exist in the database.");
//             }
//             // Seteo el track para que ef me cambie el estado de la entidad a modified
//             // _context.Entry<T>().State = EntityState.Modified
//             _mapper.Map(model, dbEntity);
//             await _repository.Save();
//
//             return NoContent();
//         }
//
//         [HttpPatch("{Id}")]
//         [ProducesResponseType(204)]
//         [ProducesResponseType(404)]
//         [ProducesResponseType(400)]
//         public async Task<IActionResult> Path(string CompanyId, string Id, [FromBody] JsonPatchDocument<EmployeeUpdateDto> pacthDoc)
//         {
//
//             if (pacthDoc is null)
//             {
//                 _logger.LogInformation("The model can not be null");
//                 return BadRequest("The model can not be null");
//             }
//
//             var company = await _repository.Company.GetByCondiction(CompanyId, trackChanges: false);
//             if (company is null)
//             {
//                 _logger.LogInformation($"The company with Id:{CompanyId} does not exist.");
//                 return NotFound($"The company with Id:{CompanyId} does not exist.");
//             }
//             // Here i have to track the entity to change the state to modified and being able to save the changes.
//             var employeeDb = await _repository.Employee.GetByCondiction(CompanyId, Id, trackChanges: true);
//             if (employeeDb is null)
//             {
//                 _logger.LogInformation($"The employee with Id:{Id} does not exist in the database.");
//                 return NotFound($"The employee with Id:{Id} does not exist in the database.");
//             }
//
//             var employeeToPath = _mapper.Map<EmployeeUpdateDto>(employeeDb);
//
//             pacthDoc.ApplyTo(employeeToPath, ModelState);
//
//             TryValidateModel(employeeToPath);
//
//             if (!ModelState.IsValid)
//             {
//                 _logger.LogInformation("Invalid model state for patch document.");
//                 return UnprocessableEntity(ModelState);
//             }
//
//             _mapper.Map(employeeToPath, employeeDb);
//             
//             await _repository.Save();
//
//             return NoContent();
//         }
//     }
// }
