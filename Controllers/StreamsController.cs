using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vedect.Data;

namespace Vedect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public StreamsController(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("{cameraId}/start")]
        public async Task<IActionResult> StartStream(Guid cameraId)
        {
            var camera = await _context.Cameras.FindAsync(cameraId);
            if (camera == null || string.IsNullOrEmpty(camera.StreamUrl))
                return NotFound("Camera not found or missing streaming URL.");

            var streamKey = $"camera-{camera.Id}-user-1"; // You can customize this logic

            var client = _httpClientFactory.CreateClient();
            var payload = new
            {
                rtsp_url = camera.StreamUrl,
                stream_key = streamKey
            };

            var response = await client.PostAsJsonAsync("http://localhost:5001/start-stream", payload);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

            return Ok(await response.Content.ReadAsStringAsync());
        }
    }
}
