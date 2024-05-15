using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SimTableApplication.PLCSIM_Advanced.Utils
{
    public static class FormatHelper
    {
        public static object ConvertValue(SimDataType dataType, string value)
        {
            object returnValue = value;
            bool success;

            if (!string.IsNullOrEmpty(value))
            {
                switch (dataType)
                {
                    case SimDataType.Unknown:
                        break;
                    case SimDataType.Bool:
                        returnValue = StringToBoolean(value);
                        break;
                    case SimDataType.Real:
                        float f;
                        success = float.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out f);

                        if (success)
                        {
                            returnValue = f;
                            break;
                        }
                        else
                        {
                            //if tryparse failed return 0 as replacement value
                            returnValue = 0;
                            break;
                        }


                    case SimDataType.LReal:
                        double d;
                        success = double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out d);
                        if (success)
                        {
                            returnValue = d;
                            break;
                        }
                        else
                        {
                            //if tryparse failed return 0.0 as replacement value
                            returnValue = 0;
                            break;
                        }

                    case SimDataType.Word:
                        returnValue = WordStringToUInt16(value);
                        break;
                    case SimDataType.DWord:
                        returnValue = DWordStringToUInt32(value);
                        break;
                    case SimDataType.LWord:
                        returnValue = LWordStringToUInt64(value);
                        break;
                    case SimDataType.Byte:
                        returnValue = ByteStringToUInt8(value);
                        break;
                    case SimDataType.Time:
                        returnValue = TimeStringToInt32(value);
                        break;
                    case SimDataType.LTime:
                        returnValue = LTimeStringToInt64(value);
                        break;
                    case SimDataType.DateAndTime:
                        returnValue = DateAndTimeStringToUInt64(value);
                        break;
                    case SimDataType.Date:
                        returnValue = DateStringToUInt16(value);
                        break;
                    case SimDataType.SInt:
                        returnValue = StringToUInt8(value);
                        break;
                    case SimDataType.USInt:
                        returnValue = StringToByte(value);
                        break;
                    case SimDataType.Int:
                        returnValue = StringToInt16(value);
                        break;
                    case SimDataType.UInt:
                        returnValue = StringToUInt16(value);
                        break;
                    case SimDataType.DInt:
                        returnValue = StringToInt32(value);
                        break;
                    case SimDataType.UDInt:
                        returnValue = StringToUInt32(value);
                        break;
                    case SimDataType.LInt:
                        returnValue = StringToInt64(value);
                        break;
                    case SimDataType.ULInt:
                        returnValue = StringToUInt64(value);
                        break;
                    case SimDataType.Hw_Any:
                    case SimDataType.Hw_IoSystem:
                    case SimDataType.Hw_DpMaster:
                    case SimDataType.Hw_DpSlave:
                    case SimDataType.Hw_Io:
                    case SimDataType.Hw_Module:
                    case SimDataType.Hw_SubModule:
                    case SimDataType.Hw_Hsc:
                    case SimDataType.Hw_Pwm:
                    case SimDataType.Hw_Pto:
                    case SimDataType.Hw_Interface:
                    case SimDataType.Hw_IEPort:
                    case SimDataType.Hw_Device:
                        returnValue = StringToUInt16(value);
                        break;
                    case SimDataType.WChar:
                        returnValue = StringToChar(value);
                        break;
                    case SimDataType.Char:
                        returnValue = StringToSbyte(value);
                        break;
                    case SimDataType.LDT:
                        returnValue = StringToLDT(value);
                        break;
                    case SimDataType.TimeOfDay:
                        returnValue = StringToTod(value);
                        break;
                    case SimDataType.LTimeOfDay:
                        returnValue = StringToLTod(value);
                        break;
                    default:
                        return returnValue;

                }
            }

            return returnValue;
        }

        private static byte StringToByte(string value)
        {
            try
            {
                return byte.Parse(value, CultureInfo.InvariantCulture);
            }
            catch (OverflowException)
            {
                //if OverflowException is thrown return Max value of byte                
                return byte.MaxValue;
            }

        }
        private static bool StringToBoolean(string str, bool bDefault = false)
        {
            string[] booleanStringOff = { "0", "off", "no" };

            if (string.IsNullOrEmpty(str))
                return bDefault;
            else if (booleanStringOff.Contains(str, StringComparer.OrdinalIgnoreCase))
                return false;

            bool result;
            if (!bool.TryParse(str, out result))
                result = true;

            return result;
        }
        private static sbyte StringToSbyte(string value)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(value);
                return Convert.ToSByte(bytes[0]);
            }
            catch (OverflowException)
            {
                //if OverflowException is thrown return Max value 
                return sbyte.MaxValue;
            }

        }
        private static char StringToChar(string value)
        {
            try
            {
                byte[] bytes = Encoding.UTF32.GetBytes(value);
                return Convert.ToChar(bytes[0]);
            }
            catch (OverflowException)
            {
                //if OverflowException is thrown return Max value 
                return char.MaxValue;
            }

        }
        private static sbyte StringToUInt8(string value)
        {
            try
            {
                return sbyte.Parse(value, CultureInfo.InvariantCulture);
            }
            catch (OverflowException)
            {
                //if OverflowException is thrown return Max value 
                return sbyte.MaxValue;
            }

        }
        private static ushort StringToUInt16(string value)
        {
            try
            {
                return ushort.Parse(value, CultureInfo.InvariantCulture);
            }

            catch (OverflowException)
            {
                //if OverflowException is thrown return Max value 
                return ushort.MaxValue;
            }

        }
        private static uint StringToUInt32(string value)
        {
            try
            {
                return uint.Parse(value, CultureInfo.InvariantCulture);
            }

            catch (OverflowException)
            {
                //if OverflowException is thrown return Max value 
                return uint.MaxValue;
            }

        }
        private static ulong StringToUInt64(string value)
        {
            try
            {
                return ulong.Parse(value, CultureInfo.InvariantCulture);
            }

            catch (OverflowException)
            {
                //if OverflowException is thrown return Max value 
                return ulong.MaxValue;
            }

        }
        private static short StringToInt16(string value)
        {
            try
            {
                return short.Parse(value, CultureInfo.InvariantCulture);
            }

            catch (OverflowException)
            {
                //if OverflowException is thrown return Max value 
                return short.MaxValue;
            }

        }
        private static int StringToInt32(string value)
        {
            try
            {
                return int.Parse(value, CultureInfo.InvariantCulture);
            }

            catch (OverflowException)
            {
                //if OverflowException is thrown return Max value 
                return int.MaxValue;
            }

        }
        private static long StringToInt64(string value)
        {
            try
            {
                return long.Parse(value, CultureInfo.InvariantCulture);
            }

            catch (OverflowException)
            {
                //if OverflowException is thrown return Max value 
                return long.MaxValue;
            }

        }
        private static byte ByteStringToUInt8(string strValue)
        {
            try
            {
                byte result;
                if (strValue.Contains("#"))
                {
                    // Delete 16# from Hex
                    string truncatedHex = strValue.Substring(strValue.IndexOf('#') + 1);
                    result = byte.Parse(truncatedHex, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                }
                else
                {
                    result = byte.Parse(strValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                }

                return result;
            }

            catch (OverflowException)
            {
                //if OverflowException is thrown return Max value 
                return byte.MaxValue;
            }


        }        
        private static ushort WordStringToUInt16(string strValue)
        {
            try
            {
                ushort result;
                if (strValue.Contains("#"))
                {
                    // Delete 16# from Hex
                    string truncatedHex = strValue.Substring(strValue.IndexOf('#') + 1);
                    result = ushort.Parse(truncatedHex, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                }
                else
                {
                    result = ushort.Parse(strValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                }

                return result;
            }

            catch (OverflowException)
            {
                //if OverflowException is thrown return Max value 
                return ushort.MaxValue;
            }
        }
        private static uint DWordStringToUInt32(string strValue)
        {
            try
            {
                uint result;
                if (strValue.Contains("#"))
                {
                    // Delete 16# from Hex
                    string truncatedHex = strValue.Substring(strValue.IndexOf('#') + 1);
                    result = uint.Parse(truncatedHex, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                }
                else
                {
                    result = uint.Parse(strValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                }

                return result;
            }

            catch (OverflowException)
            {
                //if OverflowException is thrown return Max value 
                return uint.MaxValue;
            }

        }
        private static ulong LWordStringToUInt64(string strValue)
        {
            try
            {
                ulong result;
                if (strValue.Contains("#"))
                {
                    // Delete 16# from Hex
                    string truncatedHex = strValue.Substring(strValue.IndexOf('#') + 1);
                    result = ulong.Parse(truncatedHex, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                }
                else
                {
                    result = ulong.Parse(strValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                }

                return result;
            }

            catch (OverflowException)
            {
                //if OverflowException is thrown return Max value 
                return ulong.MaxValue;
            }

        }
        private static int TimeStringToInt32(string strValue)
        {
            int result = 0;

            Regex test = new Regex(@"(?:[tT]#)?([\d\.,]+)(ms|s|h|m|d)", RegexOptions.IgnoreCase);

            try
            {
                Match m = test.Match(strValue);
                int tmp;

                switch (m.Groups[2].Value.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "MS":
                        result = int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
                        break;
                    case "S":
                        tmp = int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
                        result = tmp * 1000; //1s = 1000ms
                        break;
                    case "M":
                        tmp = int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
                        result = tmp * 6000; //1min = 6000ms
                        break;
                    case "H":
                        tmp = int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
                        result = tmp * 3600000; //1min = 3600000ms
                        break;
                    case "D":
                        tmp = int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
                        result = tmp * 86400000; //1min = 86400000ms
                        break;
                }

                return result;
            }
            catch (Exception)
            {

                throw;
            }


        }
        private static long LTimeStringToInt64(string strValue)
        {
            long result = 0;

            Regex test = new Regex(@"(?:[LT]#)?([\d\.,]+)(ns|ms|s|h|m|d)", RegexOptions.IgnoreCase);

            try
            {
                Match m = test.Match(strValue);
                long tmp;

                switch (m.Groups[2].Value.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "NS":
                        result = long.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
                        break;
                    case "MS":
                        tmp = long.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
                        result = tmp * 1000000; //1ms = 1000000 ns
                        break;
                    case "S":
                        tmp = long.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
                        result = tmp * 1000000000; //1s = 1000000000 ns
                        break;
                    case "M":
                        tmp = long.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
                        result = tmp * 60000000000; //1min = 60000000000 ns
                        break;
                    case "H":
                        tmp = long.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
                        result = tmp * 3600000000000; //1 hour = 3600000000000 ns
                        break;
                    case "D":
                        tmp = long.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
                        result = tmp * 86400000000000; //1day = 86400000000000 ns
                        break;
                }

                return result;
            }
            catch (Exception)
            {

                throw;
            }


        }
        private static ushort DateStringToUInt16(string strValue)
        {
            DateTime result;

            DateTime newDateTime = new DateTime(1990, 1, 1);

            Regex regex = new Regex(@"(?:D#)?([\d\.,:\- ]+)", RegexOptions.IgnoreCase);

            try
            {
                Match m = regex.Match(strValue);

                string strDateTime = m.Groups[1].Value;
                result = DateTime.ParseExact(strDateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                var tmp = result.Subtract(newDateTime);

                return (ushort)tmp.TotalDays;

            }

            catch (OverflowException)
            {
                //if OverflowException is thrown return Max value 
                return ushort.MaxValue;
            }

        }
        private static ulong DateAndTimeStringToUInt64(string strValue)
        {
            DateTime result;
            Regex regex = new Regex(@"(?:DT#)?([\d\.,:\- ]+)", RegexOptions.IgnoreCase);

            try
            {
                Match m = regex.Match(strValue);

                string strDateTime = m.Groups[1].Value;
                result = DateTime.ParseExact(strDateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                //Convert DateTime to BCD 
                byte[] bytes = ConvertDateTimeToBCD(result);
                //Convert BCD to ulong
                return BitConverter.ToUInt64(bytes, 0);
            }

            catch (OverflowException)
            {
                //if OverflowException is thrown return Max value 
                return ulong.MaxValue;
            }

        }
        private static byte[] ConvertDateTimeToBCD(DateTime dateTime)
        {
            List<byte> bytes = new List<byte>();

            //get the last two digits from the year depending >=2000 or 1900
            var year = (dateTime.Year >= 2000) ? dateTime.Year - 2000 : dateTime.Year - 1900;

            var s = string.Format(CultureInfo.InvariantCulture, "{0:00}{1:00}{2:00}{3:00}{4:00}{5:00}{6:000}", year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);

            //shift within the string
            for (int i = 0; i < s.Length - 1; i += 2)
            {
                bytes.Add((byte)((s[i] - '0') << 4 | (s[i + 1] - '0')));
            }

            //add byte for Day of the week
            bytes.Add((byte)((s[s.Length - 1] - '0') << 4 | ((byte)dateTime.DayOfWeek)));

            return bytes.ToArray();

        }
        private static long StringToLDT(string value)
        {
            try
            {
                var date1 = DateTime.ParseExact(value, "LDT#yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

                return date1.Subtract(new DateTime(1970, 1, 1)).Ticks * 100;
            }
            catch (OverflowException)
            {

                return long.MaxValue;
            }

        }
        private static uint StringToTod(string value)
        {
            try
            {
                var date = DateTime.ParseExact(value, "TOD#HH:mm:ss.fff", CultureInfo.InvariantCulture);

                var result = (UInt32)(date.Subtract(DateTime.Today).Ticks / 10000);

                return result;

            }
            catch (OverflowException)
            {

                return uint.MinValue;
            }
        }
        private static ulong StringToLTod(string value)
        {
            try
            {
                var date = DateTime.ParseExact(value, "LTOD#HH:mm:ss.fff", CultureInfo.InvariantCulture);

                var result = (ulong)(date.Subtract(DateTime.Today).Ticks * 100);

                return result;

            }
            catch (OverflowException)
            {

                return uint.MinValue;
            }
        }

    }
}
