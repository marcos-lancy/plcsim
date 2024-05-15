using System;

namespace SimTableApplication.Services.Interfaces
{
    /// <summary>
    /// IFinishedBackgroundTask contains information about a finished background task. 
    /// </summary>
    /// <typeparam name="TResult">Type of result object</typeparam>
    public interface IFinishedBackgroundTask<out TResult>
    {
        /// <summary>
        /// Result of task
        /// </summary>
        TResult Result { get; }
        /// <summary>
        /// Is task canceled
        /// </summary>
        bool Canceled { get; }
        /// <summary>
        /// Exception in task
        /// </summary>
        Exception Exception { get; }
    }
}
