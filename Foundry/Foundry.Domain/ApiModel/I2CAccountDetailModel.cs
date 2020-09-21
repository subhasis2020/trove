using System;

namespace Foundry.Domain.ApiModel
{
    public class I2CAccountDetailModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CustomerId { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string CVV2 { get; set; }
        public string AccountHolderUniqueId { get; set; }
        public decimal Balance { get; set; }
        public string CardStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public string NameOnCard { get; set; }
        public string ReferenceId { get; set; }
    }
}
