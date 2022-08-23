using System;

namespace Reminders.util
{
    public class Validator
    {
        /*private bool IsDateValid(string date)
        {
            try
            {
                int day = -1;
                int month = -1;
                int year = -1;

                if (date.Length == 8)
                {
                    day = int.Parse(date[..2]);
                    month = int.Parse(date.Substring(2, 2));
                    year = int.Parse(date.Substring(4, 4));
                }
                else if (date.Length == 6)
                {
                    day = int.Parse(date[..2]);
                    month = int.Parse(date.Substring(2, 2));
                    year = int.Parse(DateTime.Now.Year.ToString().Remove(2) + date.Substring(4, 2));
                }
                DateTime dt = new DateTime(year, month, day);

                return true;
            }
            catch
            {
                return false;
            }
        }*/

        public bool IsDateValid(string date, out string normalizedDate)
        {
            normalizedDate = null;

            try
            {
                date = date.Replace(".", "");

                if (date.Length == 6)
                {
                    date = date[..4] + DateTime.Now.Year.ToString().Remove(2) + date.Substring(4, 2);
                }
                else if (date.Length != 8)
                {
                    return false;
                }
                string dateFormat = "ddMMyyyy";

                bool b = DateTime.TryParseExact(date, dateFormat, System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None, out _);

                if (b)
                    normalizedDate = date;

                return b;
            }
            catch
            {
                return false;
            }
        }

        /*private bool IsTimeValid(string time)
        {
            try
            {
                int hour = -1;
                int minute = -1;

                if (time.Length == 4)
                {
                    hour = int.Parse(time[..2]);
                    minute = int.Parse(time.Substring(2, 2));

                    if (hour >= 0 && hour <= 23 && minute >= 0 && minute <= 60)
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }*/

        public bool IsTimeValid(string time, out string normalizedTime)
        {
            normalizedTime = null;

            try
            {
                time = time.Replace(".", "").Replace(":", "");

                if (time.Length == 3)
                {
                    time = "0" + time;
                }
                else if (time.Length != 4)
                {
                    return false;
                }
                string timeFormat = "HHmm";

                bool b = DateTime.TryParseExact(time, timeFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out _);

                if (b)
                    normalizedTime = time;

                return b;
            }
            catch
            {
                return false;
            }
        }

        public bool IsTimespanValid(string timespan, out int time) // also used for repeat
        {
            time = -1;

            if (timespan == "0")
            {
                time = 0;
                return true;
            }

            try
            {
                if (!(timespan.Contains("h") || timespan.Contains("d") || timespan.Contains("w") || timespan.Contains("m") || timespan.Contains("y")))
                    return false;

                bool b = int.TryParse(timespan.Remove(timespan.Length - 1).Replace("mi", ""), out int t);

                if (b && t >= 0) // 0d, 0w, etc. are acceptable timespans which just result in 0
                {
                    time = t;
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
