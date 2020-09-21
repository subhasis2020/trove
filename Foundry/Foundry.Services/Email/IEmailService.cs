using Foundry.Domain.DbModel;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IEmailService : IFoundryRepositoryBase<EmailTemplate>
    {
        Task<bool> SendEmail(string ToEmail, string subject, string bodyHtml, string ccEmail, string bccEmail);

        Task<EmailTemplate> GetEmailTemplateByName(string name);
    }
}
