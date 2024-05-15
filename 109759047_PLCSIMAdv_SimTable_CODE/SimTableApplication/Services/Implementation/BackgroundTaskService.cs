using SimTableApplication.Extensions;
using SimTableApplication.Services.Interfaces;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace SimTableApplication.Services.Implementation
{
    /// <summary>
    ///     See <see cref="IBackgroundTaskService" />.
    /// </summary>
    public class BackgroundTaskService : IBackgroundTaskService , IDisposable 
    {
        #region Private Members

        private readonly TaskScheduler _taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        

        #endregion

        #region IBackgroundTasksService Members

        /// <summary>
        ///     See <see cref="IBackgroundTaskService.PerformInBackground{TResult}" />.
        /// </summary>
        public IBackgroundTask PerformInBackground<TResult>(Func<IBackgroundTask, object, TResult> taskHandler,
            Action<IFinishedBackgroundTask<TResult>> callbackHandler, object taskData)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var backgroundTask = new BackgroundTask(cancellationTokenSource, _taskScheduler);

            Task<TResult>.Factory.StartNew(data => taskHandler(backgroundTask, data), taskData, cancellationTokenSource.Token).ContinueWith(
                antecendent =>
                {
                    // Call the handler after finishing the background task.

                    var result = default(TResult);

                    // If a background task throws an exception, the result may be not available. Accessing this result would throw another exception.
                    try
                    {
                        if (!antecendent.IsFaulted)
                        {
                            result = antecendent.Result;
                        }
                    }
                    finally
                    {
                        // Attention: antecendent.Exception is of type AggregateException
                        callbackHandler(new FinishedBackgroundTask<TResult>(result, cancellationTokenSource.IsCancellationRequested, antecendent.Exception));
                    }

                }, CancellationToken.None, TaskContinuationOptions.None, _taskScheduler).ContinueWith(
                antecendent => backgroundTask.IsRunning = false, CancellationToken.None, TaskContinuationOptions.None,
                _taskScheduler);

            return backgroundTask;
        }

        #region Nested Private Classes

        /// <summary>
        ///     See <see cref="IBackgroundTask" />.
        /// </summary>
        private class BackgroundTask : IBackgroundTask
        {
            #region Constructor

            public BackgroundTask(CancellationTokenSource cancellationTokenSource, TaskScheduler taskScheduler)
            {
                _cancellationTokenSource = cancellationTokenSource;
                _taskScheduler = taskScheduler;
            }

            #endregion

            #region INotifyPropertyChanged Members

            /// <summary>
            ///     See <see cref="INotifyPropertyChanged.PropertyChanged" />.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

            #region Private Members

            private readonly CancellationTokenSource _cancellationTokenSource;
            private readonly TaskScheduler _taskScheduler;
            private int _progress;
            private bool _isRunning = true;

            #endregion

            #region IBackgroundTask Members

            /// <summary>
            ///     See <see cref="IBackgroundTask.IsRunning" />.
            /// </summary>
            public bool IsRunning
            {
                get { return _isRunning; }
                internal set { PropertyChanged.ChangeAndNotify(ref _isRunning, value, this, "IsRunning"); }
            }

            /// <summary>
            ///     See <see cref="IBackgroundTask.RequestCancel" />.
            /// </summary>
            public void RequestCancel()
            {
                if (!_cancellationTokenSource.Token.CanBeCanceled)
                    return;

                _cancellationTokenSource.Cancel();
                PropertyChanged.Notify("Canceled");
            }

            /// <summary>
            ///     See <see cref="IBackgroundTask.Canceled" />.
            /// </summary>
            public bool Canceled => _cancellationTokenSource.IsCancellationRequested;

            /// <summary>
            ///     See <see cref="IBackgroundTask.ReportProgress" />.
            /// </summary>
            public void ReportProgress(int progress)
            {
                var reportProgressAction = new Action(() => Progress = progress);
                Task.Factory.StartNew(reportProgressAction, CancellationToken.None, TaskCreationOptions.None,
                    _taskScheduler);
            }

            /// <summary>
            ///     See <see cref="IBackgroundTask.Progress" />.
            /// </summary>
            public int Progress
            {
                get { return _progress; }
                private set { PropertyChanged.ChangeAndNotify(ref _progress, value, this, "Progress"); }
            }

            #endregion
        }

        /// <summary>
        ///     See <see cref="IFinishedBackgroundTask{TResult}" />.
        /// </summary>
        private class FinishedBackgroundTask<TResult> : IFinishedBackgroundTask<TResult>
        {
            #region Constructor

            public FinishedBackgroundTask(TResult result, bool isCancelled, Exception exception)
            {
                Result = result;
                Canceled = isCancelled;
                Exception = exception;
            }

            #endregion

            #region IFinishedBackgroundTask Members

            public TResult Result { get; }

            public bool Canceled { get; }

            public Exception Exception { get; }

            #endregion
        }

        #endregion

        #endregion

        #region IDisposable Members
        public void Dispose()
        {

        }
        #endregion
    }
}
