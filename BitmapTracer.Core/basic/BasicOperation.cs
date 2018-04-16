using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmapTracer.Core.basic
{
    public class BasicOperation
    {
        public static void BlackWhite(CanvasPixel cp)
        {
            Pixel[] data = cp.Data;

            for (int i = 0; i < data.Length; i++)
            {
                Pixel pixel = data[i];

                if ((pixel.UInt & 0xffffff) > 0x7fffff)
                {
                    pixel.CB = 255;
                    pixel.CR = 255;
                    pixel.CG = 255;
                }
                else
                {
                    pixel.CB = 0;
                    pixel.CR = 0;
                    pixel.CG = 0;

                }

                data[i] = pixel;
            }
        }

        public static void PixelFilter(CanvasPixel cp, int tolerance)
        {
            Pixel[] data = cp.Data;

            for (int i = 1; i < data.Length; i++)
            {
                Pixel pixel = data[i];
                Pixel pixel2 = data[i-1];

                int color = pixel.Get_ColorClearAlpha_Int();
                int color2 = pixel2.Get_ColorClearAlpha_Int();

                if(Helpers.BasicHelpers.FastAbs( color - color2) <= tolerance)
                {
                    data[i] = data[i - 1];
                }

            }



            for (int i = 1; i < cp.Width; i++)
                for (int y = 1; y < cp.Height;y++ )
                {
                    int indexLast = (y-1) * cp.Width + i;

                    int index = y * cp.Width + i;

                    Pixel pixel = data[index];
                    Pixel pixel2 = data[indexLast ];

                    int color = pixel.Get_ColorClearAlpha_Int();
                    int color2 = pixel2.Get_ColorClearAlpha_Int();

                    if (Helpers.BasicHelpers.FastAbs(color - color2) <= tolerance)
                    {
                        data[index] = data[indexLast];
                    }

                }

        }

        public static void PixelFilter2(CanvasPixel cp, int tolerance)
        {
            Pixel[] data = cp.Data;

            Pixel[] result = new Pixel[data.Length];

            Array.Copy(data, 0, result, 0,data.Length);


            Pixel white = new Pixel();
            Pixel black = new Pixel();
            Pixel red = new Pixel();
            white.CR = 255;
            white.CG = 255;
            white.CB = 255;
            red.CR = 255;
            red.CA = 255;
            white.CA = 255;
            black.CA = 255;


            for (int y = 1; y < cp.Height-1; y++)
           
            for (int i = 1; i < cp.Width-1; i++)
                {
                    int rowindex1 = (y - 1) * cp.Width + i;
                    int rowindex2 = (y ) * cp.Width + i;
                    int rowindex3 = (y + 1) * cp.Width + i;


                    int middleColor = data[rowindex2].Get_ColorClearAlpha_Int();



                    bool b2 = Helpers.BasicHelpers.FastAbs(middleColor - data[rowindex1 ].Get_ColorClearAlpha_Int()) <= tolerance;
                    bool b4 = Helpers.BasicHelpers.FastAbs(middleColor - data[rowindex2-1].Get_ColorClearAlpha_Int()) <= tolerance;
                    bool b6 = Helpers.BasicHelpers.FastAbs(middleColor - data[rowindex2 + 1].Get_ColorClearAlpha_Int()) <= tolerance;
                    bool b8 = Helpers.BasicHelpers.FastAbs(middleColor - data[rowindex3 ].Get_ColorClearAlpha_Int()) <= tolerance;
                    
                    if(b2 || b4 || b6 || b8)
                    {
                        bool b1 = Helpers.BasicHelpers.FastAbs(middleColor - data[rowindex1-1].Get_ColorClearAlpha_Int()) <= tolerance;
                        bool b3 = Helpers.BasicHelpers.FastAbs(middleColor - data[rowindex1+1].Get_ColorClearAlpha_Int()) <= tolerance;
                        bool b7 = Helpers.BasicHelpers.FastAbs(middleColor - data[rowindex3 - 1].Get_ColorClearAlpha_Int()) <= tolerance;
                        bool b9 = Helpers.BasicHelpers.FastAbs(middleColor - data[rowindex3 + 1].Get_ColorClearAlpha_Int()) <= tolerance;

                        if (b2 && b4 && b6 && b8 && b1 && b3 && b7 && b9)
                        {
                            result[rowindex2] = white;
                        }
                        else
                        {
                            result[rowindex2] = red;
                        }
                    
                    }
                    else
                    {
                        result[rowindex2] = black;
                    }
                    

                }

            Array.Copy(result, 0, data, 0, data.Length);


        }


    }
}
