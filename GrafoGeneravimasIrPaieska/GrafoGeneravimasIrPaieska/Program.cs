using GrafoGeneravimasIrPaieska.Models;
using GrafoGeneravimasIrPaieska.Services;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;

namespace GrafoGeneravimasIrPaieska;

public class Program
{
    public static void Main(string[] args)
    {
        Graph currentGraph = null;

        while(true)
        {
            //UI
            Console.WriteLine();
            Console.WriteLine("1 - Sukurti grafa ranka");
            Console.WriteLine("2 - Generuoti atsitiktini grafa");
            Console.WriteLine("3 - Spausdinti dabartini grafa");
            Console.WriteLine("4 - Patikrinti ar grafas jungus");
            Console.WriteLine("5 - Patikrinti ar briauna yra tiltas");
            Console.WriteLine("6 - Paleisti greicio testa");
            Console.WriteLine("7 - Bellman-Ford testas");
            Console.WriteLine("8 - Bellman-Ford su eile");
            Console.WriteLine("9 - Taisyti neigiamus ciklus");
            Console.WriteLine("10 - Bellman-Ford greicio testas");
            Console.WriteLine("0 - Baigti darba");
            Console.Write("Pasirinkimas: ");

            string? choice = Console.ReadLine();

            switch(choice)
            {
                case "1":
                    currentGraph = CreateGraph();
                    break;
                case "2":
                    currentGraph = GenerateRandomGraph();
                    break;
                case "3":
                    PrintGraph(currentGraph);
                    break;
                case "4":
                    CheckIfGraphIsConnected(currentGraph);
                    break;
                case "5":
                    CheckBridge(currentGraph);
                    break;
                case "6":
                    RunPerformanceTest();
                    break;
                case "7":
                    RunBellmanFortTest(currentGraph);
                    break;
                case "8":
                    RunQueueBellmanFord(currentGraph);
                    break;
                case "9":
                    FixNegativeCycles(currentGraph);
                    break;
                case "10":
                    RunBellmanFordPerformanceTest();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Neteisinga ivestias");
                    break;
            }
        }

    }


    private static Graph CreateGraph()
    {
        bool directed;
        Console.WriteLine("Iveskite virsuniu skaiciu: ");
        int vertices = int.Parse(Console.ReadLine());

        Console.WriteLine("Ar grafas orientuotas? (taip/ne)");
        
        while(true)
        {
            var output = Console.ReadLine()?.Trim().ToLower();
            if (output == "taip")
            {
                directed = true;
                break;
            }
            else if (output == "ne")
            {
                directed = false;
                break;
            }
            else
            {
                Console.WriteLine("Neteisinga ivestis");
            }
        }
        Graph graph = new Graph(vertices, directed);

        Console.WriteLine("Iveskite briaunas formatu: e v weight");
        Console.WriteLine("Kai baigsite, rasykite: 0");

        while (true)
        {
            Console.Write("Briauna: ");
            var input = Console.ReadLine()?.Trim().ToLower();

            if (input == "0")
                break;

            if (string.IsNullOrEmpty(input))
                continue;

            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 3)
            {
                Console.WriteLine("Neteisingas formatas. Naudokite: e v");
                continue;
            }

            if (!int.TryParse(parts[0], out int e) || !int.TryParse(parts[1], out int v) || !int.TryParse(parts[2], out int weight))
            {
                Console.WriteLine("Reikia ivesti tris sveikus skaicius");
                continue;
            }

            if(!graph.HasVertex(e) || !graph.HasVertex(v))
            {
                Console.WriteLine("Tokia virsune neegzistuoja");
                continue;
            }

            if(e == v)
            {
                Console.WriteLine("Kilpa negalima.");
                continue;
            }
            if (graph.HasEdge(e, v))
            {
                Console.WriteLine("Tokia briauna jau egzistuoja.");
                continue;
            }
            graph.AddEdge(e, v, weight);
            Console.WriteLine($"Prideta briauna: {e}->{v}({weight})");
        }
        Console.WriteLine("Sukurtas grafas:");
        graph.Print();

        return graph;
    }
    private static Graph GenerateRandomGraph()
    {
        GraphGenerator graphGenerator = new GraphGenerator();
        try
        {
            bool directed;

            Console.WriteLine("Iveskite virsuniu skaiciu: ");
            int vertices = int.Parse(Console.ReadLine());
            Console.WriteLine("Iveskite kMin: ");
            int kMin = int.Parse(Console.ReadLine());
            Console.WriteLine("Iveskite kMax: ");
            int kMax = int.Parse(Console.ReadLine());

            Console.WriteLine("Ar grafas orientuotas? (taip/ne)");
            while (true)
            {
                var output = Console.ReadLine()?.Trim().ToLower();
                if (output == "taip")
                {
                    directed = true;
                    break;
                }
                else if (output == "ne")
                {
                    directed = false;
                    break;
                }
                else
                {
                    Console.WriteLine("Neteisinga ivestis");
                }
            }

            Graph graph = graphGenerator.GraphRandomGenerator(vertices, kMin, kMax, directed);

            Console.WriteLine("Grafas sekmingai sugeneruotas:");
            graph.Print();

            return graph;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
    private static void PrintGraph(Graph graph)
    {
        if (graph != null)
            graph.Print();
        else
            Console.WriteLine("Grafas neegizstuoja");
    }
    private static void CheckIfGraphIsConnected(Graph graph)
    {
        GraphDFS graphDFS = new GraphDFS();
        if (graph != null)
        {
            bool isConnected = graphDFS.IsConnected(graph);
            Console.WriteLine(isConnected ? "Grafas yra jungus." : "Grafas nera jungus.");
        }
        else
            Console.WriteLine("Grafas neegizstuoja");
    }
    private static void CheckBridge(Graph graph)
    {
        GraphDFS graphDFS = new GraphDFS();
        Console.WriteLine("Iveskite pirma virsune: ");
        int e = int.Parse(Console.ReadLine());
        Console.WriteLine("Iveskite antra virsune: ");
        int v = int.Parse(Console.ReadLine());

        if (!graph.HasEdge(e, v))
        {
            Console.WriteLine("Tokios briaunos grafe nera.");
            return;
        }

        bool isBridge = graphDFS.IsBridge(graph, e, v);

        Console.WriteLine(isBridge
            ? $"Briauna {e}-{v} yra tiltas."
            : $"Briauna {e}-{v} nera tiltas.");
    }
    private static void RunPerformanceTest()
    {
        GraphDFS graphDFS = new GraphDFS();
        GraphGenerator graphGenerator = new GraphGenerator();

        int[] vertexCounts = { 100, 200, 500, 1000 };
        int testCount = 5;

        int[] kMinValues = { 2, 4, 6 };
        int[] kMaxValues = { 4, 8, 10 };

        for (int s = 0; s < kMinValues.Length; s++)
        {
            int kMin = kMinValues[s];
            int kMax = kMaxValues[s];

            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine($"Tyrimas su kMin = {kMin}, kMax = {kMax}");
            Console.WriteLine("V\tVid. generavimo laikas (ms)\tVid. tilto tikrinimo laikas (ms)");

            foreach (int vertices in vertexCounts)
            {
                double totalGenerationTime = 0;
                double totalBridgeTime = 0;
                int successfulTests = 0;

                for (int i = 0; i < testCount; i++)
                {
                    try
                    {
                        Stopwatch stopwatch = new Stopwatch();

                        stopwatch.Start();
                        Graph graphTest = graphGenerator.GraphRandomGenerator(vertices, kMin, kMax, false);
                        stopwatch.Stop();
                        totalGenerationTime += stopwatch.Elapsed.TotalMilliseconds;

                        int e = -1;
                        int v = -1;

                        foreach (int vertex in graphTest.AdjencyList.Keys)
                        {
                            if (graphTest.AdjencyList[vertex].Count > 0)
                            {
                                e = vertex;
                                v = graphTest.AdjencyList[vertex][0].To;
                                break;
                            }
                        }

                        if (e != -1 && v != -1)
                        {
                            stopwatch.Restart();
                            graphDFS.IsBridge(graphTest, e, v);
                            stopwatch.Stop();
                            totalBridgeTime += stopwatch.Elapsed.TotalMilliseconds;
                        }

                        successfulTests++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                if (successfulTests > 0)
                {
                    double avgGenerationTime = totalGenerationTime / successfulTests;
                    double avgBridgeTime = totalBridgeTime / successfulTests;

                    Console.WriteLine($"{vertices}\t{avgGenerationTime:F3}\t\t\t{avgBridgeTime:F3}");
                }
                else
                {
                    Console.WriteLine($"{vertices}\tNepavyko sugeneruoti");
                }
            }

            Console.WriteLine();
        }
    }
    private static void RunBellmanFortTest(Graph graph)
    {
        if (graph == null)
        {
            Console.WriteLine("Grafas neegzistuoja");
            return;
        }

        Console.WriteLine("Iveskite pradine virsune: ");
        int start = int.Parse(Console.ReadLine());

        BellmanFord bellmanFord = new BellmanFord();

        BellmanFordResult result = bellmanFord.TrivialBellmanFordResult(graph, start);

        if (result.NegativeCycleEdge != null)
        {
            Console.WriteLine("Grafe yra neigiamas ciklas.");
            Console.WriteLine($"Problema rasta ties briauna: {result.NegativeCycleEdge}");
        }
        else
        {
            bellmanFord.PrintDistances(result, start);
        }
    }

    private static void RunQueueBellmanFord(Graph graph)
    {
        if (graph == null)
        {
            Console.WriteLine("Grafas neegzistuoja");
            return;
        }

        Console.WriteLine("Iveskite pradine virsune:");
        int start = int.Parse(Console.ReadLine());

        BellmanFord bellmanFord = new BellmanFord();

        BellmanFordResult result = bellmanFord.QueueBellmanFord(graph, start);

        if (result.NegativeCycleEdge != null)
        {
            Console.WriteLine("Grafe yra neigiamas ciklas.");
            Console.WriteLine($"Problema rasta ties briauna: {result.NegativeCycleEdge}");
        }
        else
        {
            bellmanFord.PrintDistances(result, start);
        }
    }

    private static void FixNegativeCycles(Graph graph)
    {
        if (graph == null)
        {
            Console.WriteLine("Grafas neegzistuoja");
            return;
        }

        Console.WriteLine("Iveskite pradine virsune:");
        int start = int.Parse(Console.ReadLine());

        BellmanFord bellmanFord = new BellmanFord();

        bellmanFord.FixNegativeCycles(graph, start);
    }
    private static void RunBellmanFordPerformanceTest()
    {
        GraphGenerator graphGenerator = new GraphGenerator();
        BellmanFord bellmanFord = new BellmanFord();

        int[] vertexCounts = { 100, 200, 500, 1000 };
        int testCount = 5;

        int[] kMinValues = { 2, 4, 6 };
        int[] kMaxValues = { 4, 8, 10 };

        for (int s = 0; s < kMinValues.Length; s++)
        {
            int kMin = kMinValues[s];
            int kMax = kMaxValues[s];

            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine($"Tyrimas su kMin = {kMin}, kMax = {kMax}");
            Console.WriteLine("V\tTrivialus B-F (ms)\tB-F su eile (ms)");

            foreach (int vertices in vertexCounts)
            {
                double totalTrivialTime = 0;
                double totalQueueTime = 0;
                int successfulTests = 0;

                for (int i = 0; i < testCount; i++)
                {
                    try
                    {
                        Graph graphTest = graphGenerator.GraphRandomGenerator(vertices, kMin, kMax, true);

                        Stopwatch stopwatch = new Stopwatch();

                        stopwatch.Start();
                        bellmanFord.TrivialBellmanFordResult(graphTest, 0);
                        stopwatch.Stop();
                        totalTrivialTime += stopwatch.Elapsed.TotalMilliseconds;

                        stopwatch.Restart();
                        bellmanFord.QueueBellmanFord(graphTest, 0);
                        stopwatch.Stop();
                        totalQueueTime += stopwatch.Elapsed.TotalMilliseconds;

                        successfulTests++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                if (successfulTests > 0)
                {
                    double avgTrivialTime = totalTrivialTime / successfulTests;
                    double avgQueueTime = totalQueueTime / successfulTests;

                    Console.WriteLine($"{vertices}\t{avgTrivialTime:F3}\t\t\t{avgQueueTime:F3}");
                }
                else
                {
                    Console.WriteLine($"{vertices}\tNepavyko sugeneruoti");
                }
            }

            Console.WriteLine();
        }
    }
}