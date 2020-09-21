using System;
using System.ComponentModel.DataAnnotations;

namespace Foundry.Domain.ApiModel
{
    public class I2CCardBankAccountModel
    {
        public int Id { get; set; }
        public int I2CAccountDetailId { get; set; }

        [Required]
        public string AccountNumber { get; set; }
        public string AccountTitle { get; set; }
        public string AccountType { get; set; }
        public string AccountNickName { get; set; }
        public string ACHType { get; set; }
        public string BankName { get; set; }
        public string RoutingNumber { get; set; }
        public string Comments { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedDate{ get; set; }
        public string IpAddress { get; set; }
        public string AccountHolderUniqueId { get; set; }
        public bool IsSandBoxAccount { get; set; }
        public string AccountSrNo { get; set; }
        public string idRequesteeUserEnc { get; set; }
        public string CardBankName { get; set; }
        public string IdValue { get; set; }
        public bool CardStatus { get; set; }
    }
}
