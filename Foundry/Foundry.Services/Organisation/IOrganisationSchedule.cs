using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;

namespace Foundry.Services
{
    public interface IOrganisationSchedule : IFoundryRepositoryBase<OrganisationSchedule>
    {
        Task<int> AddEditOrganisationSchedule(OrganisationScheduleModel model);
        Task<int> AddEditOrganisationSchedule(List<OrganisationScheduleModel> model, bool isHoliday);
        Task<OrganisationScheduleAndHolidayDto> GetOrganisationSchedule(int organisationId);
    }
}
