using System.Text.Json;
using FieldManagementSystem.Services.User.Core.Interfaces;
using FieldManagementSystem.Services.User.Core.Types;
using FieldManagementSystem.Services.User.Core.Types.DTOs;
using FieldManagementSystem.Services.User.Infrastructure.types;
using Microsoft.AspNetCore.Mvc;

namespace FieldManagementSystem.Services.User.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _service;

    public UserController(ILogger<UserController> logger, IUserService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var response = await _service.GetUsersAsync();
            if (response.IsSuccess) return Ok(response.Data);

            _logger.LogInformation("Get Users, response: {response}", JsonSerializer.Serialize(response));

            return BadRequest(response.Error!.Message);
        }
        catch (Exception e)
        {
            return ErrorHandler(e);
        }
    }

    /// <summary>
    /// Get User by user property.
    /// </summary>
    /// <param name="tokenType">User property type, 'email' or 'id' </param>
    /// <param name="tokenValue">User property value</param>
    /// <returns></returns>
    [HttpGet("{tokenType}/{tokenValue}")]
    public async Task<IActionResult> GetUser(string tokenType, string tokenValue)
    {
        try
        {
            Result<UserEntity> response;
            if (tokenType.Equals("email"))
                response = await _service.GetUserByEmailAsync(tokenValue);
            else if (tokenType.Equals("id"))
                response = await _service.GetUserAsync(tokenValue);
            else
                throw new ArgumentException($"I valid user token type, {tokenType}, can be email/id only", nameof(tokenType));

            if (response.IsSuccess) return Ok(response.Data);

            _logger.LogInformation(
                "Get User token type - {Success}: tokenType: {tokenType}, tokenValue: {tokenValue}, response: {response}",
                response.IsSuccess, tokenType, tokenValue, JsonSerializer.Serialize(response));

            return BadRequest(response.Error!.Message);
        }
        catch (Exception e)
        {
            return ErrorHandler(e);
        }
    }


    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        try
        {
            var response = await _service.CreateUserAsync(createUserDto);
            if (response.IsSuccess)
                return CreatedAtAction(nameof(GetUser), new { email = response.Data!.Email }, response.Data);

            _logger.LogInformation("Create - {Success}: createUserDto: {createUserDto} response: {response}",
                response.IsSuccess, createUserDto, JsonSerializer.Serialize(response));

            if (response.Error is UserValidationException validationException)
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
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
    {
        try
        {
            var response = await _service.UpdateUser(updateUserDto);
            if (response.IsSuccess) return Ok(response.Data);

            _logger.LogInformation("Create - {Success}: updateUserDto: {updateUserDto} response: {response}",
                response.IsSuccess, updateUserDto, JsonSerializer.Serialize(response));

            if (response.Error is UserValidationException validationException)
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

    /// <summary>
    /// DeleteUserAsync by User id.
    /// User id is User is id registration.
    /// </summary>
    /// <param name="id">User registration id</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        try
        {
            var response = await _service.DeleteUserAsync(id);
            if (response.IsSuccess) return Ok(response.Data);

            _logger.LogInformation("Delete User by id - {Success}: id: {id} response: {response}",
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

        // var problem = new ProblemDetails
        // {
        //     Status = StatusCodes.Status500InternalServerError,
        //     Title = "Bad Request",
        //     Detail = error.Message,
        // };
        return Problem(title: "Server Error", detail: error.Message, statusCode: 500);
    }
}