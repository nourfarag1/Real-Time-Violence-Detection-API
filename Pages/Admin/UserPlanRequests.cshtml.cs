using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Vedect.Data;
using Vedect.Models.Domain;

namespace Vedect.Pages.Admin
{
    public class UserPlanRequestsModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly ILogger<UserPlanRequestsModel> _logger;

        public UserPlanRequestsModel(AppDbContext db, ILogger<UserPlanRequestsModel> logger)
        {
            _db = db;
            _logger = logger;
        }

        public List<UserPlanRequests> Requests { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminUsername")))
            {
                return RedirectToPage("/Admin/Login");
            }

            _logger.LogInformation("Fetching user plan requests...");
            Requests = await _db.UserPlanRequests
                .Include(r => r.User)
                .Include(r => r.RequestedPlan)
                .OrderByDescending(r => r.RequestedAt)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string action)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminUsername")))
            {
                return RedirectToPage("/Admin/Login");
            }

            _logger.LogInformation("Handling post action: {Action}", action);

            if (string.IsNullOrWhiteSpace(action) || (!action.StartsWith("approve_") && !action.StartsWith("reject_")))
            {
                _logger.LogWarning("Invalid action: {Action}", action);
                TempData["Message"] = "Invalid action.";
                return RedirectToPage();
            }

            var parts = action.Split('_');
            if (parts.Length != 2 || !int.TryParse(parts[1], out int requestId))
            {
                _logger.LogWarning("Invalid request ID in action: {Action}", action);
                TempData["Message"] = "Invalid request ID.";
                return RedirectToPage();
            }

            var request = await _db.UserPlanRequests
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null || request.User == null)
            {
                _logger.LogError("Request or User not found for ID: {RequestId}", requestId);
                TempData["Message"] = "Request not found.";
                return RedirectToPage();
            }

            if (parts[0] == "approve")
            {
                _logger.LogInformation("Approving request {RequestId}", requestId);
                request.Status = "Approved";
                request.ReviewedAt = DateTime.UtcNow;

                request.User.SubscriptionPlanId = request.RequestedPlanId;
                _db.Users.Update(request.User);
            }
            else if (parts[0] == "reject")
            {
                _logger.LogInformation("Rejecting request {RequestId}", requestId);
                request.Status = "Rejected";
                request.ReviewedAt = DateTime.UtcNow;
            }

            _db.UserPlanRequests.Update(request);
            await _db.SaveChangesAsync();

            TempData["Message"] = $"Request {parts[0]}ed successfully.";
            return RedirectToPage();
        }
    }
}
