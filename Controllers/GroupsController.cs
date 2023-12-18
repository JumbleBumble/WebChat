using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using WebChat.Data;
using WebChat.Models;
using WebChat.Repositories.Interfaces;

namespace WebChat.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		private readonly IGroups _igroups;
        private readonly ILogger<GroupsController> _logger;

        public GroupsController(ApplicationDbContext context, UserManager<AppUser> userManager, IGroups igroups, ILogger<GroupsController> logger)
        {
            _context = context;
			_userManager = userManager;
            _igroups = igroups;
            _logger = logger;
        }

   		[HttpPost("CreateGroup")]
		public async Task<ActionResult<Group>> CreateGroup([FromBody] List<string> userNames)
		{
			if (userNames == null || userNames.Count == 0)
			{
                _logger.LogWarning("Usernames list is null or empty");
                return BadRequest("The userNames field is required and must not be empty.");
			}
			IEnumerable<Group> groups = _igroups.GetGroups();
            groups ??= new List<Group>();
			Group? matchingGroup = groups.FirstOrDefault(group =>
	        group.Users?.All(u => u.UserName != null && userNames.Contains(u.UserName)) ?? false);
            bool matchCheck = false;

			if (matchingGroup != null && matchingGroup.Users != null)
            {
                matchCheck = userNames.All(u => matchingGroup.Users.All(gu => u == gu.UserName));

			}
            if (matchCheck == false)
            {
				List<AppUser>? users = null;
   
				users = await _userManager.Users.Where(u =>
				u.UserName != null && userNames.Contains(u.UserName)).ToListAsync();

				if (users.Count > 0)
				{
					var newGroup = new Group
					{
						Name = "NewGroup",
						Users = new Collection<AppUser>(),
                        Messages = new Collection<GroupMessage>()
					};
					newGroup.Users = users;

					await _igroups.CreateGroup(newGroup);
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
			AppUser? actionUser = await _userManager.GetUserAsync(HttpContext.User);
			if (actionUser != null) 
			{
				Group? group = _igroups.GetGroupById(id);
				if (group != null && group.Users != null)
				{
                    var users = group.Users.ToList();
                    if (users.Any() && users.Contains(actionUser))
                    {
                        group.Name = name;
                        await _igroups.UpdateGroup(group);
                        _logger.LogInformation("Group {GroupId} renamed to {NewName}", id, name);
                        return Ok();
                    }
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

            Group? group = _igroups.GetGroupById(id);
            if (group == null)
            {
                return NotFound();
            }
            AppUser? actionUser = await _userManager.GetUserAsync(HttpContext.User);
			if (group.Users != null && group.Users.Any())
			{
                if (actionUser != null && group.Users.Contains(actionUser))
                {
                    group.Users.Remove(actionUser);
                    if (group.Users.Count <= 1)
                    {
                        await _igroups.DeleteGroup(id); 
                    }
                    else
                    {
                        await _igroups.UpdateGroup(group);
                    }

                    return Ok("Removed");
                }
            }
            return NotFound();
        }

        private bool GroupExists(int id)
        {
            return (_context.Groups?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
