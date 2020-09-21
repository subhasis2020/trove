
namespace Foundry.Domain.ApiModel
{
    public class FirebaseDynamicLinkResponseModel
    {
        public string shortLink { get; set; }
        public Warning[] warning { get; set; }
        public string previewLink { get; set; }
    }


    public class Warning
    {
        public string warningCode { get; set; }
        public string warningMessage { get; set; }
    }

    public class FirebaseDynamicResultModel
    {
        public FirebaseDynamicLinkResponseModel result { get; set; }
    }
}
