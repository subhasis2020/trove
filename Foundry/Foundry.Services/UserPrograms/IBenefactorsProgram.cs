using Foundry.Domain.DbModel;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IBenefactorsProgram : IFoundryRepositoryBase<BenefactorProgram>
    {
        Task<int> AddBenefactorInProgram(int userId, int programId);
    }
}
