using System;

namespace Foundry.Domain.Dto
{
    public class GeneralErrorsDto
    {
        public int id { get; set; }
        public string ErrorParameterType { get; set; }
        public string ErrorMessages { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
