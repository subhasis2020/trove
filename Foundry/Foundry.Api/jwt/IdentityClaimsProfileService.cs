using Foundry.Domain.DbModel;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Foundry.Services;
using static Foundry.Domain.Constants;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Foundry.LogService;

namespace Foundry.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class IdentityClaimsProfileService : IProfileService
    {
        private readonly ILoggerManager _logger;
        private readonly IUserClaimsPrincipalFactory<User> _claimsFactory;
        private readonly UserManager<User> _userManager;
        private readonly IPrograms _program;
        private readonly IPhotos _entityPhotos;
        private readonly IConfiguration _configuration;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="claimsFactory"></param>
        /// <param name="program"></param>
        /// <param name="photos"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public IdentityClaimsProfileService(UserManager<User> userManager, IUserClaimsPrincipalFactory<User> claimsFactory, IPrograms program, IPhotos photos
            , IConfiguration configuration, ILoggerManager logger)
        {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
            _program = program;
            _entityPhotos = photos;
            _configuration = configuration;
            _logger = logger;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            try
            {
                var sub = context.Subject.GetSubjectId();
                var user = await _userManager.FindByIdAsync(sub);
                var principal = await _claimsFactory.CreateAsync(user);
                var roles = await _userManager.GetRolesAsync(user);
                var claims = principal.Claims.ToList();
                var program = await _program.GetUserProgramByUserId(Convert.ToInt32(sub));
                var userImage = await _entityPhotos.GetEntityPhotoPath(Convert.ToInt32(sub), (int)Foundry.Domain.PhotoEntityType.UserProfile, Convert.ToInt32(sub));
                string imagePath = string.Empty;
                if (userImage != null)
                {
                    imagePath = userImage?.photoPath ?? string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.UserDefaultImage);
                }
                claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();
                claims.Add(new Claim(ClaimTypes.Name, string.Concat(user.FirstName, " ", user.LastName)));
                claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, string.Concat(user.FirstName, " ", user.LastName)));
                claims.Add(new Claim(IdentityServerConstants.StandardScopes.Email, user.Email));
                foreach (string role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                //add our custom claims
                claims.Add(new Claim("programId", program != null ? program.Id.ToString() : ""));
                claims.Add(new Claim("isProgramLinkedWithUser", program != null ? program.IsProgramLinkedWithUser.ToString() : ""));
                claims.Add(new Claim("programName", program != null ? program.Name.ToString() : ""));
                claims.Add(new Claim("logoPath", program != null ? program.LogoPath.ToString() : ""));
                claims.Add(new Claim("userId", user?.Id.ToString()));
                claims.Add(new Claim("organistionId", user?.OrganisationId.ToString()));
                claims.Add(new Claim(JwtClaimTypes.Picture, imagePath));
                claims.Add(new Claim("lastName", user.LastName));
                claims.Add(new Claim("firstName", user.FirstName));
                claims.Add(new Claim("isUserActive", user.IsActive.ToString()));
                if (!string.IsNullOrEmpty(user.UserCode))
                    claims.Add(new Claim("UserUniqueId", user.UserCode));
                if (user.SessionId != null)
                {
                    claims.Add(new Claim("sessionMobileId", user.SessionId));
                }
                claims.Add(new Claim("jposAccountHolderId", (!string.IsNullOrEmpty(user.JPOS_AccoutHolderId) ? user.JPOS_AccoutHolderId : "0")));
                claims.Add(new Claim("UserDeviceId", (!string.IsNullOrEmpty(user.UserDeviceId) ? user.UserDeviceId : "")));
                claims.Add(new Claim("UserDeviceType", (!string.IsNullOrEmpty(user.UserDeviceType) ? user.UserDeviceType : "")));
                context.IssuedClaims = claims;
            }
            catch (Exception ex)
            {
                _logger.LogInfo(ex.Message + " " + ex.InnerException);
            }


        }

        /// <summary>
        /// This method is called when the user data needs to fetch for active or inactive.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}
