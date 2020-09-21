
namespace Foundry.Domain.ApiModel
{
    public class I2CLogViewModel
    {
        public virtual int Id { get; set; }

        public virtual int? UserId { get; set; }

        public virtual string AccountHolderUniqueId { get; set; }

        public virtual string ApiName { get; set; }

        public virtual string ApiUrl { get; set; }

        public virtual string Request { get; set; }

        public virtual string Response { get; set; }
    
        public virtual string Status { get; set; }

        public virtual string IpAddress { get; set; }
    }
}
