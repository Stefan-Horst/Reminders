﻿using System;
using System.Collections.Generic;
using SimultaneousConsoleIO;

namespace Reminders
{
    class OutputTextWriter //call class "outputtext"?
    {
        private SimulConsoleIO simio;
        
        public OutputTextWriter (SimulConsoleIO simio)
        {
            this.simio = simio;
        }
        
        public void ShowWelcome() //todo split into two first one only logo
        {
            // ASCII text from https://patorjk.com/software/taag/#p=display&h=3&v=0&f=Standard
            simio.WriteLine("=======================================================    " + Environment.NewLine +
                              "=   ____                _           _                 =    " + Environment.NewLine +
                              "=  |  _ \\ ___ _ __ ___ (_)_ __   __| | ___ _ __ ___   =   " + Environment.NewLine +
                              "=  | |_) / _ | '_ ` _ \\| | '_ \\ / _` |/ _ | '__/ __|  =  " + Environment.NewLine +
                              "=  |  _ |  __| | | | | | | | | | (_| |  __| |  \\__ \\  =  " + Environment.NewLine +
                              "=  |_| \\_\\___|_| |_| |_|_|_| |_|\\__,_|\\___|_|  |___/  =" + Environment.NewLine +
                              "=                                                     =    " + Environment.NewLine +
                              "=======================================================    " + Environment.NewLine);

            simio.WriteLine("Welcome to Reminders! Today's date is: " + DateTime.Today.ToShortDateString());
        }

        public void ShowWelcomeReminders(int days, string reminders)
        {
            simio.WriteLine("Here is everything for the next " + FormatTime(days) + ":");
            simio.WriteLine(GetUpcomingRemindersRaw(reminders));
        }

        public void CreateReminder()
        {
            simio.WriteLine("");

        }

        public void DeleteReminder(Reminder r)
        {
            simio.WriteLine("");

        }

        public void ShowReminder(Reminder r)
        {
            simio.WriteLine(r.ToString());
        }

        // only show upcoming reminders like in the welcome message
        public void ShowUpcomingReminders(int days, string reminders)
        {
            simio.WriteLine("Reminders for the next " + FormatTime(days) + ":");
            simio.WriteLine(GetUpcomingRemindersRaw(reminders));
        }

        public void ListReminders(List<Reminder> reminders)
        {
            foreach (Reminder r in reminders)
            {
                string s = r.ToString();

                if (s.Length > Console.BufferWidth && ! (s.Length - r.Content.Length > Console.BufferWidth))
                    s = s.Remove(Console.BufferWidth); // trim content so that each reminder is not longer than one line in console

                simio.WriteLine(s);
            }
        }

        public void UpdateReminder()
        {
            simio.WriteLine("");

        }

        public void ShowLog(int type) //use outputwriter here
        {
            switch (type)
            {
                case 0:
                    simio.WriteLine("");
                    break;
                case 2:
                    simio.WriteLine("log");
                    break;
                default:
                    return; //add error message but only for development mode not the user in general
            }
        }

        // shows error messages for the different possible errors caused directly by user input
        public void ShowError(int type, string info) //use outputwriter here
        {
            simio.WriteLine("ERROR");
            switch (type)
            {
                case 0:
                    simio.WriteLine(info);
                    break;
                case 2:
                    simio.WriteLine("error" + info);
                    break;
                default:
                    return; //add error message but only for development mode not the user in general
            }
        }

        public void ShowHelp()
        {
            simio.WriteLine("todo help");
        }

        public void ShowCommands()
        {
            simio.WriteLine("todo command list");
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