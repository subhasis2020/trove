using Foundry.Domain.DbModel;
using Foundry.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Foundry.Identity
{
    public class RoleStore : IRoleStore<Role>
    {
        private readonly IRoleRepository _roleRepository;
        public RoleStore(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            try
            {
                await _roleRepository.AddAsync(role);
                return await Task.FromResult(IdentityResult.Success).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            try
            {
                return await _roleRepository.GetSingleDataByConditionAsync(new { NormalizedName = normalizedRoleName });
            }
            catch (Exception)
            {
                throw;
            }

        }

        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            try
            {
                return Task.FromResult(role.NormalizedName.ToString());
            }
            catch (Exception)
            {
                throw;
            }

        }

        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            try
            {
                return Task.FromResult(role.Id.ToString());
            }
            catch (Exception)
            {
                throw;
            }

        }

        public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            try
            {
                return Task.FromResult(role.Name.ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            try
            {
                role.NormalizedName = normalizedName;
                return Task.CompletedTask;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            try
            {
                role.NormalizedName = roleName;
                return Task.CompletedTask;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


    }
}
