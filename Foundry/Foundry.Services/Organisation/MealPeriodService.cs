using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public class MealPeriodService : FoundryRepositoryBase<OrganisationMealPeriod>, IMealPeriodService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IConfiguration _configuration;
        public MealPeriodService(IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration)
       : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _configuration = configuration;
        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public async Task<int> AddEditMealPeriod(List<OrganisationMealPeriodModel> model)
        {
            int isSucces = 0;
            try
            {
                var orgid = model.FirstOrDefault().organisationId;
                await DeleteEntityAsync(new { organisationId = orgid });
                foreach (var item in model)
                {
                    var obj = new OrganisationMealPeriod();
                    obj.id = item.id;
                    obj.organisationId = item.organisationId;
                    obj.closeTime = TimeSpan.Parse(item.closeTime.ToString());// (item.closeTime.ToString(), "HH:mm:ss", CultureInfo.InvariantCulture);
                    obj.createdBy = item.createdBy;
                    obj.createdDate = DateTime.UtcNow;
                    obj.days = item.days;
                    obj.isSelected = item.isSelected;
                    obj.modifiedBy = item.modifiedBy;
                    obj.modifiedDate = DateTime.UtcNow;
                    obj.openTime = TimeSpan.Parse(item.openTime.ToString());//DateTime.ParseExact(item.openTime.ToString(), "HH:mm:ss", CultureInfo.InvariantCulture);
                    obj.title = item.title;

                    await InsertOrUpdateAsync(obj, new { id = item.id });
                }
                return isSucces;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<int> AddEditMealPeriod(List<OrganisationMerchantTerminalModel> model)
        {
            throw new NotImplementedException();
        }
    }
}
