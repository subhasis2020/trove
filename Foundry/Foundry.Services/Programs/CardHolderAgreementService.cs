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
    public class CardHolderAgreementService : FoundryRepositoryBase<CardHolderAgreement>, ICardHolderAgreementService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        public CardHolderAgreementService(IDatabaseConnectionFactory databaseConnectionFactory)
     : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }



        public async Task<List<CardholderAgreementDto>> GetCardHolderAgreements(int programId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var cardHolderAgreements = (await sqlConnection.QueryAsync<CardholderAgreementDto>(SQLQueryConstants.GetCardHolderAgreementsBasedOnProgram, new { ProgramId = programId })).ToList();
                    cardHolderAgreements.ForEach(x => x.CardHolderAgreementIdEnc = Cryptography.EncryptPlainToCipher(x.CardHolderAgreementId.ToString()));
                    cardHolderAgreements.ForEach(x => x.ProgramIdEnc = Cryptography.EncryptPlainToCipher(x.ProgramId.ToString()));
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

        public async Task<CardholderAgreementDto> GetCardHolderAgreementByIdNProgram(int programId, int cardHolderAgreementId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var cardHolderAgreement = (await sqlConnection.QuerySingleOrDefaultAsync<CardholderAgreementDto>(SQLQueryConstants.GetCardHolderAgreementsBasedOnProgramNId, new { ProgramId = programId, Id = cardHolderAgreementId }));
                    if (cardHolderAgreement != null)
                    {
                        cardHolderAgreement.CardHolderAgreementIdEnc = Cryptography.EncryptPlainToCipher(Convert.ToString(cardHolderAgreement.CardHolderAgreementId));
                    }
                    else
                    {
                        cardHolderAgreement = new CardholderAgreementDto();
                        cardHolderAgreement.CardHolderAgreementIdEnc = Cryptography.EncryptPlainToCipher("0");

                    }
                    return cardHolderAgreement;
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

        public async Task<CardholderAgreementDto> GetCardHolderAgreementByIdNProgramNUser(int programId, int userId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var cardHolderAgreement = (await sqlConnection.QuerySingleOrDefaultAsync<CardholderAgreementDto>(SQLQueryConstants.GetCardHolderAgreementsBasedOnProgramIdAndUser, new { ProgramId = programId, UserId = userId }));
                    if (cardHolderAgreement != null)
                    {
                        cardHolderAgreement.CardHolderAgreementIdEnc = Cryptography.EncryptPlainToCipher(Convert.ToString(cardHolderAgreement.CardHolderAgreementId));
                    }
                    else
                    {
                        cardHolderAgreement = new CardholderAgreementDto();
                        cardHolderAgreement.CardHolderAgreementIdEnc = Cryptography.EncryptPlainToCipher("0");

                    }
                    return cardHolderAgreement;
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

        public async Task<CardholderAgreementDto> GetCardHolderAgreementsExistence(int programId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var cardHolderAgreement = (await sqlConnection.QuerySingleOrDefaultAsync<CardholderAgreementDto>(SQLQueryConstants.GetCardHolderAgreementsExistence, new { ProgramId = programId }));

                    return cardHolderAgreement;
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

        public async Task<string> AddUpdateCardHolderAgreement(CardHolderAgreement model)
        {
            try
            {
                if (model.id <= 0)
                {
                    model.versionNo = "Version.1";
                }
                else
                {
                    model.id = 0;
                    var detail = await GetCardHolderAgreementByIdNProgram(model.programID, 0);
                    if (detail != null)
                    {
                        model.versionNo = string.Concat("Version.", Convert.ToInt32(detail.versionNo.Split(".")[1]) + 1);
                    }

                }
                var resultId = await AddAsync(model);
                return Cryptography.EncryptPlainToCipher(resultId.ToString()) + "_" + model.versionNo;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
