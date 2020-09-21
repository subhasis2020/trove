using System.ComponentModel.DataAnnotations;

namespace Foundry.Web.Models
{
    public class BankAccountModel
    {
        public int CardIdSelected { get; set; }
        public int BankIdSelected { get; set; }
        [Required(ErrorMessage = "Please enter account number.")]
        public string AccountNumber { get; set; }
        [Required(ErrorMessage = "Please enter account title.")]
        public string AccountTitle { get; set; }
        [Required(ErrorMessage = "Please enter account nick name.")]
        public string AccountNickName { get; set; }
        [Required(ErrorMessage = "Please enter bank name.")]
        public string BankName { get; set; }
        [Required(ErrorMessage = "Please enter routing number.")]
        public string RoutingNumber { get; set; }
        public string Comments { get; set; }
        [Required(ErrorMessage = "Please select account type.")]
        public string AccountType { get; set; }
     //   [Required(ErrorMessage = "Please enter ACH type.")]
        public string ACHType { get; set; }

        public string ClientIPAddress { get; set; }
        public int RequesteeUserId { get; set; }
        public int CardDetailIdI2C { get; set; }
        public string CardBankName { get; set; }
        public string IdValue { get; set; }
    }
}
