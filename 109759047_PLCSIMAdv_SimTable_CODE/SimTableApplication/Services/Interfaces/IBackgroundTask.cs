using System.ComponentModel;

namespace SimTableApplication.Services.Interfaces
{
    /// <summary>
    ///     Information about a background task.
    /// </summary>
    public interface IBackgroundTask : INotifyPropertyChanged
    {
        /// <summary>
        ///     Gets a value indicating whether this instance is running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        ///     Determines whether this instance is cancelled.
        /// </summary>
        bool Canceled { get; }

        /// <summary>
        ///     Gets the progress (in percents).
        /// </summary>
        int Progress { get; }

        /// <summary>
        ///     Request the executing task to cancel.
        /// </summary>
        void RequestCancel();

        /// <summary>
        ///     Reports the progress.
        /// </summary>
        void ReportProgress(int progress);
    }
}
