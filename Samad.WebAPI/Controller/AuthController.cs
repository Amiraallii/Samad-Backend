using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Samad.Application.Dtos;
using Samad.Application.IServices;

namespace Samad.WebAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IUserService userService) : SamadController
    {
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterDto registerDto,
            CancellationToken ct)
        {
            await userService.RegisterUser(registerDto);

            return Ok(new
            {
                message = "ثبت نام با موفقیت انجام شد."
            });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginDto loginDto,
            CancellationToken ct)
        {
            var result =
                await userService.LoginUser(loginDto);

            return Ok(result);
        }
    }
}