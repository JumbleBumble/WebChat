using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebChat.Models
{
	public class Group
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; } = null!;
		public ICollection<AppUser> Users { get; set; } = null!;
		public ICollection<GroupMessage> Messages { get; set; } = null!;
    }
}
