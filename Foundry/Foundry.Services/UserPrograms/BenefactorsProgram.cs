using Foundry.Domain.DbModel;
using System;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public class BenefactorsProgram : FoundryRepositoryBase<BenefactorProgram>, IBenefactorsProgram
    {
        /*Creating database connection*/
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public BenefactorsProgram(IDatabaseConnectionFactory databaseConnectionFactory)
         : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public async Task<int> AddBenefactorInProgram(int userId, int programId)
        {
            var userPg = new BenefactorProgram
            {
                programId = programId,
                benefactorId = userId,
            };
            return await InsertOrUpdateAsync(userPg, new { BenefactorId = userId, ProgramId = programId });
        }
    }
}
