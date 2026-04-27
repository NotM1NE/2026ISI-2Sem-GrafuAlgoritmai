using GrafoGeneravimasIrPaieska.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrafoGeneravimasIrPaieska.Services
{
    public class BellmanFord
    {
        public BellmanFordResult TrivialBellmanFordResult(Graph graph, int start)
        {
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));

            if (!graph.HasVertex(start))
                throw new ArgumentOutOfRangeException(nameof(start), "Tokios virsunes nera");

            int?[] distance = new int?[graph.Vertices];
            int[] previous = new int[graph.Vertices];

            for (int i = 0; i < graph.Vertices; i++)
            {
                distance[i] = null;
                previous[i] = -1;
            }

            distance[start] = 0;

            List<Edge> edges = graph.GetAllEdges();
            for (int i = 0; i < graph.Vertices - 1; i++)
            {
                bool changed = false;

                foreach (Edge edge in edges)
                {
                    if (distance[edge.From] != null)
                    {
                        int newDistance = distance[edge.From].Value + edge.Weight;

                        if (distance[edge.To] == null || newDistance < distance[edge.To].Value)
                        {
                            distance[edge.To] = newDistance;
                            previous[edge.To] = edge.From;
                            changed = true;
                        }
                    }
                }

                if (!changed)
                    break;
            }
            Edge negativeCycleEdge = FindNegativeCycleEdge(graph, distance);

            return new BellmanFordResult(distance, previous, negativeCycleEdge);
        }
        private Edge FindNegativeCycleEdge(Graph graph, int?[] distance)
        {
            foreach (Edge edge in graph.GetAllEdges())
            {
                if (distance[edge.From] != null)
                {
                    int newDistance = distance[edge.From].Value + edge.Weight;

                    if (distance[edge.To] == null || newDistance < distance[edge.To].Value)
                    {
                        return edge;
                    }
                }
            }

            return null;
        }
        public void PrintDistances(BellmanFordResult result, int start)
        {
            Console.WriteLine($"Trumpiausi keliai is virsunes {start}:");

            for (int i = 0; i < result.Distance.Length; i++)
            {
                if (result.Distance[i] == null)
                {
                    Console.WriteLine($"{start} -> {i}: kelio nera");
                }
                else
                {
                    string path = GetPath(result.Previous, start, i);
                    Console.WriteLine($"{start} -> {i}: ilgis = {result.Distance[i]}, kelias: {path}");
                }
            }
        }
        private string GetPath(int[] previous, int start, int end)
        {
            List<int> path = new List<int>();
            int current = end;

            while (current != -1)
            {
                path.Add(current);

                if (current == start)
                    break;

                current = previous[current];
            }

            path.Reverse();

            if (path.Count == 0 || path[0] != start)
                return "kelio nera";

            return string.Join(" -> ", path);
        }
    }
}
