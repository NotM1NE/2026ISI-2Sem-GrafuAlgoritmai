using Grafo_generavimas_ir_paprasta_paieska.Models;
using Grafo_generavimas_ir_paprasta_paieska.Services;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;

namespace Grafo_generavimas_ir_paprasta_paieska;

public class Program
{
    public static void Main(string[] args)
    {
        Graph graph = new Graph(5, false);

        graph.AddEdge(0, 1);
        graph.AddEdge(0, 2);
        graph.AddEdge(1, 3);
        graph.AddEdge(3, 4);

        graph.Print();
        Console.WriteLine($"Ar yra briauna 0-1: {graph.HasEdge(0, 1)}");
        Console.WriteLine($"Ar yra briauna 1-4: {graph.HasEdge(1, 4)}");
        Console.WriteLine($"Ar yra grafas 1: {graph.HasVertex(1)}");

        GraphGenerator graphGenerator = new GraphGenerator();
        Graph graph1 = null; 
        try
        {
            graph1 = graphGenerator.GraphRandomGenerator(20, 4, 8, false);

            graph1.Print();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        GraphDFS graphDFS = new GraphDFS();
        var result = graphDFS.IsConnected(graph1);


        //paleidziame testa
        GraphGenerator graphGeneratorTest = new GraphGenerator();
        GraphDFS graphDFSTest = new GraphDFS();

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
                        Graph graphTest = graphGeneratorTest.GraphRandomGenerator(vertices, kMin, kMax, false);
                        stopwatch.Stop();
                        totalGenerationTime += stopwatch.Elapsed.TotalMilliseconds;

                        int e = -1;
                        int v = -1;

                        foreach (int vertex in graphTest.AdjencyList.Keys)
                        {
                            if (graphTest.AdjencyList[vertex].Count > 0)
                            {
                                e = vertex;
                                v = graphTest.AdjencyList[vertex][0];
                                break;
                            }
                        }

                        if (e != -1 && v != -1)
                        {
                            stopwatch.Restart();
                            graphDFSTest.IsBridge(graphTest, e, v);
                            stopwatch.Stop();
                            totalBridgeTime += stopwatch.Elapsed.TotalMilliseconds;
                        }

                        successfulTests++;
                    }
                    catch(Exception ex)
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
}