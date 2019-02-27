using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BitmapTracer.Core.basic;

namespace BitmapTracer.Core.Trace
{
    public class RegionManipulator
    {
        private int _widthPixels;
        private int _heightPixels;

        private CanvasPixel _originalCanvasPixel;
        private CanvasPixel _canvasPixel;

        ObjectRecyclerTS<HashSet<RegionVO>> _pool_Hashset_RegionVO = new ObjectRecyclerTS<HashSet<RegionVO>>(1);
        ObjectRecyclerTS<List<RegionVO>> _pool_List_RegionVO = new ObjectRecyclerTS<List<RegionVO>>(1);
       

        public RegionManipulator(CanvasPixel originalCanvasPixel, CanvasPixel canvasPixel)
        {
            this._canvasPixel = canvasPixel;
            this._originalCanvasPixel = originalCanvasPixel;

            this._widthPixels = canvasPixel.Width;
            this._heightPixels = canvasPixel.Height;
        }

        public int Get_IndexFromPoint(Point point)
        {
            return ((int)point.Y)* this._widthPixels + ((int)point.X);
        }



        public Point Get_PointFromIndex(int index)
        {
            return new Point( index % this._widthPixels, index / this._widthPixels);
        }

        public int[] Get_RegionEdgePeaks(RegionVO region)
        {
            HashSet<int> lookup = new HashSet<int>(region.Pixels);

            List<int> result = new List<int>();

            for (int i = 0; i < region.Pixels.Length; i++)
            {
                int item = region.Pixels[i];
                int itemLeft = Helper_GetPixelIndex_Left(item);
                int itemRight = Helper_GetPixelIndex_Right(item);
                int itemTop = Helper_GetPixelIndex_Top(item);
                int itemBottom = Helper_GetPixelIndex_Bottom(item);

                int itemLeftTop = (itemLeft >= 0 && itemTop >= 0) ? itemTop - 1 : -1;
                int itemRightTop = (itemRight >= 0 && itemTop >= 0) ? itemTop + 1 : -1;
                int itemLeftBottom = (itemLeft >= 0 && itemBottom >= 0) ? itemBottom - 1 : -1;
                int itemRightBottom = (itemRight >= 0 && itemBottom >= 0) ? itemBottom + 1 : -1;


                bool leftPixelValid = itemLeft >= 0 && lookup.Contains(itemLeft);
                bool rightPixelValid = itemRight >= 0 && lookup.Contains(itemRight);
                bool topPixelValid = itemTop >= 0 && lookup.Contains(itemTop);
                bool bottomPixelValid = itemBottom >= 0 && lookup.Contains(itemBottom);

                bool leftTopValid = itemLeftTop >= 0 && lookup.Contains(itemLeftTop);
                bool rightTopValid = itemRightTop >= 0 && lookup.Contains(itemRightTop);
                bool leftBottomValid = itemLeftBottom >= 0 && lookup.Contains(itemLeftBottom);
                bool rightBottomValid = itemRightBottom >= 0 && lookup.Contains(itemRightBottom);

                if ((!leftPixelValid && !topPixelValid) ||
                    (!rightPixelValid && !topPixelValid) ||
                    (!leftPixelValid && !bottomPixelValid) ||
                    (!leftPixelValid && !bottomPixelValid)
                    || !leftBottomValid || !leftTopValid || !rightBottomValid || !rightTopValid)
                {
                    result.Add(item);
                }




            }

            return result.ToArray();
        }

        public int[] Get_RegionEdges(RegionVO region)
        {
            HashSet<int> lookup = new HashSet<int>(region.Pixels);

            List<int> result = new List<int>();

            for (int i = 0; i < region.Pixels.Length; i++)
            {
                int item = region.Pixels[i];
                int itemLeft = Helper_GetPixelIndex_Left(item);
                int itemRight = Helper_GetPixelIndex_Right(item);
                int itemTop = Helper_GetPixelIndex_Top(item);
                int itemBottom = Helper_GetPixelIndex_Bottom(item);

                int itemLeftTop = (itemLeft >= 0 && itemTop >= 0) ? itemTop - 1 : -1;
                int itemRightTop = (itemRight >= 0 && itemTop >= 0) ? itemTop + 1 : -1;
                int itemLeftBottom = (itemLeft >= 0 && itemBottom >= 0) ? itemBottom - 1 : -1;
                int itemRightBottom = (itemRight >= 0 && itemBottom >= 0) ? itemBottom + 1 : -1;


                bool leftPixelValid = itemLeft >= 0 && lookup.Contains(itemLeft);
                bool rightPixelValid = itemRight >= 0 && lookup.Contains(itemRight);
                bool topPixelValid = itemTop >= 0 && lookup.Contains(itemTop);
                bool bottomPixelValid = itemBottom >= 0 && lookup.Contains(itemBottom);

                bool leftTopValid = itemLeftTop >= 0 && lookup.Contains(itemLeftTop);
                bool rightTopValid = itemRightTop >= 0 && lookup.Contains(itemRightTop);
                bool leftBottomValid = itemLeftBottom >= 0 && lookup.Contains(itemLeftBottom);
                bool rightBottomValid = itemRightBottom >= 0 && lookup.Contains(itemRightBottom);

                if (!leftPixelValid || !topPixelValid ||
                    !rightPixelValid ||!bottomPixelValid
                    
                    || !leftBottomValid || !leftTopValid || !rightBottomValid || !rightTopValid)
                {
                    result.Add(item);
                }
            }

            return result.ToArray();
        }

        public int [] Get_CreatePixelNeighbourEdge(RegionVO region)
        {
            HashSet<int> lookup = new HashSet<int>(region.Pixels);

            HashSet<int> pixelEdgesLookup = new HashSet<int>();
            List<int> pixelsNeighbours = new List<int>();


            for (int i = 0; i < region.Pixels.Length; i++)
            {
                int item = region.Pixels[i];
                {

                    int itemLeft = Helper_GetPixelIndex_Left(item);
                    if (itemLeft >= 0)
                    {
                        if (!lookup.Contains(itemLeft))
                        {
                            if (pixelEdgesLookup.Add(itemLeft))
                            {
                                pixelsNeighbours.Add(itemLeft);
                            }

                        }
                    }

                    if (item + this._widthPixels < this._widthPixels * this._heightPixels)
                    {
                        if (!lookup.Contains(item + this._widthPixels))
                        {
                            if (pixelEdgesLookup.Add(item + this._widthPixels))
                            {
                                pixelsNeighbours.Add(item + this._widthPixels);
                            }

                        }
                    }

                    if ((item) % this._widthPixels != 0)
                    {
                        if (!lookup.Contains(item - 1))
                        {
                            if (pixelEdgesLookup.Add(item - 1))
                            {
                                pixelsNeighbours.Add(item - 1);
                            }

                        }
                    }

                    if ((item + 1) % this._widthPixels != this._widthPixels - 1)
                    {
                        if (!lookup.Contains(item + 1))
                        {
                            if (pixelEdgesLookup.Add(item + 1))
                            {
                                pixelsNeighbours.Add(item + 1);
                            }

                        }
                    }
                }
            }

            return pixelsNeighbours.ToArray();

        }

        public int Helper_GetPixelIndex_Left(int index)
        {
            if ((index) % this._widthPixels == 0) return -1;

            return index - 1;
        }

        public int Helper_GetPixelIndex_Right(int index)
        {
            if ((index+1) % this._widthPixels == 0 ) return -1;

            return index + 1;
        }

        public int Helper_GetPixelIndex_Top(int index)
        {
            int result = index - this._widthPixels;

            if (result < 0) return -1;

            return result;
        }

        public int Helper_GetPixelIndex_Bottom(int index)
        {
            int result = index + this._widthPixels;

            if (result >= this._widthPixels * this._heightPixels) return -1;

            return result;
        }

        private int [] CreatePixelNeighbourEdge_p(RegionVO region, RegionVO[] regionPixelLookup)
        {
            //HashSet<int> pixelEdgesLookup = new HashSet<int>();
            List<int> pixelsNeighbours = new List<int>();

            for (int i = 0; i < region.Pixels.Length; i++)
            {
                int item = region.Pixels[i];
                int WidthPixels = this._widthPixels;
                int HeightPixels = this._heightPixels;
                {
                    

                    if ((item) % WidthPixels != 0)
                    {
                        if (regionPixelLookup[item - 1] != region)
                        {
                            // if (pixelEdgesLookup.Add(item - 1))
                            {
                                pixelsNeighbours.Add(item - 1);
                            }

                        }
                    }

                    if ((item ) % WidthPixels != WidthPixels - 1)
                    {
                        if (regionPixelLookup[item + 1] != region)
                        {
                            // if (pixelEdgesLookup.Add(item + 1))
                            {
                                pixelsNeighbours.Add(item + 1);
                            }
                        }
                    }

                    if (item - WidthPixels >= 0)
                    {
                        if (regionPixelLookup[item - WidthPixels] != region)
                        {

                            // if (pixelEdgesLookup.Add(item - WidthPixels))
                            {
                                pixelsNeighbours.Add(item - WidthPixels);
                            }
                        }
                    }

                    if (item + WidthPixels < WidthPixels * HeightPixels)
                    {
                        if (regionPixelLookup[item + WidthPixels] != region)
                        {
                            //  if (pixelEdgesLookup.Add(item + WidthPixels))
                            {
                                pixelsNeighbours.Add(item + WidthPixels);
                            }

                        }
                    }
                }
            }

            return pixelsNeighbours.ToArray();
        }

        //private static HashSet<RegionVO> tmpLookup = new HashSet<RegionVO>();

        public void Create_NeighbourRegions(RegionVO region, RegionVO[] regionPixelLookup)
        {
            int[] pixelsEdgeNeighbour = CreatePixelNeighbourEdge_p(region, regionPixelLookup);

            HashSet<RegionVO> tmpLookup = _pool_Hashset_RegionVO.GetNewOrRecycle();
            tmpLookup.Clear();


            List<RegionVO> newNeightbours = _pool_List_RegionVO.GetNewOrRecycle();
            newNeightbours.Clear();

            for (int i = 0; i < pixelsEdgeNeighbour.Length; i++)
            {
                int index = pixelsEdgeNeighbour[i];

                RegionVO regionForTest = regionPixelLookup[index];

                if (regionForTest != null)
                {
                    if (tmpLookup.Add(regionForTest))
                    {
                        newNeightbours.Add(regionForTest);
                    }

                }
            }

            if(newNeightbours.Count > 0)
            {
                newNeightbours.AddRange(region.NeighbourRegions);
                region.NeighbourRegions = newNeightbours.ToArray();
            }

            _pool_List_RegionVO.PutForRecycle(newNeightbours);
            tmpLookup.Clear();
            _pool_Hashset_RegionVO.PutForRecycle(tmpLookup);

        }

        public short Get_X(int pixelIndex)
        {
            return (short)(pixelIndex % _widthPixels);
        }

        public short Get_Y(int pixelIndex)
        {
            return (short)(pixelIndex / _widthPixels);
        }

        public (short X, short Y) Get_XY(int pixelIndex)
        {
            return (Get_X(pixelIndex), Get_Y(pixelIndex));
        }



        public RectangleArea GetDimensionArea(RegionVO region)
        {
            RectangleArea result = new RectangleArea(short.MaxValue,short.MaxValue,short.MinValue, short.MinValue) ;

            for (int i =0;i < region.Pixels.Length; i++)
            {
                int pixelIndex = region.Pixels[i];
                var (x, y) = Get_XY(pixelIndex);

                if (result.X > x) result.X = x;
                if (result.X2 < x) result.X2 = x;
                if (result.Y > y) result.Y = y;
                if (result.Y2 < y) result.Y2 = y;

            }

            return result;
        }

        public RegionVO[] GetOrderedForRendering(RegionVO [] regions)
        {
            List<(RectangleArea area, RegionVO region)> regionsForOrdering = PrepareForOrdering(regions);

            regionsForOrdering.Sort(CompareRegionInsideRegion);
            return regionsForOrdering.Select(x => x.region).ToArray();
        }

        private int CompareRegionInsideRegion((RectangleArea area, RegionVO region) region, (RectangleArea area, RegionVO region) region2)
        {
            if (IsInsideRange(region.area.X, region.area.X2, region2.area.X) &&
                IsInsideRange(region.area.X, region.area.X2, region2.area.X2) &&
                IsInsideRange(region.area.Y, region.area.Y2, region2.area.Y) &&
                IsInsideRange(region.area.Y, region.area.Y2, region2.area.Y2))
                return 1;

            if (IsInsideRange(region2.area.X, region2.area.X2, region.area.X) &&
                IsInsideRange(region2.area.X, region2.area.X2, region.area.X2) &&
                IsInsideRange(region2.area.Y, region2.area.Y2, region.area.Y) &&
                IsInsideRange(region2.area.Y, region2.area.Y2, region.area.Y2))
                return -1;

            return 0;
        }
        
        private bool IsInsideRange(short rangeFrom , short rangeTo, short value)
        {
            return (rangeFrom < value && rangeTo > value);
        }

        private List<(RectangleArea,RegionVO)> PrepareForOrdering(RegionVO [] regions)
        {
            return regions.Select(x => (area: GetDimensionArea(x), region: x)).ToList();
        }

        
        
    }
}
