using Foundry.Domain;
using Foundry.Domain.DbModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Foundry.Services
{
    public class ResetPassword : FoundryRepositoryBase<ResetUserPassword>, IResetPassword
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        public ResetPassword(IDatabaseConnectionFactory databaseConnectionFactory, UserManager<User> userManager, IUserRepository userRepository)
   : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _userManager = userManager;
            _userRepository = userRepository;
        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public async Task<User> CheckUser(string email)
        {
            try
            {
                var usr = await _userRepository.GetSingleDataByConditionAsync(new { Email = email });
                return usr;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public async Task<ResetUserPassword> GenerateForgotPasswordToken(string email, bool isFromWeb = false)
        {
            try
            {
                var resetUserPassword = await GetSingleDataAsync(SQLQueryConstants.GetResetPasswordContentQuery, new { Email = email });
                var generatedToken = Utilities.GenerateNumToken(4);
                var usr = await CheckUser(email).ConfigureAwait(false);
                if (resetUserPassword != null)
                {
                    resetUserPassword.resetToken = !isFromWeb ? generatedToken : null;
                    resetUserPassword.validTill = DateTime.UtcNow.AddMinutes(30);
                    resetUserPassword.updatedDate = DateTime.UtcNow;
                    resetUserPassword.isPasswordReset = false;
                    await UpdateAsync(resetUserPassword, new { Id = resetUserPassword.Id });
                }
                else
                {
                    resetUserPassword = new ResetUserPassword
                    {
                        resetToken = !isFromWeb ? generatedToken : null,
                        validTill = DateTime.UtcNow.AddMinutes(30),
                        updatedDate = DateTime.UtcNow,
                        isPasswordReset = false,
                        userId = usr.Id,
                    };
                    await AddAsync(resetUserPassword);
                }
                return resetUserPassword;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ResetUserPassword(string email, string password)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                using (var multi = sqlConnection.QueryMultipleAsync(SQLQueryConstants.GetMultipleResetPasswordNUserContentQuery, new { Email = email }).Result)
                {
                    try
                    {
                        var user = multi.Read<User>().FirstOrDefault();
                        var resetusrPwd = multi.Read<ResetUserPassword>().FirstOrDefault();
                        if (user != null)
                        {
                            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, password);
                            user.EmailConfirmed = !user.EmailConfirmed ? true : user.EmailConfirmed;
                            if (user != null && user.IsAdmin.HasValue && user.IsAdmin.Value && user.EmailConfirmed)
                            {
                                user.InvitationStatus = 2;
                            }
                            await _userRepository.UpdateAsync(user, new { Id = user.Id });
                            if (resetusrPwd != null)
                            {
                                resetusrPwd.isPasswordReset = true;
                                await UpdateAsync(resetusrPwd, new { Id = resetusrPwd.Id });
                            }
                            return true;
                        }
                        else { return false; }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        sqlConnection.Close();
                        sqlConnection.Dispose();
                    }

                }
            }
        }

        public async Task<ResetUserPassword> VerifyPasswordReset(string email, string token)
        {
            try
            {
                var user = await _userRepository.GetSingleDataByConditionAsync(new { Email = email.Trim() });
                if (user == null)
                {
                    return null;
                }
                var userPwd = await GetSingleDataByConditionAsync(new { ResetToken = token });
                return userPwd;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> SaveResetPassword(int userId)
        {
            var reset = new ResetUserPassword
            {
                createdDate = DateTime.UtcNow,
                inviteeId = userId,
                validTill = DateTime.UtcNow.AddMonths(1),
                isPasswordReset = false
            };
            return await AddAsync(reset);
        }

        public async Task<ResetUserPassword> VerifyPasswordResetWeb(string email)
        {
            try
            {
                var user = await _userRepository.GetSingleDataByConditionAsync(new { Email = email.Trim() }); 
                if (user == null)
                {
                    return null;
                }
                return await GetSingleDataByConditionAsync(new { UserId = user.Id });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
