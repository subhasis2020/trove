﻿using System.ComponentModel.DataAnnotations;

namespace Foundry.Domain.ApiModel
{
    public class LinkAccountModel
    {
        [Required]
        [StringLength(80, ErrorMessage = "Email address must not be greater than 80 characters long.")]
        [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Please enter a valid email address.")]
        public string EmailAddress { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int ProgramId { get; set; }

    }
}
