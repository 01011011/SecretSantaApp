using SecretSantaApp.Infrastructure.Helpers;
using SecretSantaApp.Models;

namespace SecretSantaApp.Services
{
    public class Graph
    {
        public List<string> Vertices { get; set; }
        public Dictionary<string, List<string>> AdjacencyList { get; } = new Dictionary<string, List<string>>();

        public Graph() { }

        public Graph(List<string> users, IEnumerable<Tuple<string, string>> edges)
        {
            Vertices = users;

            foreach (var user in users)
            {
                AddVertex(user);
            }

            foreach (var edge in edges)
            {
                AddEdge(edge);
            }
        }

        public void AddVertex(string vertex)
        {
            AdjacencyList[vertex] = new List<string>();
        }

        public void AddEdge(Tuple<string, string> edge)
        {
            if (AdjacencyList.ContainsKey(edge.Item1) && AdjacencyList.ContainsKey(edge.Item2))
            {
                AdjacencyList[edge.Item1].Add(edge.Item2);
            }
        }

        // Augmenting-path DFS for maximum bipartite matching.
        // matchReceiver maps receiver → gifter currently assigned to them.
        private bool TryAugment(string gifter, Dictionary<string, string> matchReceiver, HashSet<string> visited)
        {
            foreach (var receiver in AdjacencyList[gifter])
            {
                if (visited.Contains(receiver))
                    continue;

                visited.Add(receiver);

                // If receiver is unmatched, or we can reassign its current gifter elsewhere
                if (!matchReceiver.ContainsKey(receiver) ||
                    TryAugment(matchReceiver[receiver], matchReceiver, visited))
                {
                    matchReceiver[receiver] = gifter;
                    return true;
                }
            }
            return false;
        }

        public Dictionary<string, string> GetMatchedData()
        {
            // Randomize each adjacency list for variety in results
            foreach (var vertex in Vertices)
            {
                AdjacencyList[vertex].Randomize();
            }

            // matchReceiver: receiver → gifter
            var matchReceiver = new Dictionary<string, string>();

            // Process most-constrained gifters first
            var orderedGifters = Vertices
                .OrderBy(x => AdjacencyList[x].Count)
                .ToList();

            foreach (var gifter in orderedGifters)
            {
                var visited = new HashSet<string>();
                TryAugment(gifter, matchReceiver, visited);
            }

            if (matchReceiver.Count != Vertices.Count)
                return new Dictionary<string, string>();

            // Invert to gifter → receiver
            var result = new Dictionary<string, string>();
            foreach (var kvp in matchReceiver)
            {
                result[kvp.Value] = kvp.Key;
            }
            return result;
        }
    }
}