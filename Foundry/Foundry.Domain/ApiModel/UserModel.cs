namespace Foundry.Domain
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string UserName { get; set; }
        public string SessionId { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string ImagePath { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public int? ProgramCode { get; set; }
        public string ProgramImagePath { get; set; }
        public string ProgramName { get; set; }
        public bool? IsProgramVerificationDone { get; set; }
    }
}
