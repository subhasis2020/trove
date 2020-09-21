using System;

namespace Foundry.Domain.Dto
{
    [Serializable]
    public class LinkedUsersDto
    {
        public int linkedUserId { get; set; }
        public string ImageUserPath { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string School { get; set; }
        public string UserCode { get; set; }
        public string DateAdded { get; set; }
        public bool CanViewTransaction { get; set; }

    }
}
