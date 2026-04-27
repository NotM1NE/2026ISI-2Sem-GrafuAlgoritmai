using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace GrafoGeneravimasIrPaieska.Models
{
    public class Graph
    {
        public int Vertices { get; set; }
        public bool Directed { get; set; }
        public Dictionary<int, List<Edge>> AdjencyList { get; set; }      //gretimo sarasas

        public Graph(int vertices, bool directed = false)
        {
            Vertices = vertices;
            Directed = directed;
            AdjencyList = new Dictionary<int, List<Edge>>();

            for (int i = 0; i < vertices; i++)
            {
                AdjencyList[i] = new List<Edge>();
            }
        }

        public bool HasVertex(int vertex)
        {
            return AdjencyList.ContainsKey(vertex);
        }
        public bool HasEdge(int from, int to)
        {
            if (!HasVertex(from) || !HasVertex(to))
                return false;

            return AdjencyList[from].Any(e => e.To == to);
        }
        public void AddEdge(int from, int to, int weight)
        {
            if (!HasVertex(from))
                throw new ArgumentException(nameof(from), "tokios virsunes grafas neturi");
            if (!HasVertex(to))
                throw new ArgumentException(nameof(to), "tokios virsunes grafas neturi");
            if (from == to)
                throw new ArgumentException("Kilpos paprastame grafe negalimos");
            if (HasEdge(from, to))
                throw new ArgumentException("Tokia briauna jau egzistuoja");

            AdjencyList[from].Add(new Edge(from, to, weight));
            if (Directed == false)
                AdjencyList[to].Add(new Edge(to, from, weight));
        }
        public void ChangeEdgeWeight(int from, int to, int weight)
        {
            Edge edge = AdjencyList[from].First(e => e.To == to);

            if(edge == null)
                throw new ArgumentException(nameof(edge), "tokios briaunos grafas neturi");

            edge.Weight = weight;
        }
        public void RemoveEdge(int from, int to, int weight)
        {
            if (!HasEdge(from, to))
                throw new ArgumentException("tokios brianos grafas neturi");

            AdjencyList[from].RemoveAll(edge => edge.To == to);
            if (Directed == false)
                AdjencyList[to].RemoveAll(edge => edge.To == from);
        }
        public int GetDegree(int vertex)
        {
            if (!HasVertex(vertex))
                throw new ArgumentException(nameof(vertex), "tokios virsunes grafas neturi");

            return AdjencyList[vertex].Count;
        }
        public List<Edge> GetNeighbours(int vertex)
        {
            if (!HasVertex(vertex))
                throw new ArgumentException(nameof(vertex), "tokios virsunes grafas neturi");

            return AdjencyList[vertex];
        }
        public List<Edge> GetAllEdges()
        {
            List<Edge> edges = new List<Edge>();

            foreach(var vertex in AdjencyList)
            {
                foreach(var edge in vertex.Value)
                {
                    edges.Add(edge);
                }
            }
            return edges;
        }
        public int GetInDegree(int vertex)
        {
            int count = 0;

            foreach (var list in AdjencyList.Values)
            {
                foreach (var edge in list)
                {
                    if (edge.To == vertex)
                        count++;
                }
            }
            return count;
        }
        public void Print()
        {
            foreach (var vertex in AdjencyList)
            {
                Console.Write($"{vertex.Key} : ");
                foreach (var edge in vertex.Value)
                {
                    Console.Write($"{edge.To}({edge.Weight}) ");
                }
                Console.WriteLine();
            }
        }
    }
}
