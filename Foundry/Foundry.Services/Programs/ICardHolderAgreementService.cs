using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface ICardHolderAgreementService : IFoundryRepositoryBase<CardHolderAgreement>
    {
        Task<List<CardholderAgreementDto>> GetCardHolderAgreements(int programId);
        Task<CardholderAgreementDto> GetCardHolderAgreementByIdNProgram(int programId, int cardHolderAgreementId);
        Task<string> AddUpdateCardHolderAgreement(CardHolderAgreement model);
        Task<CardholderAgreementDto> GetCardHolderAgreementByIdNProgramNUser(int programId, int userId);
        Task<CardholderAgreementDto> GetCardHolderAgreementsExistence(int programId);
    }
}
