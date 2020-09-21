using System;

namespace Foundry.Domain.ApiModel
{
    public class EmailModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public string Ccemail { get; set; }
        public string Bccemail { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class EmailSMTP
    {
        public string SMPTHost { get; set; }
        public int SMTPPort { get; set; }
        public string SMTPUsername { get; set; }
        public string SMTPPassword { get; set; }
        public bool SMTPEnableSSL { get; set; }
        public string SMTPEmailFrom { get; set; }
    }
}
