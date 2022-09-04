using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Reminders.util
{
    public static class ConverterFormatter
    {
        public static string StandardizeTimespan(string timespan, out int time, out string unit)
        {
            Regex r = new Regex(@"(\d+)([a-zA-Z]+)");
            Match m = r.Match(timespan);
            
            time = int.Parse(m.Groups[1].Value);
            unit = m.Groups[2].Value;

            if (time == 0)
            {
                unit = "";
                return "0";
            }

            unit = unit.Replace("s", "");
            
            if (unit == "min")
                unit = "minute";
            else if (unit == "h")
                unit = "hour";
            else if (unit == "d")
                unit = "day";
            else if (unit == "w")
                unit = "week";
            else if (unit == "m")
                unit = "month";
            else if (unit == "y")
                unit = "year";
            
            if (time > 1)
                unit += "s";

            return time + unit;
        }
        
        public static int ConvertToMinutes(string raw, int time)
        {
            if (raw.Contains("min"))
            {
                return time;
            }
            if (raw.Contains("h"))
            {
                return time * 60;
            }
            if (raw.Contains("d"))
            {
                return time * 60 * 24;
            }
            if (raw.Contains("w"))
            {
                return time * 60 * 24 * 7;
            }
            if (raw.Contains("m"))
            {
                return time * 60 * 24 * 30;
            }
            if (raw.Contains("y"))
            {
                return time * 60 * 24 * 365;
            }
            
            return -1;
        }
        
        // converts amount of days in weeks or months if possible
        public static string FormatTime(int days)
        {
            string s;
            
            if (days % 7 == 0)
            {
                if (days / 7 > 1)
                    s = days / 7 + " weeks";
                else
                    s = "week";
            }
            // assuming a month has exactly 30 days
            else if (days % 30 == 0)
            {
                if (days / 30 > 1)
                    s = days / 30 + " months";
                else
                    s = "month";
            }
            else
            {
                if (days > 1)
                    s = days + " days";
                else
                    s = "day";
            }

            return s;
        }

        public static DateTime ConvertStringToDate(string dateString)
        {
            int day = int.Parse(dateString[..2]);
            int month = int.Parse(dateString.Substring(2, 2));
            int year = int.Parse(dateString.Substring(4, 4));

            return new DateTime(year, month, day);
        }

        public static DateTime ConvertStringToDateTime(string dateString)
        {
            //date format in string has to be DDMMYYYYhhmm
            int day = int.Parse(dateString[..2]);
            int month = int.Parse(dateString.Substring(2, 2));
            int year = int.Parse(dateString.Substring(4, 4));
            int hour = int.Parse(dateString.Substring(8, 2));
            int minute = int.Parse(dateString.Substring(10, 2));

            return new DateTime(year, month, day, hour, minute, 0);
        }
        
        public static string FormatRemindersFull(List<Reminder> rmdrs, string reminderStartText)
        {
            /*string s = "";

            foreach (Reminder r in rmdrs)
            {
                s += "\t> " + r.ToString() + Environment.NewLine;
            }*/
            string s = reminderStartText + rmdrs[0].ToString();

            for (int i = 1; i < rmdrs.Count; i++)
            {
                s += Environment.NewLine + reminderStartText + rmdrs[i].ToString();
            }

            return s;
        }
    }
}
