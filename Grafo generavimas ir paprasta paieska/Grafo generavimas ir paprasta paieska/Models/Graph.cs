using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Grafo_generavimas_ir_paprasta_paieska.Models
{
    public class Graph
    {
        public int Vertices { get; set; }
        public bool Directed { get; set; }
        public Dictionary<int, List<int>> AdjencyList { get; set; }      //gretimo sarasas

        public Graph(int vertices, bool directed = false)
        {
            Vertices = vertices;
            Directed = directed;
            AdjencyList = new Dictionary<int, List<int>>();

            for (int i = 0; i < vertices; i++)
            {
                AdjencyList[i] = new List<int>();
            }
        }
        public bool HasVertex(int vertex)
        {
            return AdjencyList.ContainsKey(vertex);
        }
        public bool HasEdge(int e, int v)
        {
            if (!HasVertex(e) || !HasVertex(v))
                return false;

            return AdjencyList[e].Contains(v);
        }
        public void AddEdge(int e, int v)
        {
            if (!HasVertex(e))
                throw new ArgumentException(nameof(e), "tokios virsunes grafas neturi");
            if (!HasVertex(v))
                throw new ArgumentException(nameof(v), "tokios virsunes grafas neturi");
            if (e == v)
                throw new ArgumentException("Kilpos paprastame grafe negalimos");
            if (HasEdge(e, v))
                throw new ArgumentException("Tokia briauna jau egzistuoja");

            AdjencyList[e].Add(v);
            if (Directed == false)
                AdjencyList[v].Add(e);
        }
        public void RemoveEdge(int e, int v)
        {
            if (!HasEdge(e, v))
                throw new ArgumentException("tokios brianos grafas neturi");

            AdjencyList[e].Remove(v);
            if (Directed == false)
                AdjencyList[v].Remove(e);
        }
        public int GetDegree(int vertex)
        {
            if (!HasVertex(vertex))
                throw new ArgumentException(nameof(vertex), "tokios virsunes grafas neturi");

            return AdjencyList[vertex].Count;
        }
        public List<int> GetNeighbours(int vertex)
        {
            if(!HasVertex(vertex))
                throw new ArgumentException(nameof(vertex), "tokios virsunes grafas neturi");

            return new List<int>(AdjencyList[vertex]);
        }
        public void Print()
        {
            foreach(var vertex in AdjencyList)
            {
                Console.Write($"{vertex.Key} : ");
                foreach(var neighbour in vertex.Value)
                {
                    Console.Write($"{neighbour} ");
                }
                Console.WriteLine();
            }
        }
    }
}
