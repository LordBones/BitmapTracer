using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitmapTracer.Core.basic
{
    class PixelMedian<T> where T: IComparable
    {
        struct MedianItem
        {
            public int PixelId;
            public T Value;
        }

        private MedianItem[] _mItems;
        private int _countMItems;

        public PixelMedian(int maxPixels)
        {
            _mItems = new MedianItem[maxPixels];
            _countMItems = 0;
        }

        public void Add(int id, T value)
        {
            int index = _countMItems;
            while(index-1 >=0)
            {
                MedianItem item = _mItems[index - 1];
                if(item.Value.CompareTo( value) < 0)
                {
                    break;
                }

                _mItems[index] = item;
                index--;
            }

            _mItems[index] = new MedianItem() { PixelId = id, Value = value };
            _countMItems++;
        }

        public int GetMedianLower_asId()
        {
            int index = _countMItems / 2 +  (_countMItems & 1);
            return _mItems[index].PixelId;
        }

        public T GetMedianLower_asValue()
        {
            int index = _countMItems / 2 + (_countMItems & 1);
            return _mItems[index].Value;
        }
    }
}
