using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Foundry.Services
{
    public class EmailService : FoundryRepositoryBase<EmailTemplate>, IEmailService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IGeneralSettingService _setting;
        private IOptions<SendGridSettings> _sendGridSettings;
        public EmailService(IGeneralSettingService setting, IDatabaseConnectionFactory databaseConnectionFactory,
                            IOptions<SendGridSettings> sendGridSettings)
            : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _setting = setting;
            this._sendGridSettings = sendGridSettings;
        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public async Task<bool> SendEmail(string ToEmail, string subject, string bodyHtml, string ccEmail, string bccEmail)
        {
            try
            {
                var message = new SendGridMessage();

                message.SetFrom(new EmailAddress(this._sendGridSettings.Value.SenderEmail, this._sendGridSettings.Value.fromName));
                var lstRecipients = new List<EmailAddress>();
                if (!string.IsNullOrEmpty(ccEmail))
                {
                    var lstCC = ccEmail.Trim(new char[] { ' ', '.', ',' }).Split(',').ToList();
                    foreach (var item in lstCC)
                    {
                        lstRecipients.Add(new EmailAddress() { Email = item });
                    }
                    message.AddCcs(lstRecipients);
                }

                lstRecipients = new List<EmailAddress>();
                if (!string.IsNullOrEmpty(bccEmail))
                {
                    var lstBCC = bccEmail.Trim(new char[] { ' ', '.', ',' }).Split(',').ToList();
                    foreach (var item in lstBCC)
                    {
                        lstRecipients.Add(new EmailAddress() { Email = item });
                    }
                    message.AddBccs(lstRecipients);
                }
             
                message.SetSubject(subject);
                message.AddTo(new EmailAddress(ToEmail));
                message.HtmlContent = bodyHtml;
                
                await SendMailDispatch(message);

                return true;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<EmailSMTP> GetSMTPHost()
        {
            try
            {
                var emailSettings = (await _setting.GetGeneratSettingValueByKeyGroup(Constants.SMTPConstants.SMTP)).ToList();

                EmailSMTP smtp = new EmailSMTP();
                foreach (var item in emailSettings)
                {
                    switch (item.KeyName)
                    {
                        case Constants.SMTPConstants.SMTP_Host:
                            smtp.SMPTHost = item.Value;
                            break;
                        case Constants.SMTPConstants.SMTP_Port:
                            smtp.SMTPPort = Convert.ToInt32(item.Value);
                            break;
                        case Constants.SMTPConstants.SMTP_UserName:
                            smtp.SMTPUsername = item.Value;
                            break;
                        case Constants.SMTPConstants.SMTP_Password:
                            smtp.SMTPPassword = item.Value;
                            break;
                        case Constants.SMTPConstants.SMTP_EnableSSL:
                            smtp.SMTPEnableSSL = item.Value == "1" ? true : false;
                            break;
                        case Constants.SMTPConstants.EmailFrom:
                            smtp.SMTPEmailFrom = item.Value;
                            break;
                        default:
                            break;
                    }
                }
                return smtp;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<EmailTemplate> GetEmailTemplateByName(string name)
        {
            object obj = new { Name = name };
            return await GetDataByIdAsync(obj);
        }


        public async Task SendMailDispatch(SendGridMessage message)
        {
            try
            {
                var client = new SendGridClient(this._sendGridSettings.Value.Key);
                await client.SendEmailAsync(message);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }


    }
}
