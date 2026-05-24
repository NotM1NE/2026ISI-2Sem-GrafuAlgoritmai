using GrafoGeneravimasIrPaieska.Models;
using GrafoGeneravimasIrPaieska.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;

namespace GrafoGeneravimasIrPaieska;

public class Program
{
    private static ILogger<Program> _logger = null!;
    public static void Main(string[] args)
    {
        using var serviceProvider = new ServiceCollection()
            .AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            }).BuildServiceProvider();

        _logger = serviceProvider.GetRequiredService<ILogger<Program>>();
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
            Console.WriteLine("7 - Bellman-Ford");
            Console.WriteLine("8 - Bellman-Ford su eile");
            Console.WriteLine("9 - Taisyti neigiamus ciklus");
            Console.WriteLine("10 - Bellman-Ford greicio testas");
            Console.WriteLine("11 - Eulerio ciklas - HierHolzer");
            Console.WriteLine("12 - Eulerio ciklas - Fluery");
            Console.WriteLine("13 - Eulerio ciklai - Fluery vs HierHolzee greicio testas");
            Console.WriteLine("0 - Baigti darba");
            Console.Write("Pasirinkimas: ");

            string? choice = Console.ReadLine();

            try
            {
                switch (choice)
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
                    case "11":
                        RunHierHolzer(currentGraph);
                        break;
                    case "12":
                        RunFluery(currentGraph);
                        break;
                    case "13":
                        RunFlueryVsHolzerBenchmark();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Neteisinga ivestias");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ivyko klaida");
                throw;
            }
        }
    }

    private static void RunFlueryVsHolzerBenchmark()
    {
        GraphGenerator graphGenerator = new GraphGenerator();

        int[] vertexCount = { 10, 50, 100, 200, 500, 1000 };
        int[] degrees = { 2, 4, 6, 8 };
        int testCount = 5;



        foreach (int degree in degrees)
        {

            Console.WriteLine("===============================================");
            Console.WriteLine($"Tyrimas su k = {degree}");
            Console.WriteLine($"{"Virsunes",-12}{"Briaunos",-12}{"Fleury ms",-15}{"Hierholzer ms",-15}");

            foreach (int vertex in vertexCount)
            {
                if (degree >= vertex)
                {
                    Console.WriteLine($"{vertex}\t\t-\t\tNetinka: k >= V");
                    continue;
                }
                if (vertex * degree % 2 != 0)
                {
                    Console.WriteLine($"{vertex}\t\t-\t\tNetinka: V*k nelyginis");
                    continue;
                }
                double totalFleuryTime = 0;
                double totalHierholzerTime = 0;
                int edgeCount = vertex * degree / 2;
                int successfulTests = 0;
                for (int i = 0; i < testCount; i++)
                {
                    try
                    {

                        Graph graph = graphGenerator.GraphRandomGenerator(vertex, degree, degree, false);
                        EulerAlgorithms eulerAlgorithms = new EulerAlgorithms(graph);
                        Stopwatch stopwatch = new Stopwatch();

                        stopwatch.Start();
                        eulerAlgorithms.Fluery();
                        stopwatch.Stop();
                        totalFleuryTime += stopwatch.Elapsed.TotalMilliseconds;

                        stopwatch.Restart();
                        eulerAlgorithms.HierHolzer();
                        stopwatch.Stop();
                        totalHierholzerTime += stopwatch.Elapsed.TotalMilliseconds;

                        successfulTests++;
                    }
                    catch (ArgumentException aex)
                    {
                        _logger.LogWarning(aex.Message);
                        return;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        return;
                    }
                }
                if (successfulTests > 0)
                {
                    double avgFleuryTime = totalFleuryTime / successfulTests;
                    double avgHierholzerTime = totalHierholzerTime / successfulTests;

                    Console.WriteLine($"{vertex}\t\t{edgeCount}\t\t{avgFleuryTime:F4}\t\t{avgHierholzerTime:F4}");
                }
                else
                {
                    Console.WriteLine($"{vertex}\t\t{edgeCount}\t\tNepavyko sugeneruoti");
                }
            }
        }
    }

    private static void RunHierHolzer(Graph currentGraph)
    {
        try
        {
            EulerAlgorithms eulerAlgorithms = new EulerAlgorithms(currentGraph);
            var path = eulerAlgorithms.HierHolzer();
            Console.WriteLine("Eulerio ciklas Hierholzer algoritmu");
            Console.WriteLine(string.Join(" --> ", path));
        }
        catch(ArgumentException aex)
        {
            _logger.LogWarning(aex.Message);
            return;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.Message);
            return;
        }
    }
    private static void RunFluery(Graph currentGraph)
    {
        try
        {
            EulerAlgorithms eulerAlgorithms = new EulerAlgorithms(currentGraph);
            var path = eulerAlgorithms.Fluery();
            Console.WriteLine("Eulerio ciklas Fluery algoritmu");
            Console.WriteLine(string.Join(" --> ", path));
        }
        catch (ArgumentException aex)
        {
            _logger.LogWarning(aex.Message);
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return;
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
            
            _logger.LogError(ex, "Ivyko klaida GenerateRandomGraph");
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

        if (graph == null)
        {
            _logger.LogError("Nera grafo");
            return;
        }
           

        if (!graph.HasEdge(e, v))
        {
            _logger.LogInformation("Tokios briaunos grafe nera.");
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
            _logger.LogInformation("Grafe yra neigiamas ciklas.");
            _logger.LogInformation($"Problema rasta ties briauna: {result.NegativeCycleEdge}");
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
            _logger.LogWarning("Grafas neegzistuoja");
            return;
        }

        Console.WriteLine("Iveskite pradine virsune:");
        int start = int.Parse(Console.ReadLine());

        BellmanFord bellmanFord = new BellmanFord();

        BellmanFordResult result = bellmanFord.QueueBellmanFord(graph, start);

        if (result.NegativeCycleEdge != null)
        {
            _logger.LogInformation("Grafe yra neigiamas ciklas.");
            _logger.LogInformation($"Problema rasta ties briauna: {result.NegativeCycleEdge}");
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
            _logger.LogWarning("Grafas neegzistuoja");
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

                    _logger.LogInformation($"{vertices}\t{avgTrivialTime:F3}\t\t\t{avgQueueTime:F3}");
                }
                else
                {
                    _logger.LogError($"{vertices}\tNepavyko sugeneruoti");
                }
            }

            Console.WriteLine();
        }
    }
}