using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Vedect.Data;

public class CameraProcessManager : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly Dictionary<Guid, Process> _procs = new();
    private const int BasePort = 9100;

    public CameraProcessManager(IServiceProvider sp) => _sp = sp;

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            using var scope = _sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Get all USB‑type cameras (CameraType == "IVCam")
            var usbCams = await db.Cameras
                                  .Where(c => c.CameraType == "IVCam")
                                  .ToListAsync(ct);

            foreach (var cam in usbCams)
            {
                if (_procs.ContainsKey(cam.Id)) continue;           // already running

                int port = BasePort + _procs.Count;                 // 9100, 9101…
                string script = Path.Combine("services", "ingest-mjpeg", "app.py");
                int index = cam.DeviceIndex ?? 0;

                var psi = new ProcessStartInfo("python",
                    $"{script} --index {index} --port {port} --cam-id {cam.Id}")
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                var proc = Process.Start(psi)!;
                _procs[cam.Id] = proc;

                // update DB with port & stream URL
                cam.IngestPort = port;
                cam.StreamURL = $"http://localhost:{port}/stream/{cam.Id}.mjpg";
                db.Cameras.Update(cam);
                await db.SaveChangesAsync(ct);
            }

            // clean up exited processes
            foreach (var kv in _procs.Where(k => k.Value.HasExited).ToList())
            {
                _procs.Remove(kv.Key);
            }

            await Task.Delay(TimeSpan.FromSeconds(15), ct);
        }
    }

    public override Task StopAsync(CancellationToken ct)
    {
        foreach (var p in _procs.Values)
            if (!p.HasExited) p.Kill(true);
        return base.StopAsync(ct);
    }
}
