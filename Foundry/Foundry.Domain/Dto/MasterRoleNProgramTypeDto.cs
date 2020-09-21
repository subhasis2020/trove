using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class MasterRoleNProgramTypeDto
    {
        public List<RoleDto> Roles { get; set; }
        public List<ProgramTypesDto> ProgramTypes { get; set; }
    }

    public class MasterRoleNOrganizationProgramDto
    {
        public List<RoleDto> Roles { get; set; }
        public List<OrganisationProgramDto> OrgProgram { get; set; }
    }

    public class MasterOfferCodeNWeekDayDto
    {
        public List<OfferCodeDto> OfferCodes { get; set; }
        public List<WeekDayDto> WeekDays { get; set; }
    }
}
