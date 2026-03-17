using SecretSantaApp.Models;

namespace SecretSantaApp.Infrastructure.Helpers
{
    public class UserComparer : IEqualityComparer<User>
    {
        public bool Equals(User u1, User u2)
        {
            return u1.Id == u2.Id;
        }

        public int GetHashCode(User p)
        {
            return p.Id;
        }
    }

    public static class RandomizationExtensions
    {
        private static readonly Random Random = new Random();

        public static void Randomize<T>(this T[] items)
        {
            for (int i = 0; i < items.Length - 1; i++)
            {
                int j = Random.Next(i, items.Length);
                T temp = items[i];
                items[i] = items[j];
                items[j] = temp;
            }
        }

        public static void Randomize<T>(this List<T> items)
        {
            T[] array = items.ToArray();
            array.Randomize();
            items.Clear();
            items.AddRange(array);
        }
    }
}