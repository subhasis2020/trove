namespace Foundry.Domain.ApiModel
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int? OrganisationId { get; set; }
        public string UserCode { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

    }
}
