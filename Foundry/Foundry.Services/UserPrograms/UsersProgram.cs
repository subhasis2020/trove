using System;
using System.Threading.Tasks;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using AutoMapper;
using Foundry.Domain.ApiModel;
using Foundry.Domain;
using Dapper;
using System.Linq;
using System.Globalization;

namespace Foundry.Services
{
    public class UsersProgram : FoundryRepositoryBase<UserProgram>, IUsersProgram
    {

        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IUserRepository _userRepository;

        public UsersProgram(IDatabaseConnectionFactory databaseConnectionFactory, IUserRepository userRepository)
         : base(databaseConnectionFactory)
        {
            _userRepository = userRepository;
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public async Task<UserProgram> GetUserProgram(int userId)
        {
            object obj = new { UserId = userId, IsLinkedProgram = true, IsActive = true, IsDeleted = false };
            return await GetDataByIdAsync(obj);
        }

        public async Task<UserProgram> GetUserProgramLinkingByEmailNID(int programId, string userId, string emailAddress)
        {
            object obj = new { ProgramId = programId, UserId = userId, IsActive = true, IsDeleted = false, Email = emailAddress };
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = await sqlConnection.QuerySingleOrDefaultAsync<UserProgram>(SQLQueryConstants.GetUserProgramLinkingByEmailNIDQuery, obj);
                    return result;
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

        public async Task<UserProgram> GetUserProgramLinkingByEmail(int programId, string emailAddress)
        {
            object obj = new { ProgramId = programId, EmailAddress = emailAddress.ToLower(CultureInfo.InvariantCulture).Trim(), IsActive = true, IsDeleted = false };
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = await sqlConnection.QuerySingleOrDefaultAsync<UserProgram>(SQLQueryConstants.GetUserProgramLinkingByEmailQuery, obj);
                    return result;
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

        public async Task<UserProgram> UpdateUserProgramNUserReturn(string userId, int programId, string emailAddress)
        {
            try
            {
                var user = await _userRepository.FindAsync(new { userCode = userId, Email = emailAddress, isActive = true, isDeleted = false });
                if (user != null)
                {
                    user.EmailConfirmed = true;
                    user.PhoneNumberConfirmed = true;
                    await _userRepository.UpdateAsync(user, new { id = user.Id });

                    object obj = new { UserId = user.Id, ProgramId = programId, IsActive = true, IsDeleted = false };
                    var userProgram = await GetDataByIdAsync(obj);

                    if (userProgram != null)
                    {
                        var generatedToken = Utilities.GenerateNumToken(4);
                        userProgram.userEmailAddress = emailAddress;
                        userProgram.linkAccountVerificationCode = generatedToken;
                        userProgram.verificationCodeValidTill = DateTime.UtcNow.AddMinutes(30);
                        userProgram.modifiedDate = DateTime.UtcNow;
                        userProgram.isVerificationCodeDone = false;
                        await UpdateAsync(userProgram, new { Id = userProgram.id });
                    }
                    return userProgram;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UserProgram> ValidateLinkAccountCode(int userId, int programId, string verificationCode)
        {
            object obj = new { UserId = userId, ProgramId = programId, LinkAccountVerificationCode = verificationCode, IsActive = true, IsDeleted = false };
            return await GetDataByIdAsync(obj);
        }

        public async Task<UserDto> UpdateValidationCodeNUserDetailReturn(int userId, int programId, bool isForAuthorizedUser)
        {
            try
            {
                if (isForAuthorizedUser)
                {
                    object objProgramLinkingCheck = new { UserId = userId, ProgramId = programId, isLinkedProgram = true, IsActive = true, IsDeleted = false };
                    var validateExistingProgramLinking = await GetDataByIdAsync(objProgramLinkingCheck);
                    if (validateExistingProgramLinking != null)
                    {
                        return null;
                    }
                }
                var dataUserPrograms = await UpdateMultipleAsync("Update UserProgram set isLinkedProgram=0 where userId=@userId", new { userId = userId });
                object obj = new { UserId = userId, ProgramId = programId, IsActive = true, IsDeleted = false };
                var validate = await GetDataByIdAsync(obj);
                if (validate != null)
                {
                    validate.isLinkedProgram = true;
                    validate.isVerificationCodeDone = true;
                    validate.modifiedDate = DateTime.UtcNow;
                    await UpdateAsync(validate, new { Id = validate.id });
                }
                return await _userRepository.GetUserById(userId);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<int> AddUserInProgram(int userId, int programId)
        {
            try
            {
                object obj = new { UserId = userId, ProgramId = programId, IsActive = true, IsDeleted = false };
                int userProgramId = 0;
                var userProgram = await GetDataByIdAsync(obj);
                if (userProgram == null)
                {
                    var userPg = new UserProgram
                    {
                        programId = programId,
                        userId = userId,
                        isLinkedProgram = true,
                        createdDate = DateTime.UtcNow,
                        modifiedDate = DateTime.UtcNow


                    };
                    userProgramId = await AddAsync(userPg);

                }
                return userProgramId;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// This method is created to check for the program linking and user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserProgram> CheckUserLinkingWithProgram(int userId, int programId)
        {
            return await GetDataByIdAsync(new { UserId = userId, programId = programId, IsActive = true, IsDeleted = false });
        }
    }
}
