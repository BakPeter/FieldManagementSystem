using System.Text.Json;
using FieldManagementSystem.Services.Controller.Core.Interfaces;
using FieldManagementSystem.Services.Controller.Core.Types.DTOs;
using FieldManagementSystem.Services.Controller.Infrastructure.types;
using Microsoft.AspNetCore.Mvc;

namespace FieldManagementSystem.Services.Controller.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ControllerController : ControllerBase
{
    private readonly ILogger<ControllerController> _logger;
    private readonly IControllerService _service;

    public ControllerController(ILogger<ControllerController> logger, IControllerService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var response = await _service.GetControllersAsync();
            if (response.IsSuccess) return Ok(response.Data);

            _logger.LogInformation("111Get Users, response: {response}", JsonSerializer.Serialize(response));

            return BadRequest(response.Error!.Message);
        }
        catch (Exception e)
        {
            return ErrorHandler(e);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetController(string id)
    {
        try
        {
            var response = await _service.GetControllerAsync(id);
            if (response.IsSuccess) return Ok(response.Data);
    
            _logger.LogInformation("Get controller by id - {Success}: id: {id} response: {response}",
                response.IsSuccess, id, JsonSerializer.Serialize(response));
    
            return BadRequest(response.Error!.Message);
        }
        catch (Exception e)
        {
            return ErrorHandler(e);
        }
    }
    
    
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateControllerDto createControllerDto)
    {
        try
        {
            var response = await _service.CreateControllerAsync(createControllerDto);
            if (response.IsSuccess)
                return CreatedAtAction(nameof(GetController), new { id = response.Data!.Id}, response.Data);
    
            _logger.LogInformation("Create - {Success}: createControllerDto: {createControllerDto} response: {response}",
                response.IsSuccess, createControllerDto, JsonSerializer.Serialize(response));
    
            if (response.Error is ControllerValidationException validationException)
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = validationException.Message,
                    Detail = JsonSerializer.Serialize(validationException.ValidationErrors),
                });
    
            return BadRequest(response.Error!.Message);
        }
        catch (Exception e)
        {
            return ErrorHandler(e);
        }
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateController([FromBody] UpdateControllerDto updateControllerDto)
    {
        try
        {
            var response = await _service.UpdateController(updateControllerDto);
            if (response.IsSuccess) return Ok(response.Data);
    
            _logger.LogInformation("Create - {Success}: updateControllerDto: {updateControllerDto} response: {response}",
                response.IsSuccess, updateControllerDto, JsonSerializer.Serialize(response));
    
            if (response.Error is ControllerValidationException validationException)
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = validationException.Message,
                    Detail = JsonSerializer.Serialize(validationException.ValidationErrors),
                });
    
            return BadRequest(response.Error!.Message);
        }
        catch (Exception e)
        {
            return ErrorHandler(e);
        }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteController(string id)
    {
        try
        {
            var response = await _service.DeleteControllerAsync(id);
            if (response.IsSuccess) return Ok(response.Data);
    
            _logger.LogInformation("Delete Controller by id - {Success}: id: {id} response: {response}",
                response.IsSuccess, id, JsonSerializer.Serialize(response));
    
            return BadRequest(response.Error!.Message);
        }
        catch (Exception e)
        {
            return ErrorHandler(e);
        }
    }
    
    private ActionResult ErrorHandler(Exception error)
    {
        _logger.LogError(error.Message);
    
        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Bad Request",
            Detail = error.Message,
        };
        return BadRequest(problem);
    }
}