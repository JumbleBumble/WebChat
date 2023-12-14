using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using WebChat.Data;
using WebChat.Models;
using WebChat.Models.Interfaces;

namespace WebChat.Hubs
{
    [Authorize]
    public class ChatHub : Hub, IChatHub
	{
        private static readonly ConcurrentDictionary<string, string> UserConnections = new ConcurrentDictionary<string, string>();
		private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public override Task OnConnectedAsync()
        {
            var username = Context.User?.Identity?.Name ?? "UnknownUser";

            UserConnections.TryAdd(username, Context.ConnectionId);

            Clients.All.SendAsync("ConnectMessage", "Connected", username);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
			var username = Context.User?.Identity?.Name ?? "UnknownUser";

			UserConnections.TryRemove(username, out _);

            Clients.All.SendAsync("ConnectMessage", "User Disconnected", username);

            return base.OnDisconnectedAsync(exception);
        }

        // You can expose a method to get the list of connected users
        public List<string> GetConnectedUsers()
        {
            var connectedUsers = UserConnections.Keys.ToList();
            return connectedUsers;
        }

        public ConcurrentDictionary<string, string> GetConnectedUsersConnections()
        {
            return UserConnections;
        }

        public async Task SendMessage(string message)
        {
            var user = Context.User?.Identity?.Name ?? "UnknownUser";
            if (user != null && message != null && message != "")
            {
                var dbContext = _context;
                var messageNew = new Message
                {
                    User = user,
                    Text = message,
                    Timestamp = DateTime.UtcNow
                };

                dbContext.Messages.Add(messageNew);
                dbContext.SaveChanges();
                await Clients.All.SendAsync("ReceiveMessage", user, message);
            }
        }

        [HubMethodName("SendMessageToUser")]
        public async Task DirectMessage(string user, string message)
        {
            var Sender = Context.User?.Identity?.Name ?? "UnknownUser";
            var allUsers = GetConnectedUsersConnections();
            if (Sender != null && allUsers != null && allUsers.TryGetValue(user, out string? userId) && message != null && message != "")
            {
                if (Clients.Client(userId) != null)
                {
                    await Clients.Client(userId).SendAsync("ReceiveMessage", Sender, message);
                }
            }
        }
    }
}
