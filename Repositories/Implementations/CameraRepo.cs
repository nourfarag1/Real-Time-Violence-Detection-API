using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Validations;
using Vedect.Data;
using Vedect.Models.Domain;
using Vedect.Repositories.Interfaces;

namespace Vedect.Repositories.Implementations
{
    public class CameraRepo : ICameraRepo
    {
        private readonly AppDbContext _appDbContext;

        public CameraRepo(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Camera> AddCameraAsync(Camera camera, string userId)
        {
            var user = await _appDbContext.Users.FindAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            var existingCamera = await _appDbContext.Cameras
                .FirstOrDefaultAsync(c => c.StreamUrl == camera.StreamUrl);

            if (existingCamera != null)
            {
                var alreadyLinked = await _appDbContext.UserCameras
                    .AnyAsync(uc => uc.UserId == userId && uc.CameraId == existingCamera.Id);

                if (alreadyLinked)
                {
                    throw new InvalidOperationException("You have already added this camera.");
                }

                var userCamera = new UserCamera
                {
                    UserId = userId,
                    CameraId = existingCamera.Id
                };

                _appDbContext.UserCameras.Add(userCamera);
                await _appDbContext.SaveChangesAsync();

                return existingCamera;
            }

            camera.Id = Guid.NewGuid();
            _appDbContext.Cameras.Add(camera);

            var newUserCamera = new UserCamera
            {
                UserId = userId,
                CameraId = camera.Id
            };

            _appDbContext.UserCameras.Add(newUserCamera);

            await _appDbContext.SaveChangesAsync();
            return camera;
        }

        public async Task<List<Camera>> GetUserCameras(string userId)
        {
            var userCameras = await _appDbContext.UserCameras.Where(u => u.UserId == userId)
                .Select(uc => uc.CameraId).ToListAsync();

            if (userCameras.Count == 0)
                return null;

            var cameras = new List<Camera>();

            foreach (var usercamera in userCameras)
            {
                var camera = _appDbContext.Cameras.Where(c => c.Id == usercamera).FirstOrDefault();

                if (camera != null)
                    cameras.Add(camera);
            }

            return cameras;
        }
    }
}
