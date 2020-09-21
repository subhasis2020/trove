using Foundry.Domain.DbModel;
using Foundry.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Foundry.Identity
{
    public class CustomUserManager : UserManager<User>
    {
        private readonly IUserClaimRepository _userClaimRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserRepository _userRepository;
        public CustomUserManager(IUserStore<User> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<User> passwordHasher,
        IEnumerable<IUserValidator<User>> userValidators,
        IEnumerable<IPasswordValidator<User>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManager<User>> logger, IUserClaimRepository userClaimRepository, IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository, IUserRepository userRepository)
            : base(store, optionsAccessor, passwordHasher,
            userValidators, passwordValidators, keyNormalizer,
            errors, services, logger)
        {
            _userClaimRepository = userClaimRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _userRepository = userRepository;
        }

        public async override Task<IdentityResult> AddClaimAsync(User user, Claim claim)
        {
            try
            {
                UserClaim userClaim = new UserClaim
                {
                    UserId = user.Id,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                };
                await _userClaimRepository.AddAsync(userClaim);

                return await Task.FromResult(IdentityResult.Success).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override async Task<IdentityResult> AddToRoleAsync(User user, string role)
        {
            try
            {
                //get role
                var myroles = await _roleRepository.GetSingleDataByConditionAsync(new { Name = role });

                if (myroles != null)
                {
                    UserRole userRole = new UserRole
                    {
                        RoleId = myroles.Id,
                        UserId = user.Id
                    };
                    await _userRoleRepository.AddAsync(userRole);
                }
                return await Task.FromResult(IdentityResult.Success);
            }
            catch (Exception)
            {
                throw;
            }

        }
        public async Task<IdentityResult> AddUserRole(string user, string role)
        {
            try
            {
                //get role
                var myroles = await _roleRepository.GetSingleDataByConditionAsync(new { Name = role });
                //get user
                var myuser = await _userRepository.GetSingleDataByConditionAsync(new { Email = user, IsActive = true, IsDeleted = false });
                if (myroles != null)
                {
                    UserRole userRole = new UserRole
                    {
                        RoleId = myroles.Id,
                        UserId = myuser.Id
                    };
                    await _userRoleRepository.AddUserRole(userRole);
                }
                return await Task.FromResult(IdentityResult.Success);
            }
            catch (Exception)
            {
                throw;
            }

        }

    }
}
