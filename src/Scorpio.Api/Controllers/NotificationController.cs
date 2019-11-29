using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Scorpio.Api.Hubs;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Scorpio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class NotificationController : ControllerBase
    {
        private readonly IHubContext<MainHub> _hubContext;

        public NotificationController(IHubContext<MainHub> hubContext)
        {
            _hubContext = hubContext;
        }

        /// <summary>
        /// Posts raw notification to SignalR on specific topic.
        /// The request body will be send as message parameter.
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        [HttpPost("{topic}")]
        public async Task<IActionResult> Publish(string topic)
        {
            var notification = await ReadBody();

            await _hubContext.Clients.All.SendAsync(topic, notification);

            return Ok();
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
