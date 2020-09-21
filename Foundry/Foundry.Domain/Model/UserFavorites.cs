using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class UserFavorites
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OrgnisationId { get; set; }
        public bool IsFavorite { get; set; }

        public virtual Organisation Orgnisation { get; set; }
        public virtual User User { get; set; }
    }
}
