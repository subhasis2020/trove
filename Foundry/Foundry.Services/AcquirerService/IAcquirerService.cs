using foundry;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services.AcquirerService
{
    public interface IAcquirerService
    {
        AcquirerType GetAcquirerType(string id, string userId, string password);
        Task<echoResponse> Echo(AcquirerType acquirerType);
        Task<addCardResponse> AddCard(AddCardType addCardType);
        Task<activateCardResponse> ActivateCard(ActivateCardType activateCardType);
        Task<getCardStatusResponse> GetCardStatus(GetCardStatusType getCardStatusType);
        Task<createBankAccountResponse> CreateBankAccount(CreateBankAccountType createBankAccountType);
        Task<verifyBankAccountResponse> VerifyBankAccount(VerifyBankAccountType verifyBankAccountType);
        Task<removeBankAccountResponse> RemoveBankAccount(RemoveBankAccountType removeBankAccountType);
        Task<creditFundsResponse> CreditFunds(CreditFundsType creditFundsTypeObj);
        Task<reversalResponse> Reversal(ReversalType reversal);
        Task<debitFundsResponse> DebitFunds(DebitFundsType reversal);

        Task<bankAccountsListResponse> GetBankAccountList(BankAccountsListType bankAccountsListType);

        Task<b2CTransferListResponse> Bank2CardTransferLists(B2CTransferListType bankAccountsListType);
        Task<b2CTransferResponse> BankToCardTransfer(B2CTransferType b2CTransferType);
        Task<b2CTransferCancelResponse> CancelBankToCardTransfer(B2CTransferCancelType b2CTransferCancelType);
        Task<bankAccountsListResponse> BankAccountsList(BankAccountsListType bankAccountsListType);
        Task<transactionHistoryResponse> TransactionHistory(TransactionHistoryType transactionHistoryType);
        Task<getCardholderProfileAdvanceResponse> GetCardHolderProfileAdvance(GetCardholderProfileAdvanceType getCardholderProfileAdvanceType);
        Task<balanceInquiryResponse> BalanceInquiry(BalanceInquiryType balanceInquiryType);
    }
}
