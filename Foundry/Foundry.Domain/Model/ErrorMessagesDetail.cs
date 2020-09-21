using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class ErrorMessagesDetail
    {
        public int Id { get; set; }
        public string ErrorParameterType { get; set; }
        public string ErrorMessages { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
