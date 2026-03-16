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

            foreach (int neighbor in graph.AdjencyList[startVertex])
            {
                if (!visited[neighbor])
                {
                    DFS(graph, neighbor, visited);
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
                    {
                        //Console.WriteLine("Nesujungtas");
                        return false;
                    }

                }
                //Console.WriteLine("Sujungtas");
                return true;
            }
            else
            {
                Console.WriteLine("Grafas neegzistuoja");
                return false;
            }

        }
        public bool IsBridge(Graph graph, int e, int v)
        {
            if (!graph.HasEdge(e, v))
                throw new ArgumentOutOfRangeException("Tokios briaunos nera");

            graph.RemoveEdge(e, v);

            bool isConnected = IsConnected(graph);
            graph.AddEdge(e, v);

            return !isConnected;
        }
    }
}
