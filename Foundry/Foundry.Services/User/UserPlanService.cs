using Dapper;
using Foundry.Domain;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public class UserPlanService : FoundryRepositoryBase<UserPlans>, IUserPlanService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
       
        /// <summary>
        /// Constructor for assigning value to database connection.
        /// </summary>
        /// <param name="databaseConnectionFactory"></param>
        public UserPlanService(IDatabaseConnectionFactory databaseConnectionFactory)
            : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
           
        }

        public async Task<int> AddUpdateUserPlans(List<UserPlans> model)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = await sqlConnection.ExecuteAsync(SQLQueryConstants.DeleteUserPlansQuery, new { UserId = model.FirstOrDefault().userId });
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

            return await AddAsync(model);

        }

        public async Task<List<PlanListingDto>> GetUserPlanDetail(int userId, int programId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                   return (await sqlConnection.QueryAsync<PlanListingDto>(SQLQueryConstants.GetUserPlanDetail, new { UserId = userId, ProgramId = programId, IsActive = true, IsDeleted = false })).ToList();
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
