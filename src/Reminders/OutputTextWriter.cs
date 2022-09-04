using System;
using System.Collections.Generic;
using Reminders.util;
using SimultaneousConsoleIO;

namespace Reminders
{
    public class OutputTextWriter //call class "outputtext"?
    {
        private SimulConsoleIO simio;

        private const string ReminderStartText = "\t> ";
        
        public OutputTextWriter (SimulConsoleIO simio)
        {
            this.simio = simio;
        }
        
        public void ShowWelcome()
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

            simio.WriteLine("Welcome to your Reminders! Today's date is: " + DateTime.Today.ToShortDateString());
        }

        public void ShowWelcomeReminders(int days, List<Reminder> reminders)
        {
            simio.WriteLine("Here are all reminders for the next " + ConverterFormatter.FormatTime(days) + ":");
            simio.WriteLine(ConverterFormatter.FormatRemindersFull(reminders, ReminderStartText));
        }

        public void CreateReminder(Reminder r)
        {
            simio.WriteLine("Reminder created:");
            PrintReminder(r);
        }

        public void DeleteReminder(Reminder r)
        {
            simio.WriteLine("Reminder deleted:");
            PrintReminder(r);
        }

        public void ShowReminder(Reminder r)
        {
            simio.WriteLine("Details of reminder:");
            PrintReminder(r);
        }
        
        public void UpdateReminder(Reminder rOld, Reminder rNew)
        {
            simio.WriteLine("Reminder updated, old details:");
            PrintReminder(rOld);
            simio.WriteLine("New details:");
            PrintReminder(rNew);
        }
        
        public string EditReminder(string content)
        {
            simio.WriteLine("Update the content of the reminder below:");
            return simio.ReadLine("", content);
        }
        
        public void ShowHelp()
        {
            simio.WriteLine("Use the console to create, modify and delete reminders.");
            simio.WriteLine("Type \"commands\" for a detailed list of commands you can use");
        }

        public void ShowCommands()
        {
            simio.WriteLine("A list of all commands including abbreviations and with parameters:" + Environment.NewLine +
                            "- read[/r] {id}" + Environment.NewLine +
                            "- create[/c] {dd(.)mm(.)(yy)yy} ({hh(:[/.])mm}) ({x}min[/h/d/m/y]) {text}" + Environment.NewLine +
                            "- delete[/del/d] {id}" + Environment.NewLine +
                            "- update[/u] {id} ({dd(.)mm(.)(yy)yy}) ({hh(:[/.])mm}) ({x}min[/h/d/m/y]) ({text})" + Environment.NewLine +
                            "- edit[/e] {id}" + Environment.NewLine +
                            "- search[/se] {term} (\"{term2..n}\")" + Environment.NewLine +
                            "- show[/s] ([un]read[/[u/]r]) {dd(.)mm(.)(yy)yy)}[/today[/t]/tomorrow[/to]/yesterday[/ye](last[/l])/week[/w]/month[/m]/year[/y]/{x}d/{x}w/{x}y/]" + Environment.NewLine +
                            "\tshow[/s] ([un]read[/[u/]r]) (s{dd(.)mm(.)(yy)yy)}) (e{dd(.)mm(.)(yy)yy)})" + Environment.NewLine +
                            "- config[/co/settings] {parameter} {value}" + Environment.NewLine +
                            "\tconfig[/co/settings] reset" + Environment.NewLine +
                            "- exit");
        }
        
        public void ShowConfig(string path, bool autostart, int time)
        {
            simio.WriteLine("Configurable parameters and their current values:" + Environment.NewLine +
                            "\tpath = " + path + Environment.NewLine +
                            "\tautostart = " + autostart + Environment.NewLine +
                            "\tupcomingRemindersTime = " + time + " (in days)" + Environment.NewLine +
                            "(You can also change these values in the config.txt file, but a restart of this program will be needed for them to take effect)");
        }

        public void EditConfig(string value)
        {
            simio.WriteLine("Config updated:");
            simio.WriteLine("\t" + value);
        }

        // only show upcoming reminders like in the welcome message
        /*public void ShowUpcomingReminders(int days, string reminders)
        {
            simio.WriteLine("Reminders for the next " + converter.FormatTime(days) + ":");
            simio.WriteLine(GetUpcomingRemindersRaw(reminders));
        }*/

        public string DueReminders(List<Reminder> rmdrs) // cant print here because need to be printed from outputwriter
        {
            string s;

            if (rmdrs.Count == 1)
            {
                s = "========== Due Reminder: ==========" + Environment.NewLine;

                s += ConverterFormatter.FormatRemindersFull(rmdrs, ReminderStartText) + Environment.NewLine;
                
                s += "===================================";
            }
            else
            {
                s = "========== Due Reminders: ==========";

                s += ConverterFormatter.FormatRemindersFull(rmdrs, ReminderStartText) + Environment.NewLine;
                
                s += "====================================";
            }
            
            return s;
        }
        
        // lists multiple reminders in a shortened way so that none breaks line due to its length
        public void ListReminders(List<Reminder> reminders)
        {
            foreach (Reminder r in reminders)
            {
                string s = r.ToString();

                if (s.Length > Console.BufferWidth && ! (s.Length - r.Content.Length > Console.BufferWidth))
                    s = s.Remove(Console.BufferWidth); // trim content so that each reminder is not longer than one line in console

                simio.WriteLine(ReminderStartText + s);
            }
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
        
        private void PrintReminder(Reminder r)
        {
            simio.WriteLine(ReminderStartText + r.ToString());
        }
    }
}
