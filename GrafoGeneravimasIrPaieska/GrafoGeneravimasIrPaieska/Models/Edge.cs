using System;
using System.Collections.Generic;
using System.Text;

namespace GrafoGeneravimasIrPaieska.Models
{
    public class Edge
    {
        public int From { get; set; }
        public int To { get; set; }
        public int Weight { get; set; }

        public Edge(int from, int to, int weight)
        {
            From = from;
            To = to;
            Weight = weight;
        }
        public override string ToString()
        {
            return $"{From} -> {To} ({Weight})";
        }
    }
}
