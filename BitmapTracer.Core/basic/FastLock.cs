using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BitmapTracer.Core.basic
{
    class FastLock : IDisposable
    {
        private const int CONST_LOCK = 1;
        private const int CONST_UNLOCK = 0;

        SpinWait spinner = new SpinWait();

        private int _isLock = CONST_UNLOCK;

        public FastLock() { }

        public FastLock Lock()
        {
            Helper_LockSection();

            return this;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Helper_LockSection()
        {
            if (Interlocked.CompareExchange(ref _isLock, CONST_LOCK, CONST_UNLOCK) != CONST_UNLOCK)
            {
                do
                {
                    //Thread.Sleep(1);
                    spinner.SpinOnce();
                }
                while (Interlocked.CompareExchange(ref _isLock, CONST_LOCK, CONST_UNLOCK) != CONST_UNLOCK);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Helper_UnlockSection()
        {
            if (Interlocked.CompareExchange(ref _isLock, CONST_UNLOCK, CONST_LOCK) != CONST_LOCK)
            {
                throw new Exception("This cant happend");

                //var spinner = new SpinWait();

                //do
                //{
                //    spinner.SpinOnce();
                //}
                //while (Interlocked.CompareExchange(ref _isLock, CONST_UNLOCK, CONST_LOCK) != CONST_LOCK);
            }
        }

        public void Dispose()
        {
            Helper_UnlockSection();
        }
    }
}
