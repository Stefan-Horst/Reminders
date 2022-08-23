﻿using System;

namespace Reminders.util
{
    public class ConverterFormatter
    {
        public int ConvertToMinutes(string raw, int time)
        {
            if (raw.Contains("min"))
            {
                return time;
            }
            if (raw.Contains("h"))
            {
                return time *= 60;
            }
            if (raw.Contains("d"))
            {
                return time *= 60 * 24;
            }
            if (raw.Contains("w"))
            {
                return time *= 60 * 24 * 7;
            }
            if (raw.Contains("m"))
            {
                return time *= 60 * 24 * 30;
            }
            if (raw.Contains("y"))
            {
                return time *= 60 * 24 * 30 * 12;
            }
            
            return -1;
        }
        
        // converts amount of days in weeks or months if possible
        public string FormatTime(int days)
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

        public DateTime ConvertStringToDate(string dateString)
        {
            int day = int.Parse(dateString[..2]);
            int month = int.Parse(dateString.Substring(2, 2));
            int year = int.Parse(dateString.Substring(4, 4));

            return new DateTime(year, month, day);
        }

        public DateTime ConvertStringToDateTime(string dateString)
        {
            //date format in string has to be DDMMYYYYhhmm
            int day = int.Parse(dateString[..2]);
            int month = int.Parse(dateString.Substring(2, 2));
            int year = int.Parse(dateString.Substring(4, 4));
            int hour = int.Parse(dateString.Substring(8, 2));
            int minute = int.Parse(dateString.Substring(10, 2));

            return new DateTime(year, month, day, hour, minute, 0);
        }
    }
}