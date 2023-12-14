using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebChat.Data;
using WebChat.Models;

namespace WebChat.Hubs
{
    [Authorize]
    public class GroupHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public GroupHub(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task SendMessageToGroup(string groupName, string sentMessage)
        {
            var user = Context.User?.Identity?.Name ?? "UnknownUser";
            if (string.IsNullOrWhiteSpace(groupName) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(sentMessage) || user == null)
            {
                return;
            }
            await Clients.Group(groupName).SendAsync("ReceiveMessage", user, sentMessage);
            Console.WriteLine(groupName+ " " + sentMessage+ " "+ user);
            int? grp = null;
            if (int.TryParse(groupName, out int result))
            {
                grp = result;
            }
            else
            {
                return;
            }
            var @group = await _context.Groups
            .Include(g => g.Messages)
            .FirstAsync(g => g.Id == grp);

            if (@group == null)
            {
                return;
            }
            GroupMessage NewMessage = new()
            {
                User = user,
                Text = sentMessage,
                Timestamp = DateTime.UtcNow
            };
            @group.Messages.Add(NewMessage);
            await _context.SaveChangesAsync();
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
