using System;

namespace ProcessTools
{
    /// <summary>
    /// This just feels like a hack, but it works.  I originally was passing the windows form 
    /// object for the progress bar directly and having it modify the values however I've found
    /// out that updates to the UI needs to be done on the same thread as the owner of the UI 
    /// requiring the use of Invoke (or it's async variations).
    /// So this interface is implemented by the ProgressBar UI object so that I can update the 
    /// UI from a different thread.  The implementation there uses Invoke to ensure it is 
    /// updated correctly
    /// </summary>
    public interface IProgressIndicator
    {
        /// <summary>
        /// Sets the maximum for a progress indicator to <paramref name="Maximum"/>.
        /// </summary>
        void SetMaximum(UInt64 Maximum);
        /// <summary>
        /// Sets the current value for a progress indicator to <paramref name="Progress"/>.
        /// </summary>
        void SetCurrent(UInt64 Progress);
    }
}
