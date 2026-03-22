using GrafoGeneravimasIrPaieska.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrafoGeneravimasIrPaieska.Services
{
    public class GraphGenerator
    {
        private readonly Random _random = new Random();
        public Graph GraphRandomGenerator(int vertices, int kMin, int kMax, bool directed)
        {
            if (vertices <= 0)
                throw new ArgumentOutOfRangeException("Virsuniu skaicius turetu but didesnis nei 0");
            if (kMin < 0 || kMax < 0)
                throw new ArgumentOutOfRangeException("kMin ir kMax negali buti neigiami");
            if (kMin > kMax)
                throw new ArgumentOutOfRangeException("kMin negali buti didesnis uz kMax");

            if (kMin == kMax)
                return GenerateConnectedRegularGraph(vertices, kMax,directed);

            Graph graph = new Graph(vertices, directed);

            for (int e = 0; e < vertices; e++)
            {
                while (graph.GetDegree(e) < kMin)
                {
                    List<int> possibleVertices = new List<int>();

                    for (int v = 0; v < vertices; v++)
                    {
                        if (e != v && !graph.HasEdge(e, v) && graph.GetDegree(v) < kMax && graph.GetDegree(e) < kMax)
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
        private Graph GenerateConnectedRegularGraph(int vertices, int k, bool directed)
        {
            if (directed)
                throw new Exception("Kol kas veikia tik su neorientuotu grafu");

            if (k >= vertices)
                throw new Exception("k turi buti mazesnis uz virsuniu skaiciu");

            if ((vertices * k) % 2 != 0)
                throw new Exception("Tokio grafo sugeneruoti negalima");

            if (k == 0)
            {
                if (vertices == 1)
                    return new Graph(1, false);

                throw new Exception("Jungus grafas negali tureti visu virsuniu su 0 laipsniu");
            }

            if (k == 1)
            {
                if (vertices == 2)
                {
                    Graph graph = new Graph(2, false);
                    graph.AddEdge(0, 1);
                    return graph;
                }

                throw new Exception("Jungus 1-reguliarus grafas galimas tik kai yra 2 virsunes");
            }

            for (int attempt = 0; attempt < 100; attempt++)
            {
                Graph graph = new Graph(vertices, false);

                // 1. Sukuriam cikla, kad grafas butu jungus
                for (int i = 0; i < vertices; i++)
                {
                    int next = (i + 1) % vertices;
                    graph.AddEdge(i, next);
                }

                // Jei reikia tik 2 laipsnio, ciklo uztenka
                if (k == 2)
                    return graph;

                // 2. Pridedam papildomas briaunas
                bool success = AddRemainingEdges(graph, vertices, k);

                if (success && AllVerticesHaveDegree(graph, vertices, k))
                    return graph;
            }

            throw new Exception("Nepavyko sugeneruoti jungaus reguliaraus grafo");
        }
        private bool AddRemainingEdges(Graph graph, int vertices, int k)
        {
            int tries = 0;
            int maxTries = 100000;

            while (!AllVerticesHaveDegree(graph, vertices, k))
            {
                tries++;

                if (tries > maxTries)
                    return false;

                List<int> notFullVertices = new List<int>();

                for (int i = 0; i < vertices; i++)
                {
                    if (graph.GetDegree(i) < k)
                        notFullVertices.Add(i);
                }

                if (notFullVertices.Count < 2)
                    return false;

                int a = notFullVertices[_random.Next(notFullVertices.Count)];
                int b = notFullVertices[_random.Next(notFullVertices.Count)];

                if (a == b)
                    continue;

                if (graph.HasEdge(a, b))
                    continue;

                if (graph.GetDegree(a) >= k)
                    continue;

                if (graph.GetDegree(b) >= k)
                    continue;

                graph.AddEdge(a, b);
            }

            return true;
        }

        private bool AllVerticesHaveDegree(Graph graph, int vertices, int k)
        {
            for (int i = 0; i < vertices; i++)
            {
                if (graph.GetDegree(i) != k)
                    return false;
            }

            return true;
        }
    }
}
