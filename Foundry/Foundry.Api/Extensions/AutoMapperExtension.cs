using AutoMapper;
using Foundry.Domain.Dto;
using Foundry.Domain.DbModel;
using Foundry.Domain.ApiModel;
using System;

namespace Foundry.Api.Extensions
{
    /// <summary>
    /// This class is used to set the auto-mapper settings
    /// </summary>
    public class AutoMapperExtension : Profile
    {
        /// <summary>
        /// This is the constructor method in which all the setting for entities are given.
        /// </summary>
        public AutoMapperExtension()
        {
            // Customizing automapper functionality according to systems need as to ignore the parameter(s) or mapping parameters with different names.
            CreateMap<User, UserDto>().ForMember(t => t.ImagePath, opt => opt.Ignore());
            CreateMap<User, UserDto>().ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.Email));
            CreateMap<UserRelations, RelationshipDto>().ForMember(dest => dest.id, opt => opt.MapFrom(src => src.id));
            CreateMap<UserRelations, RelationshipDto>().ForMember(dest => dest.RelationName, opt => opt.MapFrom(src => src.relationName));
            CreateMap<ErrorMessagesDetail, GeneralErrorsDto>().ForMember(dest => dest.id, opt => opt.MapFrom(src => src.id));
            CreateMap<Organisation, OrganisationDto>().ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.description));
            CreateMap<UserNotificationsSettingModel, UserNotificationSettings>().ForMember(dest => dest.userId, opt => opt.MapFrom(src => src.UserId));
            CreateMap<UserNotificationsSettingModel, UserNotificationSettings>().ForMember(dest => dest.notificationId, opt => opt.MapFrom(src => src.NotificationId));
            CreateMap<DeviceLocationModel, User>().ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailAddress));
            CreateMap<OrganisationProgramModel, OrganisationProgram>().ForMember(t => t.programId, opt => opt.MapFrom(src => src.ProgramId));
            CreateMap<OrganisationProgramModel, OrganisationProgram>().ForMember(t => t.organisationId, opt => opt.MapFrom(src => src.OrganisationId));
            CreateMap<OrganisationProgramTypeModel, OrganisationProgramType>().ForMember(t => t.Id, opt => opt.Ignore());
            CreateMap<OrganisationProgramTypeModel, OrganisationProgramType>().ForMember(t => t.OrganisationId, opt => opt.Ignore());
            CreateMap<OrganisationProgramTypeModel, AdminProgramAccess>().ForMember(t => t.UserId, opt => opt.Ignore());
            CreateMap<OrganisationProgramDBDto, OrganisationProgram>().ForMember(t => t.Program, opt => opt.Ignore());
            CreateMap<OfferCode, OfferCodeDto>().ForMember(t => t.isCheckedCodeOffer, opt => opt.Ignore());
            CreateMap<OrganisationScheduleModel, OrganisationScheduleDto>().ForMember(t => t.HolidayDate, opt => opt.MapFrom(src => src.holidayDate));
            CreateMap<OrganisationScheduleModel, OrganisationSchedule>().ForMember(t => t.openTime, opt => opt.MapFrom(src => TimeSpan.Parse(src.openTime.ToString())));
            CreateMap<Promotion, PromotionsDto>().ForMember(t => t.encPromId, opt => opt.Ignore());
            CreateMap<AccountType, AccountTypeDto>().ForMember(t => t.ProgramId, opt => opt.Ignore());
            CreateMap<PlanProgramAccountModel, PlanProgramAccountsLinking>().ForMember(t => t.ProgramAccount, opt => opt.Ignore());
            CreateMap<PlanProgramAccountModel, PlanProgramAccountsLinking>().ForMember(t => t.ProgramPackage, opt => opt.Ignore());
            CreateMap<ProgramAccounts, ProgramAccountDto>().ForMember(t => t.strId, opt => opt.Ignore());
            CreateMap<ProgramBranding, BrandingDetailsWithMasterDto>().ForMember(t => t.accountType, opt => opt.Ignore());
            CreateMap<ProgramBranding, BrandingDetailsWithMasterDto>().ForMember(t => t.programAccount, opt => opt.Ignore());
            CreateMap<ProgramBranding, BrandingDetailsWithAccountType>().ForMember(t => t.ImagePath, opt => opt.Ignore());
            CreateMap<ProgramBranding, BrandingDetailsWithAccountType>().ForMember(t => t.accountType, opt => opt.Ignore());
            CreateMap<OrganisationProgramIdModel, AdminProgramAccess>().ForMember(t => t.User, opt => opt.Ignore());
            CreateMap<OrganisationProgramIdModel, AdminProgramAccess>().ForMember(t => t.UserId, opt => opt.Ignore());
            CreateMap<OrganisationProgramIdModel, AdminProgramAccess>().ForMember(t => t.Id, opt => opt.Ignore());
            CreateMap<ProgramAccounts, ProgramAccountDetailsWithMasterDto>().ForMember(t => t.WeekDays, opt => opt.Ignore());
            CreateMap<ProgramAccounts, ProgramAccountDetailsWithMasterDto>().ForMember(t => t.ResetPeriod, opt => opt.Ignore());
            CreateMap<ProgramAccounts, ProgramAccountDetailsWithMasterDto>().ForMember(t => t.ExchangeResetPeriod, opt => opt.Ignore());
            CreateMap<ProgramAccounts, ProgramAccountDetailsWithMasterDto>().ForMember(t => t.lstPassType, opt => opt.Ignore());
            CreateMap<ProgramAccounts, ProgramAccountDetailsWithMasterDto>().ForMember(t => t.InitialBalance, opt => opt.Ignore());
            CreateMap<Domain.DbModel.Program, ProgramDto>().ForMember(t => t.IsProgramLinkedWithUser, opt => opt.Ignore());
            CreateMap<i2cCardBankAccount, I2CCardBankAccountModel>()
     .ForMember(dest => dest.IpAddress, opt => opt.Ignore())
     .ForMember(dest => dest.AccountHolderUniqueId, opt => opt.Ignore())
     .ForMember(dest => dest.IsSandBoxAccount, opt => opt.Ignore());
            CreateMap<Domain.DbModel.Program, ProgramInfoDto>().ForMember(t => t.OrganisationJPOSId, opt => opt.Ignore());
            CreateMap<Domain.DbModel.Invitation, InvitationDto>().ForMember(t => t.EmailConfirmed, opt => opt.Ignore());

        }
    }
}
