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
        public BellmanFordResult QueueBellmanFord(Graph graph, int start)
        {
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));

            if (!graph.HasVertex(start))
                throw new ArgumentOutOfRangeException(nameof(start), "Tokios virsunes nera");

            int?[] distance = new int?[graph.Vertices];
            int[] previous = new int[graph.Vertices];
            bool[] inQueue = new bool[graph.Vertices];
            int[] relaxCount = new int[graph.Vertices];

            for (int i = 0; i < graph.Vertices; i++)
            {
                distance[i] = null;
                previous[i] = -1;
            }

            Queue<int> queue = new Queue<int>();

            distance[start] = 0;
            queue.Enqueue(start);
            inQueue[start] = true;

            while (queue.Count > 0)
            {
                int current = queue.Dequeue();
                inQueue[current] = false;

                foreach (Edge edge in graph.GetNeighbours(current))
                {
                    if (distance[current] != null)
                    {
                        int newDistance = distance[current].Value + edge.Weight;

                        if (distance[edge.To] == null || newDistance < distance[edge.To].Value)
                        {
                            distance[edge.To] = newDistance;
                            previous[edge.To] = current;
                            relaxCount[edge.To]++;

                            if (relaxCount[edge.To] >= graph.Vertices)
                            {
                                return new BellmanFordResult(distance, previous, edge);
                            }

                            if (!inQueue[edge.To])
                            {
                                queue.Enqueue(edge.To);
                                inQueue[edge.To] = true;
                            }
                        }
                    }
                }
            }

            return new BellmanFordResult(distance, previous, null);
        }
        public void FixNegativeCycles(Graph graph, int start)
        {
            int changeNumber = 1;

            while (true)
            {
                BellmanFordResult result = TrivialBellmanFordResult(graph, start);

                if (result.NegativeCycleEdge == null)
                {
                    Console.WriteLine("Neigiamu ciklu nebeliko.");
                    break;
                }

                List<Edge> cycle = GetNegativeCycle(graph, result.Previous, result.NegativeCycleEdge);

                if (cycle.Count == 0)
                {
                    Console.WriteLine("Nepavyko atkurti neigiamo ciklo.");
                    break;
                }

                int cycleSum = 0;

                foreach (Edge edge in cycle)
                {
                    cycleSum += edge.Weight;
                }

                Console.WriteLine();
                Console.WriteLine($"Pakeitimas #{changeNumber}");
                Console.WriteLine("Rastas neigiamas ciklas:");

                foreach (Edge edge in cycle)
                {
                    Console.WriteLine(edge);
                }

                Console.WriteLine($"Ciklo ilgis: {cycleSum}");

                Edge edgeToChange = null;

                foreach (Edge edge in cycle)
                {
                    if (edge.Weight < 0)
                    {
                        edgeToChange = edge;
                        break;
                    }
                }

                if (edgeToChange == null)
                {
                    Console.WriteLine("Cikle nerasta neigiama briauna.");
                    break;
                }

                int oldWeight = edgeToChange.Weight;
                int newWeight = oldWeight - cycleSum;

                graph.ChangeEdgeWeight(edgeToChange.From, edgeToChange.To, newWeight);

                Console.WriteLine($"Keiciama briauna: {edgeToChange.From} -> {edgeToChange.To}");
                Console.WriteLine($"Senas svoris: {oldWeight}");
                Console.WriteLine($"Naujas svoris: {newWeight}");
                Console.WriteLine($"Po pakeitimo ciklo ilgis tampa: 0");

                changeNumber++;
            }

            Console.WriteLine();
            Console.WriteLine("Galutinis grafas:");
            graph.Print();
        }
        private List<Edge> GetNegativeCycle(Graph graph, int[] previous, Edge problemEdge)
        {
            List<int> cycleVertices = new List<int>();

            int current = problemEdge.To;

            for (int i = 0; i < graph.Vertices; i++)
            {
                current = previous[current];

                if (current == -1)
                    return new List<Edge>();
            }

            int start = current;
            cycleVertices.Add(start);

            current = previous[start];

            while (current != start && current != -1)
            {
                cycleVertices.Add(current);
                current = previous[current];
            }

            if (current == -1)
                return new List<Edge>();

            cycleVertices.Reverse();

            List<Edge> cycleEdges = new List<Edge>();

            for (int i = 0; i < cycleVertices.Count; i++)
            {
                int from = cycleVertices[i];
                int to = cycleVertices[(i + 1) % cycleVertices.Count];

                Edge edge = graph.AdjencyList[from].Find(e => e.To == to);

                if (edge != null)
                    cycleEdges.Add(edge);
            }

            return cycleEdges;
        }
    }
}
