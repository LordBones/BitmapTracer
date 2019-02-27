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

        public void RenderPolygons(ICollection<RegionVO> regionsInput, RegionManipulator regMan)
        {

            RegionVO [] regions = regMan.GetOrderedForRendering(regionsInput.ToArray());

            RegionToPolygonBO regionToPolygon = new RegionToPolygonBO(regMan);

            HashSet<Point> lp = new HashSet<Point>();
            int maxY = 0;
            List<Point> kkkk = new List<Point>();

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
            pl.Fill = new SolidColorBrush(fillColor);
            pl.StrokeThickness = 0.5;

            return pl;
        }
    }
}
