using Application.DTOs;
using Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace SMedia.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class AuthenticationController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly IUserService _userService;

    public AuthenticationController(IEmailService emailService, IUserService userService)
    {
        _emailService = emailService;
        _userService = userService;
    }
    
    /// <summary>
    /// Send a one-time password (OTP) to the user's email for verification
    /// </summary>
    /// <param name="req">Contains email address to send OTP to</param>
    /// <returns>Success or failure message</returns>
    /// <response code="200">OTP sent successfully</response>
    /// <response code="400">Email already exists or invalid</response>
    [HttpPost("send-otp")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(string), 400)]
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
        
        return Ok("OTP has been sent to your email.");    }
    
    /// <summary>
    /// Register a new user account with OTP verification
    /// </summary>
    /// <param name="otp">One-time password sent to the user's email</param>
    /// <param name="registerDto">User registration information</param>
    /// <returns>Authentication details including token on successful registration</returns>
    /// <response code="200">Successfully registered and returns auth token</response>
    /// <response code="400">Registration failed (invalid OTP, email exists, etc.)</response>
    [AllowAnonymous]
    [HttpPost("register")]
    [SwaggerRequestExample(typeof(RegisterDto), typeof(SMedia.SwaggerExamples.Auth.RegisterDtoExample))]
    [SwaggerResponseExample(200, typeof(SMedia.SwaggerExamples.Auth.AuthResponseExample))]
    [SwaggerResponseExample(400, typeof(SMedia.SwaggerExamples.Auth.AlertExample))]public async Task<ActionResult<AuthResponse>> Register([FromQuery] string otp, [FromForm] RegisterDto registerDto)
    {
        if (string.IsNullOrEmpty(registerDto.Email))
        {
            return BadRequest("Email is required.");
        }
        
        var isOtpValid = await _emailService.VerifyOtpAsync(registerDto.Email, otp);
        if (!isOtpValid)
        {
            return BadRequest("Invalid, expired OTP or exceeded attempts.");
        }
        
        var authResponse = await _userService.Register(registerDto);
        if (authResponse == null)
        {
            return BadRequest("There was an error registering.");
        }
        
        return Ok(authResponse);    }
    
    /// <summary>
    /// Authenticate a user and get access token
    /// </summary>
    /// <param name="loginDto">Login credentials including email and password</param>
    /// <returns>Authentication details including access token</returns>
    /// <response code="200">Successfully authenticated</response>
    /// <response code="401">Authentication failed (incorrect email or password)</response>
    [AllowAnonymous]
    [HttpPost("login")]
    [SwaggerRequestExample(typeof(LoginDto), typeof(SMedia.SwaggerExamples.Auth.LoginDtoExample))]
    [SwaggerResponseExample(200, typeof(SMedia.SwaggerExamples.Auth.AuthResponseExample))]
    public async Task<ActionResult<AuthResponse>> Login(LoginDto loginDto)
    {
        var authResponse = await _userService.Login(loginDto);
        if (authResponse == null) return Unauthorized("Incorrect email or password.");
        return Ok(authResponse);
    }
}