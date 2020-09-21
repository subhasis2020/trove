using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel
{
    public class SendGridSettings
    {
        public string Key { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string SenderEmail { get; set; }

        public string fromName { get; set; }
    }
}
