using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Foundry.Domain.DbModel
{
    public partial class Role : IdentityRole<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }
        public override string Name { get; set; }
        public override string NormalizedName { get; set; }
        public override string ConcurrencyStamp { get; set; }
    }
}
