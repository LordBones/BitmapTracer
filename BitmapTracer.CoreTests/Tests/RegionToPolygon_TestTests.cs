using Microsoft.VisualStudio.TestTools.UnitTesting;
using BitmapTracer.Core.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitmapTracer.Core.basic;
using System.Windows;

namespace BitmapTracer.Core.Tests.Tests
{
    [TestClass()]
    public class RegionToPolygon_TestTests
    {
        [TestMethod()]
        public void Test_PolygonCrossTest()
        {
            CanvasPixel canvas = new CanvasPixel(20, 20);
            Trace.RegionManipulator rm = new Trace.RegionManipulator(canvas, canvas);

            Trace.RegionVO region = new Trace.RegionVO(new Pixel());
            region.Pixels = Helper_GetCrossPolygon(rm);

            Helper_Test_EdgeCrawler(region, rm,null);
        }

        [TestMethod()]
        public void Test_PolygonRectangleTest()
        {
            CanvasPixel canvas = new CanvasPixel(20, 20);
            Trace.RegionManipulator rm = new Trace.RegionManipulator(canvas, canvas);

            Trace.RegionVO region = new Trace.RegionVO(new Pixel());
            region.Pixels = Helper_GetRectanglePolygon(rm);

            Helper_Test_EdgeCrawler(region, rm, new Point(9, 9));


        }

        [TestMethod()]
        public void Test_PolygonCustomTest()
        {
            CanvasPixel canvas = new CanvasPixel(11, 20);
            Trace.RegionManipulator rm = new Trace.RegionManipulator(canvas, canvas);

            Trace.RegionVO region = new Trace.RegionVO(new Pixel());
            region.Pixels = Helper_GetCustomPolygon(rm);

            Helper_Test_EdgeCrawler(region, rm, new Point(9, 9));


        }

        private void Helper_Test_EdgeCrawler(Trace.RegionVO region, Trace.RegionManipulator rm, Point? startPoint)
        {
            List<int> result = 
                (startPoint.HasValue)?
                new RegionToPolygon_Test().EdgeCrawler(region, rm, rm.Get_IndexFromPoint(startPoint.Value)) :
            new RegionToPolygon_Test().EdgeCrawler(region, rm);

            int[] correctResult = rm.Get_RegionEdges(region);

            bool resultCompare = Helper_ComparePixelRegions(result.ToArray(), correctResult);

            Assert.IsTrue(resultCompare);
        }

        private int [] Helper_GetCrossPolygon(Trace.RegionManipulator rm)
        {
            List<Point> points = new List<Point>();
            points.Add(new Point(10, 9));
            points.Add(new Point(9, 10));
            points.Add(new Point(10, 10));
            points.Add(new Point(11, 10));
            points.Add(new Point(10, 11));

            return points.Select(x => rm.Get_IndexFromPoint(x)).ToArray();
        }

        

        private int[] Helper_GetRectanglePolygon(Trace.RegionManipulator rm)
        {
            List<Point> points = new List<Point>();
            points.Add(new Point(10, 9));
            points.Add(new Point(10, 10));
            points.Add(new Point(10, 11));
            points.Add(new Point(10, 12));
            points.Add(new Point(9, 12));
            points.Add(new Point(9, 11));
            points.Add(new Point(9, 10));
            points.Add(new Point(9, 9));

            return points.Select(x => rm.Get_IndexFromPoint(x)).ToArray();
        }

        private int[] Helper_GetCustomPolygon(Trace.RegionManipulator rm)
        {
            List<Point> points = new List<Point>();
            points.Add(new Point(10, 9));
            points.Add(new Point(10, 10));
            points.Add(new Point(10, 11));
            points.Add(new Point(10, 12));
            points.Add(new Point(9, 12));
            points.Add(new Point(9, 11));
            points.Add(new Point(9, 10));
            points.Add(new Point(9, 9));

            return points.Select(x => rm.Get_IndexFromPoint(x)).ToArray();
        }



        private bool Helper_ComparePixelRegions(int [] data, int [] data2)
        {
            data = data.Distinct().OrderBy(x => x).ToArray();
            data2 = data2.Distinct().OrderBy(x => x).ToArray();

            if (data.Length != data2.Length) return false;

            for(int i = 0;i< data.Length;i++)
            {
                if (data[i] != data2[i]) return false;
            }

            return true;
        }

    }
}