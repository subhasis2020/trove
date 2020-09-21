using Dapper;
using Foundry.Domain;
using Foundry.Domain.DbModel;
using System;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public class UsersGroup : FoundryRepositoryBase<UserGroup>, IUsersGroup
    {

        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        public UsersGroup(IDatabaseConnectionFactory databaseConnectionFactory)
         : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }


        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<Group> GetGroupIdByName(string name)
        {
            var obj = new { Name = name };
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    return await sqlConnection.QuerySingleAsync<Group>(SQLQueryConstants.GetGroupByNameQuery, obj);
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

        public async Task<int> AddUpdateUserGroup(int userId, int groupId)
        {
            try
            {
                var userGroupId = 0;
                var obj = new { UserId = userId, GroupId = groupId };
                var userGroup = await GetDataByIdAsync(obj);
                if (userGroup == null)
                {
                    var usrGrp = new UserGroup
                    {
                        groupId = groupId,
                        userId = userId
                    };
                    await AddAsync(usrGrp);
                    userGroupId = usrGrp.id;
                }
                return userGroupId;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
