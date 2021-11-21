using System;
using System.Collections.Generic;
using System.Text;

namespace Reminder.src
{
    class Reminder
    {
        private string content;
        private string dateString;
        private DateTime date;
        private int repeat; //interval in days,user can enter w or y which gets converted to amount of days, problem with months and leap years, maybe use -1 and -2 to handle special cases

        public Reminder(string dateString, int repeat, string content)
        {
            this.dateString = dateString;
            this.repeat = repeat;
            this.content = content;

            ConvertStringToDate();
        }

        public void ConvertStringToDate()
        {
            //date format in string has to be YYYYMMDDhhmm
            int year = int.Parse(dateString.Substring(0, 4));
            int month = int.Parse(dateString.Substring(4, 2));
            int day = int.Parse(dateString.Substring(6, 2));
            int hour = int.Parse(dateString.Substring(8, 2));
            int minute = int.Parse(dateString.Substring(10, 2));

            date = new DateTime(year, month, day, hour, minute, 0); //maybe add for recurring reminders only dd.mm and time is needed
            //DateTime.Today.Year
        }

        public override string ToString()
        {
            string s = date.ToShortDateString();

            if (content != "")
                s += " " + content;

            return s;
        }

        public string Content { get => content; set => content = value; }
        public string DateString { get => dateString; set { dateString = value; ConvertStringToDate(); } }

        public int Repeat { get => repeat; set => repeat = value; }
        public DateTime Date { get => date; set => date = value; }
    }
}
