using Application.DTOs;
using Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SMedia.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly IUserService _userService;

    public AuthenticationController(IEmailService emailService, IUserService userService)
    {
        _emailService = emailService;
        _userService = userService;
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp([FromBody] ReqSendOtp req)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid email address");
        }
        
        var emailExists = await _userService.IsEmailExists(req.Email);
        if (emailExists)
        {
            return BadRequest("Email already exists.");
        }
        
        var usernameExists = await _userService.IsUsernameExists(req.UserName);
        if (usernameExists)
        {
            return BadRequest("Username already exists.");
        }
        
        var otp = await _emailService.GenerateOtpAsync(req.Email);
        var message = $"Your OTP code is: {otp}";
        await _emailService.SendEmailAsync(req.Email, "Your OTP Code", message);
        
        return Ok("OTP has been sent to your email.");
    }
    
    // Xác thực OTP và đăng ký người dùng
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromQuery] string otp, [FromForm] RegisterDto registerDto)
    {
        var isOtpValid = await _emailService.VerifyOtpAsync(registerDto.email, otp);
        if (!isOtpValid)
        {
            return BadRequest("Invalid, expired OTP or exceeded attempts.");
        }
        
        var authResponse = await _userService.Register(registerDto);
        if (authResponse == null)
        {
            return BadRequest("There was an error registering.");
        }
        
        return Ok(authResponse);
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginDto loginDto)
    {
        var authResponse = await _userService.Login(loginDto);
        if (authResponse == null) return Unauthorized("Incorrect email or password.");
        return Ok(authResponse);
    }
}