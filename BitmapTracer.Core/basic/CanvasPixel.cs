using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmapTracer.Core.basic
{
    public class CanvasPixel
    {
        private int _width;
        private int _height;

        private Pixel[] _data;

        public int Width { get { return _width; } }
        public int Height { get { return _height; } }
        public Pixel [] Data { get { return _data; } }



        public CanvasPixel(int width, int height)
        {
            this._height = height;
            this._width = width;
            _data = new Pixel[height * width];
        }

        public static CanvasPixel CreateBitmpaFromCanvas(CanvasARGB canvas)
        {
            CanvasPixel result = new CanvasPixel(canvas.WidthPixel, canvas.HeightPixel);

            Pixel[] dest = result._data;
            byte[] source = canvas.Data;

            int di = 0;
            for (int i = 0; i < source.Length; i += 4)
            {
                Pixel tmpP = Pixel.Create(source[i+3], source[i + 2], source[i + 1], source[i]);
                dest[di] = tmpP;
                di++;
            }

            return result;
        }

        public static CanvasARGB CreateBitmpaFromCanvas(CanvasPixel canvasPixel)
        {
            CanvasARGB result = new CanvasARGB(canvasPixel.Width, canvasPixel.Height);

            byte[] dest = result.Data;
            Pixel[] source = canvasPixel.Data;

            int di = 0;
            for (int i = 0; i < source.Length; i ++)
            {
                Pixel p = source[i];

                dest[di] = p.B0;
                dest[di+1] = p.B1;
                dest[di+2] = p.B2;
                dest[di+3] = p.B3;

                di += 4;
                
            }

            return result;
        }

        public void TransformToInterleaveRGB()
        {
            for(int i = 0;i< _data.Length;i++)
            {
                Data[i].TransformToInterleaveRGB();
              
            }
        }

        public void TransformFromInterleaveRGB()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                Data[i].TransformFromInterleaveRGB();

            }
        }

        public void ReduceNotSeenColors()
        {
            for(int i =0;i<_data.Length;i++)
            {
                Pixel p = _data[i];
                if (p.CB < 50) p.CB = 0;
                if (p.CG < 50) p.CG = 0;
                if (p.CR < 50) p.CR = 0;
                _data[i] = p;

            }
        }

        public void ConvertPixelsRGBToHSL()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                Data[i].ConvertRGBToHSL();
            }
        }

        public void ConvertPixelsHSLToRGB()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                Data[i].ConvertHSLToRGB();
            }
        }
        //public void TransformBackFromInterleaveRGB()
        //{
        //    for(int i = 0;i< _data.Length;i++)
        //    {
        //        Data[i]. TransformToInterleaveRGB();
        


    }
}
