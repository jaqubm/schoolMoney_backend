using schoolMoney_backend.Data;
using Microsoft.AspNetCore.Mvc;

namespace schoolMoney_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController(IConfiguration config) : ControllerBase
{
    private readonly DataContext _entityFramework = new(config);

    [HttpGet("Status")]
    public async Task<ActionResult<Dictionary<string, string>>> GetStatus()
    {
        var databaseConnectionStatus = await _entityFramework.Database.CanConnectAsync();

        return Ok(new Dictionary<string, string>
        {
            { "apiStatus", "OK" },
            { "databaseConnectionStatus", databaseConnectionStatus ? "OK" : "Failed" }
        });
    }
    
    [HttpGet("WakeUpDatabase")]
    public async Task<ActionResult> GetWakeUpDatabase()
    {
        var databaseWakeUpConnectAsync = await _entityFramework.Database.CanConnectAsync();
        
        return Ok(databaseWakeUpConnectAsync);
    }
}