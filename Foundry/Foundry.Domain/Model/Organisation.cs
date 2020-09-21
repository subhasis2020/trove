using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class Organisation
    {
        public Organisation()
        {
            OfferMerchant = new HashSet<OfferMerchant>();
            OrganisationGroup = new HashSet<OrganisationGroup>();
            OrganisationMappingOrganisation = new HashSet<OrganisationMapping>();
            OrganisationMappingParentOrganisation = new HashSet<OrganisationMapping>();
            OrganisationSchedule = new HashSet<OrganisationSchedule>();
            Program = new HashSet<Program>();
            ProgramMerchant = new HashSet<ProgramMerchant>();
            ProgramMerchantAccountType = new HashSet<ProgramMerchantAccountType>();
            User = new HashSet<User>();
            UserFavorites = new HashSet<UserFavorites>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Location { get; set; }
        public int OrganisationType { get; set; }
        public string EmailAddress { get; set; }
        public string ContactNumber { get; set; }
        public bool? IsMaster { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public int? MaxCapacity { get; set; }
        public string Description { get; set; }
        public string WebsiteUrl { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual User ModifiedByNavigation { get; set; }
        public virtual ICollection<OfferMerchant> OfferMerchant { get; set; }
        public virtual ICollection<OrganisationGroup> OrganisationGroup { get; set; }
        public virtual ICollection<OrganisationMapping> OrganisationMappingOrganisation { get; set; }
        public virtual ICollection<OrganisationMapping> OrganisationMappingParentOrganisation { get; set; }
        public virtual ICollection<OrganisationSchedule> OrganisationSchedule { get; set; }
        public virtual ICollection<Program> Program { get; set; }
        public virtual ICollection<ProgramMerchant> ProgramMerchant { get; set; }
        public virtual ICollection<ProgramMerchantAccountType> ProgramMerchantAccountType { get; set; }
        public virtual ICollection<User> User { get; set; }
        public virtual ICollection<UserFavorites> UserFavorites { get; set; }
    }
}
