using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Vedect.Data;

public class CameraPingService : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly IHttpClientFactory _http;

    public CameraPingService(IServiceProvider sp, IHttpClientFactory http)
    {
        _sp = sp; _http = http;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var client = _http.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(2);

        while (!ct.IsCancellationRequested)
        {
            using var scope = _sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var cams = await db.Cameras.AsNoTracking().ToListAsync(ct);

            foreach (var cam in cams)
            {
                bool online = false;
                try
                {
                    using var req = new HttpRequestMessage(HttpMethod.Get, cam.StreamURL);
                    req.Headers.Range = new RangeHeaderValue(0, 1023); // grab first KB
                    using var resp = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
                    online = resp.IsSuccessStatusCode;
                }
                catch { online = false; }

                if (cam.IsOnline != online)
                {
                    cam.IsOnline = online;
                    cam.LastChecked = DateTime.UtcNow;
                    db.Cameras.Update(cam);
                }
                else
                {
                    // still same status—just bump timestamp every few minutes
                    cam.LastChecked = DateTime.UtcNow;
                    db.Cameras.Update(cam);
                }
            }

            await db.SaveChangesAsync(ct);
            await Task.Delay(TimeSpan.FromSeconds(30), ct);
        }
    }
}
