using System;
using System.Collections.Generic;
using System.Text;
using Grafo_generavimas_ir_paprasta_paieska.Models;

namespace Grafo_generavimas_ir_paprasta_paieska.Services
{
    
    public class GraphGenerator
    {
        private readonly Random _random = new Random();
        //nevisada sugeneruoja
        //public Graph GraphRandomGenerator(int vertices, int kMin, int kMax, bool directed)
        //{
        //    if (vertices <= 0)
        //        throw new ArgumentOutOfRangeException("Virsuniu skaicius turetu but didesnis nei 0");
        //    if (kMin < 0 || kMax < 0)
        //        throw new ArgumentOutOfRangeException("kMin ir kMax negali buti neigiami");
        //    if (kMin < 0 || kMax < 0)
        //        throw new ArgumentOutOfRangeException("kMin ir kMax negali buti neigiami");

        //    Graph graph = new Graph(vertices, directed);

        //    while (!AllVericesHaveMinDegree(graph, kMin))
        //    {
        //        if (!CanAddAnyMoreEdges(graph, kMax))
        //            throw new Exception("Nepavyko sugeneruoti grafo su nurodytais parametrais");

        //        int e = _random.Next(vertices);
        //        int v = _random.Next(vertices);

        //        bool canAdd = true;

        //        if (e == v)
        //            canAdd = false;
        //        if (graph.HasEdge(e, v))
        //            canAdd = false;
        //        if (graph.GetDegree(e) >= kMax || graph.GetDegree(v) >= kMax)
        //            canAdd = false;
        //        if (canAdd)
        //            graph.AddEdge(e, v);
        //    }
        //    return graph;

        //}
        public Graph GraphRandomGenerator(int vertices, int kMin, int kMax, bool directed)
        {
            if (vertices <= 0)
                throw new ArgumentOutOfRangeException("Virsuniu skaicius turetu but didesnis nei 0");
            if (kMin < 0 || kMax < 0)
                throw new ArgumentOutOfRangeException("kMin ir kMax negali buti neigiami");
            if (kMin > kMax)
                throw new ArgumentOutOfRangeException("kMin negali buti didesnis uz kMax");

            Graph graph = new Graph(vertices, directed);

            for (int e = 0; e < vertices; e++)
            {
                while (graph.GetDegree(e) < kMin)
                {
                    List<int> possibleVertices = new List<int>();

                    for (int v = 0; v < vertices; v++)
                    {
                        if (e != v && !graph.HasEdge(e, v)  && graph.GetDegree(v) < kMax) // && graph.GetDegree(e) < kMax
                            possibleVertices.Add(v);
                    }
                    if (possibleVertices.Count == 0)
                        throw new Exception("Nepavyko sugeneruoti grafo su nurodytais parametrais");

                    int randomIndex = _random.Next(possibleVertices.Count);
                    graph.AddEdge(e, possibleVertices[randomIndex]);
                }
            }
            return graph;
        }
    }
}
