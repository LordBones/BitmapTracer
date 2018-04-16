using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmapTracer.Core.basic
{
    public class ColorReductor
    {
        public void Reduce(CanvasPixel canvas, int threshold)
        {
            Pixel [] data = canvas.Data;

            double coef = 255.0 / threshold;

            for(int i =0;i< data.Length;i++)
            {
                int b = data[i].CB;
                b = (int)(Math.Round((b / 255.0) * threshold)*coef);
                data[i].CB = (byte)b;

                int r = data[i].CR;
                r = (int)(Math.Round((r / 255.0) * threshold) * coef);
                data[i].CR = (byte)r;

                int g = data[i].CG;
                g = (int)(Math.Round((g / 255.0) * threshold) * coef);
                data[i].CG = (byte)g;

            }
        }

        public void ReduceByMask(CanvasPixel canvas, int mask)
        {
            Pixel[] data = canvas.Data;

            int maskOne = 0;
            for(int i = 0;i< mask;i++)
            {
                maskOne = (maskOne << 1) | 1;
            }

            for (int i = 0; i < data.Length; i++)
            {
                int b = data[i].CB;

                if (((b >> 7) & 1) == 0) b = b & (~maskOne);
                else b = b | (maskOne);

                data[i].CB = (byte)b;

                int r = data[i].CR;
                if (((r >> 7) & 1) == 0) r = r & (~maskOne);
                else r = r | (maskOne);

                data[i].CR = (byte)r;

                int g = data[i].CG;
                if (((g >> 7) & 1) == 0) g = g & (~maskOne);
                else g = g | (maskOne);

                data[i].CG = (byte)g;

            }
        }

#warning co tim chtel autor rici 
        public void Reduce2(CanvasPixel canvas, int threshold)
        {
            //0.2126 * R + 0.7152 * G + 0.0722

            Pixel[] data = canvas.Data;

            double coefb = 255.0 / threshold* 0.0722;
            double coefr = 255.0 / (threshold*(0.2126));
            double coefg = 255.0 / (threshold * (0.7152 ));


            for (int i = 0; i < data.Length; i++)
            {
                int b = data[i].CB;
                b = (int)(Math.Floor((b / 255.0) * threshold * 0.0722) * coefb);
                if (b >= 256) throw new Exception("");
                data[i].CB = (byte)b;

                int r = data[i].CR;
                int tr = (int)(Math.Floor((r / 255.0) * (threshold * (0.2126 ))) * coefr);
                if (tr >= 256) throw new Exception("");
                data[i].CR = (byte)tr;

                int g = data[i].CG;
                g = (int)(Math.Floor((g / 255.0) * (threshold * (0.7152 ))) * coefg);
                if (g >= 256) throw new Exception("");
                data[i].CG = (byte)g;

            }
        }

        public CanvasPixel Reduce3(CanvasPixel canvas, int threshold)
        {
            Pixel[] data = canvas.Data;
            CanvasPixel result = new CanvasPixel(canvas.Width, canvas.Height);

            for(int y = 0;y<canvas.Height;y++)
            for (int x = 0; x < canvas.Width; x++)
            {
                    int destIndex = y * canvas.Width + x;

                    List<Pixel> allValidPixels = GetAllValidPixels(canvas, x, y);
                    //Pixel avgPixel = GetAVGPixel(allValidPixels);
                    //List<Pixel> allNearest = GetNeightboursValidPixels(canvas,x,y);
                    //Pixel avgPixel = GetAVGPixel(allNearest);
                    Pixel avgPixel = GetMedianPixel2(allValidPixels, data[destIndex]);
                    //Pixel avgPixel =result.Data[destIndex];

                    Pixel nearestPixel = GetNearestPixel(allValidPixels, avgPixel, data[destIndex], threshold);
                   
                    result.Data[destIndex] = nearestPixel;
            }

            return result;
        }

        public CanvasPixel Reduce31(CanvasPixel canvas, int threshold)
        {
            Pixel[] data = canvas.Data;

            CanvasPixel result = new CanvasPixel(canvas.Width, canvas.Height);

            data.CopyTo(result.Data, 0);

            for (int y = 0; y < canvas.Height; y+=2)
            {
                for (int x = 0; x < canvas.Width; x+=2)
                {
                    Reduce31_OnePixel(canvas, result, x, y, threshold);
                }
            }

            for (int y = 1; y < canvas.Height; y += 2)
            {
                for (int x = 1; x < canvas.Width; x += 2)
                {
                    Reduce31_OnePixel(canvas, result, x, y, threshold);
                }
            }

            for (int y = 0; y < canvas.Height; y += 2)
            {
                for (int x = 1; x < canvas.Width; x += 2)
                {
                    Reduce31_OnePixel(canvas, result, x, y, threshold);
                }
            }

            for (int y = 1; y < canvas.Height; y += 2)
            {
                for (int x = 0; x < canvas.Width; x += 2)
                {
                    Reduce31_OnePixel(canvas, result, x, y, threshold);
                }
            }

            return result;
        }

        private void Reduce31_OnePixel(CanvasPixel canvas, CanvasPixel result, int x, int y, int threshold)
        {
            int destIndex = y * canvas.Width + x;


            List<Pixel> allValidPixels = GetAllValidPixels(result, x, y);
            //Pixel avgPixel = GetAVGPixel(allValidPixels);
            //List<Pixel> allNearest = GetNeightboursValidPixels(canvas,x,y);
            //Pixel avgPixel = GetAVGPixel(allNearest);
            Pixel avgPixel = GetMedianPixel2(allValidPixels, result.Data[destIndex]);
            //Pixel avgPixel =result.Data[destIndex];

            Pixel nearestPixel = GetNearestPixel(allValidPixels, avgPixel, result.Data[destIndex], threshold);

            result.Data[destIndex] = nearestPixel;
        }

        private List<Pixel> GetNeightboursValidPixels(CanvasPixel canvas, int x, int y)
        {
            
            List<Pixel> result = new List<Pixel>();

            int baseIndex = y * canvas.Width;
            int cy = -1;
            int cx = 0;

            if (((x + cx) >= 0 && (x + cx) < canvas.Width) && ((y + cy) >= 0 && (y + cy) < canvas.Height))
            {
                int index = baseIndex + (cy * canvas.Width) + x + cx;
                result.Add(canvas.Data[index]);
            }

            cy = 0;
            cx = -1;
            if (((x + cx) >= 0 && (x + cx) < canvas.Width) && ((y + cy) >= 0 && (y + cy) < canvas.Height))
            {
                int index = baseIndex + (cy * canvas.Width) + x + cx;
                result.Add(canvas.Data[index]);
            }

            cy = 0;
            cx = 1;
            if (((x + cx) >= 0 && (x + cx) < canvas.Width) && ((y + cy) >= 0 && (y + cy) < canvas.Height))
            {
                int index = baseIndex + (cy * canvas.Width) + x + cx;
                result.Add(canvas.Data[index]);
            }
            cy = 1;
            cx = 0;
            if (((x + cx) >= 0 && (x + cx) < canvas.Width) && ((y + cy) >= 0 && (y + cy) < canvas.Height))
            {
                int index = baseIndex + (cy * canvas.Width) + x + cx;
                result.Add(canvas.Data[index]);
            }

            return result;
        }

        private List<Pixel> GetAllValidPixels(CanvasPixel canvas, int x, int y)
        {
            const int kernelSize = 1;
            List<Pixel> result = new List<Pixel>();

            int baseIndex = y * canvas.Width;

            for (int cy = -(kernelSize); cy< (kernelSize+1); cy++)
            for (int cx = -(kernelSize); cx < (kernelSize+1); cx++)
            {
                    //if (cx == 0 && cy == 0) continue;

                if(((x+cx)>= 0 && (x + cx) < canvas.Width) &&
                   ((y + cy) >= 0 && (y + cy) < canvas.Height))
                    {
                        int index = baseIndex + (cy * canvas.Width) + x + cx;
                        result.Add(canvas.Data[index]);
                    }
            }

            return result;
        }

        private Pixel GetAVGPixel(List<Pixel> pixels)
        {
            int sumR = 0;
            int sumB = 0;
            int sumG = 0;
            
            for(int i = 0;i < pixels.Count;i++)
            {
                Pixel tmpP = pixels[i];
                sumR += tmpP.CR;
                sumG += tmpP.CG;
                sumB += tmpP.CB;
            }

            Pixel result = new Pixel();
            result.CR = (byte)(sumR / pixels.Count);
            result.CG = (byte)(sumG / pixels.Count);
            result.CB = (byte)(sumB / pixels.Count);
            result.CA = 255;
            return result;
        }

        private Pixel GetMedianPixel (List<Pixel> pixels, Pixel orig)
        {
            List<Pixel> order = new List<Pixel>();
            List<int> diff = new List<int>();

            for (int i = 0; i < pixels.Count; i++)
            {
                Pixel tmpP = pixels[i];
                int tmpDiff = 0;
                tmpDiff += Helpers.BasicHelpers.FastAbs(tmpP.CR - orig.CR);
                tmpDiff += Helpers.BasicHelpers.FastAbs(tmpP.CB - orig.CB);
                tmpDiff += Helpers.BasicHelpers.FastAbs(tmpP.CG - orig.CG);

                //tmpDiff += ( tmpP.CR - threshold.CR)* (tmpP.CR - threshold.CR);
                //tmpDiff += (tmpP.CB - threshold.CB) * (tmpP.CB - threshold.CB);
                //tmpDiff += (tmpP.CG - threshold.CG)*(tmpP.CG - threshold.CG);

                //tmpDiff = (int)Math.Sqrt(tmpDiff);

                int index = diff.BinarySearch(tmpDiff);
                if (index >= 0)
                {
                    diff.Insert(index, tmpDiff);
                    order.Insert(index, tmpP);
                }
                else
                {
                    index = ~index;
                    diff.Insert(index, tmpDiff);
                    order.Insert(index, tmpP);
                }

                // System.Diagnostics.Trace.WriteLine($"dd:{tmpDiff}");
            }


            int indexMedian = (order.Count / 2) + (order.Count & 1) - 1;

            return order[indexMedian];
        }

        private Pixel GetMedianPixel2(List<Pixel> pixels, Pixel orig)
        {
            PixelMedian<int> computeMedian = new PixelMedian<int>(pixels.Count); 

            for (int i = 0; i < pixels.Count; i++)
            {
                Pixel tmpP = pixels[i];
                int tmpDiff = 0;
                tmpDiff += Helpers.BasicHelpers.FastAbs(tmpP.CR - orig.CR);
                tmpDiff += Helpers.BasicHelpers.FastAbs(tmpP.CB - orig.CB);
                tmpDiff += Helpers.BasicHelpers.FastAbs(tmpP.CG - orig.CG);

                computeMedian.Add(i, tmpDiff);

                //tmpDiff += ( tmpP.CR - threshold.CR)* (tmpP.CR - threshold.CR);
                //tmpDiff += (tmpP.CB - threshold.CB) * (tmpP.CB - threshold.CB);
                //tmpDiff += (tmpP.CG - threshold.CG)*(tmpP.CG - threshold.CG);

                //tmpDiff = (int)Math.Sqrt(tmpDiff);
                
                // System.Diagnostics.Trace.WriteLine($"dd:{tmpDiff}");
            }


            int indexMedian = computeMedian.GetMedianLower_asId();

            return pixels[indexMedian];
        }

        

        private Pixel GetNearestPixel(List<Pixel> pixels, Pixel threshold, Pixel orig, int diffTreshold)
        {

            Pixel bestPixel = new Pixel();

            int bestPixelDiff = int.MaxValue;

            for (int i = 0; i < pixels.Count; i++)
            {
                Pixel tmpP = pixels[i];
                int tmpDiff = 0;
                tmpDiff += Helpers.BasicHelpers.FastAbs(tmpP.CR - threshold.CR);
                tmpDiff += Helpers.BasicHelpers.FastAbs(tmpP.CB - threshold.CB);
                tmpDiff += Helpers.BasicHelpers.FastAbs(tmpP.CG - threshold.CG);

                //tmpDiff += ( tmpP.CR - threshold.CR)* (tmpP.CR - threshold.CR);
                //tmpDiff += (tmpP.CB - threshold.CB) * (tmpP.CB - threshold.CB);
                //tmpDiff += (tmpP.CG - threshold.CG)*(tmpP.CG - threshold.CG);

                //tmpDiff = (int)Math.Sqrt(tmpDiff);

                if (tmpDiff < bestPixelDiff)
                {
                    bestPixelDiff = tmpDiff;
                    bestPixel = tmpP;
                }

               // System.Diagnostics.Trace.WriteLine($"dd:{tmpDiff}");
            }

            
            if (bestPixelDiff <= diffTreshold)
            {
                return bestPixel;
            }

            return orig;
        }

        private Pixel GetNearestPixelMedian(List<Pixel> pixels, Pixel threshold, Pixel orig, int diffTreshold)
        {
            List<Pixel> order = new List<Pixel>();
            List<int> diff = new List<int>();
            Pixel bestPixel = new Pixel();

            int bestPixelDiff = int.MaxValue;

            for (int i = 0; i < pixels.Count; i++)
            {
                Pixel tmpP = pixels[i];
                int tmpDiff = 0;
                tmpDiff += Helpers.BasicHelpers.FastAbs(tmpP.CR - threshold.CR);
                tmpDiff += Helpers.BasicHelpers.FastAbs(tmpP.CB - threshold.CB);
                tmpDiff += Helpers.BasicHelpers.FastAbs(tmpP.CG - threshold.CG);

                //tmpDiff += ( tmpP.CR - threshold.CR)* (tmpP.CR - threshold.CR);
                //tmpDiff += (tmpP.CB - threshold.CB) * (tmpP.CB - threshold.CB);
                //tmpDiff += (tmpP.CG - threshold.CG)*(tmpP.CG - threshold.CG);

                //tmpDiff = (int)Math.Sqrt(tmpDiff);

                int index = diff.BinarySearch(tmpDiff);
                if(index >= 0)
                {
                    diff.Insert(index, tmpDiff);
                    order.Insert(index, tmpP);
                }
                else
                {
                    index = ~index;
                    diff.Insert(index, tmpDiff);
                    order.Insert(index, tmpP);
                }
                
                // System.Diagnostics.Trace.WriteLine($"dd:{tmpDiff}");
            }


            int indexMedian = (order.Count / 2) + (order.Count & 1) - 1;

            
            if (diff[indexMedian] <= diffTreshold)
            {
                Pixel medianPixel = order[indexMedian];
                return medianPixel;
            }

            return orig;
        }
    }
}
