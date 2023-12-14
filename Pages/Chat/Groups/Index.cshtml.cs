using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebChat.Data;
using WebChat.Models;

namespace WebChat.Pages.Chat.Groups
{
	[Authorize]
	public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        IdentityUser? user = null;
        public List<Models.Group> userGroups = new();


        public void OnGet()
        {
            user = _userManager.GetUserAsync(HttpContext.User).GetAwaiter().GetResult();
            if (user != null)
            {
                userGroups = _context.Groups
                  .Where(g => g.Users.Contains(user)).Include(g => g.Users).ToList();
                userGroups.ForEach(userGroup => userGroup.Users.Remove(user));
            }
        }
		public void TestMethod()
		{
			
		}
	}
}
