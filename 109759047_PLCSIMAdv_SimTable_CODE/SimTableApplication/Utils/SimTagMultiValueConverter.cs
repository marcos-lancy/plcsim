using SimTableApplication.PLCSIM_Advanced.Utils;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SimTableApplication.Utils
{
    public class SimTagMultiValueConverter : IMultiValueConverter
    {
        
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0]==null || values[1] == null || values[1] == DependencyProperty.UnsetValue))
            {
                SimDataType dataType = (SimDataType) values[0];

                object value = values[1];

                switch (dataType)
                {
                    case SimDataType.Bool:
                        value = string.Format(CultureInfo.InvariantCulture, "{0}", value);
                        break;
                    case SimDataType.Word:
                    case SimDataType.DWord:
                    case SimDataType.LWord:
                    case SimDataType.Byte:
                        value = string.Format(CultureInfo.InvariantCulture, "16#{0:X}", value);
                        break;                    
                    case SimDataType.Time:                    
                        value = string.Format(CultureInfo.InvariantCulture, "T#{0}ms", value);
                        break;
                    case SimDataType.LTime:
                        value = string.Format(CultureInfo.InvariantCulture, "LT#{0}ns", value);
                        break;
                    case SimDataType.DateAndTime:
                        value = string.Format(CultureInfo.InvariantCulture, "DT#{0}", UInt64ToDateTime((ulong) value));
                        break;
                    case SimDataType.Date:                
                        value = string.Format(CultureInfo.InvariantCulture, "D#{0}", UInt16ToDate((ushort) value));
                        break;
                    case SimDataType.Real:                        
                    case SimDataType.LReal:
                        value = string.Format(CultureInfo.InvariantCulture, "{0}", value);
                        break;
                    case SimDataType.Char:
                        value = string.Format(CultureInfo.InvariantCulture, "{0}", System.Convert.ToChar(value).ToString());
                        break;
                    case SimDataType.LDT:
                        value = string.Format(CultureInfo.InvariantCulture, "LDT#{0}", Int64ToDate((Int64)value));
                        break;
                    case SimDataType.TimeOfDay:
                        value = string.Format(CultureInfo.InvariantCulture, "TOD#{0}", Int64ToTod((uint)value));                                             
                        break;                                            
                    case SimDataType.LTimeOfDay:
                        value = string.Format(CultureInfo.InvariantCulture, "LTOD#{0}", Int64ToLTod((ulong)value));
                        break;                    
                    case SimDataType.DTL:
                        break;                  
                                         
                    default:                        
                        return value.ToString();
                        
                }

                return value.ToString();
            }

            return values[1];
        }

        

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static string UInt64ToDateTime(ulong dateAndTime)
        {
            var bytes = BitConverter.GetBytes(dateAndTime);

            var year = ConvertToInt(bytes[0]) + (bytes[0] >= 90 ? 1900 : 2000);
            var month = ConvertToInt(bytes[1]);
            var day = ConvertToInt(bytes[2]);
            var hour = ConvertToInt(bytes[3]);
            var minute = ConvertToInt(bytes[4]);
            var second = ConvertToInt(bytes[5]);           

            return new DateTime(year, month, day, hour, minute, second, 0, DateTimeKind.Utc).ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);
        }

        private static int ConvertToInt(int bcd)
        {
            var result = 0;
            result += 10 * (bcd >> 4);
            result += bcd & 0xf;

            return result;
        }
        
        private static string UInt16ToDate(ushort date)
        {
            DateTime startTime = new DateTime(1990, 1, 1);           

            return (startTime.AddDays(date).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        }

        private static string Int64ToDate(long date)
        {
            DateTime startTime = new DateTime(1970, 1, 1,0,0,0,0);
           
            return (startTime.AddTicks(date/100).ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture));
        }

        private static string Int64ToTod(long tod)
        {
            DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);            
            return (startTime.AddTicks(tod * 10000).ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture));
        }

        private static string Int64ToLTod(ulong tod)
        {
            DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            
            return (startTime.AddTicks((long)tod / 100).ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture));
        }




    }
}
