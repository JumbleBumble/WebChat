using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebChat.Data;
using WebChat.Models;
using WebChat.Repositories.Interfaces;

namespace WebChat.Pages.Chat.Groups
{
	[Authorize]
	public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IGroups _igroups;

        public IndexModel(ApplicationDbContext context, UserManager<AppUser> userManager, IGroups igroups)
        {
            _context = context;
            _userManager = userManager;
            _igroups = igroups;
        }
        AppUser? user = null;
        public List<Models.Group> userGroups = new();


        public void OnGet()
        {
            user = _userManager.GetUserAsync(HttpContext.User).GetAwaiter().GetResult();
            if (user != null)
            {
                //userGroups = _igroups.GetGroups()
                //.Where(g => g.Users != null && g.Users.Contains(user)).ToList();
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
