using Dapper;
using Foundry.Domain;
using Foundry.Domain.Dto;
using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Foundry.Domain.Constants;
using Microsoft.Extensions.Configuration;

namespace Foundry.Services
{
    public class BenefactorNotifications : FoundryRepositoryBase<ReloadBalanceRequest>, IBenefactorNotifications
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IConfiguration _configuration;
        private readonly IPhotos _photos;
        public BenefactorNotifications(IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration, IPhotos photos)
        : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _configuration = configuration;
            _photos = photos;
        }


        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<List<BenefactorNotificationsDto>> GetBenefactorNotifications(int benefactorId)//Left To implement Dapper
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var user = await sqlConnection.QueryAsync<User>(SQLQueryConstants.GetUserByIdQuery, new { Id = benefactorId, IsActive = true, IsDeleted = false });
                    var userEmail = user.FirstOrDefault().Email ?? "";
                    var obj = new
                    {
                        BenefactorId = benefactorId,
                        Email = userEmail
                    };
                    var oNotifications = (await sqlConnection.QueryAsync<BenefactorNotificationsDto>(SQLQueryConstants.GetBenefactorNotificationQuery, obj)).ToList();
                    if (oNotifications.Count() > 0)
                    {
                        for (int i = 0; i < oNotifications.Count; i++)
                        {
                            oNotifications[i].ImagePath = await _photos.GetAWSBucketFilUrl(oNotifications[i].ImagePath, string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.UserDefaultImage));
                        }
                       
                    }
                    return oNotifications.OrderByDescending(x => x.ModifiedDate).ToList();
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
