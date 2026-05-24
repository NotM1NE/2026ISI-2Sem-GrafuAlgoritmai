using GrafoGeneravimasIrPaieska.Models;
using GrafoGeneravimasIrPaieska.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrafoGeneravimasIrPaieska.Services
{
    public class EulerAlgorithms
    {
        private readonly Graph _currentGraph;
        private readonly GraphDFS _graphDFS = new GraphDFS();
        public EulerAlgorithms(Graph currentGraph)
        {
            if (currentGraph == null)
                throw new ArgumentException("Grafas neegzistuoja");
          
            if (currentGraph.Directed)
                throw new ArgumentException("Grafas turi buti neorientuotas");

            if (!_graphDFS.IsConnected(currentGraph))
                throw new ArgumentException("Grafas ner jungus, Eulerio ciklo nera");

            for (int i = 0; i < currentGraph.Vertices; i++)
            {
                if (currentGraph.GetDegree(i) % 2 != 0)
                {
                    throw new ArgumentException("Ne visu virsuniu laipsniai lyginiai, Eulerio ciklo nera");
                }
            }

            _currentGraph = currentGraph;
        }
        public List<int> HierHolzer()
        {
            Graph tempGraph = _currentGraph.CloneGraph();

            List<int> path = new List<int>();
            List<int> cycle = new List<int>();

            path.Add(0);

            while (path.Count > 0)
            {
                int current = path[path.Count - 1];

                if (tempGraph.AdjencyList[current].Count > 0)
                {
                    Edge edge = tempGraph.AdjencyList[current][0];

                    path.Add(edge.To);

                    tempGraph.RemoveEdge(edge);
                }
                else
                {
                    cycle.Add(current);
                    path.RemoveAt(path.Count - 1);
                }
            }

            cycle.Reverse();

            return cycle;
        }
        public List<int> Fluery()
        {
            Graph tempGraph = _currentGraph.CloneGraph();

            List<int> path = new List<int>();

            path.Add(0);

            int current = 0;
            while(tempGraph.GetAllEdges().Count > 0)
            {
                var neighbours = tempGraph.GetNeighbours(current).ToList();

                Edge selectedEdge = null;
                if(neighbours.Count == 1)
                    selectedEdge = neighbours[0];
                else
                {
                    foreach(var edge in neighbours)
                    {
                        if(!_graphDFS.IsBridgeForEuler(tempGraph, edge.From, edge.To))
                        {
                            selectedEdge = edge;
                            break;
                        }
                    }
                    if (selectedEdge == null)
                        selectedEdge = neighbours[0];
                }

                tempGraph.RemoveEdge(selectedEdge);

                current = selectedEdge.To;
                path.Add(current);
                    
            }
            return path;
        }
    }
}
