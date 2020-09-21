using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using AutoMapper;
using ElmahCore;
using Foundry.Domain;
using Foundry.Domain.ActionFilters;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using Foundry.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Foundry.Domain.Constants;

namespace Foundry.Api.Controllers
{
    /// <summary>
    /// This Class is used to include methods for uploading and removing images. 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IPhotos _entityPhotos;
        private static readonly FormOptions _defaultFormOptions = new FormOptions();
        private readonly IMapper _mapper;
        private readonly IGeneralSettingService _generalRepository;
        private readonly RegionEndpoint bucketRegion = RegionEndpoint.CACentral1;
        private readonly IUserRepository _userRepository;
        private readonly ApiResponse someIssueInProcessing = new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing);
        /// <summary>
        /// Constructor for injecting services with APIs.
        /// </summary>
        /// <param name="photos"></param>
        /// <param name="mapper"></param>
        /// <param name="generalRepository"></param>
        /// <param name="userRepository"></param>
        public ImageController(IPhotos photos, IMapper mapper, IGeneralSettingService generalRepository, IUserRepository userRepository)
        {
            _entityPhotos = photos;
            _mapper = mapper;
            _generalRepository = generalRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// This API will upload file in the server.
        /// </summary>
        /// <param name="fileForUploadImage"></param>
        /// <returns>ApiResponse</returns>
        [Route("Upload")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadImage(IFormFile fileForUploadImage)
        {
            try
            {
                if (fileForUploadImage == null)
                {
                    return BadRequest(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest, false, MessagesConstants.UploadedFileEmpty));
                }
                /* Upload file inside S3 bucket */
                var uploadResultForUploadImage = await UploadToS3(fileForUploadImage);
                if (!uploadResultForUploadImage.IsUploaded)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, uploadResultForUploadImage?.MessagePath));
                }

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.ImageUploadedSuccessfully, uploadResultForUploadImage.MessagePath));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Image := UploadImage)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok();
            }
        }

        /// <summary>
        /// This Api is called to post an images using request header.
        /// </summary>
        /// <returns></returns>
        [Route("PostImage")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostImage()
        {
            if (Request.HasFormContentType)
            {
                var form = Request.Form;

                /* Upload file inside S3 bucket */
                var uploadResult = await UploadToS3(form.Files[0]);
                if (uploadResult == null || !uploadResult.IsUploaded)
                {
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, uploadResult?.MessagePath));
                }

                return Ok(new ApiResponse(StatusCodes.Status200OK, true, MessagesConstants.ImageUploadedSuccessfully, uploadResult.MessagePath, 1));
            }
            return Ok(new ApiResponse(StatusCodes.Status200OK, true, MessagesConstants.UploadedFileEmpty, ""));

        }

        /// <summary>
        /// This API will upload file in the server.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>ApiResponse</returns>
        [Route("UploadImageInsideApp")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadImageInsideApp(IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    return BadRequest(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest, false, MessagesConstants.UploadedFileEmpty));
                }
                /* Upload file inside S3 bucket */
                var uploadResult = await UploadToS3(file);
                if (!uploadResult.IsUploaded)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, uploadResult?.MessagePath));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.ImageUploadedSuccessfully, uploadResult.MessagePath));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Image := UploadImageInsideApp)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This API will upload file in the server.
        /// </summary>
        /// <returns></returns>
        [Route("UploadLarge")]
        [HttpPost]
        [DisableFormValueModelBinding]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> UploadImageLarge()
        {
            try
            {
                if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
                {
                    return BadRequest(string.Format(MessagesConstants.ExpectedDifferentRequest, Request.ContentType));
                }
                // Used to accumulate all the form url encoded key value pairs in the 
                // request.
                var formAccumulator = new KeyValueAccumulator();
                string targetFilePath = null;

                var boundary = MultipartRequestHelper.GetBoundary(
                    MediaTypeHeaderValue.Parse(Request.ContentType),
                    _defaultFormOptions.MultipartBoundaryLengthLimit);
                var reader = new MultipartReader(boundary, HttpContext.Request.Body);

                var section = await reader.ReadNextSectionAsync();
                while (section != null)
                {
                    ContentDispositionHeaderValue contentDisposition;
                    var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

                    if (hasContentDispositionHeader)
                    {
                        if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                        {
                            targetFilePath = Path.GetTempFileName();
                            using (var targetStream = System.IO.File.Create(targetFilePath))
                            {
                                await section.Body.CopyToAsync(targetStream);
                            }
                        }
                        else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                        {
                            formAccumulator = await UploadImageLargeRefactor(formAccumulator, section, contentDisposition);
                        }
                    }

                    // Drains any remaining section body that has not been consumed and
                    // reads the headers for the next section.
                    section = await reader.ReadNextSectionAsync();
                }

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, "Image Uploaded Successfully.", targetFilePath));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Image := UploadImageLarge)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        private static async Task<KeyValueAccumulator> UploadImageLargeRefactor(KeyValueAccumulator formAccumulator, MultipartSection section, ContentDispositionHeaderValue contentDisposition)
        {
            // Do not limit the key name length here because the 
            // multipart headers length limit is already in effect.
            var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name);
            var encoding = MultipartRequestHelper.GetEncoding(section);
            using (var streamReader = new StreamReader(
                section.Body,
                encoding,
                detectEncodingFromByteOrderMarks: true,
                bufferSize: 1024,
                leaveOpen: true))
            {
                // The value length limit is enforced by MultipartBodyLengthLimit
                var value = await streamReader.ReadToEndAsync().ConfigureAwait(false);

                if (String.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                {
                    value = String.Empty;
                }
                formAccumulator.Append(key.ToString(), value);

                if (formAccumulator.ValueCount > _defaultFormOptions.ValueCountLimit)
                {
                    throw new InvalidDataException(string.Format(MessagesConstants.FormCountLimitExceeded, _defaultFormOptions.ValueCountLimit));
                }
            }

            return formAccumulator;
        }

        /// <summary>
        /// This Api is called to upload an image using base 64.
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [Route("UploadBase")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadBase(FileuploadModel inputModel)
        {
            string resultForUploadBase = string.Empty;
            bool flagForUploadBase = false;
            string messageForUploadBase = string.Empty;
            if (!string.IsNullOrEmpty(inputModel.ImageUrl) && inputModel.ImageUrl.Substring(0, 4) == "data")
            {
                try
                {
                    //Byte data
                    byte[] imageByteUploadBase;
                    string extensionUploadBase = inputModel.ImageUrl.Split(',')[0].Split(';')[0].Split('/')[1];
                    string imgDataUploadBase = inputModel.ImageUrl.Split(',')[1];
                    string nameUploadBase = Guid.NewGuid().ToString() + "." + extensionUploadBase;
                    extensionUploadBase = extensionUploadBase.ToLower(CultureInfo.InvariantCulture);

                    if (extensionUploadBase == "png" || extensionUploadBase == "jpg" || extensionUploadBase == "jpeg" || extensionUploadBase == "tiff")
                    {
                        imageByteUploadBase = CommonHelper.Base64ToImage(imgDataUploadBase);
                        /* Upload file inside S3 bucket */
                        var uploadResult = await UploadToS3Base64(imageByteUploadBase, nameUploadBase, extensionUploadBase);
                        if (uploadResult != null)
                        {
                            inputModel.ImageUrl = uploadResult.MessagePath;
                            flagForUploadBase = true;
                            messageForUploadBase = MessagesConstants.ImageUploadedSuccessfully;
                        }
                        else
                        {
                            inputModel.ImageUrl = resultForUploadBase;
                            flagForUploadBase = false;
                            messageForUploadBase = MessagesConstants.ImageSavingIssue;
                        }
                    }
                    else
                    {
                        inputModel.ImageUrl = resultForUploadBase;
                        flagForUploadBase = false;
                        messageForUploadBase = "Not a valid image type.";
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.RiseError(new Exception(string.Concat("API := (Image := UploadBase)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                    return Ok(new { result = inputModel.ImageUrl, success = false, msg = ex.Message + " " + ex.InnerException });
                }
            }
            return Ok(new { result = inputModel.ImageUrl, success = flagForUploadBase, msg = messageForUploadBase });
        }

        /// <summary>
        /// This Api is called to uplaod an image using base64.
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [Route("UploadBaseInsideApp")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadBaseInsideApp(FileuploadModel inputModel)
        {
            string result = string.Empty;
            bool flag = false;
            string message = string.Empty;
            if (!string.IsNullOrEmpty(inputModel.ImageUrl) && inputModel.ImageUrl.Substring(0, 4) == "data")
            {
                try
                {
                    //Byte data
                    byte[] imageByte;
                    string extension = inputModel.ImageUrl.Split(',')[0].Split(';')[0].Split('/')[1];
                    string imgData = inputModel.ImageUrl.Split(',')[1];
                    string name = Guid.NewGuid().ToString() + "." + extension;
                    extension = extension.ToLower(CultureInfo.InvariantCulture);
                    if (extension == "png" || extension == "jpg" || extension == "jpeg" || extension == "tiff")
                    {
                        imageByte = CommonHelper.Base64ToImage(imgData);
                        /* Upload file inside S3 bucket */
                        var uploadResult = await UploadToS3Base64(imageByte, name, extension);
                        if (uploadResult != null)
                        {
                            inputModel.ImageUrl = uploadResult.MessagePath;
                            flag = true;
                            message = MessagesConstants.ImageUploadedSuccessfully;
                        }
                        else
                        {
                            inputModel.ImageUrl = result;
                            flag = false;
                            message = MessagesConstants.ImageSavingIssue;
                        }
                    }
                    else
                    {
                        inputModel.ImageUrl = result;
                        flag = false;
                        message = "Not a valid image type.";
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.RiseError(new Exception(string.Concat("API := (Image := UploadBaseInsideApp)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                    return Ok(new { result = inputModel.ImageUrl, success = false, msg = ex.Message + " " + ex.InnerException });
                }

            }
            return Ok(new { result = inputModel.ImageUrl, success = flag, msg = message });
        }

        /// <summary>
        /// This Api is called to remove image from database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("RemoveImage")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveImageFromDB(PhotosDto model)
        {
            var result = 0;
            try
            {
                var photoPath = Path.GetFileName(model.photoPath);
                result = await _entityPhotos.RemoveImage(new PhotosDto { entityId = model.entityId, photoType = model.photoType, photoPath = model.photoPath });
                await DeleteObjectNonVersionedBucketAsync(photoPath);
                if (result > 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.ImageDeletedSuccessfully, result));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.ImageNotDeletedSuccessfully, result));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Image := RemoveImage)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);

            }
        }

        /// <summary>
        /// This Api is called to get image from S3.
        /// </summary>
        /// <returns></returns>
        [Route("S3ImageWithExpiry")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> GetS3ImageWithExpiry()
        {
            try
            {
                var identityForS3Image = User.Identity as ClaimsIdentity;
                var userIdClaimForS3Image = Convert.ToInt32(identityForS3Image.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimForS3Image = identityForS3Image.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimForS3Image, sessionIdClaimForS3Image)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                var resultForS3Image = await _entityPhotos.GetAWSBucketFileUrlWithExpiration(userIdClaimForS3Image, 1);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.ImageReturnedSuccessfully, resultForS3Image));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Image := RemoveImage)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }


        /// <summary>
        /// This API is called to get the Presigned URL
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [Route("S3ImageForFileName")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPreSignedURLFromFileName(string fileName)
        {
            try
            {
                var result = await _entityPhotos.GetAWSBucketFilUrl(fileName, null);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.ImageReturnedSuccessfully, result));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Image := RemoveImage)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(someIssueInProcessing);
            }
        }

        /// <summary>
        /// This method is used to upload the s3 file using services.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [NonAction]
        private async Task<UploadFileResponseDto> UploadToS3(IFormFile file)
        {
            var generalSettings = await GetAWSBucketGeneralSettings();
            string actualFileName = String.Empty, extension = String.Empty, fileName = String.Empty;
            try
            {
                //S3 configuration
                string keyName = generalSettings[0].Value;
                string awsSecretAccessKey = generalSettings[1].Value;
                string AWSBucketName = generalSettings[2].Value;

                AmazonS3Client s3Client = new AmazonS3Client(keyName, awsSecretAccessKey, bucketRegion);
                var fileTransferUtility = new TransferUtility(s3Client);
                if (file == null)
                {
                    return new UploadFileResponseDto();
                }
                actualFileName = file.FileName;
                extension = Path.GetExtension(actualFileName);
                var a = Path.GetFileNameWithoutExtension(actualFileName);
                fileName = GenerateFileName(a);
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    try
                    {
                        await fileTransferUtility.UploadAsync(memoryStream, AWSBucketName, fileName + extension);
                    }
                    catch (Exception)
                    {
                        return new UploadFileResponseDto();
                    }
                }
                /* Get recent save file url from S3Bucket */
                string filePath = fileName + extension;
                if (!string.IsNullOrEmpty(filePath))
                {
                    return new UploadFileResponseDto
                    {
                        IsUploaded = true,
                        MessagePath = filePath
                    };
                }
                else
                {
                    return new UploadFileResponseDto
                    {
                        IsUploaded = false,
                        MessagePath = "Something Wrong"
                    };
                }
            }
            catch (Exception ex)
            {
                return new UploadFileResponseDto()
                {
                    IsUploaded = false,
                    MessagePath = ex.Message
                };
            }
        }

        /// <summary>
        /// This method is used to upload images in s3 which are in base 64 format.
        /// </summary>
        /// <param name="fileByte"></param>
        /// <param name="fileName"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<UploadFileResponseDto> UploadToS3Base64(byte[] fileByte, string fileName, string extension)
        {
            var generalSettings = await GetAWSBucketGeneralSettings();
            try
            {
                //S3 configuration
                string keyName = generalSettings[0].Value;
                string awsSecretAccessKey = generalSettings[1].Value;
                string AWSBucketName = generalSettings[2].Value;
                using (var s3Client = new AmazonS3Client(keyName, awsSecretAccessKey, bucketRegion))
                {
                    var request = new PutObjectRequest
                    {
                        BucketName = AWSBucketName,
                        CannedACL = S3CannedACL.PublicRead,
                        Key = string.Format("{0}", fileName)
                    };
                    using (var ms = new MemoryStream(fileByte))
                    {
                        request.InputStream = ms;
                        await s3Client.PutObjectAsync(request);
                    }
                    /* Get recent save file url from S3Bucket */
                    string filePath = fileName;
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        return new UploadFileResponseDto
                        {
                            IsUploaded = true,
                            MessagePath = filePath
                        };
                    }
                    else
                    {
                        return new UploadFileResponseDto
                        {
                            IsUploaded = false,
                            MessagePath = "Something Wrong"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new UploadFileResponseDto()
                {
                    IsUploaded = false,
                    MessagePath = ex.Message
                };
            }
        }



        /// <summary>
        /// Delete file from S3 Bucket
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        // [HttpGet("DeleteImage")]
        [NonAction]
        public async Task<IActionResult> DeleteObjectNonVersionedBucketAsync(string key)
        {
            try
            {
                /* Get Aws general settings */
                var settings = await GetAWSBucketGeneralSettings();
                if (settings.Count <= 0)
                {
                    return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.AWSSetting));
                }
                //S3 configuration
                string keyName = settings[0].Value;
                string awsSecretAccessKey = settings[1].Value;
                string AWSBucketName = settings[2].Value;
                AmazonS3Client client = new AmazonS3Client(keyName, awsSecretAccessKey, bucketRegion);
                var response = await client.DeleteObjectAsync(new DeleteObjectRequest() { BucketName = AWSBucketName, Key = key });

                if (response.HttpStatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return Ok(new ApiResponse(StatusCodes.Status200OK, true, MessagesConstants.FileDeleteSuccessfully));
                }
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.NoErrorMessagesExist));
            }
            catch (AmazonS3Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Image := DeleteImage)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        private async Task<List<GeneralSetting>> GetAWSBucketGeneralSettings()
        {
            var settings = await _generalRepository.GetDataAsync(new { keyGroup = MessagesConstants.S3Bucket });
            return _mapper.Map<List<GeneralSetting>>(settings);
        }

        private string GenerateFileName(string fileName)
        {
            Random generator = new Random();
            return fileName + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + generator.Next(0, 9999).ToString("D4");

        }
    }
}