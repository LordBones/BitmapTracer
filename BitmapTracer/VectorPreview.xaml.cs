using System;
using System.Collections.Generic;
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

            Polyline pl = new Polyline();
            pl.Points.Add(new Point(10, 10));
            pl.Points.Add(new Point(50, 10));
            pl.Points.Add(new Point(50, 40));
            pl.Points.Add(new Point(10, 40));
            pl.Stroke = new SolidColorBrush(Colors.Black);
            pl.Fill = new SolidColorBrush(Colors.Blue);
            pl.StrokeThickness = 2;
            
            CanvasContainer.Children.Add(pl);
        }
    }
}
