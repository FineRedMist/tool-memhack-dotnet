using System;

namespace ProcessTools
{
    class AutoDispose<T> : IDisposable
    {
        public delegate void DisposeFunction(T value);

        readonly T mValue;
        readonly DisposeFunction mDisposer;

        public AutoDispose(T value, DisposeFunction disposer)
        {
            mValue = value;
            mDisposer = disposer;
        }

        public T Value { get { return mValue; } }

        public void Dispose()
        {
            if (mDisposer != null)
            {
                mDisposer(mValue);
            }
        }
    }
}
