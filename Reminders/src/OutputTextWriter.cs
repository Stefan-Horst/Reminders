using System;
using System.Collections.Generic;
using System.Text;

namespace Reminders.src
{
    class OutputTextWriter //call class "outputtext"?
    {
        private IOutputWriter outputWriter;
        
        public OutputTextWriter (IOutputWriter outputWriter)
        {
            this.outputWriter = outputWriter;
        }
        
        public void ShowWelcome() //todo split into two first one ónly logo
        {
            // ASCII text from https://patorjk.com/software/taag/#p=display&h=3&v=0&f=Standard
            Console.WriteLine("=======================================================    " + Environment.NewLine +
                              "=   ____                _           _                 =    " + Environment.NewLine +
                              "=  |  _ \\ ___ _ __ ___ (_)_ __   __| | ___ _ __ ___   =   " + Environment.NewLine +
                              "=  | |_) / _ | '_ ` _ \\| | '_ \\ / _` |/ _ | '__/ __|  =  " + Environment.NewLine +
                              "=  |  _ |  __| | | | | | | | | | (_| |  __| |  \\__ \\  =  " + Environment.NewLine +
                              "=  |_| \\_\\___|_| |_| |_|_|_| |_|\\__,_|\\___|_|  |___/  =" + Environment.NewLine +
                              "=                                                     =    " + Environment.NewLine +
                              "=======================================================    " + Environment.NewLine);

            Console.WriteLine("Welcome to Reminders! Today's date is: " + DateTime.Today.ToShortDateString());
        }

        public void ShowWelcomeReminders(int days, string reminders)
        {
            Console.WriteLine("Here is everything for the next {0} days:", FormatTime(days)); //todo alt text for 0 or 1 day
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
        public void ShowUpcomingReminders(int days, string reminders)
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

        public void ShowLog(int type) //use outputwriter here
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
        public void ShowError(int type, string info) //use outputwriter here
        {
            Console.WriteLine("ERROR");
            switch (type)
            {
                case 0:
                    Console.WriteLine("");
                    break;
                case 2:
                    Console.WriteLine("error" + info);
                    break;
                default:
                    return; //add error message but only for development mode not the user in general
            }
        }

        public void ShowHelp()
        {
            Console.WriteLine("todo help");
        }

        // converts amount of days in weeks or months if possible
        private string FormatTime(int days)
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

        private string GetUpcomingRemindersRaw(string reminders) //reminder as string or array, should already be correct amount for time, this method is only for formatting
        {

            return "";
        }
    }
}
