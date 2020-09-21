namespace Foundry.Domain.ApiModel
{
    public class OrganisationProgramModel
    {
        public int OrganisationId { get; set; }
        public int ProgramId { get; set; }
        public bool IsPrimaryAssociation { get; set; }
    }

    public class OrganisationProgramIdModel
    {
        public int ProgramTypeId { get; set; }
    }

    public class MerchantIdModel
    {
        public int merchantId { get; set; }
    }

    public class ProgramIdModel
    {
        public int programId { get; set; }
    }
}