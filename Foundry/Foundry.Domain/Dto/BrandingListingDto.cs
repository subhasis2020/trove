using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class BrandingListingDto
    {
        public int Id { get; set; }
        public string StrId { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public string ImagePath { get; set; }
        public string CardNumber { get; set; }
        public string BrandingColor { get; set; }
        public int AccountTypeId { get; set; }
    }
}
