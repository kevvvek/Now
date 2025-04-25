using Microsoft.AspNetCore.Mvc;

namespace Now.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HelloWorldController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "Hello World from ASP.NET Core!" });
    }

    [HttpGet("greeting/{name}")]
    public IActionResult GetGreeting(string name)
    {
        return Ok(new { message = $"Hello, {name}! Welcome to Now Project." });
    }
} 