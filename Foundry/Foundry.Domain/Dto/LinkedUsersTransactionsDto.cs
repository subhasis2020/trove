
namespace Foundry.Domain.Dto
{
    public class LinkedUsersTransactionsDto
    {
        public string MerchantFullName { get; set; }
        public string Period { get; set; }
        public string Amount { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string PlanName { get; set; }
        public string ImagePath { get; set; }
        public string AccountType { get; set; }

    }

    public class UsersTransactionsAccountDto
    {
        public int CreditUserId { get; set; }
        public int PlanId { get; set; }
        public int ProgramAccountId { get; set; }
        public int CreditUserType { get; set; }
        public int AccountTypeId { get; set; }
    }
}
