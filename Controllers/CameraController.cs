using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Vedect.Data;
using Vedect.Models.DTOs;

namespace Vedect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CameraController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CameraController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("streams")]
        public async Task<IActionResult> GetUserCameraStreams()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if(userId == null) 
                return NotFound("User Not Found");

            var cameras = await _db.UserCameras
                .Where(uc => uc.UserId == userId)
                .Include(uc => uc.Camera)
                .Select(uc => new CameraStreamDto
                {
                    CameraId = uc.Camera.Id,
                    CameraName = uc.Camera.CameraName,
                    StreamURL = uc.Camera.StreamURL,
                    IsOnline = uc.Camera.IsOnline
                })
                .ToListAsync();

            return Ok(cameras);
        }
    }
}
