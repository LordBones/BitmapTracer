using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BitmapTracer.Core.EdgeDetector
{
    using BitmapTracer.Core.basic;
    using BitmapTracer.Core.Trace;

    public enum GradientDirection : int { none = 0, horizontal = 1, vertical = 2, askewRaise = 3, askewFall = 4 }

    public struct EdgePoint
    {
        public GradientDirection Direction;
        public int Intensity;

        public EdgePoint(GradientDirection direction, int intensity)
        {
            this.Direction = direction;
            this.Intensity = intensity;
        }
    }

    public class CanvasEdgeVO
    {
        private int _width;
        private int _height;

        private EdgePoint[] _data;

        public int Width { get { return _width; } }
        public int Height { get { return _height; } }
        public EdgePoint[] Data { get { return _data; } }



        public CanvasEdgeVO(int width, int height)
        {
            this._height = height;
            this._width = width;
            _data = new EdgePoint[height * width];
        }

        public void Clear()
        {
            for(int i = 0;i < this.Data.Length;i++)
            {
                this.Data[i] = new EdgePoint(GradientDirection.none, 0);
            }
        }

        public static CanvasEdgeVO CreateEdgesFromCanvas(CanvasPixel canvasPixel)
        {
            CanvasEdgeVO result = new CanvasEdgeVO(canvasPixel.Width, canvasPixel.Height);

            EdgePoint[] dest = result.Data;
            Pixel[] source = canvasPixel.Data;

            Pixel[] matrix = new Pixel[9];

            for (int y = 0; y < canvasPixel.Height; y++)
            {
                for (int x = 0; x < canvasPixel.Width; x++)
                {
                    int index = y * canvasPixel.Width + x;

                    matrix[0] = CreateEdgesFromCanvas_GetPixel(canvasPixel, x, y, -1, -1, index);
                    matrix[1] = CreateEdgesFromCanvas_GetPixel(canvasPixel, x, y, 0, -1, index);
                    matrix[2] = CreateEdgesFromCanvas_GetPixel(canvasPixel, x, y, 1, -1, index);

                    matrix[3] = CreateEdgesFromCanvas_GetPixel(canvasPixel, x, y, -1, 0, index);
                    matrix[4] = CreateEdgesFromCanvas_GetPixel(canvasPixel, x, y, 0, 0, index);
                    matrix[5] = CreateEdgesFromCanvas_GetPixel(canvasPixel, x, y, 1, 0, index);

                    matrix[6] = CreateEdgesFromCanvas_GetPixel(canvasPixel, x, y, -1, 1, index);
                    matrix[7] = CreateEdgesFromCanvas_GetPixel(canvasPixel, x, y, 0, 1, index);
                    matrix[8] = CreateEdgesFromCanvas_GetPixel(canvasPixel, x, y, 1, 1, index);

                    (GradientDirection direction, int intensity) gResult = DirectionGradientBO.Detect(matrix);

                    dest[index] = new EdgePoint(gResult.direction, gResult.intensity);

                }
            }
            return result;
        }

        private static Pixel CreateEdgesFromCanvas_GetPixel(CanvasPixel canvas, int x, int y, int diffX, int diffY, int index)
        {
            x += diffX;
            y += diffY;
            bool isOutOfRange = x < 0 || y < 0 || canvas.Width <= x || canvas.Height <= y;

            if (isOutOfRange)
            {
                return canvas.Data[index];
            }
            else
            {
                return canvas.Data[index + diffY * canvas.Width + diffX];
            }
        }

        

    }
}
