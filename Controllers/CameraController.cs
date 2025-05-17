using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Security.Claims;
using Vedect.Data;
using Vedect.Models.Domain;
using Vedect.Models.DTOs;
using Vedect.Repositories.Implementations;
using Vedect.Repositories.Interfaces;

namespace Vedect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CameraController : ControllerBase
    {
        private readonly ICameraRepo _camRepo;
        
        public CameraController(ICameraRepo camRepo)
        {
            _camRepo = camRepo;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddCamera([FromBody] AddCameraRequest request)
        {   
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var camera = new Camera
            {
                CameraName = request.CameraName,
                StreamUrl = request.StreamUrl
            };

            try
            {
                var addedCamera = await _camRepo.AddCameraAsync(camera, userId);
                return Ok(new
                {
                    Message = "Camera added successfully.",
                    CameraId = addedCamera.Id,
                    CameraName = addedCamera.CameraName,
                    StreamUrl = addedCamera.StreamUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

    }
}
