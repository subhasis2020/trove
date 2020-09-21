using Foundry.Domain.DbModel;
using System.Collections.Generic;

namespace Foundry.Domain.ApiModel
{
    public class ProfileViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }

        public ProfileViewModel()
        {

        }

        public ProfileViewModel(User user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            UserName = user.UserName;
        }

        public static IEnumerable<ProfileViewModel> GetUserProfiles(IEnumerable<User> users)
        {
            var profiles = new List<ProfileViewModel> { };
            foreach (User user in users)
            {
                profiles.Add(new ProfileViewModel(user));
            }

            return profiles;
        }
    }
}
