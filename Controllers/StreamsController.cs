using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vedect.Data;
using Vedect.Models.Domain;

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

            var existingSession = await _context.CameraStreamsSessions
                .Where(s => s.CameraId == camera.Id && s.IsActive)
                .FirstOrDefaultAsync();

            if (existingSession != null)
            {
                var existingUrl = $"http://localhost:8080/live/{existingSession.StreamKey}.flv";
                return Ok(new { message = "Stream already active.", url = existingUrl });
            }

            var streamKey = $"camera-{camera.Id}-user-1";

            var client = _httpClientFactory.CreateClient();
            var payload = new
            {
                rtsp_url = camera.StreamUrl,
                stream_key = streamKey
            };

            var response = await client.PostAsJsonAsync("http://localhost:5001/start-stream", payload);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

            var session = new CameraStreamSession
            {
                CameraId = camera.Id,
                StreamKey = streamKey,
                StartedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.CameraStreamsSessions.Add(session);
            await _context.SaveChangesAsync();

            var streamUrl = $"http://localhost:8080/live/{streamKey}.flv";

            return Ok(new
            {
                message = "Stream started successfully.",
                url = streamUrl
            });
        }


        [HttpPost("{cameraId}/stop")]
        public async Task<IActionResult> StopStream(Guid cameraId)
        {
            var session = await _context.CameraStreamsSessions
                .Where(s => s.CameraId == cameraId)
                .FirstOrDefaultAsync();

            if (session == null)
                return NotFound("No active stream session found for this camera.");

            var client = _httpClientFactory.CreateClient();
            var payload = new { stream_key = session.StreamKey };

            var response = await client.PostAsJsonAsync("http://localhost:5001/stop-stream", payload);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

            //session.IsActive = false;
            //session.EndedAt = DateTime.UtcNow;

            _context.CameraStreamsSessions.Remove(session);

            await _context.SaveChangesAsync();

            return Ok("Stream stopped successfully.");
        }
    }
}