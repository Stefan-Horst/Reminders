using System;
using Reminders.util;

namespace Reminders
{
    public class Reminder
    {
        private string content;
        private string dateString;
        private DateTime date;
        private string repeat; //interval in days,user can enter w or y which gets converted to amount of days, problem with months and leap years, maybe use -1 and -2 to handle special cases
        private bool read; //false by default, true when user marks reminder as read
        private int id;
        
        private ConverterFormatter converter = new ConverterFormatter();

        public Reminder(string dateString, string repeat, bool read, string content)
        {
            this.dateString = dateString;
            this.repeat = repeat;
            this.read = read;
            this.content = content;
            id = -1; //default id, must be set to unique id

            date = converter.ConvertStringToDateTime(dateString); //todo check for right format of string
        }

        public Reminder(string dateString, string repeat, string content)
            : this(dateString, repeat, false, content) { }

        /*public void ConvertStringToDate()
        {
            //date format in string has to be DDMMYYYYhhmm
            int day = int.Parse(dateString[..2]);
            int month = int.Parse(dateString.Substring(2, 2));
            int year = int.Parse(dateString.Substring(4, 4));
            int hour = int.Parse(dateString.Substring(8, 2));
            int minute = int.Parse(dateString.Substring(10, 2));

            date = new DateTime(year, month, day, hour, minute, 0);
        }*/

        private void ConvertDateToString()
        {
            dateString = date.ToString("ddMMyyyyhhmm");
        }

        public override string ToString()
        {
            return id + ": " + date.ToShortDateString() + " " + date.ToShortTimeString() + " " + repeat + " " + content;
        }

        public string Content { get => content; set => content = value; }
        public string DateString { get => dateString; set { dateString = value; date = converter.ConvertStringToDate(value); } }
        public string Repeat { get => repeat; set => repeat = value; }
        public bool Read { get => read; set => read = value; }
        public DateTime Date { get => date; set { date = value; ConvertDateToString(); } }
        public int Id { get => id; set => id = value; }
    }
}
