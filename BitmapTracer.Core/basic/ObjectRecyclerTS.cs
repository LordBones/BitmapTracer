using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BitmapTracer.Core.basic
{
    public class ObjectRecyclerTS<T> where T : new()
    {

        private readonly int CONST_MaxElementForRecycle;

        private T[] _objects = new T[0];
        private int _objIndex = 0;

        FastLock _fastLock = new FastLock();

        public ObjectRecyclerTS()
            : this(10)
        {

        }

        public ObjectRecyclerTS(int maxElementsForRecycle)
        {
            CONST_MaxElementForRecycle = maxElementsForRecycle;
            _objects = new T[CONST_MaxElementForRecycle];
        }

        public T GetNewOrRecycle()
        {
            {
                using (_fastLock.Lock())
                {
                    if (_objIndex > 0)
                    {
                        _objIndex--;
                        return _objects[_objIndex];
                    }
                }

            }

            return Instance.Invoke();// new T();
        }

        public static readonly Func<T> Instance =
     Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();

        public void PutForRecycle(T[] objects)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                PutForRecycle(objects[i]);
            }
        }

        public void PutForRecycle(T pobject)
        {
            using (_fastLock.Lock())
            {
                if (_objIndex < this.CONST_MaxElementForRecycle)
                {
                    _objects[_objIndex] = pobject;
                    _objIndex++;
                }
            }

        }

        public void Clear()
        {
            using (_fastLock.Lock())
            {
                _objIndex = 0;
            }
        }


    }


}
