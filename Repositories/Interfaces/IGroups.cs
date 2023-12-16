using WebChat.Models;

namespace WebChat.Repositories.Interfaces
{
    public interface IGroups : IDisposable
    {
        IEnumerable<Group> GetGroups();
        Group GetGroupById(int id);
        Task<int> CreateGroup(Group group);
        Task<int> UpdateGroup(Group group);
        Task<int> DeleteGroup(int id);

    }
}
