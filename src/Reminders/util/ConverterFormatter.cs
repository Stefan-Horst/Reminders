﻿using System;
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

        // cutoff adds the timespan to just the date by removing the time i.e. setting it to 0
        public static DateTime AddTimespanToDateTime(int time, string unit, DateTime datetime, bool cutoff = false)
        {
            if (time > 0)
            {
                unit = unit.Replace("s", "");
                
                if (unit == "minute")
                {
                    return datetime.AddMinutes(time);
                }
                if (unit == "hour")
                {
                    return datetime.AddHours(time);
                }
                if (unit == "day")
                {
                    return cutoff ? datetime.Date.AddDays(time) : datetime.AddDays(time);
                }
                if (unit == "week")
                {
                    return cutoff ? datetime.Date.AddDays(time * 7) : datetime.AddDays(time * 7);
                }
                if (unit == "month")
                {
                    return cutoff ? datetime.Date.AddMonths(time) : datetime.AddMonths(time);
                }
                if (unit == "year")
                {
                    return cutoff ? datetime.Date.AddYears(time) : datetime.AddYears(time);
                }
            }
            
            return new DateTime();
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
            //date format in string has to be ddMMyyyyHHmm
            int day = int.Parse(dateString[..2]);
            int month = int.Parse(dateString.Substring(2, 2));
            int year = int.Parse(dateString.Substring(4, 4));
            int hour = int.Parse(dateString.Substring(8, 2));
            int minute = int.Parse(dateString.Substring(10, 2));

            return new DateTime(year, month, day, hour, minute, 0);
        }
    }
}
