using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using WebChat.Data;
using WebChat.Models;

namespace WebChat.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
		private readonly UserManager<IdentityUser> _userManager;

		public GroupsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
			_userManager = userManager;

		}

       

		[HttpPost("CreateGroup")]
		public async Task<ActionResult<Group>> CreateGroup([FromBody] List<string> userNames)
		{
			if (userNames == null || userNames.Count == 0)
			{
				return BadRequest("The userNames field is required and must not be empty.");
			}
			Group? matchingGroup = await _context.Groups
	        .Where(g => g.Users.Any(u =>
		        u.UserName != null && userNames.Contains(u.UserName)))
	        .FirstOrDefaultAsync();
			if (matchingGroup == null)
            {
				List<IdentityUser>? users = null;
   

				users = await _userManager.Users.Where(u =>
				u.UserName != null && userNames.Contains(u.UserName)).ToListAsync();

				if (users.Count > 0)
				{
					var newGroup = new Group
					{
						Name = "NewGroup",
						Users = new Collection<IdentityUser>(),
                        Messages = new Collection<GroupMessage>()
					};
					newGroup.Users = users;

					await _context.Groups.AddAsync(newGroup);
					await _context.SaveChangesAsync();
					return Ok("Group Created");
				}
				else
				{
					return BadRequest("No users with those usernames found.");
				}
			} else
            {
                return BadRequest("Group already exists");
            }

		}

		[HttpPost("Name/{id}")]
		public async Task<ActionResult<Group>> EditName(int id, [FromBody] string name)
		{
			IdentityUser? actionUser = await _userManager.GetUserAsync(HttpContext.User);
			if (actionUser != null) 
			{
				Group? group = await _context.Groups.Include(g => g.Users).FirstAsync(g => g.Id == id);
				if (group != null && group.Users.Contains(actionUser))
				{
					group.Name = name;
					await _context.SaveChangesAsync();
					return Ok();
				}
			}
			return NotFound();

		}


		// DELETE: api/Groups/5
		[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            if (_context.Groups == null)
            {
                return NotFound();
            }

            Group? @group = null;
            try
            {
                @group = await _context.Groups
            .Include(g => g.Users)
            .Include(g => g.Messages)
            .FirstAsync(g => g.Id == id);
            } catch(InvalidOperationException)
            {
                return NotFound();
            }
            if (@group == null)
            {
                return NotFound();
            }
			IdentityUser? actionUser = await _userManager.GetUserAsync(HttpContext.User);

			var users = @group.Users.ToList();
            if (actionUser != null && users.Contains(actionUser))
            {
				@group.Users.Remove(actionUser);
                if (@group.Users.Count <= 1)
                {
					var messages = @group.Messages.ToList();
					foreach (var message in messages)
					{
						@group.Messages.Remove(message);
					}
					_context.Groups.Remove(@group);
				}
				await _context.SaveChangesAsync();

				return Ok("Removed");
			} else
            {
				return NotFound();
			}
        }

        private bool GroupExists(int id)
        {
            return (_context.Groups?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
