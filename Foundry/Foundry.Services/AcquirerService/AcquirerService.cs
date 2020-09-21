using foundry;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services.AcquirerService
{
    public class AcquirerService : IAcquirerService
    {

        private AcquirerType acquirerField;
        private MCPWSHandlerPortClient client;
        public AcquirerService()
        {
            client = new MCPWSHandlerPortClient();
        }

        public async Task<echoResponse> Echo(AcquirerType acquirerType)
        {
            var echotype = new EchoType();
            echotype.Acquirer = acquirerType;
            return await client.EchoAsync(echotype);
        }

        public AcquirerType GetAcquirerType(string id, string userId, string password)
        {
            acquirerField = new AcquirerType();
            acquirerField.Id = id;
            acquirerField.UserId = userId;
            acquirerField.Password = password;
            return acquirerField;
        }


        public Task<addCardResponse> AddCard(AddCardType addCardType)
        {
            return client.AddCardAsync(addCardType);
        }

        public Task<activateCardResponse> ActivateCard(ActivateCardType activateCardType)
        {
            return client.ActivateCardAsync(activateCardType);
        }
        public Task<getCardStatusResponse> GetCardStatus(GetCardStatusType getCardStatusType)
        {
            return client.GetCardStatusAsync(getCardStatusType);
        }

        public Task<createBankAccountResponse> CreateBankAccount(CreateBankAccountType createBankAccountType)
        {
            return client.CreateBankAccountAsync(createBankAccountType);
        }

        public Task<verifyBankAccountResponse> VerifyBankAccount(VerifyBankAccountType verifyBankAccountType)
        {
            return client.VerifyBankAccountAsync(verifyBankAccountType);
        }

        public Task<removeBankAccountResponse> RemoveBankAccount(RemoveBankAccountType removeBankAccountType)
        {
            return client.RemoveBankAccountAsync(removeBankAccountType);
        }

        public Task<creditFundsResponse> CreditFunds(CreditFundsType creditFundsType)
        {
            return client.CreditFundsAsync(creditFundsType);
        }

        public Task<reversalResponse> Reversal(ReversalType reversal)
        {
            return client.ReversalAsync(reversal);
        }

        public Task<debitFundsResponse> DebitFunds(DebitFundsType reversal)
        {
            return client.DebitFundsAsync(reversal);
        }



        public Task<bankAccountsListResponse> GetBankAccountList(BankAccountsListType bankAccountsListType)
        {
            return client.BankAccountsListAsync(bankAccountsListType);
        }



        public Task<b2CTransferListResponse> Bank2CardTransferLists(B2CTransferListType bankAccountsListType)
        {
            return client.B2CTransferListAsync(bankAccountsListType);
        }

        public Task<b2CTransferResponse> BankToCardTransfer(B2CTransferType b2CTransferType)
        {
            return client.B2CTransferAsync(b2CTransferType);
        }

        public Task<b2CTransferCancelResponse> CancelBankToCardTransfer(B2CTransferCancelType b2CTransferCancelType)
        {
            return client.B2CTransferCancelAsync(b2CTransferCancelType);
        }

        public Task<bankAccountsListResponse> BankAccountsList(BankAccountsListType bankAccountsListType)
        {
            return client.BankAccountsListAsync(bankAccountsListType);
        }


        public Task<transactionHistoryResponse> TransactionHistory(TransactionHistoryType transactionHistoryType)
        {
            return client.TransactionHistoryAsync(transactionHistoryType);
        }

        public Task<getCardholderProfileAdvanceResponse> GetCardHolderProfileAdvance(GetCardholderProfileAdvanceType getCardholderProfileAdvanceType)
        {
            return client.GetCardholderProfileAdvanceAsync(getCardholderProfileAdvanceType);
        }


        public Task<balanceInquiryResponse> BalanceInquiry(BalanceInquiryType balanceInquiryType)
        {
            return client.BalanceInquiryAsync(balanceInquiryType);
        }
    }
}
