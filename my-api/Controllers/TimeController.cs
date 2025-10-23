using Microsoft.AspNetCore.Mvc;

namespace my_api.Controllers;

[ApiController]
[Route("[controller]")]
public class TimeController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var utcNow = DateTime.UtcNow;
        var brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        var localNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, brasiliaTimeZone);
        
        string greeting = localNow.Hour switch
        {
            >= 5 and < 12 => "Bom dia!",
            >= 12 and < 18 => "Boa tarde!",
            _ => "Boa noite!"
        };

        return Ok(new
        {
            greeting,
            utcNow = utcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'"),
            localNow = localNow.ToString("yyyy-MM-dd HH:mm:ss"),
            timezone = brasiliaTimeZone.DisplayName
        });
    }
}