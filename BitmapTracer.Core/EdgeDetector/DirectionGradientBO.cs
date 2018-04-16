using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;


namespace BitmapTracer.Core.Trace
{
    using BitmapTracer.Core.basic;
    using BitmapTracer.Core.EdgeDetector;
    using BitmapTracer.Core.Helpers;

    public static class DirectionGradientBO
    {
    
        public static (GradientDirection direction, int intensity) Detect(Pixel [] matrix3x3 )
        {
            int sumVectorX = 0, sumVectorY = 0;
            

            Pixel middle = matrix3x3[4];



            SumDiffVector(-1, 1, middle, matrix3x3[0], ref sumVectorX, ref sumVectorY);
            SumDiffVector(0, 1, middle, matrix3x3[1], ref sumVectorX, ref sumVectorY);
            SumDiffVector(1, 1, middle, matrix3x3[2], ref sumVectorX, ref sumVectorY);

            SumDiffVector(-1, 0, middle, matrix3x3[3], ref sumVectorX, ref sumVectorY);
            SumDiffVector(1, 0, middle, matrix3x3[5], ref sumVectorX, ref sumVectorY);

            SumDiffVector(-1, -1, middle, matrix3x3[6], ref sumVectorX, ref sumVectorY);
            SumDiffVector(0, -1, middle, matrix3x3[7], ref sumVectorX, ref sumVectorY);
            SumDiffVector(1, -1, middle, matrix3x3[8], ref sumVectorX, ref sumVectorY);



            (GradientDirection direction, int intensity) result = ResolveDirectionAndIntesity(sumVectorX, sumVectorY);

            return result;
        }

        private static (GradientDirection direction, int intensity) ResolveDirectionAndIntesity(int sumVectorX, int sumVectorY)
        {
            GradientDirection resultDirection =  GradientDirection.none;
            int resultIntensity = 0;

            if (sumVectorX == 0 && sumVectorY != 0) resultDirection = GradientDirection.vertical;
            if (sumVectorX != 0 && sumVectorY == 0) resultDirection = GradientDirection.horizontal;


            DetectQuadrant1(sumVectorX, sumVectorY, ref resultDirection);
            DetectQuadrant2(sumVectorX, sumVectorY, ref resultDirection);
            DetectQuadrant3(sumVectorX, sumVectorY, ref resultDirection);
            DetectQuadrant4(sumVectorX, sumVectorY, ref resultDirection);



            resultIntensity = (int)Math.Sqrt((sumVectorX * sumVectorX) + (sumVectorY * sumVectorY));

            return (resultDirection, resultIntensity);
        }

        private static void DetectQuadrant4(int sumVectorX, int sumVectorY, ref GradientDirection resultDirection)
        {
            if (sumVectorX < 0 && sumVectorY > 0)
            {
                int absSumVectorX = BasicHelpers.FastAbs(sumVectorX);

                if (sumVectorY > absSumVectorX)
                {
                    if (absSumVectorX * 2 < sumVectorY) resultDirection = GradientDirection.vertical;
                    else resultDirection = GradientDirection.askewFall;
                }
                else
                {
                    if (absSumVectorX < sumVectorY * 2) resultDirection = GradientDirection.horizontal;
                    else resultDirection = GradientDirection.askewFall;
                }
            }
        }

        private static void DetectQuadrant1(int sumVectorX, int sumVectorY, ref GradientDirection resultDirection)
        {
            if (sumVectorX > 0 && sumVectorY > 0)
            {
                int absSumVectorX = BasicHelpers.FastAbs(sumVectorX);

                if (sumVectorY > absSumVectorX)
                {
                    if (absSumVectorX * 2 < sumVectorY) resultDirection = GradientDirection.vertical;
                    else resultDirection = GradientDirection.askewRaise;
                }
                else
                {
                    if (absSumVectorX < sumVectorY * 2) resultDirection = GradientDirection.horizontal;
                    else resultDirection = GradientDirection.askewRaise;
                }
            }
        }

        private static void DetectQuadrant2(int sumVectorX, int sumVectorY, ref GradientDirection resultDirection)
        {
            if (sumVectorX > 0 && sumVectorY < 0)
            {
                int absSumVectorX = BasicHelpers.FastAbs(sumVectorX);
                int absSumVectorY = BasicHelpers.FastAbs(sumVectorY);

                if (absSumVectorY > absSumVectorX)
                {
                    if (absSumVectorX * 2 < absSumVectorY) resultDirection = GradientDirection.vertical;
                    else resultDirection = GradientDirection.askewFall;
                }
                else
                {
                    if (absSumVectorX < absSumVectorY * 2) resultDirection = GradientDirection.horizontal;
                    else resultDirection = GradientDirection.askewFall;
                }
            }
        }

        private static void DetectQuadrant3(int sumVectorX, int sumVectorY, ref GradientDirection resultDirection)
        {
            if (sumVectorX < 0 && sumVectorY < 0)
            {
                int absSumVectorX = BasicHelpers.FastAbs(sumVectorX);
                int absSumVectorY = BasicHelpers.FastAbs(sumVectorY);

                if (absSumVectorY > absSumVectorX)
                {
                    if (absSumVectorX * 2 < absSumVectorY) resultDirection = GradientDirection.vertical;
                    else resultDirection = GradientDirection.askewRaise;
                }
                else
                {
                    if (absSumVectorX < absSumVectorY * 2) resultDirection = GradientDirection.horizontal;
                    else resultDirection = GradientDirection.askewRaise;
                }
            }
        }



        private static void SumDiffVector(int x,int y, Pixel p1, Pixel p2, ref int sumVectorX, ref int sumVectorY)
        {
            int intensity = PixelDiff(p1, p2);

            sumVectorX += x * intensity;
            sumVectorY += y * intensity;
        }

        private static int PixelDiff(Pixel p1, Pixel p2)
        {
            return PixelDiff_Custom(p1, p2);
        }

        private static int PixelDiff_Custom(Pixel p1, Pixel p2)
        {
            int result = 0;

            result += BasicHelpers.FastAbs(p1.CR - p2.CR);
            result += BasicHelpers.FastAbs(p1.CG - p2.CG);
            result += BasicHelpers.FastAbs(p1.CB - p2.CB);
            return result;
        }

    }
}
