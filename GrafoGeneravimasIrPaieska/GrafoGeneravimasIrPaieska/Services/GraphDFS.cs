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

            Edge edge = graph.AdjencyList[from].Find(x => x.To == to);

            if (edge == null)
                throw new ArgumentNullException(nameof(edge), "Tokios brianos nera");

            int weight = edge.Weight;

            graph.RemoveEdge(from, to, weight);

            bool isConnected = IsConnected(graph);
            graph.AddEdge(from, to, weight);

            return !isConnected;
        }
    }
}
