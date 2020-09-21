using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class AccountListingDto
    {
        public int Id { get; set; }
        public string StrId { get; set; }
        public string ProgramAccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public string PlanName { get; set; }
        public bool Status { get; set; }
        public string Jpos_ProgramAccountId { get; set; }
        public string Jpos_ProgramEncAccountId { get; set; }
        public int accountTypeId { get; set; }
    }

    public class AccountInfoDto
    {
        public int Id { get; set; }
        public string AccountType { get; set; }
        public int BrandingCount { get; set; }
        public string accountName { get; set; }
        public string Jpos_ProgramAccountId { get; set; }
    }

    public class AccountPlanProgramDto {
        public int ProgramAccountId { get; set; }
        public int planId { get; set; }
        public int programId { get; set; }
        public int OrganizationId { get; set; }
        public int AccountTypeId { get; set; }
    }
}
