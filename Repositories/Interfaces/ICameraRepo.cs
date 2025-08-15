using Vedect.Models.Domain;

namespace Vedect.Repositories.Interfaces
{
    public interface ICameraRepo
    {
        Task<Camera> AddCameraAsync(Camera camera, string userId);
        Task<List<Camera>> GetUserCameras(string userId);
    }
}
