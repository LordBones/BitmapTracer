using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BitmapTracer.Core.basic;
using BitmapTracer.Core.Trace;

namespace BitmapTracer
{
    /// <summary>
    /// Interaction logic for VectorPreview.xaml
    /// </summary>
    public partial class VectorPreview : Window
    {
        public VectorPreview()
        {
            InitializeComponent();

            //Polyline pl = new Polyline();
            //pl.Points.Add(new Point(10, 10));
            //pl.Points.Add(new Point(50, 10));
            //pl.Points.Add(new Point(50, 40));
            //pl.Points.Add(new Point(10, 40));
            //pl.Stroke = new SolidColorBrush(Colors.Black);
            //pl.Fill = new SolidColorBrush(Colors.Blue);
            //pl.StrokeThickness = 2;
            
            //CanvasContainer.Children.Add(pl);
        }

        public void RenderLines(CanvasPixel original, CanvasPixel final)
        {
            int rowIndex = 0;
            for (int y = 0; y < final.Height ; y++)
            {
                int endIndex = rowIndex + final.Width;

                int currIndex = rowIndex;
                while (currIndex < endIndex)
                {
                    int startRangeIndex = currIndex;
                    int endRangeIndex = startRangeIndex;
                    Pixel color = final.Data[startRangeIndex];

                    while (endRangeIndex + 1 < endIndex && final.Data[endRangeIndex + 1].CompareTo(color) == 0)
                    {
                        endRangeIndex++;
                    }

                    int startX = startRangeIndex % final.Width;
                    int endX = endRangeIndex % final.Width;

                    Color colorTmp = Color.FromArgb(color.CA, color.CR, color.CG, color.CB);
                    CanvasContainer.Children.Add( Create_Line(startX, y, endX + 1, y, colorTmp));
                    

                    currIndex = endRangeIndex + 1;
                }

                rowIndex += final.Width;
            }

            //_output.WriteLine(Helper_CreateSVGLine(10,10,100,10,new Pixel() {CR=255,CG=255 }));

        }

        public void RenderPolygons(ICollection<RegionVO> regionsInput, RegionManipulator regMan)
        {

            RegionVO [] regions = regMan.GetOrderedForRendering(regionsInput.ToArray());

            RegionToPolygonBO regionToPolygon = new RegionToPolygonBO(regMan);

            HashSet<Point> lp = new HashSet<Point>();
            int maxY = 0;
            List<Point> kkkk = new List<Point>();

            foreach(var i in regions.OrderBy(x => x.Pixels.Length))
            {
                Trace.WriteLine($"{i.Pixels.Length},");
            }

            foreach (RegionVO region in regions)
            {
                Point[] points = regionToPolygon.ToPolygon(region);

                

                kkkk.AddRange(points);
                int kk = (int)points.Max(x => x.Y);
                if (maxY < kk) maxY = kk;

                Color color = Color.FromArgb(region.Color.CA, region.Color.CR, region.Color.CG, region.Color.CB);
                CanvasContainer.Children.Add(Create_Polyline(points, Colors.Black, color));
            }

            kkkk= kkkk.OrderByDescending(x => x.Y).ThenByDescending(x => x.X).ToList();
        }

        private Shape Create_Polyline(Point[] points, Color edgeColor, Color fillColor)
        {
            Polyline pl = new Polyline();
            pl.Points = new PointCollection(points);
            pl.Stroke = new SolidColorBrush(edgeColor);

            //pl.Fill = new SolidColorBrush(new Co);
            pl.Fill = new SolidColorBrush(fillColor);
            pl.StrokeThickness = 0.5;

            return pl;
        }

        private Shape Create_Line(double x1, double y1,double x2,double y2, Color fillColor)
        {
            Line pl = new Line();
            pl.X1 = x1;
            pl.X2 = x2;
            pl.Y1 = y1;
            pl.Y2 = y2;
            pl.Stroke = new SolidColorBrush(fillColor);
            pl.StrokeThickness = 1;
            
            return pl;
        }
    }
}
