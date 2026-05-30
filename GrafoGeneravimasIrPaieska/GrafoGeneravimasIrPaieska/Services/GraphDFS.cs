using GrafoGeneravimasIrPaieska.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrafoGeneravimasIrPaieska.Services
{
    public class GraphDFS
    {
        public void DFS(Graph graph, int startVertex, List<bool> visited)
        {
            if (!graph.HasVertex(startVertex))
                throw new ArgumentOutOfRangeException(nameof(startVertex), "Tokios virsunes nera");

            visited[startVertex] = true;

            foreach (Edge edge in graph.AdjencyList[startVertex])
            {
                if (!visited[edge.To])
                {
                    DFS(graph, edge.To, visited);
                }
            }
        }
        public bool IsConnected(Graph graph)
        {
            if (graph != null)
            {
                List<bool> visited = new List<bool>();
                for (int i = 0; i < graph.Vertices; i++)
                {
                    visited.Add(false);
                }

                DFS(graph, 0, visited);
                for (int i = 0; i < visited.Count; i++)
                {
                    if (!visited[i])
                        return false;

                }
                return true;
            }
            else
            {
                Console.WriteLine("Grafas neegzistuoja");
                return false;
            }

        }
        public bool IsBridge(Graph graph, int from, int to)
        {
            if (!graph.HasEdge(from, to))
                throw new ArgumentOutOfRangeException("Tokios briaunos nera");

            Graph copy = graph.CloneGraph();

            Edge edge = copy.AdjencyList[from].Find(x => x.To == to);

            if (edge == null)
                throw new ArgumentNullException(nameof(edge), "Tokios brianos nera");

            int weight = edge.Weight;

            copy.RemoveEdge(from, to, weight);

            bool isConnected = IsConnected(copy);
            copy.AddEdge(from, to, weight);

            return !isConnected;
        }
        public bool IsBridgeForEuler(Graph graph, int from, int to)
        {
            if (!graph.HasEdge(from, to))
            {
                Console.WriteLine("Tokios briaunos nera");
                return false;
            }

            // Jei is sitos virsunes yra tik viena briauna,
            // tai Fleury algoritme ja galima imti, net jei ji "tiltas"
            if (graph.GetNeighbours(from).Count == 1)
                return false;

            int reachableBefore = CountReachableVertices(graph, from);

            Graph copy = graph.CloneGraph();

            Edge edge = copy.AdjencyList[from].Find(e => e.To == to);

            if (edge == null)
            {
                Console.WriteLine(nameof(edge), "Tokios briaunos nera");
                return false;
            }

            copy.RemoveEdge(edge.From, edge.To, edge.Weight);

            int reachableAfter = CountReachableVertices(copy, from);

            return reachableAfter < reachableBefore;
        }
        //kiek dbr virsuniu yra pasiekiama
        private int CountReachableVertices(Graph graph, int start)
        {
            List<bool> visited = new List<bool>();

            for (int i = 0; i < graph.Vertices; i++)
            {
                visited.Add(false);
            }

            DFS(graph, start, visited);

            int count = 0;

            for (int i = 0; i < visited.Count; i++)
            {
                if (visited[i])
                    count++;
            }

            return count;
        }
    }
}
