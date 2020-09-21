using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Foundry.Domain.ApiModel
{
    public class DeviceLocationModel
    {
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string UserDeviceId { get; set; }
        [Required]
        public string UserDeviceType { get; set; }
        [Required]
        public string Location { get; set; }
    }

    public class UserCardholderAgreementModel
    {
        public int Id { get; set; }
        public string AgreementVersionNo { get; set; }
        public bool IsAgreementRead { get; set; }
        public decimal InitialAmount { get; set; }
        public string IPAddress { get; set; }
    }
}
