using Dapper;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public class OrganisationSchedules : FoundryRepositoryBase<OrganisationSchedule>, IOrganisationSchedule
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IConfiguration _configuration;
        public OrganisationSchedules(IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration)
       : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _configuration = configuration;
        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public async Task<int> AddEditOrganisationSchedule(OrganisationScheduleModel model)
        {
            try
            {
                var obj = new OrganisationSchedule();
                obj.closedTime = TimeSpan.Parse(model.closedTime.ToString());
                obj.createdBy = model.createdBy;
                obj.createdDate = model.createdDate;
                obj.holidayDate = model.holidayDate;
                obj.HolidayName = model.HolidayName;
                obj.IsForHolidayNameToShow = model.IsForHolidayNameToShow;
                obj.id = model.id;
                obj.isActive = model.isActive;
                obj.isDeleted = model.isDeleted;
                obj.isHoliday = model.isHoliday;
                obj.modifiedBy = model.modifiedBy;
                obj.modifiedDate = model.modifiedDate;
                obj.openTime = TimeSpan.Parse(model.openTime.ToString());
                obj.organisationId = model.organisationId;
                obj.workingDay = model.workingDay ?? null;
                return await InsertOrUpdateAsync(obj, new { id = model.id });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> AddEditOrganisationSchedule(List<OrganisationScheduleModel> model, bool isHoliday)
        {
            int isSucces = 0;
            try
            {
                var orgid = model.FirstOrDefault().organisationId;
                await DeleteEntityAsync(new { organisationId = model.FirstOrDefault().organisationId, isHoliday });
                foreach (var item in model)
                {
                    var obj = new OrganisationSchedule();
                    obj.closedTime = TimeSpan.Parse(item.closedTime.ToString());
                    obj.createdBy = item.createdBy;
                    obj.createdDate = item.createdDate;
                    obj.holidayDate = item.holidayDate;
                    obj.HolidayName = item.HolidayName;
                    obj.IsForHolidayNameToShow = item.IsForHolidayNameToShow;
                    obj.id = item.id;
                    obj.isActive = item.isActive;
                    obj.isDeleted = item.isDeleted;
                    obj.isHoliday = item.isHoliday;
                    obj.modifiedBy = item.modifiedBy;
                    obj.modifiedDate = item.modifiedDate;
                    obj.openTime = TimeSpan.Parse(item.openTime.ToString());
                    obj.organisationId = item.organisationId;
                    obj.workingDay = !string.IsNullOrEmpty(item.workingDay) ? item.workingDay : string.Empty;
                    isSucces = await InsertOrUpdateAsync(obj, new { id = item.id });
                }
                return isSucces;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<OrganisationScheduleAndHolidayDto> GetOrganisationSchedule(int organisationId)
        {
            OrganisationScheduleAndHolidayDto ogranisationScheduleHoliday = new OrganisationScheduleAndHolidayDto();

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    using (var multi = sqlConnection.QueryMultipleAsync(SQLQueryConstants.GetOrganisationScheduleByIdSP, new { Id = organisationId, IsActive = true, IsDeleted = false }, commandType: CommandType.StoredProcedure).Result)
                    {
                        try
                        {
                            ogranisationScheduleHoliday.OrganisationSchedule = new List<OrganisationScheduleDto>();
                            ogranisationScheduleHoliday.HolidaySchedule = new HolidayScheduleDto();
                            ogranisationScheduleHoliday.OrganisationSchedule = multi.Read<OrganisationScheduleDto>().ToList();
                            var holidaySchedule = multi.Read<HolidayScheduleDto>().ToList();
                            if (holidaySchedule.Count > 0)
                            {
                                if (holidaySchedule.Any(x => x.IsForHolidayNameToShow == true))
                                { ogranisationScheduleHoliday.HolidaySchedule = holidaySchedule.Where(x => x.IsForHolidayNameToShow == true).FirstOrDefault(); }
                                else { ogranisationScheduleHoliday.HolidaySchedule = holidaySchedule.FirstOrDefault(); }

                            }
                            return ogranisationScheduleHoliday;
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        finally
                        {
                            sqlConnection.Close();
                            sqlConnection.Dispose();
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
        }
    }
}
