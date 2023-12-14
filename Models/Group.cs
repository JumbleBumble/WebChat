using Microsoft.AspNetCore.Identity;

namespace WebChat.Models
{
	public class Group
	{
		public int Id { get; set; }
		public string Name { get; set; } = null!;
		public ICollection<IdentityUser> Users { get; set; } = null!;
		public ICollection<GroupMessage> Messages { get; set; } = null!;
    }
}
