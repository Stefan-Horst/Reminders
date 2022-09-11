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
        
        public Reminder(string dateString, string repeat, bool read, string content)
        {
            this.dateString = dateString;
            this.repeat = repeat;
            this.read = read;
            this.content = content;
            id = -1; //default id, must be set to unique id

            date = ConverterFormatter.ConvertStringToDateTime(dateString);
        }

        public Reminder(string dateString, string repeat, string content)
            : this(dateString, repeat, false, content) { }

        private void ConvertDateToString()
        {
            dateString = date.ToString("ddMMyyyyHHmm");
        }

        public override string ToString()
        {
            return id + ": " + date.ToShortDateString() + " " + date.ToShortTimeString() + " " + repeat + " " + content;
        }

        public string Content { get => content; set => content = value; }
        public string DateString { get => dateString; set { dateString = value; date = ConverterFormatter.ConvertStringToDateTime(value); } }
        public string Repeat { get => repeat; set => repeat = value; }
        public bool Read { get => read; set => read = value; }
        public DateTime Date { get => date; set { date = value; ConvertDateToString(); } }
        public int Id { get => id; set => id = value; }
    }
}
