using BitmapTracer.Core.basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmapTracer.Core.Trace
{
    public class RegionVO
    {
        // forDebug
        //    public int Id;
        // static int IdNext;
        public bool NotValid;
        public Pixel Color;
        public int [] Pixels;
       
        public RegionVO [] NeighbourRegions;

        private static int[] _emptyPixelEdgeNeighbours = new int[0];
        private static RegionVO[] _emptyNeighboursRegions = new RegionVO[0];
        

        public RegionVO(Pixel color)
        {
            Color = color;
            Pixels = _emptyPixelEdgeNeighbours;
            NeighbourRegions = _emptyNeighboursRegions;
          
          //  Id = IdNext;
          //  IdNext++;
        }

        public void Clear()
        {
            this.Pixels = _emptyPixelEdgeNeighbours;
            NeighbourRegions = _emptyNeighboursRegions;
        }

        public void Add_Pixel(int pixel)
        {
            int length = this.Pixels.Length;
            int[] newPixels = new int[length + 1];

            Array.Copy(this.Pixels,0, newPixels,0, length);

            newPixels[length] = pixel;
            this.Pixels = newPixels;
        }

        public void Add_Pixels(int [] pixel)
        {
            if (pixel.Length == 0) return;

            int length = this.Pixels.Length;
            int[] newPixels = new int[length + pixel.Length];

            Array.Copy(this.Pixels,0, newPixels,0, length);
            Array.Copy(pixel,0, newPixels, length,pixel.Length);
            
            this.Pixels = newPixels;
        }

        public void Add_NeightbourRegion(RegionVO region)
        {
            int countRegions = Helper_GetCountNeighbour(region);
            if (countRegions == 0)
            {
                int lenght = this.NeighbourRegions.Length;

                RegionVO[] newNeighbours = new RegionVO[lenght + 1];

                Array.Copy(this.NeighbourRegions,0, newNeighbours,0, lenght);
                
                newNeighbours[lenght] = region;

                this.NeighbourRegions = newNeighbours;
            }
        }

    

        public void Remove_NeightbourRegion(RegionVO region)
        {
            int countForRemove = Helper_GetCountNeighbour(region);
            if (countForRemove > 0)
            {
                int lenght = this.NeighbourRegions.Length;

                System.Buffers.ArrayPool<RegionVO> pool = System.Buffers.ArrayPool<RegionVO>.Shared;

                RegionVO[] newNeighbours = new RegionVO[this.NeighbourRegions.Length - countForRemove];
                int newNIndex = 0;
                for (int i = 0; i < lenght; i++)
                {
                    if (this.NeighbourRegions[i] != region)
                    {
                        newNeighbours[newNIndex] = this.NeighbourRegions[i];
                        newNIndex++;
                    }
                }

                this.NeighbourRegions = newNeighbours;
            }


        }

        private int Helper_GetCountNeighbour(RegionVO region)
        {
            int lenght = this.NeighbourRegions.Length;

            int  count = 0;
            for (int i = 0; i < lenght; i++)
            {
                if (this.NeighbourRegions[i] == region)
                {
                    count++;
                }
            }

            return count;
        }

        //public static void ClearIdNext()
        //{
        //    IdNext = 0;
        //}


        //public override string ToString()
        //{
        //    return $"{this.Id}";
        //}

        public void SetColor_AsAverageToAll(CanvasPixel originalCanvasPixel, CanvasPixel canvasPixel)
        {
            Pixel[] data = canvasPixel.Data;
            Pixel[] origData = originalCanvasPixel.Data;

            Int64 sumR = 0;
            Int64 sumG = 0;
            Int64 sumB = 0;

            for (int i = 0; i < Pixels.Length; i++)
            {
                int item = Pixels[i];
                Pixel origDataItem = origData[item];
                sumR += origDataItem.CR;
                sumG += origDataItem.CG;
                sumB += origDataItem.CB;

            }

            //int color = (int)(sum / Pixels.Count);

            Color.CR = (byte)(sumR / Pixels.Length);
            Color.CG = (byte)(sumG / Pixels.Length);
            Color.CB = (byte)(sumB / Pixels.Length);

            

            int tmp = Color.Get_ColorClearAlpha_Int();
            for (int i = 0; i < Pixels.Length; i++)
            {
                int item = Pixels[i];
                data[item].Set_ColorClearAlpha_Int(tmp);
            }
        }

        public void SetColor_AsNearToAverageToAll(CanvasPixel originalCanvasPixel, CanvasPixel canvasPixel)
        {
            Pixel[] data = canvasPixel.Data;
            Pixel[] origData = originalCanvasPixel.Data;

            Int64 sumR = 0;
            Int64 sumG = 0;
            Int64 sumB = 0;

            for (int i = 0; i < Pixels.Length; i++)
            {
                int item = Pixels[i];
                Pixel origDataItem = origData[item];
                    sumR += origDataItem.CR;
                sumG += origDataItem.CG;
                sumB += origDataItem.CB;

            }

            //int color = (int)(sum / Pixels.Count);

            Color.CR = (byte)(sumR / Pixels.Length);
            Color.CG = (byte)(sumG / Pixels.Length);
            Color.CB = (byte)(sumB / Pixels.Length);

            Color = GetNearestPixel(origData, Pixels, Color);

            int tmp = Color.Get_ColorClearAlpha_Int();
            for (int i = 0;i<Pixels.Length; i++)
            {
                    int item = Pixels[i];
                data[item].Set_ColorClearAlpha_Int(tmp);
            }
        }

        public static Pixel GetNearestPixel(Pixel [] pixelData, int [] pixelIndexs, Pixel pivot)
        {
            Pixel result = new Pixel();
            int resultDiff = int.MaxValue;

            for(int i = 0;i< pixelIndexs.Length;i++)
            {
                int index = pixelIndexs[i];
                Pixel pixel = pixelData[index];
                int tmpDiff = Pixel.SumAbsDiff(pixel, pivot);

                if(resultDiff > tmpDiff)
                {
                    result = pixel;
                    resultDiff = tmpDiff;
                }
            }

            return result;
        }

        public void SetColorAsMedinToAll(CanvasPixel originalCanvasPixel, CanvasPixel canvasPixel)
        {
            Pixel[] data = canvasPixel.Data;
            Pixel[] origData = originalCanvasPixel.Data;

            PixelMedian<Pixel> median = new PixelMedian<Pixel>(Pixels.Length);
            
            for (int i = 0; i < Pixels.Length; i++)
            {
                int item = Pixels[i];
                median.Add(0, origData[item]);
            }

            this.Color = median.GetMedianLower_asValue();
            
            int tmp = Color.Get_ColorClearAlpha_Int();
            for (int i = 0; i < Pixels.Length; i++)
            {
                int item = Pixels[i];
                data[item].Set_ColorClearAlpha_Int(tmp);
            }
        }

        public void SetColorAsFixToAll(CanvasPixel originalCanvasPixel, CanvasPixel canvasPixel)
        {
            Pixel[] data = canvasPixel.Data;
            Pixel[] origData = originalCanvasPixel.Data;

            Pixel tmpPixel = origData[Pixels[0]];
            
            this.Color = tmpPixel;

            int tmp = Color.Get_ColorClearAlpha_Int();
            for (int i = 0; i < Pixels.Length; i++)
            {
                int item = Pixels[i];
                data[item].Set_ColorClearAlpha_Int(tmp);
            }
        }


        public void SetColorHSLAsAverageToAll(CanvasPixel canvasPixelOriginal,CanvasPixel canvasPixel)
        {
            Pixel[] data = canvasPixel.Data;
            Pixel[] dataOrig = canvasPixelOriginal.Data;


            Int64 sumH = 0;
            Int64 sumS = 0;
            Int64 sumL = 0;

            foreach (int item in Pixels)
            {
                var hsl = dataOrig[item];
                hsl.ConvertRGBToHSL();

                sumH += hsl.CH;
                sumS += hsl.CS;
                sumL += hsl.CL;

            }
            
            Pixel tmp = new Pixel();
            tmp.CH = (byte)(sumH / Pixels.Length);
            tmp.CS = (byte)(sumS / Pixels.Length);
            tmp.CL = (byte)(sumL / Pixels.Length);
            tmp.ConvertHSLToRGB();
            int colorToSet = tmp.Get_ColorClearAlpha_Int();


            Color.Set_ColorClearAlpha_Int(colorToSet);

            foreach (int item in Pixels)
            {
                data[item].Set_ColorClearAlpha_Int(colorToSet);
            }
        }
        
    }
}
