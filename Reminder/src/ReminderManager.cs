using System;
using System.Collections.Generic;
using System.Text;

namespace Reminder.src
{
    class ReminderManager
    {
        private OutputTextWriter writer;
        private FileManager fileMgr;

        private int upcomingDays;

        public ReminderManager(OutputTextWriter outputTextWriter)
        {
            writer = outputTextWriter;
            fileMgr = new FileManager(writer);

            upcomingDays = fileMgr.UpcomingDays;
        }

        // checks if any reminders are due
        public void checkReminders()
        {

        }

        public void readReminder()
        {

        }

        public void createReminder()
        {

        }

        public void deleteReminder()
        {

        }

        public int UpcomingDays { get => upcomingDays; set => upcomingDays = value; }
    }
}
