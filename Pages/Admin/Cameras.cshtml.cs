using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Vedect.Data;
using Vedect.Models.Domain;

namespace Vedect.Pages.Admin;

public class CamerasModel : PageModel
{
    private readonly AppDbContext _db;

    public CamerasModel(AppDbContext db) => _db = db;

    public List<Camera> Cameras { get; set; } = new();

    public async Task OnGetAsync()
    {
        Cameras = await _db.Cameras.AsNoTracking()
                                   .OrderBy(c => c.CameraName)
                                   .ToListAsync();
    }
}
