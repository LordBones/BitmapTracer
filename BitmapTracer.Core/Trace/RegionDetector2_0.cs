using BitmapTracer.Core.basic;
using BitmapTracer.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmapTracer.Core.Trace
{
    public class RegionDetector2_0
    {
        // index odpovida pixelu a odkazuje se na oblast do ktere spada
        private RegionVO[] _regionPixelLookup;

        private List<RegionVO> _listRegion;

        public int TotalRegions { get { return _listRegion.Count; } }

        private RegionManipulator _regionManipulator = null;
        private CanvasPixel _canvasPixel;
        private CanvasPixel _canvasPixelOriginal;

        private int CONST_maxCountPixelForRemove = 10;
         
        public RegionDetector2_0(CanvasARGB canvas)
        {
            this._canvasPixel = CanvasPixel.CreateBitmpaFromCanvas(canvas);
            this._canvasPixelOriginal = CanvasPixel.CreateBitmpaFromCanvas(canvas);

            this._regionManipulator = new RegionManipulator(this._canvasPixelOriginal, this._canvasPixel);
        }

        public void Detect_ByColorTolerance(int tolerance)
        {
           // RegionVO.ClearIdNext();
            _regionPixelLookup = new RegionVO[this._canvasPixelOriginal.Data.Length];
            _listRegion = new List<RegionVO>();

            CanvasPixel canvasPixel = this._canvasPixel;
            CanvasPixel canvasPixelOriginal = this._canvasPixelOriginal;

            //canvasPixel.ReduceNotSeenColors();

            //canvasPixel.TransformToInterleaveRGB();

            List<int> singleAlonePoints = CreateRegionFromStart(canvasPixel, 0);//tolerance);
            //List<int> singleAlonePoints = CreateRegionFromStartCicCac(canvasPixel, tolerance);



            FuseAllSinglePoints(singleAlonePoints, canvasPixel, tolerance);

            foreach (var item in _listRegion)
            {
                this._regionManipulator.Create_NeighbourRegions(item,_regionPixelLookup);
            }

            ReduceAllRegionUnderColorTolerance(canvasPixel, 0, canvasPixelOriginal);

            int colorTolerance = tolerance;
            int countMinPixel = CONST_maxCountPixelForRemove;
            for (int i = 1; i <= countMinPixel; i++)
            {
                ReduceAllRegionUnderCountPixel(canvasPixel, i, canvasPixelOriginal);
            }

            for (int i = colorTolerance; i >= 0; i--)
                ReduceAllRegionUnderColorTolerance(canvasPixel, i, canvasPixelOriginal);
        }

        public CanvasPixel Get_CanvasFromRegions()
        {
            return CreateCanvasFromRegions(this._canvasPixelOriginal, this._listRegion);
        }

        public void ConvertRegionsToSVG(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

            //new ImageToSVG(fs).BasicToSVG(this._canvasPixelOriginal, this._canvasPixel);
            new ImageToSVG(fs).AreasToSVG(this._canvasPixelOriginal, this._listRegion, this._regionManipulator);


            fs.Close();


        }

        public void Statistic_GroupRegion()
        {
            Dictionary<int, int> groups = new Dictionary<int, int>();
            foreach (var item in _listRegion)
            {
                if (groups.ContainsKey(item.Pixels.Length))
                {
                    groups[item.Pixels.Length]++;
                }
                else
                {
                    groups.Add(item.Pixels.Length, 1);
                }
            }
        }

        public void Detect_ByColorToleranceNew( int tolerance)
        {
            //RegionVO.ClearIdNext();
            _regionPixelLookup = new RegionVO[this._canvasPixelOriginal.Data.Length];
            _listRegion = new List<RegionVO>();

            CanvasPixel canvasPixel = this._canvasPixel;
            CanvasPixel canvasPixelOriginal = this._canvasPixelOriginal;
          
            //canvasPixel.ConvertPixelsRGBToHSL();
            //canvasPixel.ReduceNotSeenColors();
            //canvasPixel.TransformToInterleaveRGB();

            List<int> singleAlonePoints = CreateRegionFromStart(canvasPixel, 0);// tolerance);
            //List<int> singleAlonePoints = CreateRegionFromStartCicCac(canvasPixel, tolerance);



            FuseAllSinglePoints(singleAlonePoints, canvasPixel, tolerance);

            foreach (var item in _listRegion)
            {
                this._regionManipulator.Create_NeighbourRegions(item, _regionPixelLookup);
            }

            ReduceAllRegionUnderColorTolerance(canvasPixel, 0, canvasPixelOriginal);

            int colorTolerance = tolerance;
            int countMinPixel = CONST_maxCountPixelForRemove;
            for (int i = 1; i <= countMinPixel; i++)
            {
                ReduceAllRegionUnderCountPixel(canvasPixel, i, canvasPixelOriginal);

            }


            for (int i = 0;
                i <= colorTolerance; i++)
            {
                ReduceAllRegionUnderColorTolerance(canvasPixel, i,canvasPixelOriginal);
                //ReduceAllRegionUnderColorTolerance(canvasPixel, i);
            }
            
        }

        private CanvasPixel CreateCanvasFromRegions(CanvasPixel canvasPixelOriginal, List<RegionVO> regions)
        {
            CanvasPixel result = new CanvasPixel(canvasPixelOriginal.Width, canvasPixelOriginal.Height);

            foreach(var region in regions)
            {
                Pixel color = region.Color;
                for (int i = 0;i<region.Pixels.Length; i++)
                {
                    result.Data[region.Pixels[i]] = color;
                }
            }

            return result;
        }

        private List<int> CreateRegionFromStartCicCac(CanvasPixel canvasPixel, int tolerance)
        {
            List<int> listSinglePoints = new List<int>();

            CanvasPixel cp = canvasPixel;
            Pixel[] data = cp.Data;

            int startX = 1;
            int startY = 1;

            for (int i = 1; i < cp.Width - 1; i++)
            {

                int tmpX = startX;
                int tmpY = startY;
                while (tmpX >= 1 && tmpY < (cp.Height - 1))
                {
                    CreateRegionFromStartPixel(tmpX, tmpY, listSinglePoints, canvasPixel, tolerance);
                    tmpX--;
                    tmpY++;
                }

                startX++;
            }

            startX = cp.Width - 2;
            startY = 2;

            for (int i = 2; i < cp.Height - 1; i++)
            {

                int tmpX = startX;
                int tmpY = startY;
                while (tmpX >= 1 && tmpY < (cp.Height - 1))
                {
                    CreateRegionFromStartPixel(tmpX, tmpY, listSinglePoints, canvasPixel, tolerance);
                    tmpX--;
                    tmpY++;
                }

                startY++;
            }

            return listSinglePoints;
        }

        private List<int> CreateRegionFromStart(CanvasPixel canvasPixel, int tolerance)
        {
            List<int> listSinglePoints = new List<int>();

            CanvasPixel cp = canvasPixel;
            Pixel[] data = cp.Data;

            //for (int y = 1; y < cp.Height - 1; y += 1)
            //    for (int i = 1; i < cp.Width - 1; i += 1)
            //    {
            //        CreateRegionFromStartPixel(i, y, listSinglePoints, canvasPixel, tolerance);
            //    }

             
            for (int y = 0; y < cp.Height ; y += 2)
                for (int i = 0; i < cp.Width ; i += 2)
                {
                    CreateRegionFromStartPixel(i, y, listSinglePoints, canvasPixel, tolerance);
                }

            for (int y = 1; y < cp.Height ; y += 2)
                for (int i = 0; i < cp.Width ; i += 2)
                {
                    CreateRegionFromStartPixel(i, y, listSinglePoints, canvasPixel, tolerance);
                }


            for (int y = 0; y < cp.Height ; y += 2)
                for (int i = 1; i < cp.Width ; i += 2)
                {
                    CreateRegionFromStartPixel(i, y, listSinglePoints, canvasPixel, tolerance);
                }

            for (int y = 1; y < cp.Height ; y += 2)
                for (int i = 1; i < cp.Width ; i += 2)
                {
                    CreateRegionFromStartPixel(i, y, listSinglePoints, canvasPixel, tolerance);
                }


            //for (int i = 0; i < _listRegion.Count;i++ )
            //{
            //    _listRegion[i].SetColorAsAverageToAll(canvasPixel);
            //}

          


            return listSinglePoints;

        }

        private void CreateRegionFromStartPixel(int x, int y, List<int> listSinglePoints, CanvasPixel canvasPixel, int tolerance)
        {
            CanvasPixel cp = canvasPixel;
            Pixel[] data = cp.Data;

            int tmpRow = (y) * cp.Width;

            int rowindex1 = tmpRow - cp.Width + x;
            int rowindex2 = tmpRow + x;
            int rowindex3 = tmpRow + cp.Width + x;


            if (_regionPixelLookup[rowindex2] != null) return;

            int middleColor = data[rowindex2].Get_ColorClearAlpha_Int();

            int v2 = int.MaxValue;
            int v4 = int.MaxValue;
            int v6 = int.MaxValue;
            int v8 = int.MaxValue;

            //int v2 = Helpers.BasicHelpers.FastAbs(middleColor - data[rowindex1].IntClearAlpha());
            //int v4 = Helpers.BasicHelpers.FastAbs(middleColor - data[rowindex2 - 1].IntClearAlpha());
            //int v6 = Helpers.BasicHelpers.FastAbs(middleColor - data[rowindex2 + 1].IntClearAlpha());
            //int v8 = Helpers.BasicHelpers.FastAbs(middleColor - data[rowindex3].IntClearAlpha());

            if(y > 1)
             v2 = Pixel.SumAbsDiff(data[rowindex2], data[rowindex1]);
            if(x > 1)
             v4 = Pixel.SumAbsDiff(data[rowindex2], data[rowindex2 - 1]);
            if (x < (cp.Width - 1 - 1))
                v6 = Pixel.SumAbsDiff(data[rowindex2], data[rowindex2 + 1]);
            if (y < (cp.Height - 1 - 1))
             v8 = Pixel.SumAbsDiff(data[rowindex2], data[rowindex3]);


            bool b2 = v2 <= tolerance;
            bool b4 = v4 <= tolerance;
            bool b6 = v6 <= tolerance;
            bool b8 = v8 <= tolerance;

            if (b2 || b4 || b6 || b8)
            {
                RegionVO region = null;

                {
                    // detekce uz existujiciho regionu v okoli
                    if (b2 && _regionPixelLookup[rowindex1] != null)
                    {
                        region = _regionPixelLookup[rowindex1];
                    }
                    else if (b4 && _regionPixelLookup[rowindex2 - 1] != null)
                    {
                        region = _regionPixelLookup[rowindex2 - 1];
                    }
                    else if (b6 && _regionPixelLookup[rowindex2 + 1] != null)
                    {
                        region = _regionPixelLookup[rowindex2 + 1];
                    }
                    else if (b8 && _regionPixelLookup[rowindex3] != null)
                    {
                        region = _regionPixelLookup[rowindex3];
                    }

                }

                if (region != null)
                {
                    region.Add_Pixel(rowindex2);
                    _regionPixelLookup[rowindex2] = region;
                    data[rowindex2] = region.Color;
                    //this._regionManipulator.CreatePixelNeighbourEdge(region);

                }
                else
                {
                    Pixel tmpMiddleColor = data[rowindex2];
                    RegionVO newRegion = new RegionVO(tmpMiddleColor);
                    List<int> tmpPixels = new List<int>(4);
                    tmpPixels.Add(rowindex2);
                    _regionPixelLookup[rowindex2] = newRegion;

                    if (b2 && y > 1)
                    {
                        tmpPixels.Add(rowindex1);
                        _regionPixelLookup[rowindex1] = newRegion;
                        data[rowindex1] = tmpMiddleColor;
                    }

                    if (b4 && x > 1)
                    {
                        tmpPixels.Add(rowindex2 - 1);
                        _regionPixelLookup[rowindex2 - 1] = newRegion;
                        data[rowindex2 - 1] = tmpMiddleColor;
                    }

                    if (b6 && x < (cp.Width - 1 - 1))
                    {
                        tmpPixels.Add(rowindex2 + 1);
                        _regionPixelLookup[rowindex2 + 1] = newRegion;
                        data[rowindex2 + 1] = tmpMiddleColor;
                    }

                    if (b8 && y < (cp.Height - 1 - 1))
                    {
                        tmpPixels.Add(rowindex3);
                        _regionPixelLookup[rowindex3] = newRegion;
                        data[rowindex3] = tmpMiddleColor;
                    }

                    newRegion.Add_Pixels(tmpPixels.ToArray());
                    _listRegion.Add(newRegion);
                    // newRegion.CreatePixelEdges();
                    //this._regionManipulator.CreatePixelNeighbourEdge(newRegion);
                }
            }
            else
            {
                Pixel tmpMiddleColor = data[rowindex2];
                RegionVO newRegion = new RegionVO(tmpMiddleColor);
                newRegion.Add_Pixel(rowindex2);
                _regionPixelLookup[rowindex2] = newRegion;
                _listRegion.Add(newRegion);
            
            }

          
        }

        private void FuseAllSinglePoints(List<int> singleAlonePoints, CanvasPixel canvasPixel, int tolerance)
        {
            List<int> tmpSinglePoints = new List<int>();

            int toleranceModif = 1024;

            while (singleAlonePoints.Count > 0)
            {
                tolerance += toleranceModif;

                for (int i = 0; i < singleAlonePoints.Count; i++)
                {
                    int index = singleAlonePoints[i];
                    int x = index % canvasPixel.Width;
                    int y = index / canvasPixel.Width;

                    CreateRegionFromStartPixel(x, y, tmpSinglePoints, canvasPixel, tolerance);
                }

                if (singleAlonePoints.Count == tmpSinglePoints.Count)
                {
                    tmpSinglePoints.Clear();
                }
                else
                {
                    singleAlonePoints = tmpSinglePoints;
                    tmpSinglePoints = new List<int>();
                }

                toleranceModif <<= 1;
            }
        }



        

        private void ReduceAllRegionUnderColorTolerance(CanvasPixel canvasPixel, int maxColorDiff, CanvasPixel canvasPixelOriginal)
        {

            int origRegionCount = 0;

            //List<RegionVO> tmpRegions = new List<RegionVO>(_listRegion.Count);

            //HashSet<RegionVO> tmpRegions = new HashSet<RegionVO>();

            do
            {
              //  tmpRegions.Clear();

                //if (!Check_ValidRegionData(_regionPixelLookup, _listRegion))
                //{
                //    System.Diagnostics.Trace.WriteLine($"Start Invalid region data {_listRegion.Count}");
                //}

                

                origRegionCount = _listRegion.Count;
                for (int i = 0; i < _listRegion.Count; i++)
                {
                    RegionVO region = _listRegion[i];

                    if (region == null) break;
                    if (region.NotValid) continue;
                    

                    RegionVO bestForFuse = BestRegionToFuse2(region, canvasPixel, maxColorDiff);
                    if (bestForFuse == null)
                    {
                        continue;
                    }
                

                    if (bestForFuse != region) 
                    {
                        //// swap for faster merge
                        if (region.Pixels.Length > bestForFuse.Pixels.Length)
                        {
                            RegionVO tmp = region;
                            region = bestForFuse;
                            bestForFuse = tmp;
                        }

                        FuseRegions(bestForFuse, region, canvasPixel, canvasPixelOriginal);

                        region.NotValid = true;
                        region.Clear();
                    }





                    //if (!Check_ValidRegionData(_regionPixelLookup, _listRegion))
                    //{
                    //    throw new Exception();
                    //}
                }

             //   _listRegion = _listRegion.Where(x => !x.NotValid ).ToList();
                List<RegionVO> kk = new List<RegionVO>(_listRegion.Count);
                for(int i = 0;i< _listRegion.Count;i++)
                {
                    if(!_listRegion[i].NotValid)
                    {
                        kk.Add(_listRegion[i]);
                    }
                }

                _listRegion = kk;
                //if (!Check_ValidRegionData(_regionPixelLookup, _listRegion))
                //{
                //    System.Diagnostics.Trace.WriteLine($"End Invalid region data {_listRegion.Count}");
                //}

                //if (tmpRegions.Count == 0)
                //{
                //    throw new Exception();
                //}


            } while (origRegionCount > _listRegion.Count);
        }

        private void ReduceAllRegionUnderCountPixel(CanvasPixel canvasPixel, int maxCountPixel, CanvasPixel canvasPixelOriginal)
        {

            int origRegionCount = 0;
         
            do
            {
                origRegionCount = _listRegion.Count;
                for (int i = 0; i < _listRegion.Count; i++)
                {
                    RegionVO region = _listRegion[i];

                    if (region == null) break;
                    if (region.NotValid) continue;

                    if (region.Pixels.Length <= maxCountPixel)
                    {


                        RegionVO bestForFuse = BestRegionToFuse2(region, canvasPixel, int.MaxValue,true);
                        if (bestForFuse == null)
                        {
                            continue;
                        }

                        if (bestForFuse != region)
                        {
                            //// swap for faster merge
                            if (region.Pixels.Length > bestForFuse.Pixels.Length)
                            {
                                RegionVO tmp = region;
                                region = bestForFuse;
                                bestForFuse = tmp;
                            }

                            FuseRegions(bestForFuse, region, canvasPixel, canvasPixelOriginal);

                            region.NotValid = true;
                            region.Clear();
                        }
                    }

                }

                List<RegionVO> kk = new List<RegionVO>(_listRegion.Count);
                for (int i = 0; i < _listRegion.Count; i++)
                {
                    if (!_listRegion[i].NotValid)
                    {
                        kk.Add(_listRegion[i]);
                    }
                }

                _listRegion = kk;

            } while (origRegionCount > _listRegion.Count);
        }

        

        #region process regions


        private RegionVO BestRegionToFuse(RegionVO region, CanvasPixel canvasPixel, int maxColorDiff, bool ignoreColorCategory = false)
        {
            return BestRegionToFuse2(region, canvasPixel, maxColorDiff, ignoreColorCategory);
        }
        
        

        /// <summary>
        /// vraci region ktery je nejlepsi k slouceni s timto
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        private RegionVO BestRegionToFuse2(RegionVO region, CanvasPixel canvasPixel, int maxColorDiff, bool ignoreColorCategory = false)
        {
            

            int isRegionColorCategory = Helper_GetPixelColorCategory(region.Color);

            RegionVO result = null;
            int resultDiff = int.MaxValue;
            int resultCount = -1;
            foreach (var item in region.NeighbourRegions)
            {
                // int diff = BasicHelpers.FastAbs(item.Key.Color.IntClearAlpha() - region.Color.IntClearAlpha());

                //int maxDiff = Pixel.MaxDiff(item.Key.Color, region.Color);
                //if (maxDiff > (maxColorDiff / 2) + 1) continue;

                bool colorRegionEqual = true;

                if (!ignoreColorCategory)
                {
                    int isItemColorCategory = Helper_GetPixelColorCategory(item.Color);
                    colorRegionEqual = isRegionColorCategory == isItemColorCategory;
                }


                int diff = Pixel.SumAbsDiff(item.Color, region.Color);

                if (diff <= maxColorDiff && colorRegionEqual)
                {
                    if (diff < resultDiff
                      )
                    {
                        resultDiff = diff;
                        result = item;
                    }
                }
            }

            return result;
        }

       

        private void BestRegionToFuse_DetectRegionFromPixel(int indexPixel, RegionVO regionSource, Dictionary<RegionVO, int> regionsArround, HashSet<RegionVO> fuseIgnore)
        {

            RegionVO region = _regionPixelLookup[indexPixel];
            if (region != null && region != regionSource //&& !fuseIgnore.Contains(region)
                )
            {
                int outInt;
               if(regionsArround.TryGetValue(region,out outInt))
                {
                    regionsArround[region] = outInt + 1;
                }
               else
                {
                    regionsArround.Add(region, 1);
                }
                //if (regionsArround.Keys.Contains(region))
                //{
                //    regionsArround[region]++;
                //}
                //else
                //{
                //    regionsArround.Add(region, 1);
                //}
            }
        }

        private void BestRegionToFuse_DetectRegionFromPixel(int indexPixel, RegionVO regionSource, List<KeyValuePair<RegionVO, int>> regionsArround, HashSet<RegionVO> fuseIgnore)
        {

            RegionVO region = _regionPixelLookup[indexPixel];
            if (region != null && region != regionSource //&& !fuseIgnore.Contains(region)
                )
            {
                int regionIndex = -1;
                for(int i =0;i < regionsArround.Count; i++)
                {
                    if(regionsArround[i].Key == region)
                    {
                        regionIndex = i;
                        break;
                    }
                }

                if(regionIndex < 0)
                {

                    regionsArround.Add(new KeyValuePair<RegionVO, int>( region, 1));
                }
                //else
                //{
                //    var tmp = regionsArround[regionIndex];
                //    regionsArround[regionIndex] = new KeyValuePair<RegionVO, int>( tmp.Key,tmp.Value+1);
                //}
            }
        }

        /// <summary>
        /// slouci dva regiony
        /// dest region = k nemu jsou pridany pixely ze slucovaneho
        /// </summary>
        /// <param name="destRegion"></param>
        /// <param name="forFuseRegion"></param>
        private void FuseRegions(RegionVO destRegion, RegionVO forFuseRegion, CanvasPixel canvasPixel, CanvasPixel canvasPixelOriginal)
        {
            if(destRegion == forFuseRegion)
            {
                throw new Exception();
            }
            
            

            for (int i = 0; i < forFuseRegion.Pixels.Length; i++)
            {
                int item = forFuseRegion.Pixels[i];
                // reassign region
                _regionPixelLookup[item] = destRegion;
               
            }

            destRegion.Pixels = MergeOrderedPixel(destRegion.Pixels, forFuseRegion.Pixels);

            //destRegion.Pixels.Sort();
            //destRegion.CreatePixelEdges(_regionPixelLookup);
            //destRegion.CreatePixelEdges();
            //_regionManipulator.CreatePixelNeighbourEdge(destRegion,_regionPixelLookup);

            // _regionManipulator.MergePixelNeighbourEdge(destRegion,forFuseRegion, _regionPixelLookup);


            //if (destRegion.Pixels.Length > 100)
            //{
            //    destRegion.SetColor_AsNearToAverageToAll(canvasPixelOriginal, canvasPixel);
            //}
            //else
            //{
            //    destRegion.SetColorAsMedinToAll(canvasPixelOriginal, canvasPixel);
            //}

            //destRegion.SetColor_AsNearToAverageToAll(canvasPixelOriginal, canvasPixel);


            //destRegion.SetColorAsFixToAll(canvasPixelOriginal, canvasPixel);
            destRegion.SetColor_AsAverageToAll(canvasPixelOriginal, canvasPixel);
            //destRegion.SetColorHSLAsAverageToAll(canvasPixelOriginal, canvasPixel);

            //List<RegionVO> neighbours = Helper_GetMergedNeighbours2(destRegion, forFuseRegion);
            //destRegion.NeighbourRegions = neighbours.ToArray();
            RegionVO[] neighbours = Helper_GetMergedNeighbours(destRegion, forFuseRegion);
            destRegion.NeighbourRegions = neighbours;

            foreach(RegionVO item in neighbours)
            {
                item.Add_NeightbourRegion(destRegion);
                item.Remove_NeightbourRegion(forFuseRegion);

                //item.Create_NeighbourRegions(_regionPixelLookup);
            }
        }

        public static int[] MergeOrderedPixel(int[] destPixel, int[] forMerge)
        {
            int[] result = new int[destPixel.Length + forMerge.Length];

            Array.Copy(destPixel, result, destPixel.Length);
            Array.Copy(forMerge,0, result, destPixel.Length, forMerge.Length);

            return result;
        }

        public static  int [] MergeOrderedPixel2(int [] destPixel, int [] forMerge)
        {
            int [] result = new int[destPixel.Length + forMerge.Length];
            int resultIndex = 0;
            int destIndex = 0;
            int forMergeIndex = 0;

            while(destIndex < destPixel.Length && forMergeIndex < forMerge.Length)
            {
                while(destIndex < destPixel.Length && forMergeIndex < forMerge.Length &&  destPixel[destIndex] < forMerge[forMergeIndex])
                {
                    result[resultIndex] =destPixel[destIndex];
                    resultIndex++;
                    destIndex++;
                }

                while (destIndex < destPixel.Length && forMergeIndex < forMerge.Length && destPixel[destIndex] >= forMerge[forMergeIndex])
                {
                    result[resultIndex] = forMerge[forMergeIndex];
                    resultIndex++;
                    forMergeIndex++;
                }
            }

            while (destIndex < destPixel.Length)
            {
                result[resultIndex] = destPixel[destIndex];
                resultIndex++;
                destIndex++;
            }

            while ( forMergeIndex < forMerge.Length)
            {
                result[resultIndex] = forMerge[forMergeIndex];
                resultIndex++;
                forMergeIndex++;
            }


            return result;
        }

        private static HashSet<RegionVO> neighbours2 = new HashSet<RegionVO>();

        private ObjectRecyclerTS<HashSet<RegionVO>> _pool_Hashset_RegionVO = new ObjectRecyclerTS<HashSet<RegionVO>>(1);


        private RegionVO [] Helper_GetMergedNeighbours(RegionVO destRegion, RegionVO forFuseRegion)
        {
            //List<RegionVO> lookup = new List<RegionVO>();

            HashSet<RegionVO> neighbours2 = _pool_Hashset_RegionVO.GetNewOrRecycle();
            neighbours2.Clear();

            //HashSet<RegionVO> neighbours2 = new HashSet<RegionVO>();
            foreach (var item in destRegion.NeighbourRegions)
            {
                if(item != destRegion && item != forFuseRegion )
                {
                    neighbours2.Add(item);
                }
            }

            foreach (var item in forFuseRegion.NeighbourRegions)
            {
                if (item != destRegion && item != forFuseRegion )
                {
                    neighbours2.Add(item);
                }
            }


            RegionVO[] result = neighbours2.ToArray();

            neighbours2.Clear();
            _pool_Hashset_RegionVO.PutForRecycle(neighbours2);

            return result;
        }

        private static List<RegionVO> neighbours = new List<RegionVO>();

        private static List<RegionVO> Helper_GetMergedNeighbours2(RegionVO destRegion, RegionVO forFuseRegion)
        {
            //List<RegionVO> neighbours = new List<RegionVO>();
            neighbours.Clear();    
             
            //HashSet<RegionVO> neighbours = new HashSet<RegionVO>();
            foreach (var item in destRegion.NeighbourRegions)
            {
                if (item != destRegion && item != forFuseRegion)
                {
                    if (neighbours.IndexOf(item) < 0)
                    {
                        neighbours.Add(item);    
                    } 
                }
            }

            foreach (var item in forFuseRegion.NeighbourRegions)
            {
                if (item != destRegion && item != forFuseRegion)
                {
                    if (neighbours.IndexOf(item) < 0)
                    {
                        neighbours.Add(item);
                    }
                }
            }


            return neighbours;
        }



        private bool Check_ValidRegionData(RegionVO[] regionPixelLookup, List<RegionVO> listRegions)
        {
            HashSet<RegionVO> listRegionLookup = new HashSet<RegionVO>(listRegions);

            for(int i =0;i < regionPixelLookup.Length;i++)
            {
                RegionVO tmpRegion = regionPixelLookup[i];
                // specialni pripad, nepatri zadnemu regionu
                if (tmpRegion == null) continue;

                if (!listRegionLookup.Contains(tmpRegion))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        private int Helper_GetPixelColorCategory(Pixel color)
        {
            color.ConvertRGBToHSL();

            int chGroup = Helper_GetHSL_HGroup2(color.CH);

            int clGroup = Helper_GetHSL_LGroup(color.CL);

            if (clGroup == 0) return 0;

            int csGroup = Helper_GetHSL_SGroup(color.CS, clGroup);
            return  clGroup *1000+csGroup*100+chGroup;
        }

        private int Helper_GetHSL_HGroup(int hslHValue)
        {
            if (hslHValue < 21) return 0;
            else if (hslHValue < 63) return 1;
            else if (hslHValue < 105) return 2;
            else if (hslHValue < 147) return 3;
            else if (hslHValue < 188) return 4;
            else if (hslHValue < 229) return 5;
            else return 0;

        }

        private int Helper_GetHSL_HGroup2(int hslHValue)
        {
            if (hslHValue < 21) return 0;
            else if (hslHValue < 42) return 1;
            else if (hslHValue < 63) return 2;
            else if (hslHValue < 84) return 3;
            else if (hslHValue < 105) return 4;
            else if (hslHValue < 126) return 5;
            else if (hslHValue < 147) return 6;
            else if (hslHValue < 168) return 7;
            else if (hslHValue < 189) return 8;
            else if (hslHValue < 210) return 9;
            else if (hslHValue < 231) return 10;
            else return 11;

        }

        private int Helper_GetHSL_LGroup(int hslLValue)
        {
            if (hslLValue < 51) return 0;
            else if (hslLValue < 198) return 1;
            else return 2;
        }

        private int Helper_GetHSL_SGroup(int hslSValue, int LGroup)
        {
            if (LGroup == 0) return 0;
            else if(LGroup == 2)
            {
                if (hslSValue < 85) return 0;
                if (hslSValue < 170) return 1;
                else return 2;
            }
            else if(LGroup == 1)
            {
                if (hslSValue < 51) return 0;
                if (hslSValue < 102) return 1;
                if (hslSValue < 153) return 2;
                if (hslSValue < 204) return 3;
                else return 4;
            }

            throw new NotImplementedException();
        }

    }
}
