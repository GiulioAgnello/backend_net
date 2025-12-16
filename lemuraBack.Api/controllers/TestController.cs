using Microsoft.AspNetCore.Mvc;

namespace LemuraBack.Api.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "Backend is running 🚀" });
    }
}
