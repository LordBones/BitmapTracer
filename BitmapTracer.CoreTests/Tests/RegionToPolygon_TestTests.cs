using Microsoft.VisualStudio.TestTools.UnitTesting;
using BitmapTracer.Core.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitmapTracer.Core.basic;
using System.Windows;
using BitmapTracer.Core.Trace;

namespace BitmapTracer.Core.Tests.Tests
{
    [TestClass()]
    public class RegionToPolygon_TestTests
    {
        [TestMethod()]
        public void Test_PolygonCrossTest()
        {
            // arrange
            Test2DPolygonDefinition testPolygon = Helper_Get2DCrossPolygon();
            Trace.RegionManipulator rm = Helper_Create_RegionManipulator(testPolygon);

            Trace.RegionVO region = Helper_Create_RegionFromParam(testPolygon, rm);

            int[] correctPixelsIndex = Helper_Create_PixelsOfRegionFromParam(Helper_Get2DCrossPolygon_Correct(), rm);

            // act
            int[] newPixelsIndex = new RegionToPolygon_Test().EdgeCrawler(region, rm).ToArray();

            // assert
            bool resultCompare = Helper_ComparePixelRegions(newPixelsIndex, correctPixelsIndex);

            Assert.IsTrue(resultCompare);

        }

       
        [TestMethod()]
        public void Test_PolygonRectangleTest()
        {
            // arrange
            Test2DPolygonDefinition testPolygon = Helper_Get2DRectanglePolygon();
            Trace.RegionManipulator rm = Helper_Create_RegionManipulator(testPolygon);

            Trace.RegionVO region = Helper_Create_RegionFromParam(testPolygon, rm);

            int[] correctPixelsIndex = Helper_Create_PixelsOfRegionFromParam(Helper_Get2DRectanglePolygon_Correct(), rm);

            // act
            int[] newPixelsIndex = new RegionToPolygon_Test().EdgeCrawler(region, rm).ToArray();

            // assert
            bool resultCompare = Helper_ComparePixelRegions(newPixelsIndex, correctPixelsIndex);

            Assert.IsTrue(resultCompare);
        }

        [TestMethod()]
        public void Test_PolygonRectangle_AtEdgeTest()
        {
            // arrange
            Test2DPolygonDefinition testPolygon = Helper_Get2DRectangleEdgePolygon();
            Trace.RegionManipulator rm = Helper_Create_RegionManipulator(testPolygon);
            Trace.RegionVO region = Helper_Create_RegionFromParam(testPolygon, rm);

            int[] correctPixelsIndex = Helper_Create_PixelsOfRegionFromParam(Helper_Get2DRectangleEdgePolygon_Correct(), rm);

            // act
            int [] newPixelsIndex =  new RegionToPolygon_Test().EdgeCrawler(region, rm).ToArray();
             
            // assert
            bool resultCompare = Helper_ComparePixelRegions(newPixelsIndex, correctPixelsIndex);

            Assert.IsTrue(resultCompare);
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

        private void Helper_Test_EdgeCrawler2(Trace.RegionVO region, int [] expectedPoints, Trace.RegionManipulator rm, Point? startPoint)
        {
            List<int> result =
                (startPoint.HasValue) ?
                new RegionToPolygon_Test().EdgeCrawler(region, rm, rm.Get_IndexFromPoint(startPoint.Value)) :
            new RegionToPolygon_Test().EdgeCrawler(region, rm);

            int[] correctResult = expectedPoints;

            bool resultCompare = Helper_ComparePixelRegions(result.ToArray(), correctResult);

            Assert.IsTrue(resultCompare);
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


        

        private Test2DPolygonDefinition Helper_Get2DCrossPolygon()
        {
            Test2DPolygonDefinition result = new Test2DPolygonDefinition(5, 3);
            result.Points = new byte[]
            {0,0,1,0,0,
             0,1,1,1,0,
             0,0,1,0,0,
            };

            return result;
        }

        private Test2DPolygonDefinition Helper_Get2DCrossPolygon_Correct()
        {
            Test2DPolygonDefinition result = new Test2DPolygonDefinition(5, 3);
            result.Points = new byte[]
            {0,0,1,0,0,
             0,1,1,1,0,
             0,0,1,0,0,
            };

            return result;
        }

        private Test2DPolygonDefinition Helper_Get2DRectanglePolygon()
        {
            Test2DPolygonDefinition result = new Test2DPolygonDefinition(5, 5);
            result.Points = new byte[]
            {0,0,0,0,0,
             0,1,1,1,0,
             0,1,1,1,0,
             0,1,1,1,0,
             0,0,0,0,0,
            };

            return result;
        }

        private Test2DPolygonDefinition Helper_Get2DRectanglePolygon_Correct()
        {
            Test2DPolygonDefinition result = new Test2DPolygonDefinition(5, 5);
            result.Points = new byte[]
            {0,0,0,0,0,
             0,1,0,1,0,
             0,0,0,0,0,
             0,1,0,1,0,
             0,0,0,0,0
            };

            return result;
        }

        private Test2DPolygonDefinition Helper_Get2DRectangleEdgePolygon()
        {
            Test2DPolygonDefinition result = new Test2DPolygonDefinition(5, 5);
            result.Points = new byte[]
            {1,1,1,
             1,1,1,
             1,1,1,
            };


            return result;
        }

        private Test2DPolygonDefinition Helper_Get2DRectangleEdgePolygon_Correct()
        {
            Test2DPolygonDefinition result = new Test2DPolygonDefinition(3, 3);
            result.Points = new byte[]
            {1,0,1,
             0,0,0,
             1,0,1,
            };

            return result;
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

        private Trace.RegionVO Helper_Create_RegionFromParam(Test2DPolygonDefinition testPolygon, Trace.RegionManipulator rm)
        {
            Trace.RegionVO region = new Trace.RegionVO(new Pixel());
            region.Pixels = Helper_Create_PixelsOfRegionFromParam(testPolygon, rm);
            return region;
        }



        private int[] Helper_Create_PixelsOfRegionFromParam(Test2DPolygonDefinition testPolygon, Trace.RegionManipulator rm)
        {
            List<int> result = new List<int>();

            for (int i = 0; i < testPolygon.Points.Length; i++)
            {
                if (testPolygon.Points[i] == 1)
                {
                    result.Add(i);
                }
            }

            return result.ToArray();
        }

        private Trace.RegionManipulator Helper_Create_RegionManipulator(Test2DPolygonDefinition testPolygon)
        {
            CanvasPixel canvas = new CanvasPixel(testPolygon.Width, testPolygon.Height);
            return new Trace.RegionManipulator(canvas, canvas);
        }

        private void Helper_CompareResult_Tolerate_DuplicityUnorder(int[] data, int[] data2)
        {
            data = data.Distinct().OrderBy(x => x).ToArray();
            data2 = data2.Distinct().OrderBy(x => x).ToArray();

            Helper_CompareResult_Exact(data, data2);
        }

        private void Helper_CompareResult_Tolerate_Unorder(int[] data, int[] data2)
        {
            data = data.OrderBy(x => x).ToArray();
            data2 = data2.OrderBy(x => x).ToArray();

            Helper_CompareResult_Exact(data, data2);
        }

        private void Helper_CompareResult_Exact(int[] data, int[] data2)
        {
            bool result = true;

            if (data.Length != data2.Length) result = false;

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != data2[i])
                {
                    result = false;
                    break;
                }
            }

            Assert.IsTrue(result);
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

    class Test2DPolygonDefinition
    {
        public byte[] Points;
        public int Width;
        public int Height;

        public Test2DPolygonDefinition(int width, int height)
        {
            this.Height = height;
            this.Width = width;
            this.Points = new byte[0];
        }
    }
}