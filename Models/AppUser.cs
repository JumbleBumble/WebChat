using Microsoft.AspNetCore.Identity;

namespace WebChat.Models
{
    public class AppUser : IdentityUser
    {
        public ICollection<Group>? Groups { get; set; }
    }
}
