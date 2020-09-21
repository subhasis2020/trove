using System;
using System.ComponentModel.DataAnnotations;

namespace Foundry.Domain.ApiModel
{
    public class I2CAddCardModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [StringLength(80, ErrorMessage = "Email address must not be greater than 80 characters long.")]
        [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
        public string StartingNumbers { get; set; }

        [Required]
        public int ProgramId { get; set; }
        public bool InitialAmountSpecified { get; set; }
        public decimal InitialAmount { get; set; }
        public string IPAddress { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "The AccountHolderUniqueId must not be greater than 30 characters.")]
        public string AccountHolderUniqueId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsSandBoxAccount { get; set; }
        public string CardProgramID { get; set; }
        public string Quantity { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
 
        public string Country { get; set; }
        public string StateCode { get; set; }
        public string PostalCode { get; set; }
    }
}
