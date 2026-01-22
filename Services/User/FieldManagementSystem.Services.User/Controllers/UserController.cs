using FieldManagementSystem.User.Core.DTOs;
using FieldManagementSystem.User.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FieldManagementSystem.User.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetUsersAsync();
        return Ok(users);
    }

    [HttpGet("{email}")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        var user = await _userService.GetUserByEmailAsync(email);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        var newUser = await _userService.CreateUserAsync(createUserDto);
        return CreatedAtAction(nameof(GetUserByEmail), new { email = newUser.Email }, newUser);
    }

    [HttpPut("{email}")]
    public async Task<IActionResult> UpdateUser(string email, [FromBody] UpdateUserDto updateUserDto)
    {
        await _userService.UpdateUserAsync(email, updateUserDto);
        return NoContent();
    }

    [HttpDelete("{email}")]
    public async Task<IActionResult> DeleteUser(string email)
    {
        await _userService.DeleteUserAsync(email);
        return NoContent();
    }
}