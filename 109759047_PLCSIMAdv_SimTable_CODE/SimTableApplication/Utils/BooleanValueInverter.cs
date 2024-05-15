using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SimTableApplication.Utils
{
    /// <summary>
    /// The BooleanValueInverter is a converter that inverts a boolean value.
    /// </summary>
    public class BooleanValueInverter : IValueConverter
    {
        /// <summary>
        /// Convert a boolean value to its inverted value
        /// </summary>
        /// <param name="value">Boolean</param>
        /// <param name="targetType">Target type for parameter</param>
        /// <param name="parameter">Optional second converter</param>
        /// <param name="culture">Culture info</param>
        /// <returns>Converted value</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var converter = parameter as IValueConverter;
            var boolValue = value as bool?;

            if (converter == null)
            {
                // No second converter is given as parameter: Just invert and return, if boolean input value was provided
                if (boolValue != null)
                {
                    return !boolValue.Value;
                }

                // Fallback for non-boolean input values
                return DependencyProperty.UnsetValue;
            }

            if (boolValue != null)
            {
                // If boolean input value was provided, invert and then convert
                return converter.Convert(!boolValue.Value, targetType, null, culture);
            }

            // If input value is not boolean, convert and then invert boolean result
            var convertedValue = converter.Convert(value, targetType, null, culture) as bool?;

            if (convertedValue != null)
            {
                return !convertedValue.Value;
            }

            // Fallback for non-boolean return values
            return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// Convert value back (not implemented)
        /// </summary>
        /// <param name="value">n/a</param>
        /// <param name="targetType">>n/a</param>
        /// <param name="parameter">>n/a</param>
        /// <param name="culture">>n/a</param>
        /// <returns>>n/a</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
