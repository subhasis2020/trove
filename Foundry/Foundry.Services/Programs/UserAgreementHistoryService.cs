using Dapper;
using Foundry.Domain;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public class UserAgreementHistoryService : FoundryRepositoryBase<UserAgreementHistory>, IUserAgreementHistoryService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        public UserAgreementHistoryService(IDatabaseConnectionFactory databaseConnectionFactory) : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }

        public async Task<List<UserAgreementHistoryDto>> GetCardHolderAgreementHistory(int programId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var cardHolderAgreements = (await sqlConnection.QueryAsync<UserAgreementHistoryDto>(SQLQueryConstants.GetUserAgreementHistory, new { ProgramId = programId })).ToList();
                    cardHolderAgreements.ForEach(x => x.CardHolderAgreementIdEnc = Cryptography.EncryptPlainToCipher(x.UserId.ToString()));
                    cardHolderAgreements.ForEach(x => x.ProgramIdEnc = Cryptography.EncryptPlainToCipher(programId.ToString()));
                    return cardHolderAgreements;
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

        public async Task<List<UserAgreementHistoryDto>> GetCardHolderAgreementHistoryVersions(int programId,int userId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var cardHolderAgreements = (await sqlConnection.QueryAsync<UserAgreementHistoryDto>(SQLQueryConstants.GetUserAgreementHistoryVersions, new { ProgramId = programId, UserId=userId })).ToList();
                    cardHolderAgreements.ForEach(x => x.CardHolderAgreementIdEnc = Cryptography.EncryptPlainToCipher(x.UserId.ToString()));
                    cardHolderAgreements.ForEach(x => x.ProgramIdEnc = Cryptography.EncryptPlainToCipher(programId.ToString()));
                    return cardHolderAgreements;
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
}
