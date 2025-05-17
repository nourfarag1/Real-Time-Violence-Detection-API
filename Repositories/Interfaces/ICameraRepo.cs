namespace Vedect.Repositories.Interfaces
{
    public interface ICameraRepo
    {
        Task<Camera> AddCameraAsync(Camera camera, string userId);
    }
}
