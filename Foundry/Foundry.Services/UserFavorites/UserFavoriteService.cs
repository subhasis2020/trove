using Dapper;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public class UserFavoriteService : FoundryRepositoryBase<UserFavorites>, IUserFavoriteService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        public UserFavoriteService(IDatabaseConnectionFactory databaseConnectionFactory)
        : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This method will Add and Update the Reastaurants that are made Favroite by User.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> AddUpdateUserFavorite(UserFavoriteModel model)
        {
            try
            {
                object obj = new { UserId = model.UserId, OrgnisationId = model.OrgnisationId };
                var userfavorite = await GetDataByIdAsync(obj);
                if (userfavorite != null)
                {
                    userfavorite.userId = model.UserId;
                    userfavorite.orgnisationId = model.OrgnisationId;
                    userfavorite.isFavorite = model.IsFavorite;
                    await UpdateAsync(userfavorite, new { Id = userfavorite.id });
                    return userfavorite.id;
                }
                else
                {
                    var favorite = new UserFavorites
                    {
                        userId = model.UserId,
                        orgnisationId = model.OrgnisationId,
                        isFavorite = model.IsFavorite
                    };

                    return await AddAsync(favorite);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<int>> GetUsersListForFavoriteMerchant(List<int> userIds, int merchantId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    return (await sqlConnection.QueryAsync<int>(SQLQueryConstants.GetUserFavoriteByOrganisations, new { UserIds = userIds, MerchantId = merchantId })).ToList();
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
