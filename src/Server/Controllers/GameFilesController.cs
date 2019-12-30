using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Blazork.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameFilesController : Controller
    {
        [HttpGet("{gameName}")]
        public Task<string> Get(string gameName)
        {
            return Task.FromResult(gameName);
        }
    }
}
