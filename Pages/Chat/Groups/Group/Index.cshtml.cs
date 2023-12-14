using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using WebChat.Data;
using WebChat.Models;

namespace WebChat.Pages.Chat.Groups.Group
{
    public class IndexModel : PageModel
    {
		private readonly ApplicationDbContext _context;
		public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }


        //[BindProperty(SupportsGet = true)]
		public int GroupId { get; set; }

		public ICollection<GroupMessage> Messages { get; set; } = null!;

		public void OnGet(int id)
        {
			GroupId = id;
            //var group = _context.Groups.Find(GroupId);
            Models.Group? group = null;
            try
            {
                group = _context.Groups
            .Include(g => g.Messages)
            .First(g => g.Id == GroupId);
            }catch (InvalidOperationException){}

            if (group != null)
			{
				Messages = group.Messages;
			}
		}
    }
}
