namespace SecretSantaApp.Models
{
    public class User
    {
        private static int nextId;

        public int Id { get; set; }
        public string Name { get; set; }
        public bool InGroup { get; set; }

        public User() { }

        public User(string name)
        {
            Id = Interlocked.Increment(ref nextId);
            Name = name;
        }
    }
}