using System.ComponentModel;

namespace SimTableApplication.PLCSIM_Advanced.Utils
{
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        protected void OnPropertyChanged()
        {
            OnPropertyChanged(string.Empty);
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <param name="propertyName">Property name</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

