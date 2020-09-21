using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class UserWithProgramTypeDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string IsAdmin { get; set; }
        public string UserImagePath { get; set; }
        public string RoleId { get; set; }
        public List<ProgramTypeIdDto> UserProgramType { get; set; }
        public List<MercahntIdDto> MerchantIds { get; set; }
        public List<AdminProgramIdDto> AdminProgramIds { get; set; }
        public string Title { get; set; }
        public string ImageFileName { get; set; }
    }
}
