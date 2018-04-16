using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitmapTracer.Core.Trace;

namespace BitmapTracer.Core.Tests
{
    public class RegionToPolygon_Test
    {
        public List<int> EdgeCrawler(RegionVO region, RegionManipulator rm)
        {
            RegionEdgeCrawler edgeCrawler = new RegionEdgeCrawler(region, rm);

            return edgeCrawler.GetInOrderEdgePixels();
        }

        public List<int> EdgeCrawler(RegionVO region, RegionManipulator rm, int startPoint)
        {
            RegionEdgeCrawler edgeCrawler = new RegionEdgeCrawler(region, rm);

            return edgeCrawler.GetInOrderEdgePixels(startPoint);
        }
        
    }
}
