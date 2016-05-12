using System;

namespace ProcessTools
{
    /// <summary>
    /// Helper class to automatically dispose values.
    /// </summary>
    public class AutoDispose<T> : IDisposable
    {
        /// <summary>
        /// The function to call to dispose of the <paramref name="value"/>.
        /// </summary>
        public delegate void DisposeFunction(T value);

        readonly T mValue;
        readonly DisposeFunction mDisposer;

        /// <summary>
        /// Creates a new instance of AutoDispose with the given <paramref name="value"/>
        /// and <paramref name="disposer"/> function to call on Dispose.
        /// </summary>
        public AutoDispose(T value, DisposeFunction disposer)
        {
            mValue = value;
            mDisposer = disposer;
        }

        /// <summary>
        /// The value set.
        /// </summary>
        public T Value { get { return mValue; } }

        /// <summary>
        /// Called during garbage collection.
        /// </summary>
        public void Dispose()
        {
            if (mDisposer != null)
            {
                mDisposer(mValue);
            }
        }
    }
}
