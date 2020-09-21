
namespace Foundry.Domain.ApiModel
{

    public class FirebaseDynamicLinkModel
    {
        public Dynamiclinkinfo dynamicLinkInfo { get; set; }
    }


    public class Dynamiclinkinfo
    {
        public string domainUriPrefix { get; set; }
        public string link { get; set; }
        public Androidinfo androidInfo { get; set; }
        public Iosinfo iosInfo { get; set; }
    }

    public class Androidinfo
    {
        public string androidPackageName { get; set; }
    }

    public class Iosinfo
    {
        public string iosBundleId { get; set; }
    }
}
