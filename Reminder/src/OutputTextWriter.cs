using System;
using System.Collections.Generic;
using System.Text;

namespace Reminder.src
{
    class OutputTextWriter
    {
        public void ShowWelcome() //todo split into two first one ónly logo
        {
            // ASCII text from https://patorjk.com/software/taag/#p=display&h=3&v=0&f=Standard
            Console.WriteLine("==================================================    \n" +
                              "=   ____                _           _            =    \n" +
                              "=  |  _ \\ ___ _ __ ___ (_)_ __   __| | ___ _ __  =   \n" +
                              "=  | |_) / _ | '_ ` _ \\| | '_ \\ / _` |/ _ | '__| =  \n" +
                              "=  |  _ |  __| | | | | | | | | | (_| |  __| |    =    \n" +
                              "=  |_| \\_\\___|_| |_| |_|_|_| |_|\\__,_|\\___|_|    =\n" +
                              "=                                                =    \n" +
                              "==================================================    \n");

            
        }

        public void ShowWelcomeReminders(int days, String reminders)
        {
            Console.WriteLine("Welcome to Reminder! Here is everything for the next {0}:", FormatTime(days));
            Console.WriteLine(GetUpcomingRemindersRaw(reminders));
        }

        public void CreateReminder()
        {
            Console.WriteLine("");

        }

        public void DeleteReminder()
        {
            Console.WriteLine("");

        }

        public void ShowReminder()
        {
            Console.WriteLine("");

        }

        // only show upcoming reminders like in the welcome message
        public void ShowUpcomingReminders(int days, String reminders)
        {
            Console.WriteLine("Reminders for the next {0}:", FormatTime(days));
            Console.WriteLine(GetUpcomingRemindersRaw(reminders));
        }

        public void ShowAllReminders()
        {
            Console.WriteLine("");

        }

        public void UpdateReminder()
        {
            Console.WriteLine("");

        }

        public void ShowLog(int type)
        {
            switch (type)
            {
                case 0:
                    Console.WriteLine("");
                    break;
                case 2:
                    Console.WriteLine("log");
                    break;
                default:
                    return; //add error message but only for development mode not the user in general
            }
        }

        // shows error messages for the different possible errors caused directly by user input
        public void ShowError(int type)
        {
            switch (type)
            {
                case 0:
                    Console.WriteLine("");
                    break;
                case 2:
                    Console.WriteLine("error");
                    break;
                default:
                    return; //add error message but only for development mode not the user in general
            }
        }


        // converts amount of days in weeks or months if possible
        private String FormatTime(int days)
        {
            String s;
            
            if (days % 7 == 0)
            {
                if (days / 7 > 1)
                    s = days / 7 + "weeks";
                else
                    s = "week";
            }
            // assuming a month has exactly 30 days
            else if (days % 30 == 0)
            {
                if (days / 30 > 1)
                    s = days / 30 + "months";
                else
                    s = "month";
            }
            else
            {
                if (days > 1)
                    s = days + "days";
                else
                    s = "days";
            }

            return s;
        }

        private String GetUpcomingRemindersRaw(String reminders) //reminder as string or array, should already be correct amount for time, this method is only for formatting
        {

            return "";
        }
    }
}
