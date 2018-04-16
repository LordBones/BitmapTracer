using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BitmapTracer.Core.Helpers
{
    public static class BasicHelpers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FastAbs(int value)
        {
            //int topbitreplicated = value >> 31;
            //return (value ^ topbitreplicated) - topbitreplicated;
            return (value ^ (value >> 31)) - (value >> 31);

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min( int a, int b)
        {
            if (a < b) return a;
            else return b;
            
        }

    }
}
