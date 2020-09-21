using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IOfferCodeService : IFoundryRepositoryBase<OfferCode>
    {
        Task<IEnumerable<OfferCode>> GetAllCodeOffers();
    }
}
