using SecretSantaApp.Models;

namespace SecretSantaApp.Services
{
    public interface IGroupRepository
    {
        IEnumerable<Group> GetAllGroups();
        bool SaveGroup(Group group);
        bool RemoveUserFromGroup(int id, User user);
        Group GetGroupById(int id);
        bool UpdateGroup(int id, List<User> users);
    }
}