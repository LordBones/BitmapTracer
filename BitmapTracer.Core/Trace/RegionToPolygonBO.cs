using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BitmapTracer.Core.Trace
{
    public class RegionToPolygonBO
    {
        private RegionManipulator _regionManipulator;

        enum EnumAxis { None, X, Y , Both}

        public RegionToPolygonBO(RegionManipulator regionManipulator)
        {
            this._regionManipulator = regionManipulator;
        }

        public Point [] ToPolygonOld(RegionVO region)
        {
            var points = this._regionManipulator.Get_RegionEdgePeaks(region);


            List<Point> result = new List<Point>();
            for(int i =0;i<points.Length;i++)
            {
                result.Add(_regionManipulator.Get_PointFromIndex(points[i]));
            }

            return result.ToArray();
        }

       

        public Point[] ToPolygon(RegionVO region)
        {
            List<Point> result = new List<Point>();

            List<int> edgePixelInOrder = new RegionEdgeCrawler(region, this._regionManipulator).GetInOrderEdgePixels();

            edgePixelInOrder = ReduceSameSequence(edgePixelInOrder);

            result.AddRange(edgePixelInOrder.Select(x => _regionManipulator.Get_PointFromIndex(x)));

            //result = ReduceToEdgePoints_WhenChangeDirection(result);
            return result.ToArray();
        }

        private List<int> ReduceSameSequence(List<int> data)
        {
            List<int> result = new List<int>();

            int last = -2;
            for(int i = 0;i < data.Count;i++)
            {
                int tmpIndex = data[i];
                if(last != tmpIndex)
                {
                    result.Add(tmpIndex);
                }

                last = tmpIndex;
            }

            return result;
        }

        private List<Point> ReduceToEdgePoints_WhenChangeDirection(List<Point> points)
        {
            List<Point> result = new List<Point>();

            if (points.Count <= 2) return points;

            result.Add(points[0]);
            
            for (int i = 2;i<points.Count;i++)
            {
                Point p1 = points[i - 2];
                Point p2 = points[i - 1];
                Point p3 = points[i ];


                EnumAxis sameAxis = Helper_GetPointTheSameAxis(p1, p2);
                EnumAxis sameAxis2 = Helper_GetPointTheSameAxis(p2, p3);

                if(sameAxis != sameAxis2)
                {
                    result.Add(p1);
                }
                // axis are the same, must detect line
                else
                {
                    EnumAxis sameAxis3 =  Helper_GetPointTheSameAxis(p1, p2, p3);
                    if (sameAxis3 == EnumAxis.Y)
                    {
                        bool xTest = !((p1.X < p2.X && p2.X < p3.X) || (p1.X > p2.X && p2.X > p3.X));

                        if (xTest )
                        {
                            result.Add(p2);
                        }
                    }
                    else if (sameAxis3 == EnumAxis.X)
                    {
                        bool yTest = !((p1.Y < p2.Y && p2.Y < p3.Y) || (p1.Y > p2.Y && p2.Y > p3.Y));

                        if ( yTest)
                        {
                            result.Add(p2);
                        }
                    }
                }
            }

            result.Add(points[points.Count-1]);

            return result;
        }

        private EnumAxis Helper_GetPointTheSameAxis(Point p1, Point p2)
        {
            
            bool xCompare = p1.X == p2.X;
            bool yCompare = p1.Y == p2.Y;

            if (xCompare && yCompare) return EnumAxis.Both;
            if (xCompare) return EnumAxis.X;
            if (yCompare) return EnumAxis.Y;

            return EnumAxis.None;
        }

        private EnumAxis Helper_GetPointTheSameAxis(Point p1, Point p2, Point p3)
        {
            bool xCompare = p1.X == p2.X && p2.X == p3.X;
            bool yCompare = p1.Y == p2.Y && p2.Y == p3.Y;

            if (xCompare && yCompare) return EnumAxis.Both;
            if (xCompare) return EnumAxis.X;
            if (yCompare) return EnumAxis.Y;

            return EnumAxis.None;
        }





    }

    class RegionEdgeCrawler
    {
        private RegionManipulator _regionManipulator;

        private HashSet<int> _pixels;

        enum Direction { Top, Right, Left, Bottom }

        public RegionEdgeCrawler(RegionVO region, RegionManipulator rm)
        {
            this._regionManipulator = rm;
            this._pixels = new HashSet<int>(region.Pixels);

        }

        public List<int> GetInOrderEdgePixels()
        {
            int startPixel = GetPixelEdge_TopFree();
            return GetInOrderEdgePixels(startPixel);
        }

        public List<int> GetInOrderEdgePixels(int pStartPixel)
        {
            List<int> result = new List<int>();

            int startPixel = pStartPixel;
            Direction startEdgeOrientation = Direction.Top;

            int currentPixel = startPixel;
            Direction currEdgeOrientation = startEdgeOrientation;
            StringBuilder sb = new StringBuilder();
            
            do
            {


                result.Add(currentPixel);

                if(HasPixelAtDirection(currentPixel, currEdgeOrientation))
                {
                    Direction counterClockDirection = GetCounterClockChangeDirection(currEdgeOrientation);
                    currentPixel = GetPixelAtDirection(currentPixel, currEdgeOrientation);
                    currEdgeOrientation = counterClockDirection;
                }
                else
                {
                    Direction clockwiseDirection = GetClockwiseChangeDirection(currEdgeOrientation);

                    if (HasPixelAtDirection(currentPixel, clockwiseDirection))
                    {
                        currentPixel = GetPixelAtDirection(currentPixel, clockwiseDirection);
                    }
                    else
                    {
                        currEdgeOrientation = clockwiseDirection;
                    }
                }
                Point tmp = _regionManipulator.Get_PointFromIndex(currentPixel);
                sb.Append($"{currentPixel} - {tmp.X} {tmp.Y}  {currEdgeOrientation} \n");

                if(result.Count > this._pixels.Count*4)
                {
                    List<Point> tmpp = this._pixels.Select(x => _regionManipulator.Get_PointFromIndex(x)).ToList();

                    System.Diagnostics.Trace.WriteLine(sb.ToString());
                    System.Diagnostics.Trace.WriteLine(tmpp.Count);
                    throw new NotSupportedException();
                }

                

            } while (currentPixel != startPixel || currEdgeOrientation != startEdgeOrientation);


            return result;
        }

        private bool HasPixelAtDirection( int pixel, Direction currDirection)
        {
            return IsIndexPixelExist(GetPixelAtDirection(pixel, currDirection));
        }

        private int GetPixelAtDirection(int pixel, Direction currDirection)
        {
            switch (currDirection)
            {
                case Direction.Top:
                    return _regionManipulator.Helper_GetPixelIndex_Top(pixel);
                case Direction.Right:
                    return _regionManipulator.Helper_GetPixelIndex_Right(pixel);
                case Direction.Left:
                    return _regionManipulator.Helper_GetPixelIndex_Left(pixel);
                case Direction.Bottom:
                    return _regionManipulator.Helper_GetPixelIndex_Bottom(pixel);
            }

            throw new NotSupportedException();
        }

        private Direction GetCounterClockChangeDirection(Direction d)
        {
            switch (d)
            {
                case Direction.Top: return Direction.Left;
                case Direction.Right:
                    return Direction.Top;
                case Direction.Left:
                    return Direction.Bottom;
                case Direction.Bottom:
                    return Direction.Right;
            }

            throw new NotSupportedException();
        }

        private Direction GetClockwiseChangeDirection(Direction d)
        {
            switch (d)
            {
                case Direction.Top: return Direction.Right;
                case Direction.Right:
                    return Direction.Bottom;
                case Direction.Left:
                    return Direction.Top;
                case Direction.Bottom:
                    return Direction.Left;
            }

            throw new NotSupportedException();
        }

        private bool IsIndexPixelExist(int index)
        {
            if (index < 0) return false;

            return this._pixels.Contains(index);
        }


        private int GetPixelEdge_TopFree()
        {
            int? resultPixel = null;

            foreach (int pixel in  _pixels)
            {
                int topPixel = this._regionManipulator.Helper_GetPixelIndex_Top(pixel);

            

                if (topPixel < 0 || (topPixel >= 0 && !this._pixels.Contains(topPixel)))
                {
                    if (resultPixel == null || resultPixel.Value > pixel)
                    {
                        resultPixel = pixel;
                    }
                }
            }

            if(resultPixel != null)
            {
                return resultPixel.Value;
            }

            throw new NotSupportedException();
        }
    }
}
