using System;
using System.Collections.Generic;
using System.Text;

namespace GrafoGeneravimasIrPaieska.Models
{
    public record BellmanFordResult
    (
        int?[] Distance,
        int[] Previous,
        Edge NegativeCycleEdge
    );
}
