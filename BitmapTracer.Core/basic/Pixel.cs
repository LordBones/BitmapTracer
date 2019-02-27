using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BitmapTracer.Core.basic
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Pixel : IComparable<Pixel>, IComparable
    {
        [FieldOffset(0)]
        public byte B0;

        [FieldOffset(1)]
        public byte B1;

        [FieldOffset(2)]
        public byte B2;

        [FieldOffset(3)]
        public byte B3;

        [FieldOffset(0)]
        public byte CB;

        [FieldOffset(1)]
        public byte CG;

        [FieldOffset(2)]
        public byte CR;

        [FieldOffset(3)]
        public byte CA;

        [FieldOffset(0)]
        public byte CH;

        [FieldOffset(1)]
        public byte CS;

        [FieldOffset(2)]
        public byte CL;


        [FieldOffset(0)]
        public Int32 Int;

        [FieldOffset(0)]
        public UInt32 UInt;

        
        public Int32 Get_ColorClearAlpha_Int()
        {
            return Int & 0xffffff;
        }

        public void Set_ColorClearAlpha_Int(int color)
        {
            byte tmp = CA;
            Int = color;
            CA = tmp;
        }


        public UInt32 Get_ColorClearAlpha_UInt()
        {
            return UInt & 0xffffff;
        }




        public static Pixel Create(byte a,byte r, byte g, byte b)
        {
            Pixel result = new Pixel();

            result.CB = b;
            result.CR = r;
            result.CG = g;
            result.CA = a;

            return result;
        }

        public void TransformToInterleaveRGB()
        {
            int r = CR;
            int g = CG;
            int b = CB;

            int dest = 0;

            for (int k = 0; k < 8; k++)
            {
                dest >>= 3;
                dest = dest | ((r & 1) << 21);
                //dest >>= 1;
                dest = dest | ((g & 1) << 22);
                //dest >>= 1;
                dest = dest | ((b & 1) << 23);

                r >>= 1;
                g >>= 1;
                b >>= 1;
            }

            B0 = (byte)(dest & 0xff);
            B1 = (byte)((dest>>8) & 0xff);
            B2 = (byte)((dest >> 16) & 0xff);

        }

        public void TransformFromInterleaveRGB()
        {
            int data = this.Get_ColorClearAlpha_Int();

            int r = 0;
            int g = 0;
            int b = 0;

            for (int k = 0; k < 8; k++)
            {
                r >>= 1;
                g >>= 1;
                b >>= 1;

                r = r | ((data & 1) << 7);
                g = g | (((data >> 1) & 1) << 7);
                b = b | (((data >> 2) & 1) << 7);

                data >>= 3;
            }

            CB = (byte)b;
            CG = (byte)g;
            CR = (byte)r;
        }

        static HSLColor c = new HSLColor();

        public void ConvertRGBToHSL()
        {
            c.SetRGB(CR, CG, CB);

            CH = (byte)c.Hue;
            CL = (byte)c.Luminosity;
            CS = (byte)c.Saturation;
        }

        public void ConvertHSLToRGB()
        {
            HSLColor c = new HSLColor((double)CH, (double)CS, (double)CL);

            Color tmpC = c.ToRGB();

            CR = tmpC.R;
            CG = tmpC.G;
            CB = tmpC.B;
        }



        public static Pixel Average(Pixel first, Pixel second)
        {
            Pixel result = new Pixel();
            result.B0 = (byte)((first.B0 + second.B0) / 2);
            result.B1 = (byte)((first.B1 + second.B1) / 2);
            result.B2 = (byte)((first.B2 + second.B2) / 2);
            result.B3 = (byte)((first.B3 + second.B3) / 2);

            return result;
        }

        public static Pixel AverageHsl(Pixel first, Pixel second)
        {
            Pixel firstHsl = first;
            firstHsl.ConvertRGBToHSL();
            Pixel secondHsl = first;
            secondHsl.ConvertRGBToHSL();

            Pixel result = new Pixel();
            result.B0 = (byte)((firstHsl.B0 + secondHsl.B0) / 2);
            result.B1 = (byte)((firstHsl.B1 + secondHsl.B1) / 2);
            result.B2 = (byte)((firstHsl.B2 + secondHsl.B2) / 2);
            result.B3 = (byte)((firstHsl.B3 + secondHsl.B3) / 2);

            result.ConvertHSLToRGB();

            return result;
        }

        public static Pixel Average(Pixel first,int countF,  Pixel second, int countS)
        {
            double ratio = countF / (double)(countS+countF);

            Pixel result = new Pixel();

            result.B0 = (byte)(second.B0 + (first.B0 - second.B0) * ratio);
            result.B1 = (byte)(second.B1 + (first.B1 - second.B1) * ratio);
            result.B2 = (byte)(second.B2 + (first.B2 - second.B2) * ratio);
            result.B3 = (byte)(second.B3 + (first.B3 - second.B3) * ratio);

            return result;
        }

        public static Pixel AverageByHSL(Pixel first, int countF, Pixel second, int countS)
        {
            Pixel firstHsl = first;
            firstHsl.ConvertRGBToHSL();
            Pixel secondHsl = first;
            secondHsl.ConvertRGBToHSL();


            double ratio = countF / (double)(countS + countF);

            Pixel result = new Pixel();

            result.B0 = firstHsl.B0;// (byte)(secondHsl.B0 + (firstHsl.B0 - secondHsl.B0) * ratio);
            result.B1 = (byte)(secondHsl.B1 + (firstHsl.B1 - secondHsl.B1) * ratio);
            result.B2 = (byte)(secondHsl.B2 + (firstHsl.B2 - secondHsl.B2) * ratio);
            result.B3 = (byte)(secondHsl.B3 + (firstHsl.B3 - secondHsl.B3) * ratio);

            result.ConvertHSLToRGB();

            return result;
        }

        public static int MaxDiff5(Pixel first, Pixel second)
        {
            
            int diff1 = (first.CR - second.CR);
            int diff2 = (first.CG - second.CG);
            int diff3 = (first.CB - second.CB);

            if (diff1 > diff2) Helpers.BasicHelpers.Swap(ref diff1, ref diff2);
            if (diff2 > diff3) Helpers.BasicHelpers.Swap(ref diff2, ref diff3);
            if (diff1 > diff2) Helpers.BasicHelpers.Swap(ref diff1, ref diff2);


            int diff = (diff3 - diff1)* (
                 Helpers.BasicHelpers.FastAbs(diff1)+
                 Helpers.BasicHelpers.FastAbs(diff2)+
                 Helpers.BasicHelpers.FastAbs(diff3)
                 );
            
            //if (diff > 15) diff += 15;
            //diff += (first.CR - second.CR) * (first.CR - second.CR);
            //diff += (first.CG - second.CG) * (first.CG - second.CG);
            //diff += (first.CB - second.CB) *(first.CB - second.CB);

            return diff;
        }

        

        public static int SumAbsDiff(Pixel first, Pixel second)
        {
            int diff = 0;

            diff += Helpers.BasicHelpers.FastAbs((first.CR - second.CR));
            diff += Helpers.BasicHelpers.FastAbs((first.CG - second.CG));
            diff += Helpers.BasicHelpers.FastAbs((first.CB - second.CB));
            //if (diff > 15) diff += 15;
            //diff += (first.CR - second.CR) * (first.CR - second.CR);
            //diff += (first.CG - second.CG) * (first.CG - second.CG);
            //diff += (first.CB - second.CB) *(first.CB - second.CB);



            return diff;
        }

        public static int MaxDiff(Pixel first, Pixel second)
        {
            int diff = 0;

            int tmpDiff = Helpers.BasicHelpers.FastAbs(first.CR - second.CR);
            if (diff < tmpDiff) diff = tmpDiff;
            tmpDiff = Helpers.BasicHelpers.FastAbs(first.CG - second.CG);
            if (diff < tmpDiff) diff = tmpDiff;
            tmpDiff = Helpers.BasicHelpers.FastAbs(first.CB - second.CB);
            if (diff < tmpDiff) diff = tmpDiff;

            return diff;
        }

        public static int MaxDiff3(Pixel first, Pixel second)
        {
            int diff = 0;

            int tmpDiff = Helpers.BasicHelpers.FastAbs(first.CL - second.CL);
            diff = tmpDiff;
            //int tmpDiff = Helpers.BasicHelpers.FastAbs(first.CS - second.CS);
            //diff = tmpDiff;
            
            //if (diff < tmpDiff) diff = tmpDiff;
            //tmpDiff = Helpers.BasicHelpers.FastAbs(first.CG - second.CG);
            //if (diff < tmpDiff) diff = tmpDiff;
            //tmpDiff = Helpers.BasicHelpers.FastAbs(first.CB - second.CB);
            //if (diff < tmpDiff) diff = tmpDiff;

            return diff;
        }

        public int CompareTo(Pixel other)
        {
            return other.Int - this.Int;
        }

        public int CompareTo(object obj)
        {
            return ((Pixel)obj).Int - this.Int;
        }

        public string ToString()
        {
            return $"{this.CA,3},{this.CR,3},{this.CG,3},{this.CB,3}";
        }
    }

   
}
