using SecretSantaApp.Models;
using SecretSantaApp.Infrastructure.Helpers;

namespace SecretSantaApp.Services
{
    public class GroupRepository : IGroupRepository
    {
        private readonly List<Group> _groups = new List<Group>();
        private readonly object _lock = new object();

        public IEnumerable<Group> GetAllGroups()
        {
            lock (_lock)
            {
                return _groups.ToList();
            }
        }

        public bool SaveGroup(Group group)
        {
            lock (_lock)
            {
                try
                {
                    if (GroupExistsByName(_groups, group.Name)) return false;
                    _groups.Add(group);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
        }

        public bool RemoveUserFromGroup(int id, User user)
        {
            lock (_lock)
            {
                try
                {
                    var group = _groups.FirstOrDefault(x => x.Id == id);
                    var userToRemove = group?.Users.FirstOrDefault(u => u.Id == user.Id);

                    if (userToRemove == null) return false;

                    if (group.Users.Count > 2)
                    {
                        group.Users.Remove(userToRemove);
                    }
                    else
                    {
                        _groups.Remove(group);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
        }

        public bool UpdateGroup(int id, List<User> users)
        {
            lock (_lock)
            {
                try
                {
                    var group = _groups.FirstOrDefault(x => x.Id == id);
                    if (group != null)
                    {
                        group.Users = group.Users.Union(users, new UserComparer()).ToList();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
        }

        public Group GetGroupById(int id)
        {
            lock (_lock)
            {
                return _groups.FirstOrDefault(x => x.Id == id);
            }
        }

        private static bool GroupExistsByName(IEnumerable<Group> groups, string name)
        {
            return groups.Any(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
        }
    }
}