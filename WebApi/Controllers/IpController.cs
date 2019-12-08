using System.Linq;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IpController : ControllerBase
    {
        private readonly IIpService _service;

        public IpController(IIpService service)
        {
            _service = service;
        }

        [HttpGet("{ip}")]
        public async Task<ActionResult> Get(string ip)
        {
            var result = await _service.GetLocationsByIp(ip);
            return Ok(result.FirstOrDefault());
        }
    }
}