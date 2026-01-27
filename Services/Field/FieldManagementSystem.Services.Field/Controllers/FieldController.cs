using System.Text.Json;
using FieldManagementSystem.Services.Field.Core.Interfaces;
using FieldManagementSystem.Services.Field.Core.Types.DTOs;
using FieldManagementSystem.Services.Field.Infrastructure.types;
using Microsoft.AspNetCore.Mvc;

namespace FieldManagementSystem.Services.Field.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FieldController : ControllerBase
{
    private readonly ILogger<FieldController> _logger;
    private readonly IFieldService _service;

    public FieldController(ILogger<FieldController> logger, IFieldService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetFields()
    {
        try
        {
            var response = await _service.GetFieldsAsync(CancellationToken.None);
            if (response.IsSuccess) return Ok(response.Data);

            _logger.LogInformation("Get Fields, response: {response}", JsonSerializer.Serialize(response));

            return BadRequest(response.Error!.Message);
        }
        catch (Exception e)
        {
            return ErrorHandler(e);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetField(string id)
    {
        try
        {
            var response = await _service.GetFieldAsync(id, CancellationToken.None);
            if (response.IsSuccess) return Ok(response.Data);

            _logger.LogInformation("Get Field by id - {Success}: id: {id} response: {response}",
                response.IsSuccess, id, JsonSerializer.Serialize(response));

            return BadRequest(response.Error!.Message);
        }
        catch (Exception e)
        {
            return ErrorHandler(e);
        }
    }


    [HttpPost]
    public async Task<IActionResult> CreateField([FromBody] CreateFieldDto createFieldDto)
    {
        try
        {
            var response = await _service.CreateFieldAsync(createFieldDto);
            if (response.IsSuccess)
                return CreatedAtAction(nameof(GetField), new { id = response.Data!.Id}, response.Data);

            _logger.LogInformation("Create Field - {Success}: createFieldDto: {createFieldDto} response: {response}",
                response.IsSuccess, createFieldDto, JsonSerializer.Serialize(response));

            if (response.Error is FieldValidationException validationException)
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
    public async Task<IActionResult> UpdateField([FromBody] UpdateFieldDto updateFieldDto)
    {
        try
        {
            var response = await _service.UpdateField(updateFieldDto);
            if (response.IsSuccess) return Ok(response.Data);

            _logger.LogInformation("Update - {Success}: updateFieldDto: {updateFieldDto} response: {response}",
                response.IsSuccess, updateFieldDto, JsonSerializer.Serialize(response));

            if (response.Error is FieldValidationException validationException)
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
    public async Task<IActionResult> DeleteField(string id)
    {
        try
        {
            var response = await _service.DeleteFieldAsync(id);
            if (response.IsSuccess) return Ok(response.Data);

            _logger.LogInformation("Delete Field by id - {Success}: id: {id} response: {response}",
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
        var errorMsg = error.Message;
        if (error.InnerException is not null)
            errorMsg = $"{errorMsg}, Inner Error Msg: {error.InnerException.Message}";

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Bad Request",
            Detail = $"{errorMsg}",
        };
        return BadRequest(problem);
    }
}