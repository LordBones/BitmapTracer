using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BitmapTracer.Core.basic
{
    public class HSLColor
    {
        // Private data members below are on scale 0-1
        // They are scaled for use externally based on scale
        private double hue = 1.0;
        private double saturation = 1.0;
        private double luminosity = 1.0;

        private const double scale = 255.0;

        public double Hue
        {
            get { return hue * scale; }
            set { hue = CheckRange(value / scale); }
        }
        public double Saturation
        {
            get { return saturation * scale; }
            set { saturation = CheckRange(value / scale); }
        }
        public double Luminosity
        {
            get { return luminosity * scale; }
            set { luminosity = CheckRange(value / scale); }
        }

        private double CheckRange(double value)
        {
            if (value < 0.0)
                value = 0.0;
            else if (value > 1.0)
                value = 1.0;
            return value;
        }

        public override string ToString()
        {
            return String.Format("H: {0:#0.##} S: {1:#0.##} L: {2:#0.##}", Hue, Saturation, Luminosity);
        }

        public void SetRGB(byte r, byte g, byte b)
        {
            double var_R = (r / 256.0);                   //RGB from 0 to 255
            double var_G = (g / 256.0);
            double var_B = (b / 256.0);

            double var_Min = Min(var_R, var_G, var_B);   //Min. value of RGB
            double var_Max = Max(var_R, var_G, var_B);  //Max. value of RGB
            double del_Max = var_Max - var_Min;           //Delta RGB value 

            double S;
            double H = 0.0;
            double L = (var_Max + var_Min) / 2;

            if (del_Max == 0)                     //This is a gray, no chroma...
            {
                H = 0;                                //HSL results from 0 to 1
                S = 0;
            }
            else                                    //Chromatic data...
            {
                if (L < 0.5) S = del_Max / (var_Max + var_Min);
                else S = del_Max / (2 - var_Max - var_Min);

                double del_R = (((var_Max - var_R) / 6) + (del_Max / 2)) / del_Max;
                double del_G = (((var_Max - var_G) / 6) + (del_Max / 2)) / del_Max;
                double del_B = (((var_Max - var_B) / 6) + (del_Max / 2)) / del_Max;

                if (var_R == var_Max) H = del_B - del_G;
                else if (var_G == var_Max) H = (1 / 3.0) + del_R - del_B;
                else if (var_B == var_Max) H = (2 / 3.0) + del_G - del_R;

                if (H < 0) H += 1;
                if (H > 1) H -= 1;
            }
            this.hue = H;
            this.luminosity = L;
            this.saturation = S;
        }

        private double Min(double p1, double p2, double p3)
        {
            double res = p1;
            if (res > p2) res = p2;
            if (res > p3) res = p3;

            return res;
        }

        private double Max(double p1, double p2, double p3)
        {
            double res = p1;
            if (res < p2) res = p2;
            if (res < p3) res = p3;

            return res;
        }

        public Color ToRGB()
        {
            double L = this.luminosity;
            double H = this.hue;
            double S = this.saturation;

            double R;
            double G;
            double B;

            if (S == 0)                       //HSL from 0 to 1
            {
                R = L * 255;                      //RGB results from 0 to 255
                G = L * 255;
                B = L * 255;
            }
            else
            {
                double var_2;
                if (L < 0.5) var_2 = L * (1 + S);
                else var_2 = (L + S) - (S * L);

                double var_1 = 2 * L - var_2;

                R = 255 * Hue_2_RGB(var_1, var_2, H + (1.0 / 3.0));
                G = 255 * Hue_2_RGB(var_1, var_2, H);
                B = 255 * Hue_2_RGB(var_1, var_2, H - (1.0 / 3.0));
            }

            return Color.FromRgb((byte)R, (byte)G, (byte)B);
        }

        private double Hue_2_RGB(double v1, double v2, double vH)             //Function Hue_2_RGB
        {
            if (vH < 0) vH += 1;
            if (vH > 1) vH -= 1;
            if ((6 * vH) < 1) return (v1 + (v2 - v1) * 6 * vH);
            if ((2 * vH) < 1) return (v2);
            if ((3 * vH) < 2) return (v1 + (v2 - v1) * ((2 / 3.0) - vH) * 6);
            return (v1);
        }


        public HSLColor() { }
        public HSLColor(Color color)
        {
            SetRGB(color.R, color.G, color.B);
        }
        public HSLColor(int red, int green, int blue)
        {
            SetRGB((byte)red, (byte)green, (byte)blue);
        }
        public HSLColor(double hue, double saturation, double luminosity)
        {
            this.Hue = hue;
            this.Saturation = saturation;
            this.Luminosity = luminosity;
        }

    }
}
