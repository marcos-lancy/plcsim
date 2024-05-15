using System;

namespace SimTableApplication.Services.Interfaces
{
    public interface IBackgroundTaskService
    {
        /// <summary>
        ///     Performs a task in the background.
        /// </summary>
        IBackgroundTask PerformInBackground<TResult>(Func<IBackgroundTask, object, TResult> taskHandler,
            Action<IFinishedBackgroundTask<TResult>> callbackHandler,
            object taskData);
    }
}
