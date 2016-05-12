namespace ProcessTools
{
    /// <summary>
    /// Provide an interface to the underlying memory search functionality to report
    /// scope and progress of the search.
    /// </summary>
    public interface IProgressIndicator
    {
        /// <summary>
        /// Sets the maximum for a progress indicator to <paramref name="maximum"/>.
        /// </summary>
        void SetMaximum(ulong maximum);
        /// <summary>
        /// Sets the current value for a progress indicator to <paramref name="progress"/>.
        /// </summary>
        void SetCurrent(ulong progress);
    }
}
