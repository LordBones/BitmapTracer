using BitmapTracer.Core.basic;
using BitmapTracer.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmapTracer.Core.Trace
{
    public class RegionDetektor
    {
        // index odpovida pixelu a odkazuje se na oblast do ktere spada
        private RegionVO[] _regionPixelLookup;

        private List<RegionVO> _listRegion;

        public int TotalRegions { get { return _listRegion.Count; } }

        private RegionManipulator _regionManipulator = null;

        public RegionDetektor()
        {

        }

        public CanvasPixel DetectOld(CanvasARGB canvas, int tolerance)
        {
            _regionPixelLookup = new RegionVO[canvas.Length];
            _listRegion = new List<RegionVO>();

            CanvasPixel canvasPixel = CanvasPixel.CreateBitmpaFromCanvas(canvas);
            this._regionManipulator = new RegionManipulator(null, canvasPixel);
            //canvasPixel.ReduceNotSeenColors();

            //canvasPixel.TransformToInterleaveRGB();

            List<int> singleAlonePoints = CreateRegionFromStart(canvasPixel, 0);//tolerance);
            //List<int> singleAlonePoints = CreateRegionFromStartCicCac(canvasPixel, tolerance);



            FuseAllSinglePoints(singleAlonePoints, canvasPixel, tolerance);

            int minPixels = tolerance;
            for (int i = 1; i <= minPixels; i++)
                ReduceAllRegionUnderCountPixel(canvasPixel, i);



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



            return canvasPixel;

        }

        public CanvasPixel Detect(CanvasARGB canvas, int tolerance)
        {
            _regionPixelLookup = new RegionVO[canvas.Length];
            _listRegion = new List<RegionVO>();

            CanvasPixel canvasPixel = CanvasPixel.CreateBitmpaFromCanvas(canvas);
            this._regionManipulator = new RegionManipulator(null, canvasPixel);
            //canvasPixel.ConvertPixelsRGBToHSL();
            //canvasPixel.ReduceNotSeenColors();
            //canvasPixel.TransformToInterleaveRGB();

            List<int> singleAlonePoints = CreateRegionFromStart(canvasPixel, 0);// tolerance);
            //List<int> singleAlonePoints = CreateRegionFromStartCicCac(canvasPixel, tolerance);



            FuseAllSinglePoints(singleAlonePoints, canvasPixel, tolerance);

            int minPixels = tolerance;
            //for (int i = 1; i <= 16; i++)
            //    ReduceAllRegionUnderCountPixel(canvasPixel, i);
           


            for (int i = minPixels;
                i <= minPixels; i++)
                ReduceAllRegionUnderColorTolerance(canvasPixel, i);
            //ReduceAllRegionUnderCountPixel(canvasPixel, 5);

            Dictionary<int, int> groups = new Dictionary<int, int>();
            foreach(var item in _listRegion)
            {
                if(groups.ContainsKey(item.Pixels.Length))
                {
                    groups[item.Pixels.Length]++; 
                }
                else
                {
                    groups.Add(item.Pixels.Length, 1);
                }
            }



            //canvasPixel.ConvertPixelsHSLToRGB();
            return canvasPixel;

        }

        private List<int> CreateRegionFromStartCicCac(CanvasPixel canvasPixel, int tolerance)
        {
            List<int> listSinglePoints = new List<int>();

            CanvasPixel cp = canvasPixel;
            Pixel[] data = cp.Data;

            int startX = 1;
            int startY = 1;

            for (int i = 1; i < cp.Width-1;i++ )
            {

                int tmpX = startX;
                int tmpY = startY; 
                while( tmpX>= 1 && tmpY < (cp.Height-1))
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


            for (int y = 1; y < cp.Height - 1; y += 2)
                for (int i = 1; i < cp.Width - 1; i+=2)
                {
                    CreateRegionFromStartPixel(i, y, listSinglePoints, canvasPixel, tolerance);
                }

            for (int y = 2; y < cp.Height - 1; y += 2)
                for (int i = 1; i < cp.Width - 1; i+=2)
                {
                    CreateRegionFromStartPixel(i, y, listSinglePoints, canvasPixel, tolerance);
                }


            for (int y = 1; y < cp.Height - 1; y += 2)
                for (int i = 2; i < cp.Width - 1; i+=2)
                {
                    CreateRegionFromStartPixel(i, y, listSinglePoints, canvasPixel, tolerance);
                }

            for (int y = 2; y < cp.Height - 1; y += 2)
                for (int i = 2; i < cp.Width - 1; i+=2)
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

            int rowindex1 = tmpRow-cp.Width + x;
            int rowindex2 = tmpRow + x;
            int rowindex3 = tmpRow+  cp.Width + x;


            if (_regionPixelLookup[rowindex2] != null) return;

            int middleColor = data[rowindex2].Get_ColorClearAlpha_Int();



            //int v2 = Helpers.BasicHelpers.FastAbs(middleColor - data[rowindex1].IntClearAlpha());
            //int v4 = Helpers.BasicHelpers.FastAbs(middleColor - data[rowindex2 - 1].IntClearAlpha());
            //int v6 = Helpers.BasicHelpers.FastAbs(middleColor - data[rowindex2 + 1].IntClearAlpha());
            //int v8 = Helpers.BasicHelpers.FastAbs(middleColor - data[rowindex3].IntClearAlpha());
             
            int v2 = Pixel.MaxDiff(data[rowindex2] , data[rowindex1]);
            int v4 = Pixel.MaxDiff(data[rowindex2] , data[rowindex2-1]);
            int v6 = Pixel.MaxDiff(data[rowindex2] , data[rowindex2+1]);
            int v8 = Pixel.MaxDiff(data[rowindex2], data[rowindex3]);


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

                }
                else
                {
                    Pixel tmpMiddleColor = data[rowindex2];
                    RegionVO newRegion = new RegionVO(tmpMiddleColor);
                    newRegion.Add_Pixel(rowindex2);
                    _regionPixelLookup[rowindex2] = newRegion;

                    if (b2 && y > 1)
                    {
                        newRegion.Add_Pixel(rowindex1);
                        _regionPixelLookup[rowindex1] = newRegion;
                        data[rowindex1] = tmpMiddleColor;
                    }
                    
                    if (b4 && x > 1)
                    {
                        newRegion.Add_Pixel(rowindex2 - 1);
                        _regionPixelLookup[rowindex2 - 1] = newRegion;
                        data[rowindex2 - 1] = tmpMiddleColor;
                    }

                    if (b6 && x < (cp.Width-1-1))
                    {
                        newRegion.Add_Pixel(rowindex2 + 1);
                        _regionPixelLookup[rowindex2 + 1] = newRegion;
                        data[rowindex2 + 1] = tmpMiddleColor;
                    }

                    if (b8 && y < (cp.Height-1-1))
                    {
                        newRegion.Add_Pixel(rowindex3);
                        _regionPixelLookup[rowindex3] = newRegion;
                        data[rowindex3] = tmpMiddleColor;
                    }

                    _listRegion.Add(newRegion);
                    //newRegion.CreatePixelEdges();
                }


            }
            else
            {
                Pixel tmpMiddleColor = data[rowindex2];
                RegionVO newRegion = new RegionVO(tmpMiddleColor);
                newRegion.Add_Pixel(rowindex2);
                _regionPixelLookup[rowindex2] = newRegion;
                _listRegion.Add(newRegion);
                // newRegion.CreatePixelEdges(_regionPixelLookup);
                 //newRegion.CreatePixelEdges();
                //listSinglePoints.Add(rowindex2);
            }

        }

        private void FuseAllSinglePoints(List<int> singleAlonePoints, CanvasPixel canvasPixel, int tolerance)
        {
            List<int> tmpSinglePoints = new List<int>();

            int toleranceModif = 1024;

            while(singleAlonePoints.Count > 0)
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



        private void ReduceAllRegionUnderCountPixel(CanvasPixel canvasPixel,int minCountPixels)
        {
            List<RegionVO> tmpRegions = new List<RegionVO>();

            for (int i = 0; i < _listRegion.Count;i++ )
            {
                if (_listRegion[i].Pixels.Length < minCountPixels)
                {
                    RegionVO region = _listRegion[i]; // _listRegion.FirstOrDefault(x => x.Pixels.Count < minCountPixels);

                    if (region == null) break;

                    RegionVO bestForFuse = BestRegionToFuse(region, canvasPixel, 1024);
                    if (bestForFuse == null)
                    {
                        tmpRegions.Add(_listRegion[i]);
                        continue;
                        //throw new NotImplementedException();
                    }

                    FuseRegions(bestForFuse, region, canvasPixel);
                }
                else
                {
                    tmpRegions.Add(_listRegion[i]);
                }
            }

            _listRegion = tmpRegions;
        }

        private void ReduceAllRegionUnderColorTolerance(CanvasPixel canvasPixel, int maxColorDiff)
        {

            int origRegionCount = 0;
            
            do
            {
                HashSet<RegionVO> usedInFuseLookup = new HashSet<RegionVO>();

                List<RegionVO> tmpRegions = new List<RegionVO>(_listRegion.Count);
                //tmpRegions.Clear();
                //tmpRegions.AddRange(_listRegion);
                
                _listRegion = _listRegion.OrderBy(x => x.Pixels.Length).ToList();
               
                origRegionCount = _listRegion.Count();
                for (int i = 0; i < _listRegion.Count; i++)
                {
                    RegionVO region = _listRegion[i]; // _listRegion.FirstOrDefault(x => x.Pixels.Count < minCountPixels);

                    if (region == null) break;
                    if(usedInFuseLookup.Contains(region))
                    {
                        tmpRegions.Add(_listRegion[i]);
                        continue;
                    }

                    RegionVO bestForFuse = BestRegionToFuse(region, canvasPixel, maxColorDiff, usedInFuseLookup);
                    if (bestForFuse == null)
                    {
                        usedInFuseLookup.Add(region);
                        tmpRegions.Add(region);
                        continue;
                        //throw new NotImplementedException();
                    }

                    //if (bestForFuse.Pixels.Count >= region.Pixels.Count())
                    {
                        usedInFuseLookup.Add(bestForFuse);
                        usedInFuseLookup.Add(region);

                        FuseRegions(bestForFuse, region, canvasPixel);
                    }
                    //else
                    {
                      //  tmpRegions.Add(region);
                      //  continue;

                    }
                    //else
                    //{
                    //    FuseRegions(region,bestForFuse , canvasPixel);
                    //}
                }
                //tmpRegions.CopyTo(_listRegion);
               
                _listRegion = tmpRegions;
            } while (origRegionCount > _listRegion.Count);
        }

        #region process regions


        //static Dictionary<RegionVO, int> regionsAround = new Dictionary<RegionVO, int>();


        private RegionVO BestRegionToFuse(RegionVO region, CanvasPixel canvasPixel,int maxColorDiff)
        {
            return BestRegionToFuse(region, canvasPixel, maxColorDiff, new HashSet<RegionVO>());
        }
        /// <summary>
        /// vraci region ktery je nejlepsi k slouceni s timto
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        private RegionVO BestRegionToFuse(RegionVO region, CanvasPixel canvasPixel,int maxColorDiff, HashSet<RegionVO> fuseIgnore)
        {
            //regionsAround.Clear();
            Dictionary<RegionVO, int> regionsAround = new Dictionary<RegionVO, int>();

            foreach(int pixelIndex in region.Pixels)
            {
                int pixelIndexborder = pixelIndex - 1;

                BestRegionToFuse_DetectRegionFromPixel(pixelIndexborder, region, regionsAround, fuseIgnore);
                pixelIndexborder = pixelIndex + 1;
                BestRegionToFuse_DetectRegionFromPixel(pixelIndexborder, region, regionsAround,fuseIgnore);
                pixelIndexborder = pixelIndex - canvasPixel.Width;
                BestRegionToFuse_DetectRegionFromPixel(pixelIndexborder, region, regionsAround,fuseIgnore);
                pixelIndexborder = pixelIndex + canvasPixel.Width;
                BestRegionToFuse_DetectRegionFromPixel(pixelIndexborder, region, regionsAround,fuseIgnore);
            }

            //RegionVO result = null;
            //int resultCount = int.MaxValue;
            //foreach (var item in regionsAround)
            //{
            //    if (item.Value < resultCount)
            //    {
            //        resultCount = item.Value;
            //        result = item.Key;
            //    }
            //}

            RegionVO result = null;
            int resultDiff = int.MaxValue;
            int resultCount = -1;
            Pixel re = region.Color;
            
            re.ConvertRGBToHSL();
            foreach (var item in regionsAround)
            {
               // int diff = BasicHelpers.FastAbs(item.Key.Color.IntClearAlpha() - region.Color.IntClearAlpha());

                //int maxDiff = Pixel.MaxDiff(item.Key.Color, region.Color);
                //if (maxDiff > (maxColorDiff / 2) + 1) continue;
                Pixel it = item.Key.Color;
                it.ConvertRGBToHSL();

                int diff = Pixel.SumAbsDiff(item.Key.Color, region.Color);

                int diff2_min = BasicHelpers.Min(re.CH, it.CH);

                int diff2 = BasicHelpers.FastAbs(re.CH - it.CH);
                bool firstLess = re.CH < it.CH;

                int diff2absl = BasicHelpers.FastAbs(re.CL - it.CL);

                Pixel p = new Pixel();
                p.CR = 255;
                p.CG = 255;
                p.CB = 255;

                p.ConvertRGBToHSL();
                
                if (diff <= maxColorDiff)
                {
                    if (!((re.CL < 50 && it.CL < 50) ||
                        (re.CL > 205 && it.CL > 205)) )
                    {
                        if( diff2absl > 160)
                        {
                            continue;
                        }

                        if (!(((diff2_min + diff2 + 1) * item.Key.Pixels.Length <= (diff2_min + 1) * region.Pixels.Length && firstLess) ||
                             ((diff2 + 1) * item.Key.Pixels.Length <= (diff2_min + diff2_min + 1) * region.Pixels.Length && !firstLess)
                            ))
                        {
                            continue;
                        }
                    }
                 
                        if(
                     //       ((diff2+1)*item.Key.Pixels.Count) < resultDiff
                    diff < resultDiff
                    //item.Value > resultCount
                    )
                        {
                    resultDiff = 
                        diff;
                     //((diff2 + 1) * item.Key.Pixels.Count);
                    result = item.Key;
                    resultCount = item.Value;
                        }
                }
            }

            return result;
        }

        private void BestRegionToFuse_DetectRegionFromPixel(int indexPixel, RegionVO regionSource, Dictionary<RegionVO, int> regionsArround, HashSet<RegionVO> fuseIgnore)
        {

            RegionVO region = _regionPixelLookup[indexPixel];
            if (region != null && region != regionSource && !fuseIgnore.Contains(region))
            {
                if (regionsArround.Keys.Contains(region))
                {
                    regionsArround[region]++;
                }
                else
                {
                    regionsArround.Add(region, 1);
                }
            }
        }

        /// <summary>
        /// slouci dva regiony
        /// dest region = k nemu jsou pridany pixely ze slucovaneho
        /// </summary>
        /// <param name="destRegion"></param>
        /// <param name="forFuseRegion"></param>
        private void FuseRegions(RegionVO destRegion, RegionVO forFuseRegion, CanvasPixel canvasPixel)
        {

            Pixel finalColor = new Pixel();
            //finalColor = destRegion.Color;
            //finalColor = Pixel.Average(destRegion.Color, forFuseRegion.Color);
            finalColor = Pixel.Average(destRegion.Color, destRegion.Pixels.Length, forFuseRegion.Color, forFuseRegion.Pixels.Length);



            foreach (var item in destRegion.Pixels)
            {
                canvasPixel.Data[item] = finalColor;
                destRegion.Color = finalColor;
            }

            List<int> dPixels = destRegion.Pixels.ToList();
            Pixel[] cpData = canvasPixel.Data;

            foreach(var item in forFuseRegion.Pixels)
            {
                dPixels.Add(item);
                // reassign region
                _regionPixelLookup[item] = destRegion;
                // apply color
                cpData[item] = finalColor;// destRegion.Color;
            }

            dPixels.Sort();
            destRegion.Pixels = dPixels.ToArray();
            //destRegion.CreatePixelEdges(_regionPixelLookup);
            //_regionManipulator.CreatePixelEdges(destRegion);
        }

        #endregion
    }

}

