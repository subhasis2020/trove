using Foundry.Domain.DbModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Services.Errors
{
    public interface IGeneralErrorDetail :IFoundryRepositoryBase<ErrorMessagesDetail>
    {
        Task<List<ErrorMessagesDetail>> GetGeneralErrors();
    }
}
