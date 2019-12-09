using Microsoft.AspNetCore.Mvc;
using Scorpio.ProcessRunner;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Scorpio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProcessController : ControllerBase
    {
        private readonly IGenericProcessRunner _processRunner;

        public ProcessController(IGenericProcessRunner processRunner)
        {
            _processRunner = processRunner;
        }

        [HttpPost("run")]
        public async Task<IActionResult> Run()
        {
            var command = await ReadBody();
            var stdout = _processRunner.RunCommand(command);
            return Ok(stdout);
        }

        /// <summary>
        /// Returns raw body of the request.
        /// This is not possible with string/object/dynamic/JObject binding, 
        /// because of the content-type mismatch
        /// </summary>
        /// <returns>Body request</returns>
        private async Task<string> ReadBody()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
