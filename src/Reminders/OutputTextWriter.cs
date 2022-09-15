using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Reminders.util;
using SimultaneousConsoleIO;

namespace Reminders
{
    public enum LogType
    {
        Error,
        ErrorCritical, // error which breaks program
        ErrorEx, // error caused by exception
        Problem,
        Info
    }
    
    public class OutputTextWriter
    {
        private SimulConsoleIO simio;

        private const string ReminderStartText = "\t> ";
        private int reminderStartTotalLength;
        private bool devmode;

        public bool Devmode { get => devmode; set => devmode = value; }
        
        public OutputTextWriter (SimulConsoleIO simio)
        {
            this.simio = simio;
            
            const int tabLength = 7;
            
            reminderStartTotalLength = ReminderStartText.Length;
            
            if (ReminderStartText.Contains("\t"))
                reminderStartTotalLength += tabLength;
        }
        
        public void ShowWelcome()
        {
            // ASCII text from https://patorjk.com/software/taag/#p=display&h=3&v=0&f=Standard
            simio.WriteLine(Environment.NewLine +
                              " =======================================================    " + Environment.NewLine +
                              " =   ____                _           _                 =    " + Environment.NewLine +
                              " =  |  _ \\ ___ _ __ ___ (_)_ __   __| | ___ _ __ ___   =   " + Environment.NewLine +
                              " =  | |_) / _ | '_ ` _ \\| | '_ \\ / _` |/ _ | '__/ __|  =  " + Environment.NewLine +
                              " =  |  _ |  __| | | | | | | | | | (_| |  __| |  \\__ \\  =  " + Environment.NewLine +
                              " =  |_| \\_\\___|_| |_| |_|_|_| |_|\\__,_|\\___|_|  |___/  =" + Environment.NewLine +
                              " =                                                     =    " + Environment.NewLine +
                              " =======================================================    " + Environment.NewLine);

            simio.WriteLine(" Welcome to your Reminders! Today's date is: " + DateTime.Today.ToShortDateString());
        }

        public void ShowWelcomeReminders(int days, List<Reminder> rmdrs)
        {
            if (days == -1)
            {
                simio.WriteLine(" Here are all upcoming reminders:");
                PrintRemindersList(rmdrs);
            }
            else if (days == 0)
            {
                simio.WriteLine(" Here are all reminders for today:");
                PrintRemindersList(rmdrs);
            }
            else if (rmdrs.Count > 0)
            {
                simio.WriteLine(" Here are all reminders for the next " + ConverterFormatter.FormatTime(days) + ":");
                PrintRemindersList(rmdrs);
            }
            else
            {
                simio.WriteLine(" There are no reminders for the next " + ConverterFormatter.FormatTime(days) + "." + Environment.NewLine);
            }
        }

        public void CreateReminder(Reminder r)
        {
            simio.WriteLine(" Reminder created:");
            simio.WriteLine(PrintReminder(r));
        }

        public void DeleteReminder(Reminder r)
        {
            simio.WriteLine(" Reminder deleted:");
            simio.WriteLine(PrintReminder(r));
        }

        public void ShowReminder(Reminder r)
        {
            simio.WriteLine(" Details of reminder:");
            simio.WriteLine(PrintReminder(r));
        }
        
        public void UpdateReminder(Reminder rOld, Reminder rNew)
        {
            simio.WriteLine(" Reminder updated, old details:");
            simio.WriteLine(PrintReminder(rOld));
            simio.WriteLine(" New details:");
            simio.WriteLine(PrintReminder(rNew));
        }
        
        public string EditReminder(string content)
        {
            simio.WriteLine(" Update the content of the reminder below:");
            return simio.ReadLine("", content);
        }
        
        public void ShowHelp()
        {
            simio.WriteLine(" Use the console to create, modify and delete reminders.");
            simio.WriteLine(" The following commands are available:" + Environment.NewLine +
                            " \tcreate \tdate time timespan content" + Environment.NewLine +
                            " \tdelete \tid" + Environment.NewLine +
                            " \tupdate \tid date time timespan content" + Environment.NewLine +
                            " \tedit \tid" + Environment.NewLine +
                            " \tread \tid" + Environment.NewLine +
                            " \tshow \tid / status startdate enddate / date / timespan" + Environment.NewLine +
                            " \tsearch \tterms" + Environment.NewLine +
                            " \tconfig \tsetting / reset" + Environment.NewLine +
                            " \texit");
            simio.WriteLine(" Type \"commands\" for a detailed list of commands and their options and parameters");
        }

        public void ShowCommands()
        {
            simio.WriteLine(" A list of all commands including abbreviations and with parameters:" + Environment.NewLine +
                            " - create[/c] {dd(.)mm(.)(yy)yy} ({hh(:[/.])mm}) ({x}min[/h/d/m/y]) {text}" + Environment.NewLine +
                            " - delete[/del/d] {id}" + Environment.NewLine +
                            " - update[/u] {id} ({dd(.)mm(.)(yy)yy}) ({hh(:[/.])mm}) ({x}min[/h/d/m/y]) ({text})" + Environment.NewLine +
                            " - edit[/e] {id}" + Environment.NewLine +
                            " - read[/r] {id}" + Environment.NewLine +
                            " - show[/s] ([un]read[/[u/]r]) {dd(.)mm(.)(yy)yy)}[/today[/t]/tomorrow[/to]" + Environment.NewLine +
                            "            /yesterday[/ye](last[/l])/week[/w]/month[/m]/year[/y]/{x}d/{x}w/{x}y/]" + Environment.NewLine +
                            "\tshow[/s] ([un]read[/[u/]r]) (s{dd(.)mm(.)(yy)yy)}) (e{dd(.)mm(.)(yy)yy)})" + Environment.NewLine +
                            "\tshow[/s] {id}" + Environment.NewLine +
                            " - search[/se] {term} (\"{term2..n}\")" + Environment.NewLine +
                            " - config[/co/settings] {parameter} {value}" + Environment.NewLine +
                            "\tconfig[/co/settings] reset" + Environment.NewLine +
                            " - exit");
        }
        
        public void ShowConfig(string path, bool autostart, int time, bool notification, bool quickedit)
        {
            simio.WriteLine(" Configurable parameters and their current values:" + Environment.NewLine +
                            "\tpath = " + path + Environment.NewLine +
                            "\tautostart = " + autostart + Environment.NewLine +
                            "\tupcomingRemindersTime = " + time + " (in days)" + Environment.NewLine +
                            "\tdevMode = " + devmode + Environment.NewLine +
                            "\tnotification = " + notification + Environment.NewLine +
                            "\tquickEdit = " + quickedit + Environment.NewLine +
                            " (You can also change these values in the config.txt file.)");
        }

        public void EditConfig(string value)
        {
            simio.WriteLine(" Config updated:");
            simio.WriteLine("\t" + value);
        }

        public string DueReminders(List<Reminder> rmdrs) // cant print here because need to be printed from outputwriter
        {
            string s;

            if (rmdrs.Count == 1)
            {
                s = Environment.NewLine + " ========================= Due Reminder: =========================" + Environment.NewLine;

                s += PrintReminder(rmdrs[0]) + Environment.NewLine;
                
                s += " =================================================================" + Environment.NewLine;
            }
            else
            {
                s = Environment.NewLine + " ========================= Due Reminders: =========================" + Environment.NewLine;

                s += ListReminders(rmdrs);
                
                s += " ==================================================================" + Environment.NewLine;
            }
            
            return s;
        }

        // shows log messages for the different possible errors etc.
        public void Log(LogType type, string info, [CallerFilePath] string filepath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        {
            string logStartText = "### LOG ### ";

            if (devmode)
                logStartText += Path.GetFileNameWithoutExtension(filepath) + "." + memberName + " (line: " + lineNumber + ") ~ ";
            
            switch (type)
            {
                case LogType.Error:
                    simio.WriteLine(logStartText + "[ERROR] " + info);
                    break;
                case LogType.ErrorCritical:
                    simio.WriteLine(logStartText + "[CRITICAL ERROR] " + info);
                    simio.WriteLine(logStartText + "THIS ERROR PREVENTS THE PROGRAM FROM WORKING, PLEASE RESTART OR TROUBLESHOOT");
                    simio.WriteLine(logStartText + "press any button to exit...");
                    simio.ForcePrintQueue();
                    Console.ReadKey();
                    Environment.Exit(-1);
                    break;
                case LogType.ErrorEx:
                    if (devmode)
                        simio.WriteLine(logStartText + "[Exception] " + info);
                    break;
                case LogType.Problem:
                    simio.WriteLine(logStartText + "[Problem] " + info);
                    break;
                case LogType.Info:
                    if (devmode)
                        simio.WriteLine(logStartText + "[Info] " + info);
                    break;
                default:
                    simio.WriteLine(logStartText + "Problem: unspecific log call");
                    return;
            }
        }
        
        // lists multiple reminders in a shortened way so that none breaks line due to its length
        public void PrintRemindersList(List<Reminder> rmdrs)
        {
            string s = ListReminders(rmdrs);
            
            if (s != "")
                simio.Write(s);
        }
        
        private string ListReminders(List<Reminder> rmdrs)
        {
            string output = "";
            
            if (rmdrs.Count > 0)
            {
                int maxRepeatLength = rmdrs.Max(r => r.Repeat.Length);
                int maxIdLength = rmdrs.Max(r => r.Id).ToString().Length;

                string content = "";
                foreach (Reminder r in rmdrs)
                {
                    string read = r.Read == true ? "yes," : "no, ";
                    // equal formatting for every reminder
                    string s = ReminderStartText + "Id: " + r.Id + new string(' ', maxIdLength - r.Id.ToString().Length) + ", " + r.Date.ToShortDateString() + " " + r.Date.ToShortTimeString() + ", Rpt: " + r.Repeat + "," + new string(' ', maxRepeatLength - r.Repeat.Length) + " Rd: " + read + " Cnt: " + r.Content;

                    if (reminderStartTotalLength + content.Length > Console.BufferWidth && !(reminderStartTotalLength + content.Length - r.Content.Length > Console.BufferWidth))
                    {
                        s = s.Remove(Console.BufferWidth - reminderStartTotalLength - 1); // trim content so that each reminder is not longer than one line in console
                        
                        s += "...";
                    }
                    content += s + Environment.NewLine;
                }
                return content;
            }

            return output;
        }
        
        private string PrintReminder(Reminder r)
        {
            string read = r.Read == true ? "yes" : "no";
            string output = ReminderStartText + "Id: " + r.Id + ", Date: " + r.Date.ToShortDateString() + " " + r.Date.ToShortTimeString() + ", Repeat: " + r.Repeat + ", Read: " + read + Environment.NewLine;
            
            const string lineStart = "Content: ";
            string content = new string(' ', reminderStartTotalLength) + lineStart + r.Content;
            
            while (content.Length / Console.BufferWidth > 0)
            {
                output += content.Remove(Console.BufferWidth - 2) + Environment.NewLine;

                content = new string(' ', reminderStartTotalLength + lineStart.Length) + content[(Console.BufferWidth - 2)..];
            }
            output += content;
            
            return output;
        }

        public void FileChange(string oldPath, string newPath)
        {
            simio.WriteLine(" FILE CHANGE: you have changed the data file. " + Environment.NewLine +
                            " Your reminders have been saved, new reminders have been loaded." + Environment.NewLine +
                            " Old path: " + oldPath + Environment.NewLine +
                            " New path: " + newPath);
        }
    }
}
