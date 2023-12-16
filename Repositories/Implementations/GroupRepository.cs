using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebChat.Controllers;
using WebChat.Data;
using WebChat.Models;
using WebChat.Repositories.Interfaces;

namespace WebChat.Repositories.Implementations
{
    public class GroupRepository : IGroups
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<GroupRepository> _logger;

        public GroupRepository(ApplicationDbContext context, UserManager<AppUser> userManager, ILogger<GroupRepository> logger)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public IEnumerable<Group> GetGroups()
        {
            try
            {
                return _context.Groups.Include(g => g.Users).Include(g => g.Messages).ToList();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Empty groups");
                return new List<Group>();
            }
        }


        public Group GetGroupById(int id) => _context.Groups.Include(g => g.Users).Include(g => g.Messages).FirstOrDefault(g => g.Id == id) ?? null!;

        public async Task<int> CreateGroup(Group group)
        {
            if (group == null)
            {
                return -1;
            }
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();
            return group.Id;
        }

        public async Task<int> UpdateGroup(Group group)
        {
            if (group == null)
            {
                return -1;
            }
            Group? groupUpdate = GetGroupById(group.Id);
            if (groupUpdate != null)
            {
                groupUpdate = group;
                await _context.SaveChangesAsync();
                return groupUpdate.Id;
            }
            return -1;
        }

        public async Task<int> DeleteGroup(int groupId)
        {
            Group? groupDelete = GetGroupById(groupId);
            if (groupDelete != null)
            {
                _context.Groups.Remove(groupDelete);
                await _context.SaveChangesAsync();
                return groupId;
            }
            return -1;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

    }
}
