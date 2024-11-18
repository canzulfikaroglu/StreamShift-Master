using Microsoft.AspNetCore.Identity;

namespace StreamShift.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public virtual ICollection<Transfer> Transfers { get; set; }
    }
}
