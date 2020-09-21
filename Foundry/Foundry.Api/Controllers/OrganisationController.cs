using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using ElmahCore;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
using Foundry.LogService;
using Foundry.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using dbModel = Foundry.Domain.DbModel;
using static Foundry.Domain.Constants;
using Foundry.Api.Attributes;
using System.Globalization;
using Foundry.Api.Models;

namespace Foundry.Api.Controllers
{
    /// <summary>
    /// This Api is called to include methods for Organisations and merchants.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrganisationController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IOrganisation _organisation;
        private readonly IUserFavoriteService _userFavorite;
        private readonly IOrganisationProgram _organisationProgram;
        private readonly IOrganisationSchedule _organisationSchedule;
        private readonly IRoleRepository _roleRepository;
        private readonly IProgramTypeService _programType;
        private readonly IOfferCodeService _offerCodeService;
        private readonly IMerchantTerminals _merchantTerminals;
        private readonly IMealPeriodService _mealPeriod;
        private readonly IPromotions _promotions;
        private readonly IProgramMerchantAccountTypeService _programMerchantAccountTypeService;
        private readonly IPhotos _photos;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IGeneralSettingService _setting;
        private readonly IUserNotificationSettingsService _userNotificationSettingsService;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserPushedNotificationService _userPushedNotificationService;
        private readonly IPrograms _program;
        private readonly ISharedJPOSService _sharedJPOSService;

        /// <summary>
        /// Constructor for injecting services with APIs.
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="organisation"></param>
        /// <param name="userFavorite"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        /// <param name="organisationProgram"></param>
        /// <param name="organisationSchedule"></param>
        /// <param name="roleRepository"></param>
        /// <param name="programType"></param>
        /// <param name="offerCodeService"></param>
        /// <param name="promotions"></param>
        /// <param name="merchantTerminals"></param>
        /// <param name="mealPeriod"></param>
        /// <param name="photos"></param>
        /// <param name="programMerchantAccountTypeService"></param>
        /// <param name="setting"></param>
        /// <param name="userNotificationSettingsService"></param>
        /// <param name="userRoleRepository"></param>
        /// <param name="userPushedNotificationService"></param>
        /// <param name="program"></param>
        /// <param name="sharedJPOSService"></param>
        public OrganisationController(IUserRepository userRepository, IOrganisation organisation, IUserFavoriteService userFavorite, ILoggerManager logger,
            IMapper mapper, IOrganisationProgram organisationProgram, IOrganisationSchedule organisationSchedule, IRoleRepository roleRepository, IProgramTypeService programType,
            IOfferCodeService offerCodeService, IPromotions promotions, IMerchantTerminals merchantTerminals, IMealPeriodService mealPeriod,
            IPhotos photos, IProgramMerchantAccountTypeService programMerchantAccountTypeService, IGeneralSettingService setting,
            IUserNotificationSettingsService userNotificationSettingsService, IUserRoleRepository userRoleRepository, IUserPushedNotificationService userPushedNotificationService,
            IPrograms program, ISharedJPOSService sharedJPOSService)

        {
            _userRepository = userRepository;
            _organisation = organisation;
            _userFavorite = userFavorite;
            _logger = logger;
            _mapper = mapper;
            _organisationProgram = organisationProgram;
            _organisationSchedule = organisationSchedule;
            _roleRepository = roleRepository;
            _programType = programType;
            _offerCodeService = offerCodeService;
            _merchantTerminals = merchantTerminals;
            _mealPeriod = mealPeriod;
            _promotions = promotions;
            _photos = photos;
            _programMerchantAccountTypeService = programMerchantAccountTypeService;
            _setting = setting;
            _userNotificationSettingsService = userNotificationSettingsService;
            _userRoleRepository = userRoleRepository;
            _userPushedNotificationService = userPushedNotificationService;
            _program = program;
            _sharedJPOSService = sharedJPOSService;
        }

        /// <summary>
        /// This Api is used to get all organisation(merchants) comes under the program on which the user falls.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("GetOrganisation")]
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> GetOrganisation(OrganisationModel model)
        {
            try
            {
                var userIdClaimForGetOrg = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimForGetOrg = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                var programIdClaimForGetOrg = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimForGetOrg, sessionIdClaimForGetOrg)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                if (!string.IsNullOrEmpty(model.Location))
                {
                    var locationShuffle = model.Location.Split(',');
                    if (locationShuffle.Count() > 1)
                        model.Location = string.Concat(locationShuffle[1], ",", locationShuffle[0]);
                }

                /* Get all the relations to return. */
                var lstOrganisationForGetOrg = await _organisation.GetOrganisation(model.AccuntTypeId, userIdClaimForGetOrg, model.Location, programIdClaimForGetOrg);
                if (lstOrganisationForGetOrg.Count <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoRestaurantsExist));
                }
                /* Get all the remaining meals to return. */
                var remainingMeals = await _organisation.GetRemainingMeals(userIdClaimForGetOrg, programIdClaimForGetOrg);

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, lstOrganisationForGetOrg, 0, remainingMeals.ToString()));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := GetOrganisation)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException, " Parameters: AccountTypeId:=", model.AccuntTypeId, ", offerId:=", model.OfferId, ", UserId:=", model.UserId, ",ProgramId:=", model.ProgramId, ", id:=", model.Id, ", SessionId:=", model.SessionId, ", Location:=", model.Location)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is used to get organisation(mercchant) detail of the user based on its id and other details.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("GetOrganisationDetail")]
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> GetOrganisationDetail(OrganisationModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var userIdClaimForGetOrgDetail = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimForGetOrgDetail = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                var programIdClaimForGetOrgDetail = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimForGetOrgDetail, sessionIdClaimForGetOrgDetail)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                /* Get all the relations to return. */
                var organisationdetailForGetOrgDetail = await _organisation.GetOrganisationDetails(model.Id, programIdClaimForGetOrgDetail, userIdClaimForGetOrgDetail, model.AccuntTypeId);
                if (organisationdetailForGetOrgDetail == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoRestaurantsExist));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, "Data is succefully returned.", organisationdetailForGetOrgDetail));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := GetOrganisationDetail)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException, " Parameters: AccountTypeId:=", model.AccuntTypeId, ", offerId:=", model.OfferId, ", UserId:=", model.UserId, ",ProgramId:=", model.ProgramId, ", id:=", model.Id, ", SessionId:=", model.SessionId, ", Location:=", model.Location)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is used to get the remaining meals of the user based on user transactions.
        /// </summary>
        /// <param name="model">OrganisationModel</param>
        /// <returns></returns>
        [Route("GetRemainingMeals")]
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> GetRemainingMeals(OrganisationModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var userIdClaimRemainingMeals = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimRemainingMeals = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                var programIdClaimRemainingMeals = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimRemainingMeals, sessionIdClaimRemainingMeals)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                /* Get all the relations to return. */
                var remainingMealRemainingMeals = await _organisation.GetRemainingMeals(userIdClaimRemainingMeals, programIdClaimRemainingMeals);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.UserRemainingMealsReturned, Math.Round(remainingMealRemainingMeals ?? 0, 0)));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := GetRemainingMeals)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is used to get the promotion detail of the merchant based on id.
        /// </summary>
        /// <param name="model">OrganisationModel</param>
        /// <returns></returns>
        [Route("GetOrganisationPromotionDetail")]
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> GetOrganisationPromotionDetail(OrganisationModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var userIdClaimOrgPromotionDetail = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimOrgPromotionDetail = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                var programIdClaimOrgPromotionDetail = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimOrgPromotionDetail, sessionIdClaimOrgPromotionDetail)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                /* Get all the relations to return. */
                var organisatoiondetailOrgPromotionDetail = await _organisation.GetOrganisationPromotionDetails(model.Id, model.OfferId, programIdClaimOrgPromotionDetail, userIdClaimOrgPromotionDetail, model.AccuntTypeId);
                if (organisatoiondetailOrgPromotionDetail == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoOffersExist));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, organisatoiondetailOrgPromotionDetail));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := GetOrganisationPromotionDetail)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException, " Parameters: AccountTypeId:=", model.AccuntTypeId, ", offerId:=", model.OfferId, ", UserId:=", model.UserId, ",ProgramId:=", model.ProgramId, ", id:=", model.Id, ", SessionId:=", model.SessionId, ", Location:=", model.Location)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is used to get all merchants promotions which are currently active.
        /// </summary>
        /// <param name="model">OrganisationModel</param>
        /// <returns></returns>
        [Route("GetMerchantsPromotions")]
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> GetMerchantsPromotions(OrganisationModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var userIdClaimForMerchantsPromotions = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimForMerchantsPromotions = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                var programIdClaimForMerchantsPromotions = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "programId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimForMerchantsPromotions, sessionIdClaimForMerchantsPromotions)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                if (!string.IsNullOrEmpty(model.Location))
                {
                    var locationShuffle = model.Location.Split(',');
                    if (locationShuffle.Count() > 1)
                        model.Location = string.Concat(locationShuffle[1], ",", locationShuffle[0]);
                }
                /* Get all the relations to return. */
                var organisatoiondetailForMerchantsPromotions = await _organisation.GetOffersOfMerchants(model.AccuntTypeId, userIdClaimForMerchantsPromotions, model.Location, programIdClaimForMerchantsPromotions);
                if (organisatoiondetailForMerchantsPromotions.Count <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoOffersExist));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.OffersReturnedSuccessfully, organisatoiondetailForMerchantsPromotions));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := GetMerchantsPromotions)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException, " Parameters: AccountTypeId:=", model.AccuntTypeId, ", offerId:=", model.OfferId, ", UserId:=", model.UserId, ",ProgramId:=", model.ProgramId, ", id:=", model.Id, ", SessionId:=", model.SessionId, ", Location:=", model.Location)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is used to mark favorite to the organisation(merchant).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("UserFavoriteOrganisation")]
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> UserFavoriteOrganisation(UserFavoriteModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var userIdClaimFavOrg = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimFavOrg = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;

                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimFavOrg, sessionIdClaimFavOrg)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                /* Add Update User Favorite. */
                model.UserId = userIdClaimFavOrg;
                var userFavoriteFavOrg = await _userFavorite.AddUpdateUserFavorite(model);
                if (userFavoriteFavOrg <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.RestauretMadeFavorite, model.IsFavorite, userFavoriteFavOrg));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := UserFavoriteOrganisation)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is used to get help details of the organisation.
        /// </summary>
        /// <returns></returns>
        [Route("GetHelp")]
        [HttpGet]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> GetOrganisationHelpDetails()
        {
            try
            {
                var userIdClaimForOrganisationHelpDetails = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimForOrganisationHelpDetails = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                var organisationIdClaimForOrganisationHelpDetails = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "organistionId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimForOrganisationHelpDetails, sessionIdClaimForOrganisationHelpDetails)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                /* Get Organisation Details. */
                var orgDetailsForOrganisationHelpDetails = _mapper.Map<OrganisationGetHelpDto>(await _organisation.GetOrganisationDetailsById(organisationIdClaimForOrganisationHelpDetails));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, orgDetailsForOrganisationHelpDetails));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := GetOrganisationHelpDetails)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is used to get the terms and condition of the organisation.
        /// </summary>
        /// <returns></returns>
        [Route("TermsAndCondition")]
        [HttpGet]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> GetTermsAndCondition()
        {
            try
            {
                var userIdClaimTermandCondition = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var sessionIdClaimClaimTermandCondition = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "sessionMobileId".ToLower(CultureInfo.InvariantCulture).Trim()).Value;
                /* Checks the session of the user against its Id. */
                if (string.IsNullOrEmpty(await _userRepository.CheckSessionId(userIdClaimTermandCondition, sessionIdClaimClaimTermandCondition)))
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoSessionMatchExist, null, 0, "", true));
                }
                /* Get Organisation Details. */
                var orgDetailsTermandCondition = _mapper.Map<OrganisationDto>(await _organisation.GetMasterOrganisation());
                if (orgDetailsTermandCondition == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, orgDetailsTermandCondition.Description));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := GetTermsAndCondition)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is used to get organisation list on web.
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        [Route("Organisations")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetOrganisationsList(string searchText)
        {
            try
            {
                var userIdClaim = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var roleUser = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role".ToLower(CultureInfo.InvariantCulture).Trim()).Value.ToLower(CultureInfo.InvariantCulture).Trim();
                /* Get All Organisations. */
                var orgDetails = _mapper.Map<List<OrganisationDto>>(await _organisation.GetOrganisationsListByTypeWithSearch(OrganasationType.University, true, false, searchText, roleUser, userIdClaim)).ToList();
                if (orgDetails.Count <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned, null, 0));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, orgDetails, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := GetOrganisationsList)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is used to get organisation's programs list.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <returns></returns>
        [Route("OrganisationPrograms")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetOrganisationProgramsList(int organisationId)
        {
            try
            {
                var userIdClaimForProgLst = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var roleUser = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role".ToLower(CultureInfo.InvariantCulture).Trim()).Value.ToLower(CultureInfo.InvariantCulture).Trim();
                /* Get All Organisations. */
                var orgProgramsDetailsForProgLst = (await _organisationProgram.GetOrganisationPrograms(organisationId, roleUser, userIdClaimForProgLst)).ToList();
                if (orgProgramsDetailsForProgLst.Count <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, orgProgramsDetailsForProgLst, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := GetOrganisationProgramsList)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is used to delete organisation program.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteOrganisationProgram")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteOrganisationProgram(OrganisationProgramDBDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                /* Get All Organisations. */
                var DeleteProgramsDetail = await _organisationProgram.DeleteOrganisationProgram(model.organisationId, model.programId.Value);
                if (DeleteProgramsDetail <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.OrganisationProgramNotDeletedSuccessfully));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.OrganisationProgramDeletedSuccessfully, DeleteProgramsDetail, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := DeleteOrganisationProgram)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is used to get the details of the organisation and master program types.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <returns></returns>
        [Route("OrganisationInfoNProgramTypes")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetOrganisationInfoNProgramTypes(int organisationId)
        {
            try
            {
                /* Get All Organisations. */
                var orgProgramsDetailsNProgramTypes = await _organisation.GetOrganisationInfoWithProgramTypes(OrganasationType.University, organisationId);
                if (orgProgramsDetailsNProgramTypes == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, orgProgramsDetailsNProgramTypes, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := GetOrganisationInfoNProgramTypes)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is used to get organisation admin info list.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <returns></returns>
        [Route("OrganisationsAdmins")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetOrganisationsAdminsInfoList(int organisationId)
        {
            try
            {
                /* Get All Organisations. */
                var orgAdminDetailsForInfoList = await _organisation.GetOrganisationsAdminsList(organisationId);
                if (orgAdminDetailsForInfoList == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, orgAdminDetailsForInfoList, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := GetOrganisationsAdminsInfoList)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }


        /// <summary>
        /// This Api is used to get organisation admin info list.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <returns></returns>
        [Route("MerchantAdmins")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMerchantAdminsInfoList(int organisationId)
        {
            try
            {
                /* Get All Organisations. */
                var orgAdminDetails = await _organisation.GetMerchantAdminsList(organisationId);
                if (orgAdminDetails == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, orgAdminDetails, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := GetMerchantAdminsInfoList)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is used to update organisation status to Active or inactive.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("UpdateOrganisationAdminStatus")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateOrganisationAdminStatus(OrganisationAdminStatusDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                /* Get all the relations to return. */
                var updateStatusAdminStatus = await _organisation.UpdateOrganisationAdminStatus(model.UserId, model.isActive);
                if (updateStatusAdminStatus <= 0) { return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.OrganisationAdminStatusNotUpdatedSuccessfully)); }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.OrganisationAdminStatusUpdatedSuccessfully, updateStatusAdminStatus, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := UpdateOrganisationAdminStatus)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is used to add/update organisation from web.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("CreateModifyOrganisation")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUpdateOrganisation(OrganisationViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var result = await _organisation.AddEditOrganisation(model);
                if (result <= 0) { return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.OrganisationNotAddUpdatedSuccessfully)); }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.OrganisationAddUpdatedSuccessfully, result));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := AddUpdateOrganisation)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is used to add/update merchant schedule of working hours.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("CreatModifyOrganisationSchedule")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUpdateOrganisationSchedule(OrganisationScheduleModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var resultForOrgSchedule = await _organisationSchedule.AddEditOrganisationSchedule(model);
                if (resultForOrgSchedule <= 0) { return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.OrganisationScheduleNotAddUpdatedSuccessfully)); }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.OrganisationScheduleAddUpdatedSuccessfully, resultForOrgSchedule));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := AddUpdateOrganisationSchedule)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is used to add/update organisation business information.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("CreatModifyOrganisationBusinessInformation")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUpdateOrganisationBusinessInformation(OrganisationBusinessInformationModel model)
        {
            try
            {
                var result = 0;
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                model.Merchant.id = model.Id;
                result = await _organisation.AddEditOrganisationBusinessInfo(model.Merchant);
                if (result > 0)
                {
                    model.HoursOfOperation.ForEach(x => x.organisationId = result);
                    model.HolidayHours.ForEach(x => x.organisationId = result);
                    model.MerchantTerminal.ForEach(x => x.organisationId = result);
                    model.MealPeriod.ForEach(x => x.organisationId = result);
                    result = await _organisationSchedule.AddEditOrganisationSchedule(model.HoursOfOperation, false);
                    result = await _organisationSchedule.AddEditOrganisationSchedule(model.HolidayHours, true);
                    string clientIpAddressOrgBusInfo = Convert.ToString(HttpContext.Connection.RemoteIpAddress);
                    result = await _merchantTerminals.AddEditMerchantTerminal(model.MerchantTerminal, clientIpAddressOrgBusInfo);
                    result = await _mealPeriod.AddEditMealPeriod(model.MealPeriod);
                }

                if (result <= 0) { return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.OrganisationScheduleNotAddUpdatedSuccessfully)); }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.OrganisationScheduleAddUpdatedSuccessfully, result));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := AddUpdateOrganisationBusinessInformation)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is used to delete organisation.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteOrganisation")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteOrganisation(OrganisationDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                /* Get All Organisations. */
                var DeleteOrganisationDetail = await _organisation.DeleteOrganisationById(model.Id);
                if (DeleteOrganisationDetail <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.OrganisationNotDeletedSuccessfully));
                }
                if (!string.IsNullOrEmpty(model.JPOS_MerchantId))
                {
                    if (model.OrganisationType == OrganasationType.Merchant)
                    {
                        var oOrgJPOS = new MerchantJposDto()
                        {
                            active = false
                        };
                        await _sharedJPOSService.DeleteRespectiveDataJPOS(JPOSAPIURLConstants.Merchants, oOrgJPOS, model.JPOS_MerchantId, Convert.ToString(HttpContext.Connection.RemoteIpAddress), JPOSAPIConstants.Merchants);
                    }
                    else
                    {
                        var oOrgJPOS = new OrganizationJPOSDto()
                        {
                            active = false
                        };
                        await _sharedJPOSService.DeleteRespectiveDataJPOS(JPOSAPIURLConstants.Organization, oOrgJPOS, model.JPOS_MerchantId, Convert.ToString(HttpContext.Connection.RemoteIpAddress), JPOSAPIConstants.Organization);
                    }
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.OrganisationDeletedSuccessfully, DeleteOrganisationDetail, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := DeleteOrganisation)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is used to get master roles and organisation programs added.
        /// </summary>
        /// <param name="roleType"></param>
        /// <param name="organisationId"></param>
        /// <returns></returns>
        [Route("MasterRolesNProgramType")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMasterRolesNProgramType(int roleType, int organisationId)
        {
            try
            {
                MasterRoleNProgramTypeDto model = new MasterRoleNProgramTypeDto();
                var programTypesNProgramType = _mapper.Map<List<ProgramTypesDto>>((await _programType.CheckOrganisationProgramType(organisationId)).ToList());
                if (programTypesNProgramType.Count > 0)
                {
                    model.ProgramTypes = programTypesNProgramType;
                }

                var roles = _mapper.Map<List<RoleDto>>((await _roleRepository.GetRolesByRoleType(roleType)).ToList());
                if (roles.Count > 0)
                {
                    model.Roles = roles;
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.MasterRoleNProgramType, model));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := GetMasterRolesNProgramType)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is used to get master roles and organisation programs select.
        /// </summary>
        /// <param name="roleType"></param>
        /// <param name="organisationId"></param>
        /// <returns></returns>
        [Route("MasterRolesNOrganizationPrograms")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMasterRolesNOrganizationProgramsSelect(int roleType, int organisationId)
        {
            try
            {
                var userIdClaimMasterRol = Convert.ToInt32(User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "userId".ToLower(CultureInfo.InvariantCulture).Trim()).Value);
                var roleUserMasterRol = User.Claims.FirstOrDefault(x => x.Type.ToLower(CultureInfo.InvariantCulture).Trim() == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role".ToLower(CultureInfo.InvariantCulture).Trim()).Value.ToLower(CultureInfo.InvariantCulture).Trim();
                MasterRoleNOrganizationProgramDto modelMasterRol = new MasterRoleNOrganizationProgramDto();
                var programsMasterRol = _mapper.Map<List<OrganisationProgramDto>>((await _organisationProgram.GetOrganisationPrograms(organisationId, roleUserMasterRol, userIdClaimMasterRol)).ToList());
                if (programsMasterRol.Count > 0)
                    modelMasterRol.OrgProgram = programsMasterRol;
                var roles = _mapper.Map<List<RoleDto>>((await _roleRepository.GetRolesByRoleType(roleType)).ToList());
                if (roles.Count > 0)
                    modelMasterRol.Roles = roles;
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.MasterRoleNProgramType, modelMasterRol));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := GetMasterRolesNOrganizationProgramsSelect)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is used to get admin user with program types.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Route("AdminUserInfoNProgramType")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAdminUserInfoNProgramType(int userId)
        {
            try
            {
                var userAdminInfo = await _userRepository.GetUserAdminInfoWithProgramTypes(userId);
                if (userAdminInfo == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.AdminUserDetailUnSuccessfulReturn, null));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.AdminUserDetailSuccessfulReturn, userAdminInfo));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := GetAdminUserInfoNProgramType)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is called to get all merchants with transaction.
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        [Route("GetAllMerchantTransaction")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllMerchantsWithTransaction(string programId)
        {
            try
            {
                var pId = Convert.ToInt32(programId);

                /* Get all the relations to return. */
                var organisationMerchantwithTrans = await _organisation.GetAllMerchantsWithTransaction(pId);
                if (organisationMerchantwithTrans == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoRestaurantsExist));
                }

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, organisationMerchantwithTrans, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := GetAllMerchantsWithTransaction)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programId"></param>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [Route("GetAllMerchantDropdwn")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllMerchantsDropdown(string programId, string userId, string role)
        {
            try
            {
                var pId = Convert.ToInt32(programId);

                /* Get all the relations to return. */
                var organisationAllMerchDD = await _organisation.GetAllMerchantsWithDropdwn(pId, Convert.ToInt32(Cryptography.DecryptCipherToPlain(userId)), Cryptography.DecryptCipherToPlain(role));
                if (organisationAllMerchDD == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoRestaurantsExist));
                }

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, organisationAllMerchDD, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := GetAllMerchantsDropdown)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is called to get all merchants with transaction.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Route("GetAllMerchantsByAdmin")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllMerchantsByMerchantAdminId(string userId)
        {
            try
            {
                var pId = Convert.ToInt32(userId);

                /* Get all the relations to return. */
                var organisationByMerchantAdminId = await _organisation.GetAllMerchantsByMerchantAdminId(pId);
                if (organisationByMerchantAdminId == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoRestaurantsExist));
                }

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, organisationByMerchantAdminId, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := GetAllMerchantsByMerchantAdminId)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Route("GetPrimaryOrgNPrgDetailOfMerchantAdmin")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPrimaryOrgNPrgDetailOfMerchantAdmin(string userId)
        {
            try
            {
                var pId = Convert.ToInt32(userId);

                /* Get all the relations to return. */
                var organisationForMerchantAdmin = await _organisation.GetPrimaryOrgNPrgDetailOfMerchantAdminQuery(pId);
                if (organisationForMerchantAdmin == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoRestaurantsExist));
                }

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, organisationForMerchantAdmin, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := GetPrimaryOrgNPrgDetailOfMerchantAdmin)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }
        /// <summary>
        /// This Api is used to get all merchant program.
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        [Route("GetAllMerchantByProgram")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllMerchantByProgram(string programId)
        {
            try
            {
                var pId = Convert.ToInt32(programId);

                /* Get all the relations to return. */
                var organisation = await _organisation.GetAllMerchantsByProgram(pId);
                if (organisation == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoRestaurantsExist));
                }

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, organisation, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := GetAllMerchantByProgram)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is used to add/update organisation detail.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AddUpdateOrganisationDetail")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUpdateOrganisationDetail(OrganisationViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                /* Get all the relations to return. */
                string clientIpAddressForUpdateOrgDetail = Convert.ToString(HttpContext.Connection.RemoteIpAddress);
                var updateStatus = await _organisation.AddUpdateOrganisationBasicDetail(model, clientIpAddressForUpdateOrgDetail);
                if (updateStatus == "0") { return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.OrganisationAdminStatusNotUpdatedSuccessfully)); }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.OrganisationAdminStatusUpdatedSuccessfully, updateStatus, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := AddUpdateOrganisationDetail)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is used to add/update linking of organisation and programs
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AddUpdateOrganisationProgramsDetail")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUpdateOrganisationProgramsDetail(List<OrganisationProgramDBDto> model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var orgProgramsDetails = _mapper.Map<List<dbModel.OrganisationProgram>>(model);
                var updateStatusDetails = await _organisationProgram.AddUpdateOrganisationProgram(orgProgramsDetails);
                if (string.IsNullOrEmpty(updateStatusDetails)) { return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.OrganisationAdminStatusNotUpdatedSuccessfully)); }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.OrganisationAdminStatusUpdatedSuccessfully, updateStatusDetails, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := AddUpdateOrganisationProgramsDetail)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is used to add/update merchant detail.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AddUpdateMerchantDetail")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUpdateMerchantDetail(OrganisationViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                /* Get all the relations to return. */
                string clientIpAddressForMerchantDetail = Convert.ToString(HttpContext.Connection.RemoteIpAddress);
                var updateStatus = await _organisation.AddUpdateMerchantDetailInfo(model, clientIpAddressForMerchantDetail);
                if (updateStatus == "0") { return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.OrganisationAdminStatusNotUpdatedSuccessfully)); }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.OrganisationAdminStatusUpdatedSuccessfully, updateStatus, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := AddUpdateMerchantDetail)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is used to get all transactions of merchant.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <param name="dateMonth"></param>
        /// <returns></returns>
        [Route("GetAllTransactionOfMerchant")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllTransactionOfMerchant(int organisationId, string dateMonth)
        {
            try
            {
                DateTime? dateFilter = null;
                if (!string.IsNullOrEmpty(dateMonth))
                    dateFilter = Convert.ToDateTime(dateMonth);

                /* Get all the relations to return. */
                var organisationTransOfMerchant = await _organisation.GetAllMerchantsTransaction(organisationId, dateFilter);
                if (organisationTransOfMerchant == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.NoRestaurantsExist));
                }

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, organisationTransOfMerchant, 0));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := GetAllTransactionOfMerchant)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// This Api is used to get merchant rewards.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <param name="promotionId"></param>
        /// <returns></returns>
        [Route("GetMerchantRewardDetailInfoWithBusinessType")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MerchantRewardDetailInfoWithBusinessType(int organisationId, int promotionId)
        {
            try
            {

                /* Get All Organisations With Program and Account Type. */
                var orgDetailsWithBusinessType = await _organisation.GetMerchantRewardInfoWithBusinessType(organisationId, promotionId);
                if (orgDetailsWithBusinessType == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, orgDetailsWithBusinessType));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := MerchantRewardDetailInfoWithBusinessType)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is used to get merchant reward list.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <returns></returns>
        [Route("GetMerchantRewardList")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMerchantRewardList(int organisationId)
        {
            try
            {

                /* Get All Organisations With Program and Account Type. */
                var orgDetailsMerchantReward = await _organisation.GetMerchantRewardList(organisationId);
                if (orgDetailsMerchantReward == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, orgDetailsMerchantReward));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := MerchantRewardDetailInfoWithBusinessType)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is used to add/update merchant reward detail.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AddUpdateMerchantRewardDetail")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUpdateMerchantRewardDetail(MerchantRewardViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                /* Get all the relations to return. */
                var updateStatus = await _organisation.AddUpdateMerchantRewardInfo(model);

                if (updateStatus == "0") { return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.OrganisationAdminStatusNotUpdatedSuccessfully)); }
                try
                {
                    await AddUpdateRewardMerchantDetailRefactor(model, updateStatus);
                }
                catch (Exception ex)
                {
                    HttpContext.RiseError(new Exception(string.Concat("API := Push Notification Issue (Organisation := AddUpdateMerchantRewardDetail)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                }

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.OrganisationAdminStatusUpdatedSuccessfully, updateStatus, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := AddUpdateMerchantRewardDetail)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        private async Task AddUpdateRewardMerchantDetailRefactor(MerchantRewardViewModel model, string updateStatus)
        {
            if (model.id == 0 && updateStatus != "0" && DateTime.UtcNow >= model.startDate.Value.Add(model.startTime.Value) && DateTime.UtcNow <= model.endDate.Value.Add(model.endTime.Value))
            {
                var merchantPrg = await _organisationProgram.GetDataByIdAsync(new { OrganisationId = model.MerchantId });
                if (merchantPrg != null)
                {
                    var serverApiKey = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.FireBaseConstants.FireBaseServerKey)).FirstOrDefault();
                    PushNotifications push = new PushNotifications();
                    await AddUpdateRewardMerchantDetailRefactorInside(model, updateStatus, merchantPrg, serverApiKey, push);
                }
            }
        }

        private async Task AddUpdateRewardMerchantDetailRefactorInside(MerchantRewardViewModel model, string updateStatus, dbModel.OrganisationProgram merchantPrg, dbModel.GeneralSetting serverApiKey, PushNotifications push)
        {
            // need to check 
            if (model.IsPromotion && model.isActive.HasValue && model.isActive.Value)
            {
                string notificationMessage = string.Empty, notificationTitle = string.Empty;
                var programNotificationSetCheck = await _program.FindAsync(new { id = merchantPrg.programId.Value });
                if (programNotificationSetCheck?.IsAllNotificationShow.Value == true)
                {
                    List<UserDeviceDto> userDeviceIds = await _userRepository.GetUserDeviceTokenBasedOnProgram(merchantPrg.programId.Value);
                    if (userDeviceIds.Any())
                    {
                        List<int> usrNotify = await _userNotificationSettingsService.GetUserNotificationSettingByNotificaction(userDeviceIds.Select(m => m.Id).ToList(), (int)NotificationSettingsEnum.Offers);
                        if (usrNotify.Any())
                        {
                            userDeviceIds = userDeviceIds.Where(x => usrNotify.Contains(x.Id)).ToList();
                            var chkFavorites = await _userFavorite.GetUsersListForFavoriteMerchant(userDeviceIds.Select(m => m.Id).ToList(), model.MerchantId.Value);
                            var userFavorites = userDeviceIds.Where(x => chkFavorites.Contains(x.Id)).ToList();
                            if (userFavorites.Any())
                            {
                                notificationMessage = MessagesConstants.NewOfferNotificationMessage;
                                notificationTitle = MessagesConstants.NewOfferNotificationTitle;
                                await push.SendPushBulk(userFavorites.Select(m => m.UserDeviceId).ToList(), notificationTitle, notificationMessage, "", Cryptography.DecryptCipherToPlain(updateStatus), "favorite", "icon", "offer", 1, (serverApiKey != null ? serverApiKey.Value : ""), true, "Offers", model.MerchantId.Value);
                            }
                            var userAll = userDeviceIds.Where(x => !chkFavorites.Contains(x.Id)).ToList();
                            if (userAll.Any())
                            {
                                notificationMessage = MessagesConstants.NewOfferNotificationMessage;
                                notificationTitle = MessagesConstants.NewOfferNotificationTitle;
                                await push.SendPushBulk(userAll.Select(m => m.UserDeviceId).ToList(), notificationTitle, notificationMessage, "", Cryptography.DecryptCipherToPlain(updateStatus), "offer", "icon", "offer", 1, (serverApiKey != null ? serverApiKey.Value : ""), true, "Offers", model.MerchantId.Value);
                            }
                        }
                    }
                }
                await _userPushedNotificationService.AddAsync(new dbModel.UserPushedNotifications()
                {
                    notificationMessage = notificationMessage,
                    notificationTitle = notificationTitle,
                    notificationType = (int)NotificationSettingsEnum.Offers,
                    referenceId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(updateStatus)),
                    createdBy = model.createdBy,
                    modifiedBy = model.modifiedBy,
                    ProgramId = merchantPrg.programId.Value,
                    IsRedirect = true,
                    NotificationSubType = "Offers",
                    CustomReferenceId = model.MerchantId
                });
            }
            else if (model.IsPromotion && model.IsPublished && model.isActive.HasValue && model.isActive.Value)
            {
                await AddUpdateRewardMerchantDetailRefactorInsideAgain(model, updateStatus, merchantPrg, serverApiKey, push);
            }
        }

        private async Task AddUpdateRewardMerchantDetailRefactorInsideAgain(MerchantRewardViewModel model, string updateStatus, dbModel.OrganisationProgram merchantPrg, dbModel.GeneralSetting serverApiKey, PushNotifications push)
        {
            string notificationMessage = string.Empty, notificationTitle = string.Empty;
            var programNotificationSetCheck = await _program.FindAsync(new { id = merchantPrg.programId.Value });
            if (programNotificationSetCheck != null && programNotificationSetCheck.IsAllNotificationShow.Value)
            {
                List<UserDeviceDto> userDeviceIds = await _userRepository.GetUserDeviceTokenBasedOnProgram(merchantPrg.programId.Value);
                if (userDeviceIds.Any())
                {
                    List<int> usrNotify = await _userNotificationSettingsService.GetUserNotificationSettingByNotificaction(userDeviceIds.Select(m => m.Id).ToList(), (int)NotificationSettingsEnum.Awards);
                    if (usrNotify.Any())
                    {
                        notificationMessage = MessagesConstants.NewRewardNotificationMessage;
                        notificationTitle = MessagesConstants.NewRewardNotificationTitle;
                        userDeviceIds = userDeviceIds.Where(x => usrNotify.Contains(x.Id)).ToList();
                        if (userDeviceIds.Any())
                        {
                            await push.SendPushBulk(userDeviceIds.Select(m => m.UserDeviceId).ToList(), notificationTitle, notificationMessage, "", Cryptography.DecryptCipherToPlain(updateStatus), "awards", "icon", "awards", 1, (serverApiKey != null ? serverApiKey.Value : ""), true, "ProgressRewards", model.MerchantId.Value);
                        }
                    }
                }
            }
            await _userPushedNotificationService.AddAsync(new dbModel.UserPushedNotifications()
            {
                notificationMessage = notificationMessage,
                notificationTitle = notificationTitle,
                notificationType = (int)NotificationSettingsEnum.Awards,
                referenceId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(updateStatus)),
                createdBy = model.createdBy,
                modifiedBy = model.modifiedBy,
                ProgramId = merchantPrg.programId.Value,
                IsRedirect = true,
                NotificationSubType = "ProgressRewards",
                CustomReferenceId = model.MerchantId
            });
        }

        /// <summary>
        /// This Api is used to get the merchant detail info.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <param name="universityId"></param>
        /// <param name="programId"></param>
        /// <returns></returns>
        [Route("GetMerchantDetailInfoWithProgNAccType")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MerchantDetailInfoWithProgNAccType(int organisationId, int universityId, int programId)
        {
            try
            {

                /* Get All Organisations With Program and Account Type. */
                var orgDetailsForProgNAccType = await _organisation.GetOrganisationInfoWithProgramNAccountType(OrganasationType.Merchant, organisationId, universityId, programId);
                if (orgDetailsForProgNAccType == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, orgDetailsForProgNAccType));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := MerchantDetailInfoWithProgNAccType)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is used to get master record of weekdays and offer code.
        /// </summary>
        /// <returns></returns>
        [Route("MasterOfferCodeNWeekDays")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMasterOfferCodeNWeekDays()
        {
            try
            {
                MasterOfferCodeNWeekDayDto model = new MasterOfferCodeNWeekDayDto();
                var offerCodes = _mapper.Map<List<OfferCodeDto>>((await _offerCodeService.GetAllCodeOffers()).ToList());
                if (offerCodes.Count > 0)
                    model.OfferCodes = offerCodes;
                var weekDays = _mapper.Map<List<WeekDayDto>>((await _organisation.GetWeekDaysList()).ToList());
                if (weekDays.Count > 0)
                    model.WeekDays = weekDays;
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.MasterOfferCodeNWeekDays, model, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Account := GetMasterOfferCodeNWeekDays)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is used to get merchant business info.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <returns></returns>
        [Route("GetMerchantBusinessInfo")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMerchantBusinessInfo(int organisationId)
        {
            try
            {
                /* Get All Organisations With Program and Account Type. */
                var orgDetails = await _organisation.GetMerchantBusinessInfo(organisationId);
                if (orgDetails == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }
                var weekDays = _mapper.Map<List<WeekDayDto>>((await _organisation.GetWeekDaysList()).ToList());
                if (weekDays.Count > 0)
                    orgDetails.WeekDays = weekDays;

                var dwellTime = _mapper.Map<List<DwellTimeDto>>((await _organisation.GetDwellTimeList()).ToList());
                if (dwellTime.Count > 0)
                    orgDetails.DwellTime = dwellTime;

                var terminalType = _mapper.Map<List<TerminalTypeDto>>((await _organisation.GetTerminalTypeList()).ToList());
                if (terminalType.Count > 0)
                    orgDetails.TerminalType = terminalType;

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, orgDetails));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := MerchantDetailInfoWithProgNAccType)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is used delete promotion from system.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeletePromotion")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeletePromotion(MerchantRewardDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                /* Get All Organisations. */
                var DeletePromotion = await _promotions.RemoveAsync(new { id = Convert.ToInt32(model.Id) });
                if (DeletePromotion <= 0)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.OrganisationNotDeletedSuccessfully));
                }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.OrganisationDeletedSuccessfully, DeletePromotion, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := DeletePromotion)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is used to get merchant promotions info.
        /// </summary>
        /// <param name="merchantId"></param>
        /// <returns></returns>
        [Route("GetMerchantPromotions")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMerchantPromotionsInfo(int merchantId)
        {
            try
            {
                /* Get All Organisations With Program and Account Type. */
                var prmDetails = (await _promotions.GetAllPromotionsOfMerchant(merchantId)).ToList();
                if (prmDetails == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }
                prmDetails.ForEach(x => { x.encPromId = Cryptography.EncryptPlainToCipher(x.Id.ToString()); });

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, prmDetails, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := GetMerchantPromotionsInfo)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is used to clone merchant.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("CloneMerchant")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrganisationClone(MerchantDto model)
        {
            try
            {
                var result = 0;
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                var orgDetails = await _organisation.GetDataByIdAsync(new { id = model.Id });
                result = await _organisation.AddAsync(orgDetails);
                if (result > 0)
                {
                    var merchantid = "M1000-" + result.ToString();
                    var chkExist = await _organisation.FindAsync(new { id = result });
                    chkExist.MerchantId = merchantid;
                    chkExist.name = chkExist.name + "_Copy";
                    await _organisation.UpdateAsync(chkExist, new { id = result });
                    await CreateOrganisationCloneRefactor(model, result);
                }
                if (result <= 0) { return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.MerchantNotClonedSuccessfully)); }
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.MerchantClonedSuccessfully, result));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := CreateOrganisationClone)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        private async Task CreateOrganisationCloneRefactor(MerchantDto model, int result)
        {
            var orgSchedule = await _organisationSchedule.GetDataAsync(new { organisationId = model.Id });
            if (orgSchedule != null)
            {
                foreach (var item in orgSchedule)
                {
                    item.organisationId = result;
                    await _organisationSchedule.AddAsync(item);
                }
            }
            var orgpromotion = await _promotions.GetDataAsync(new { MerchantId = model.Id, IsDeleted = false });
            if (orgpromotion != null)
            {
                foreach (var item in orgpromotion)
                {
                    item.MerchantId = result;
                    await _promotions.AddAsync(item);
                }
            }
            var orguser = await _userRepository.GetDataAsync(new { OrganisationId = model.Id, IsDeleted = false });
            if (orguser != null)
            {
                foreach (var item in orguser)
                {
                    item.OrganisationId = result;
                    await _userRepository.AddAsync(item);
                }
            }
            var orgterminal = await _merchantTerminals.GetDataAsync(new { organisationId = model.Id });
            if (orgterminal != null)
            {
                foreach (var item in orgterminal)
                {
                    item.organisationId = result;
                    await _merchantTerminals.AddAsync(item);
                }
            }
            await CreateOrganisationCloneRefactorInside(model, result);
        }

        private async Task CreateOrganisationCloneRefactorInside(MerchantDto model, int result)
        {
            var orgmealperiod = await _mealPeriod.GetDataAsync(new { organisationId = model.Id });
            if (orgmealperiod != null)
            {
                foreach (var item in orgmealperiod)
                {
                    item.organisationId = result;
                    await _mealPeriod.AddAsync(item);
                }
            }
            var orgphoto = await _photos.GetDataAsync(new { entityId = model.Id, PhotoType = Constants.PhotoType.Organisation });
            if (orgphoto != null)
            {
                foreach (var item in orgphoto)
                {
                    item.entityId = result;
                    await _photos.AddAsync(item);
                }
            }
            var orgprog = await _organisationProgram.GetDataAsync(new { organisationId = model.Id });
            if (orgprog != null)
            {
                foreach (var item in orgprog)
                {
                    item.organisationId = result;
                    await _organisationProgram.AddAsync(item);
                }
            }
            var orgprogAccount = await _programMerchantAccountTypeService.GetDataAsync(new { organisationId = model.Id });
            if (orgprogAccount != null)
            {
                foreach (var item in orgprogAccount)
                {
                    item.organisationId = result;
                    await _programMerchantAccountTypeService.AddAsync(item);
                }
            }
        }

        /// <summary>
        /// This Api is used to get promotion detail by its Id.
        /// </summary>
        /// <param name="promotionId"></param>
        /// <returns></returns>
        [Route("GetPromotionsByID")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPromotionsByID(int promotionId)
        {
            try
            {
                /* Get All Organisations With Program and Account Type. */
                var prmDetails = await _promotions.GetPromotionDetailById(promotionId);
                if (prmDetails == null)
                {
                    return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.DataNotSuccessfullyReturned));
                }
                prmDetails.encPromId = Cryptography.EncryptPlainToCipher(prmDetails.Id.ToString());
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, prmDetails, 1));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := GetPromotionsByID)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }
        }

        /// <summary>
        /// This Api is used to get organisation detail by Id.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <returns></returns>
        [Route("CheckOrganisationExistenceById")]
        [HttpGet]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> GetOrganisationDetailById(int organisationId)
        {
            try
            {
                /* Get Organisation Details. */
                var orgDetails = _mapper.Map<OrganisationMainDto>(await _organisation.GetOrganisationDetailsById(organisationId));

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, orgDetails));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := GetOrganisationDetailById)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// Get the organisation list drop down based on user role.
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        [Route("OrganisationListDropDown")]
        [HttpGet]
        //    [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> GetOrganisationLstDrpDwn(string uId)
        {
            try
            {
                int userId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(uId));
                int roleId = (await _userRoleRepository.GetDataByIdAsync(new { UserId = userId })).RoleId;
                string roleName = (await _roleRepository.GetDataByIdAsync(new { Id = roleId })).Name;
                /* Get Organisation Details. */
                var orgDrpDwnDetails = (await _organisation.GetOrganisationListBasedOnUserRole(userId, roleName.Trim().ToLower()));

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, orgDrpDwnDetails));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := GetOrganisationLstDrpDwn)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rewardsSatatusDto"></param>
        /// <returns></returns>
        [Route("EditPromotionRewardStatus")]
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(IsOrganisationActiveAttribute))]
        public async Task<IActionResult> EditPromotionRewardStatus(RewardsSatatusDto rewardsSatatusDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiBadRequestResponse(ModelState, MessagesConstants.BadRequest)); //400
                }
                int rewardId = Convert.ToInt32(Cryptography.DecryptCipherToPlain(rewardsSatatusDto.RewardId));

                /* Update reward status. */
                var orgDrpDwnDetails = await _promotions.EditPromotionStatus(rewardId, rewardsSatatusDto.rewardActiveStatus);

                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, true, MessagesConstants.DataSuccessfullyReturned, orgDrpDwnDetails));
            }
            catch (Exception ex)
            {
                HttpContext.RiseError(new Exception(string.Concat("API := (Organisation := EditPromotionRewardStatus)", ex.Message, " Stack Trace : ", ex.StackTrace, " Inner Exception : ", ex.InnerException)));
                _logger.LogInfo(ex.Message + " >>>>> Inner Exception : " + ex.InnerException);
                return Ok(new ApiResponse(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, false, MessagesConstants.SomeIssueInProcessing));
            }

        }
    }
}
