
using Foundry.Domain.DbModel;
using Foundry.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Foundry.Identity
{
    public class UserStore : IUserStore<User>, IUserPasswordStore<User>, IUserRoleStore<User>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserClaimRepository _userClaimRepository;
        public UserStore(IUserRepository userRepository, IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository, IUserClaimRepository userClaimRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _userClaimRepository = userClaimRepository;

        }

        public Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                await _userRepository.AddAsync(user);
                return await Task.FromResult(IdentityResult.Success).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            try
            {
                if (int.TryParse(userId, out int id))
                {
                    var userList = await _userRepository.GetSingleDataByConditionAsync(new { Id = id });
                    if (userList != null)
                    {
                        return userList;
                    }
                    else
                    {
                        return await Task.FromResult((User)null).ConfigureAwait(false);
                    }
                }
                else
                {
                    return await Task.FromResult((User)null).ConfigureAwait(false);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _userRepository.GetSingleDataByConditionAsync(new { UserName = normalizedUserName });
                return users;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                return await Task.FromResult(user.PasswordHash).ConfigureAwait(false);
            }
            catch (Exception)
            {
                return null;
            }

        }

        public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                var userRoles = await _userRoleRepository.GetDataAsync(new { UserId = user.Id });
                IList<string> roles = new List<string>();
                foreach (UserRole r in userRoles)
                {
                    var userroles = await _roleRepository.GetDataByIdAsync(new { Id = r.RoleId });
                    roles.Add(userroles?.Name);
                }
                return await Task.FromResult(roles).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                return Task.FromResult(user.Id.ToString());
            }
            catch (Exception)
            {
                throw;
            }

        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                return Task.FromResult(user.UserName);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
        }

        public Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            try
            {
                user.NormalizedUserName = normalizedName;
                return Task.CompletedTask;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            try
            {
                user.PasswordHash = passwordHash;
                return Task.FromResult((object)null);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException(nameof(SetUserNameAsync));
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                await _userRepository.UpdateAsync(user, new { Id = user.Id });
                return await Task.FromResult(IdentityResult.Success).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }

        }


        public async Task<User> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            try
            {
                if (!string.IsNullOrEmpty(email))
                {
                    var userList = await _userRepository.GetSingleDataByConditionAsync(new { Email = email });
                    if (userList != null)
                    {
                        return userList;
                    }
                    else
                    {
                        return await Task.FromResult((User)null).ConfigureAwait(false);
                    }
                }
                else
                {
                    return await Task.FromResult((User)null).ConfigureAwait(false);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

    }
}
