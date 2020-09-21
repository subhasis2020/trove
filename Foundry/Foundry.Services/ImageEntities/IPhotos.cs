using Foundry.Domain.Dto;
using Foundry.Domain.DbModel;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IPhotos : IFoundryRepositoryBase<Photo>
    {
        Task<Photo> GetEntityPhotoPath(int photoId, int entityType, int createdId);
        Task<UploadFileResponseDto> UploadImage(IFormFile file);
        Task<UploadFileResponseDto> WriteFile(IFormFile file);
        Task<bool> SaveUpdateImage(string imagePath, int entityId, int createdUserId, int photoType);
        Task<int> RemoveImage(PhotosDto model);
        Task<string> GetAWSBucketFilUrl(string fileName, string defaultFile);
        Task<PhotosExpirationDto> GetAWSBucketFileUrlWithExpiration(int userId, int entityPhotyType);
    }
}
