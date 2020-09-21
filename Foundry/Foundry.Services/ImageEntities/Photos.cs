using Foundry.Domain.Dto;
using Foundry.Domain.Enums;
using Foundry.Domain.DbModel;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using static Foundry.Domain.Constants;
using Microsoft.Extensions.Configuration;
using Amazon.S3;
using Amazon.S3.Model;
using System.Collections.Generic;
using Amazon;
using Foundry.Domain;
using System.Linq;

namespace Foundry.Services
{
    public class Photos : FoundryRepositoryBase<Photo>, IPhotos
    {

        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IConfiguration _configuration;
        private readonly IGeneralSettingService _generalRepository;
        private readonly RegionEndpoint bucketRegion = RegionEndpoint.CACentral1;
        public Photos(IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration, IGeneralSettingService generalRepository) : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _configuration = configuration;
            _generalRepository = generalRepository;
        }
        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<Photo> GetEntityPhotoPath(int userId, int entityType, int createdId)
        {
            object obj = new { EntityId = userId, PhotoType = entityType };
            var userImage = await GetDataByIdAsync(obj);
            if (userImage == null)
            {
                userImage = new Photo();
            }
            userImage.photoPath = userImage != null && !string.IsNullOrEmpty(userImage.photoPath) ? await GetAWSBucketFilUrl(userImage.photoPath, string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.UserDefaultImage)) : string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.UserDefaultImage);
            return userImage;
        }

        public async Task<bool> SaveUpdateImage(string imagePath, int entityId, int createdUserId, int photoType)
        {
            try
            {
                object obj = new { EntityId = entityId, PhotoType = photoType };
                var profile = await GetDataByIdAsync(obj);
                bool saveUpdateResult = false;

                if (profile == null && !string.IsNullOrEmpty(imagePath))
                {
                    profile = new Photo();
                    profile.photoPath = imagePath;
                    profile.entityId = entityId;
                    profile.photoType = photoType;
                    profile.createdDate = DateTime.UtcNow;
                    await AddAsync(profile);
                }
                else
                {
                    if (profile != null)
                    {
                        if (string.IsNullOrEmpty(imagePath))
                        {
                            await DeleteEntityAsync(new { Id = profile.Id });
                        }
                        else
                        {
                            profile.photoPath = imagePath;
                            profile.updatedDate = DateTime.UtcNow;
                            await UpdateAsync(profile, new { Id = profile.Id });
                        }
                    }
                }
                saveUpdateResult = true;
                return saveUpdateResult;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<UploadFileResponseDto> UploadImage(IFormFile file)
        {
            return await WriteFile(file).ConfigureAwait(false);
        }

        public bool CheckIfImageFile(IFormFile file)
        {

            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
            }
            return ImageFormat.GetImageFormat(fileBytes) != ImageFormat.ImageFormatEnum.unknown;
        }

        public async Task<UploadFileResponseDto> WriteFile(IFormFile file)
        {

            try
            {
                var extension = "." + file.FileName.Split(".")[file.FileName.Split(".").Length - 1];
                string fileName = Guid.NewGuid().ToString() + extension;
                var path = Path.Combine(Directory.GetCurrentDirectory(), GeneralConstants.UploadFolder, fileName);
                using (var bits = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(bits);
                }
                return new UploadFileResponseDto
                {
                    IsUploaded = true,
                    MessagePath = string.Concat(_configuration["ServiceAPIURL"], GeneralConstants.UploadFolder.Replace("wwwroot", "").Replace("\\", ""), "/", fileName)
                };
            }
            catch (Exception ex)
            {
                return new UploadFileResponseDto
                {
                    IsUploaded = false,
                    MessagePath = ex.Message
                };

            }
        }

        public async Task<int> RemoveImage(PhotosDto model)
        {
            var result = 0;
            try
            {
                result = await DeleteEntityAsync(new { entityId = model.entityId, photoType = model.photoType });
                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }

        public async Task<string> GetAWSBucketFilUrl(string fileName, string defaultFile)
        {
            try
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    var url = string.Empty;
                    var settings = await GetAWSBucketGeneralSettings();
                    AmazonS3Client s3Client = new AmazonS3Client(settings[0].Value, settings[1].Value, bucketRegion);
                    GetPreSignedUrlRequest request = new GetPreSignedUrlRequest();
                    request.BucketName = settings[2].Value;
                    request.Key = fileName;
                    request.Expires = DateTime.Now.AddMinutes(Convert.ToDouble(settings[4].Value));
                    request.Protocol = Protocol.HTTP;
                    url = s3Client.GetPreSignedURL(request);
                    return url;
                }
                else { return defaultFile; }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public async Task<PhotosExpirationDto> GetAWSBucketFileUrlWithExpiration(int userId, int entityPhotyType)
        {
            try
            {
                var photos = await GetDataByIdAsync(new { entityId = userId, photoType = entityPhotyType });
                if (photos != null && !string.IsNullOrEmpty(photos.photoPath))
                {
                    var url = string.Empty;
                    var settings = await GetAWSBucketGeneralSettings();
                    AmazonS3Client s3Client = new AmazonS3Client(settings[0].Value, settings[1].Value, bucketRegion);
                    GetPreSignedUrlRequest request = new GetPreSignedUrlRequest();
                    request.BucketName = settings[2].Value;
                    request.Key = photos.photoPath;
                    request.Expires = DateTime.Now.AddMinutes(Convert.ToDouble(settings[4].Value));
                    request.Protocol = Protocol.HTTP;
                    url = s3Client.GetPreSignedURL(request);
                    var photoResult = new PhotosExpirationDto
                    {
                        photoPath = url,
                        createdDate = DateTime.Now,
                        expirationDate = DateTime.Now.AddHours(24)
                    };
                    return photoResult;
                }
                else
                {
                    var photoResult = new PhotosExpirationDto
                    {
                        photoPath = entityPhotyType == 1 ? string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.UserDefaultImage) : string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.OtherDefaultImage),
                        createdDate = DateTime.Now,
                        expirationDate = DateTime.Now.AddHours(24)
                    };
                    return photoResult;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<List<GeneralSetting>> GetAWSBucketGeneralSettings()
        {
            return (await _generalRepository.GetDataAsync(new { keyGroup = MessagesConstants.S3Bucket })).ToList();
        }
    }
}
