using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using SimTableApplication.PLCSIM_Advanced.Utils;

namespace SimTableApplication.Utils
{
    class LedToVisibilityComparer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ledValue = value as ControllerLedMode?;
            var text = (string) parameter;
            var target = (ControllerLedMode) Enum.Parse(typeof(ControllerLedMode), text ?? string.Empty);

            return ledValue.HasValue && ledValue.Value == target ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
