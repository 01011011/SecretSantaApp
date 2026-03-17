namespace SecretSantaApp.Models
{
    public class Group
    {
        private static int nextId;

        public int Id { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; } = new List<User>();

        public Group()
        {
            Id = Interlocked.Increment(ref nextId);
        }
    }
}