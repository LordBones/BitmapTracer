using BitmapTracer.Core.basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmapTracer.Core.ColorToGrey
{
    public class ColorToGrey
    {
        public Array2D CreateColloredMatrix(int width, int height, int lineMargin)
        {
            Array2D result = new Array2D(width, height);

            for(int y = 0; y < height; y += lineMargin)
            {
                int index = y * result.Width;
                for(int x = 0; x < width; x++)
                {
                    result.Data[index + x] = 1;
                }
            }

            for (int x = 0; x < width; x += lineMargin)
            {
                int yIndex = 0;
                for (int y = 0; y < height; y++)
                {
                    result.Data[yIndex + x] = 1;
                    yIndex += result.Width;
                }
            }

            return result;
        }


        public CanvasPixel CreateColoredGreyScale(CanvasPixel source, Array2D matrix)
        {
            CanvasPixel result = new CanvasPixel(source.Width, source.Height);

            for(int i = 0;i < source.Data.Length; i++)
            {
                if (matrix.Data[i] == 1) result.Data[i] = source.Data[i];
                else
                {
                    result.Data[i] = ToGreyScale(source.Data[i]);
                }
            }

            return result;
        }

        private Pixel ToGreyScale(Pixel input)
        {
            Pixel result = new Pixel();


            //0.2126 * R + 0.7152 * G + 0.0722

            byte greyScaleColor = (byte)(0.2126 * input.CR + 0.7152 * input.CG + 0.0722 * input.CB);


            result.CR = greyScaleColor;
            result.CB = greyScaleColor;
            result.CG = greyScaleColor;
            result.CA = input.CA;

            return result;
        }
    }
}
