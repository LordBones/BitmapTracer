using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BitmapTracer.Core.basic;

namespace BitmapTracer.Core.Trace
{
    class ImageToSVG
    {
        private StreamWriter _output;

        public ImageToSVG(Stream outputStream)
        {
            this._output = new StreamWriter(outputStream, Encoding.UTF8);


        }

        public void BasicToSVG(CanvasPixel original, CanvasPixel final)
        {
            Write_StartHeader(final.Width,final.Height);

            int rowIndex = 0;
            for(int y = 0;y<final.Height;y++)
            {
                int endIndex = rowIndex + final.Width;

                int currIndex = rowIndex;
                while(currIndex < endIndex)
                {
                    int startRangeIndex = currIndex;
                    int endRangeIndex = startRangeIndex;
                    Pixel color = final.Data[startRangeIndex];

                    while(endRangeIndex+1 < endIndex && final.Data[endRangeIndex+1].CompareTo( color) == 0)
                    {
                        endRangeIndex++;
                    }

                    int startX = startRangeIndex % final.Width;
                    int endX = endRangeIndex % final.Width;


                    _output.WriteLine(Helper_CreateSVGLine(startX, y, endX+1, y, color));

                    currIndex = endRangeIndex + 1;
                }

                rowIndex += final.Width;
            }

            //_output.WriteLine(Helper_CreateSVGLine(10,10,100,10,new Pixel() {CR=255,CG=255 }));

            Write_EndHeader();
        }

        public void AreasToSVG(CanvasPixel original, List<RegionVO> regions, RegionManipulator regMan)
        {
            Write_StartHeader(original.Width, original.Height);

            RegionVO[] regionsOrdered = regMan.GetOrderedForRendering(regions.ToArray());

            RegionToPolygonBO regionToPolygon = new RegionToPolygonBO(regMan);

            for (int i = 0; i < regionsOrdered.Length; i++)
            {
                RegionVO region = regionsOrdered[i];

                Point[] points = regionToPolygon.ToPolygon(region);

                _output.WriteLine(Helper_CreateSVGPolyLine(points, region.Color));
               // _output.WriteLine(Helper_CreateSVGPolyGone(points, region.Color));
            }


            Write_EndHeader();
        }

        private void Write_StartHeader(int width, int height)
        {
            _output.WriteLine("<?xml version=\"1.0\" standalone=\"no\"?>");
            _output.WriteLine("<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">");

            double zoom = 1024 / (double)width;
            _output.WriteLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\"  width=\"1024\" height=\"auto\" viewbox=\"0,0,{width},{height}\" style=\"shape-rendering:crispEdges;zoom:{zoom.ToString(CultureInfo.InvariantCulture)}\">");

            

            _output.Flush();
        }

        private void Write_EndHeader()
        {
            _output.WriteLine("</svg>");
            _output.Flush();

        }

        private static string Helper_CreateSVGPolyGone(Point [] points, Pixel color)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<polygon points=\"");

            for(int i = 0;i<points.Length;i++)
            {
                sb.Append($@"{points[i].X},{points[i].Y} ");
            }

            if(points.Length > 0)
            {
                if(points[0] != points[points.Length - 1])
                {
                    sb.Append($@"{points[0].X},{points[0].Y} ");
                }
            }

            sb.Append($@"""  style = """);
            sb.Append($@"stroke-linecap:square;fill:rgb({color.CR},{color.CG},{color.CB});stroke:rgb({color.CR},{color.CG},{color.CB});stroke-width:1""/>");

            return sb.ToString();
        }

        private static string Helper_CreateSVGPolyLine(Point[] points, Pixel color)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<polyline points=\"");

            for (int i = 0; i < points.Length; i++)
            {
                sb.Append($@"{points[i].X},{points[i].Y} ");
            }

            if (points.Length > 0)
            {
                if (points[0] != points[points.Length - 1])
                {
                    sb.Append($@"{points[0].X},{points[0].Y} ");
                }
            }

            sb.Append($@"""  style = """);
            sb.Append($@"stroke-linecap:square;stroke:rgb({color.CR},{color.CG},{color.CB});stroke-width:1""/>");

            return sb.ToString();
        }

        private static string Helper_CreateSVGLine(int x , int y, int x2, int y2, Pixel color)
        {
            return $@"<line x1=""{x}"" y1=""{y}"" x2=""{x2}"" y2=""{y2}"" "+  
            $@"style=""stroke:rgb({color.CR},{color.CG},{color.CB})""/>";
        }
    }
}
