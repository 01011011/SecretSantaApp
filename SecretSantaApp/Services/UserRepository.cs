using SecretSantaApp.Models;

namespace SecretSantaApp.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users = new List<User>();
        private readonly IGroupRepository _groupRepository;
        private readonly object _lock = new object();

        public UserRepository(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public IEnumerable<User> GetAllUsers()
        {
            lock (_lock)
            {
                return _users.ToList();
            }
        }

        public bool SaveUser(User user)
        {
            lock (_lock)
            {
                try
                {
                    if (UserExistsByName(_users, user.Name)) return false;
                    _users.Add(user);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
        }

        public bool DeleteUser(int id)
        {
            lock (_lock)
            {
                try
                {
                    var userToDelete = _users.FirstOrDefault(x => x.Id == id);
                    if (userToDelete == null) return false;
                    return _users.Remove(userToDelete);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
        }

        public bool UpdateUser(int id, string name)
        {
            lock (_lock)
            {
                try
                {
                    var user = _users.FirstOrDefault(x => x.Id == id);
                    if (user == null) return false;
                    user.Name = name;
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
        }

        public User GetUserByGuid(int id)
        {
            lock (_lock)
            {
                return _users.FirstOrDefault(x => x.Id == id);
            }
        }

        public Dictionary<string, string> RunMatchingAlgorithm()
        {
            List<User> users;
            List<Group> groups;

            lock (_lock)
            {
                users = _users.ToList();
            }

            groups = _groupRepository.GetAllGroups()?.ToList();

            if (users == null || groups == null) return null;

            var edges = new List<Tuple<string, string>>();
            foreach (var user in users)
            {
                var group = groups.FirstOrDefault(x => x.Users.Any(m => m.Id == user.Id));

                if (group != null)
                {
                    foreach (var user2 in users)
                    {
                        if (group.Users.All(c => c.Id != user2.Id))
                        {
                            edges.Add(new Tuple<string, string>(user.Name, user2.Name));
                        }
                    }
                }
                else
                {
                    foreach (var user2 in users)
                    {
                        if (user.Id != user2.Id)
                        {
                            edges.Add(new Tuple<string, string>(user.Name, user2.Name));
                        }
                    }
                }
            }

            var stringUsers = users.Select(x => x.Name).ToList();
            var graph = new Graph(stringUsers, edges);
            return graph.GetMatchedData();
        }

        private bool UserExistsByName(IEnumerable<User> users, string name)
        {
            return users.Any(x => x.Name.ToLowerInvariant() == name.ToLowerInvariant());
        }
    }
}
