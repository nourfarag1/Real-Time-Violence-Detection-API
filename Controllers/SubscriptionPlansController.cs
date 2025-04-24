using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vedect.Data;
using Vedect.Models.Domain;
using Vedect.Models.DTOs;

namespace Vedect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionPlansController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public SubscriptionPlansController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("plans")]
        public async Task<IActionResult> GetPlans()
        {
            var plans = await _dbContext.SubscriptionPlans.ToListAsync();
            return Ok(plans);
        }

        [HttpPost]
        public async Task<IActionResult> RequestPlan([FromBody] RequestPlanDto dto)
        {
            var user = await _dbContext.Users.FindAsync(dto.UserId);
            var plan = await _dbContext.SubscriptionPlans.FindAsync(dto.RequestedPlanId);

            if (user == null || plan == null)
                return BadRequest("Invalid User or Plan");

            var existingPending = await _dbContext.UserPlanRequests
                .AnyAsync(r => r.UserId == dto.UserId && r.Status == "Pending");

            if (existingPending)
                return Conflict("There is already a request pending for this user");

            var request = new UserPlanRequests
            {
                UserId = dto.UserId,
                RequestedPlanId = dto.RequestedPlanId,
                RequestedAt = DateTime.Now,
                Status = "Pending"
            };

            _dbContext.UserPlanRequests.Add(request);
            await _dbContext.SaveChangesAsync();

            return Ok(new {Message = "Plan request is submitted successfully!"});
        }

        [HttpGet("{userId}/status")]
        public async Task<ActionResult<string>> GetPlanRequestStatus(string userId)
        {
            var status = await _dbContext.UserPlanRequests.Where(x => x.UserId.Equals(userId)).Select(x => x.Status).FirstOrDefaultAsync();

            if (status == null)
                return NotFound("Not request found for this userId");

            return Ok(status);
        }
    }
}
