using System;
using System.Collections.Generic;
using System.Linq;

namespace Reminders
{
    public class ReminderManager
    {
        private OutputTextWriter writer;
        private FileManager fileMgr;

        private int upcomingDays;
        private List<Reminder> reminders;

        private int idIterator; //no need for static as ids are assigned during (each) runtime

        public ReminderManager(OutputTextWriter outputTextWriter)
        {
            writer = outputTextWriter;
            fileMgr = new FileManager(writer);

            upcomingDays = fileMgr.UpcomingDays;

            if (fileMgr.Reminders != null)
                reminders = fileMgr.Reminders.ToList();
            else
                reminders = new List<Reminder>();

            idIterator = 0;
            
            foreach (Reminder r in reminders)
            {
                r.Id = idIterator;
                idIterator++;
            }
        }

        // updates date of reminder to next date if repeat is enabled
        public void SetReminderToNextDate(int id)
        {
            Reminder r = ReadReminder(id);

            if (r.Repeat != "0")
            {
                string s = r.Repeat;
                int time = int.Parse(s.Remove(s.Length - 1).Replace("mi", ""));

                if (s.Contains("min"))
                {
                    r.Date = r.Date.AddMinutes(time);
                }
                if (s.Contains("h"))
                {
                    r.Date = r.Date.AddHours(time);
                }
                else if (s.Contains("d"))
                {
                    r.Date = r.Date.AddDays(time);
                }
                else if (s.Contains("m"))
                {
                    r.Date = r.Date.AddMonths(time);
                }
                else if (s.Contains("y"))
                {
                    r.Date = r.Date.AddYears(time);
                }
                else
                {
                    //error
                }
            }
        }

        // marks reminders as read / not read
        public void MarkReminder(int id, bool read)
        {
            ReadReminder(id).Read = read;
        }

        public List<Reminder> GetRemindersDueOnDate(DateTime date)
        {
            return GetRemindersDueInTimespan(date, date);
        }

        public List<Reminder> GetRemindersDueInTimespan(DateTime start, DateTime end)
        {
            List<Reminder> rmndrs = new List<Reminder>();
            
            foreach (Reminder r in reminders)
            {   //first checks if r will be due not after the end date, then checks if r will be due after the start date
                //Console.WriteLine(r.Date.ToLongTimeString()+" "+end.ToLongTimeString());
                //Console.WriteLine(GetRemainingTime(r.Id, end.Date) + " " + GetRemainingTime(r.Id, start.Date));
                if (GetRemainingTime(r.Id, end) <= TimeSpan.Zero && GetRemainingTime(r.Id, start) >= TimeSpan.Zero)
                {
                    rmndrs.Add(r);
                }
            }
            return rmndrs;
        }

        // checks if any reminders are due at a certain date and returns them
        //maybe differentiation between due and overdue reminders necessary
        public List<Reminder> GetDueReminders(DateTime dueDate)
        {
            List<Reminder> rmndrs = new List<Reminder>();

            foreach(Reminder r in reminders)
            {
                if(GetRemainingTime(r.Id, dueDate) <= TimeSpan.Zero)
                {
                    rmndrs.Add(r);
                }
            }
            return rmndrs;
        }

        public TimeSpan GetRemainingTime(int id)
        {
            return GetRemainingTime(id, DateTime.Now);
        }

        //returns positive value for remaining time, 0 for exactly due r and negative value for overdue r
        public TimeSpan GetRemainingTime(int id, DateTime dateToCompare)
        {
            DateTime dt = ReadReminder(id).Date;
            //Console.WriteLine(dt.ToString()+"-"+dateToCompare.ToString()+"="+dt.Subtract(dateToCompare)); always absolute number
            return dt.Subtract(dateToCompare);
        }

        public Reminder ReadReminder(int id)
        {
            int i = reminders.FindIndex(r => r.Id == id);
            
            if (i != -1) //value equals -1 if no index found
            {
                return reminders[i];
            }
            else
            {
                writer.ShowError(0, "readreminder failed");
                throw new InvalidOperationException("No Reminder with ID " + id + " found.");
            }
        }

        public int CreateReminder(string dateString, string repeat, string content)
        {
            Reminder r = new Reminder(dateString, repeat, content);

            /*if(r.Date == null) //maybe add more checks for errors, SHOULD BE NULL OR THROW EXC IN REMINDER?
            {
                Console.WriteLine("ERROR"); //todo use outputwriter
                //return false;
                throw new FormatException("Parameter \"dateString\"(" + dateString + ") has invalid format.");
            }*/

            r.Id = idIterator;
            idIterator++;

            reminders.Add(r);
            fileMgr.Reminders = reminders.ToArray(); //filemgr then updates savefile

            return r.Id;
        }

        public void DeleteReminder(int id)
        {
            int i = reminders.FindIndex(r => r.Id == id);
            
            if(i != -1) //value equals -1 if no index found
            {
                reminders.RemoveAt(i);
                fileMgr.Reminders = reminders.ToArray(); //filemgr then updates savefile
                //return true;
            }
            else
            {
                writer.ShowError(0, "deletereminder failed");
                //return false;
                throw new InvalidOperationException("No Reminder with ID " + id + " found.");
            }
        }

        public void UpdateReminder(int id, Reminder reminder)
        {
            int i = reminders.FindIndex(r => r.Id == id);
            
            if (i != -1) //value equals -1 if no index found
            {
                reminders[i] = reminder;
                fileMgr.Reminders = reminders.ToArray(); //filemgr then updates savefile
                //return true;
            }
            else
            {
                writer.ShowError(0, "updatereminder failed");
                //return false;
                throw new InvalidOperationException("No Reminder with ID " + id + " found.");
            }
        }

        public int UpcomingDays { get => upcomingDays; set => upcomingDays = value; }
        public List<Reminder> Reminders { get => reminders; }
    }
}
