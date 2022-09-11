using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reminders.util;

namespace Reminders
{
    //taking the commands from the cmd and delegating logic to remindermanager and output to outputtextwriter
    public class CommandExecutor
    {
        private OutputTextWriter writer;
        private ReminderManager reminderMgr;

        private string[] tokens; //current user input tokenized

        private Validator validator = new Validator();
        
        public CommandExecutor(OutputTextWriter outputTextWriter, ReminderManager reminderManager)
        {
            writer = outputTextWriter;
            reminderMgr = reminderManager;
        }

        public void Execute(string input)
        {
            tokens = input.Split(' ');

            switch (tokens[0]) {
                case "help":
                case "h":
                    writer.ShowHelp();
                    break;

                case "commands":
                case "cmds":
                case "cmd":
                    writer.ShowCommands();
                    break;

                case "read": // mark reminder as read
                case "r":
                case "unread":
                case "ur":
                    CmdRead();
                    break;

                case "create":
                case "c":
                    CmdCreate();
                    break;

                case "delete":
                case "del":
                case "d":
                    CmdDelete();
                    break;

                case "update": // set any part of reminder to new value
                case "u":
                    CmdUpdate();
                    break;
                
                case "edit": // edit content of reminder
                case "e":
                    CmdEdit();
                    break;

                case "search": // search for keyword
                case "se":
                    CmdSearch();
                    break;

                case "show": // list reminders
                case "sh":
                case "s":
                    CmdShow();
                    break;

                case "settings":
                case "config":
                case "co":
                    CmdConfig();
                    break;
                
                case "exit":
                    CmdExit();
                    break;

                case "":
                    break;

                default:
                    writer.Log(LogType.Problem, "wrong command");
                    break;
            }
        }

        //command: un/read id
        //command structure: [un]read[/[u/]r] {id}
        private void CmdRead() // mark reminder as read or unread
        {
            try
            {
                if (! int.TryParse(tokens[1], out int id))
                {
                    writer.Log(LogType.Error, "wrong arguments");
                    return;
                }
                
                if (tokens[0] == "read" || tokens[0] == "r")
                    reminderMgr.MarkReminder(id, true);
                else if (tokens[0] == "unread" || tokens[0] == "ur")
                    reminderMgr.MarkReminder(id, false);
                else
                {
                    writer.Log(LogType.Error, "wrong arguments");
                    return;
                }

                writer.ShowReminder(reminderMgr.ReadReminder(id));
            }
            catch (Exception ex)
            {
                writer.Log(LogType.Error, "marking reminder as (un)read failed");
                writer.Log(LogType.ErrorEx, ex.Message);
            }
        }

        //command: create date/timespan time repeat content
        //command structure: create[/c] {dd(.)mm(.)(yy)yy[/{x}min[/h/d/m/y]]} ({hh(:[/.])mm}) ({x}min[/h/d/m/y]) {text}
        private void CmdCreate()
        {
            try
            {
                string date;
                
                if (Validator.IsTimespanValid(tokens[1]))
                {
                    ConverterFormatter.StandardizeTimespan(tokens[1], out int amount, out string unit);

                    DateTime dt = ConverterFormatter.AddTimespanToDateTime(amount, unit, DateTime.Now, true);

                    date = dt.ToString("ddMMyyyyHHmm");
                }
                else if (! validator.IsDateValid(tokens[1], out date))
                {
                    writer.Log(LogType.Error, "wrong date argument");
                    return;
                }

                string repeat = "0";
                int i = 2;
                
                if (validator.IsTimeValid(tokens[i], out string time))
                    i++;
                else
                    time = "0000";
                
                if (Validator.IsTimespanValid(tokens[i]))
                {
                    repeat = ConverterFormatter.StandardizeTimespan(tokens[i], out _, out _);
                    i++;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append(tokens[i]);

                for (int j = i + 1; j < tokens.Length; j++)
                {
                    sb.Append(" " + tokens[j]);
                }
                
                int id = reminderMgr.CreateReminder(date + time, repeat, sb.ToString());
                
                writer.CreateReminder(reminderMgr.ReadReminder(id));
            }
            catch (Exception ex)
            {
                writer.Log(LogType.Error, "creating reminder failed");
                writer.Log(LogType.ErrorEx, ex.Message);
            }
        }
        
        //command: delete id
        //command structure: delete[/del/d] {id}
        private void CmdDelete()
        {
            try
            {
                bool b = int.TryParse(tokens[1], out int id);

                if (!b)
                {
                    writer.Log(LogType.Error, "wrong id format");
                    return;
                }

                Reminder r = reminderMgr.ReadReminder(id);
                reminderMgr.DeleteReminder(id);
                writer.DeleteReminder(r);
            }
            catch (Exception ex)
            {
                writer.Log(LogType.Error, "deleting reminder failed");
                writer.Log(LogType.ErrorEx, ex.Message);
            }
        }

        //command: update id date time repeat content
        //command structure: update[/u] {id} ({dd(.)mm(.)(yy)yy}) ({hh(:[/.])mm}) ({x}min[/h/d/m/y]) ({text})
        private void CmdUpdate()
        {
            try
            {
                Reminder r = reminderMgr.ReadReminder(int.Parse(tokens[1]));
                Reminder rClone = new Reminder(r.DateString, r.Repeat, r.Read, r.Content);
                rClone.Id = r.Id;

                for (int i = 2; i < tokens.Length; i++)
                {
                    string s = tokens[i];
                    
                    if (validator.IsDateValid(s, out string date)) //date
                    {
                        r.DateString = date + r.Date.ToString("HHmm");
                    }
                    else if (validator.IsTimeValid(s, out string time)) //time
                    {
                        r.DateString = r.DateString.Remove(8) + time;
                    }
                    else if (Validator.IsTimespanValid(s)) //repeat
                    {
                        r.Repeat = ConverterFormatter.StandardizeTimespan(s, out _, out _);
                    }
                    else //content
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(tokens[i]);

                        for (int j = i + 1; j < tokens.Length; j++)
                        {
                            sb.Append(" " + tokens[j]);
                        }
                        r.Content = sb.ToString();

                        reminderMgr.UpdateReminder(r.Id, r);

                        writer.UpdateReminder(rClone, r);
                        return;
                    }
                    reminderMgr.UpdateReminder(r.Id, r);
                    
                    writer.UpdateReminder(rClone, r);
                }
            }
            catch (Exception ex)
            {
                writer.Log(LogType.Error, "updating reminder failed");
                writer.Log(LogType.ErrorEx, ex.Message);
            }
        }

        //command: edit id
        //command structure: edit[/e] {id}
        private void CmdEdit()
        {
            try
            {
                if (tokens.Length != 2)
                {
                   writer.Log(LogType.Error, "wrong arguments");
                   return;
                }

                Reminder r = reminderMgr.ReadReminder(int.Parse(tokens[1]));
                Reminder rClone = new Reminder(r.DateString, r.Repeat, r.Read, r.Content);
                rClone.Id = r.Id;
                r.Content = writer.EditReminder(r.Content);
                
                reminderMgr.UpdateReminder(r.Id, r);

                writer.UpdateReminder(rClone , r);
            }
            catch (Exception ex)
            {
                writer.Log(LogType.Error, "editing reminder failed");
                writer.Log(LogType.ErrorEx, ex.Message);
            }
        }

        //command: search term / "term1" "term2" ...
        //command structure: search[/se] {term} ("{term2..n}")
        private void CmdSearch()
        {
            try
            {
                List<string> terms = new List<string>();

                if(tokens.Length > 2)
                {
                    StringBuilder sb = new StringBuilder();

                    for (int i = 1; i < tokens.Length; i++)
                    {
                        if (tokens[i].StartsWith('"')) 
                        { 
                            if (tokens[i].EndsWith('"'))
                            {
                                terms.Add(tokens[i].Remove(tokens[i].Length - 1)[1..]); // remove " from start end end of term
                                continue;
                            }
                            if (string.Join("", tokens[i..]).Contains('"')) // make sure there is a second " in a later token
                            {
                                while (! tokens[i].EndsWith('"')) // exception possible if no second "
                                {
                                    sb.Append(tokens[i]);
                                    i++;
                                }
                                string s = sb.ToString();
                                terms.Add(s.Remove(s.Length - 1)[1..]); // remove " from start end end of term
                            }
                        }
                        else
                        {
                            terms.Add(tokens[i]);
                        }
                    }
                }
                else if (tokens.Length == 2)
                {
                    terms.Add(tokens[1]);
                }
                else
                {
                    writer.Log(LogType.Error, "missing arguments");
                    return;
                }

                List<Reminder> rmdrs = new List<Reminder>();
                foreach (string t in terms)
                {
                    rmdrs.AddRange(reminderMgr.Reminders.FindAll(r => r.ToString().Contains(t)).Except(rmdrs));
                }
                writer.PrintRemindersList(rmdrs);
            }
            catch (Exception ex)
            {
                writer.Log(LogType.Error, "search failed");
                writer.Log(LogType.ErrorEx, ex.Message);
            }
        }

        //command: show unread startdate enddate / date / last timespan / id
        //command structure: show[/s] ([un]read[/[u/]r]) (s{dd(.)mm(.)(yy)yy)}) (e{dd(.)mm(.)(yy)yy)})  /  show[/s] ([un]read[/[u/]r]) {dd(.)mm(.)(yy)yy)}[/today[/t]/tomorrow[/to]/yesterday[/ye](last[/l])/week[/w]/month[/m]/year[/y]/{x}d/{x}w/{x}y/]  /  show[/s] {id}
        private void CmdShow()
        {
            // show full text or only preview in list when multiple reminders are shown?
            try
            {
                if (tokens.Length == 1)
                {
                    writer.PrintRemindersList(reminderMgr.Reminders);
                }
                else
                {
                    int i = 1;

                    int read = 2;
                    if (tokens[1] == "read" || tokens[1] == "r")
                    {
                        read = 1;
                        i++;
                    }
                    else if (tokens[1] == "unread" || tokens[1] == "ur")
                    {
                        read = 0;
                        i++;
                    }

                    if (tokens.Length == 2 && i == 2)
                    {
                        writer.PrintRemindersList(reminderMgr.Reminders.FindAll(r => r.Read == Convert.ToBoolean(read)));
                        return;
                    }

                    string s = tokens[i]; //replace only necessary for dates
                    if (validator.IsDateValid(s, out string date1)) //startdate
                    {
                        if (tokens.Length > i + 1)
                        {
                            string e = tokens[i + 1];
                            if (validator.IsDateValid(e, out string date2)) //enddate
                            {
                                if (read == 2)
                                    writer.PrintRemindersList(reminderMgr.GetRemindersDueInTimespan(ConverterFormatter.ConvertStringToDate(date1), ConverterFormatter.ConvertStringToDate(date2)));
                                else
                                    writer.PrintRemindersList(reminderMgr.GetRemindersDueInTimespan(ConverterFormatter.ConvertStringToDate(date1), ConverterFormatter.ConvertStringToDate(date2)).FindAll(r => r.Read == Convert.ToBoolean(read)));
    
                                return;
                            }
                        }

                        if (read == 2)
                            writer.PrintRemindersList(reminderMgr.GetRemindersDueOnDate(ConverterFormatter.ConvertStringToDate(date1)));
                        else
                            writer.PrintRemindersList(reminderMgr.GetRemindersDueOnDate(ConverterFormatter.ConvertStringToDate(date1)).FindAll(r => r.Read == Convert.ToBoolean(read)));

                        return;
                    }
                    if (int.TryParse(s, out int id))
                    {
                        writer.ShowReminder(reminderMgr.ReadReminder(id));
                        return;
                    }
                    if (s == "today" || s == "t")
                    {
                        if (read == 2)
                            writer.PrintRemindersList(reminderMgr.GetRemindersDueOnDate(DateTime.Today));
                        else
                            writer.PrintRemindersList(reminderMgr.GetRemindersDueOnDate(DateTime.Today).FindAll(r => r.Read == Convert.ToBoolean(read)));

                        return;
                    }
                    if (s == "tomorrow" || s == "to")
                    {
                        if (read == 2)
                            writer.PrintRemindersList(reminderMgr.GetRemindersDueOnDate(DateTime.Today.AddDays(1)));
                        else
                            writer.PrintRemindersList(reminderMgr.GetRemindersDueOnDate(DateTime.Today.AddDays(1)).FindAll(r => r.Read == Convert.ToBoolean(read)));

                        return;
                    }
                    if (s == "yesterday" || s == "ye")
                    {
                        if (read == 2)
                            writer.PrintRemindersList(reminderMgr.GetRemindersDueOnDate(DateTime.Today.AddDays(-1)));
                        else
                            writer.PrintRemindersList(reminderMgr.GetRemindersDueOnDate(DateTime.Today.AddDays(-1)).FindAll(r => r.Read == Convert.ToBoolean(read)));

                        return;
                    }

                    bool last = false;
                    if (s == "last" || s == "l")
                    {
                        last = true;
                        i++;
                    }

                    DateTime date;
                    if (tokens[i] == "week" || tokens[i] == "w")
                    {
                        if (last)
                            date = DateTime.Today.AddDays(-7);
                        else
                            date = DateTime.Today.AddDays(7);
                    }
                    else if (tokens[i] == "month" || tokens[i] == "m")
                    {
                        if (last)
                            date = DateTime.Today.AddMonths(-1);
                        else
                            date = DateTime.Today.AddMonths(1);
                    }
                    else if (tokens[i] == "year" || tokens[i] == "y")
                    {
                        if (last)
                            date = DateTime.Today.AddYears(-1);
                        else
                            date = DateTime.Today.AddYears(1);
                    }
                    else if (Validator.IsTimespanValid(tokens[i])) //timespan
                    {
                        ConverterFormatter.StandardizeTimespan(tokens[i], out int time, out _);

                        if (time != 0)
                        {
                            if (last)
                                date = DateTime.Today.AddMinutes(-ConverterFormatter.ConvertToMinutes(tokens[i], time));
                            else
                                date = DateTime.Today.AddMinutes(ConverterFormatter.ConvertToMinutes(tokens[i], time));
                        }
                        else
                        {
                            date = DateTime.Today;
                        }
                    }
                    else
                    {
                        writer.Log(LogType.Error, "command not valid");
                        return;
                    }

                    if (last)
                    {
                        if (read == 2)
                            writer.PrintRemindersList(reminderMgr.GetRemindersDueInTimespan(date, DateTime.Today));
                        else
                            writer.PrintRemindersList(reminderMgr.GetRemindersDueInTimespan(date, DateTime.Today).FindAll(r => r.Read == Convert.ToBoolean(read)));
                    }
                    else
                    {
                        if (read == 2)
                            writer.PrintRemindersList(reminderMgr.GetRemindersDueInTimespan(DateTime.Today, date));
                        else
                            writer.PrintRemindersList(reminderMgr.GetRemindersDueInTimespan(DateTime.Today, date).FindAll(r => r.Read == Convert.ToBoolean(read)));
                    }
                }
            }
            catch (Exception ex)
            {
                writer.Log(LogType.Error, "showing reminders failed");
                writer.Log(LogType.ErrorEx, ex.Message);
            }
        }

        //command: config param value / config reset
        //command structure: config[/co/settings] {parameter} {value}  /  config[/co/settings] reset
        private void CmdConfig()
        {
            try
            {
                if (tokens.Length == 1)
                {
                    writer.ShowConfig(reminderMgr.FileMgr.DataPath, reminderMgr.FileMgr.Autostart, reminderMgr.FileMgr.UpcomingDays, reminderMgr.FileMgr.Notification, reminderMgr.FileMgr.Quickedit);
                }
                else if (tokens.Length == 2 && tokens[1] == "reset")
                {
                    if (reminderMgr.FileMgr.RestoreConfigToDefault())
                        writer.Log(LogType.Info, "config reset successful");
                    else
                        writer.Log(LogType.Error, "config reset failed");
                }
                else if (tokens.Length == 3)
                {
                    switch (tokens[1])
                    {
                        case "path":
                            reminderMgr.FileMgr.DataPath = tokens[2];
                            if (tokens[2] == "default")
                                writer.EditConfig("path = " + reminderMgr.FileMgr.DataPath);
                            else
                                writer.EditConfig("path = " + tokens[2]);
                            reminderMgr.Init();
                            break;
                        case "autostart":
                            if (Validator.StringEqualsTrue(tokens[2]))
                                reminderMgr.FileMgr.Autostart = true;
                            else if (Validator.StringEqualsFalse(tokens[2]))
                                reminderMgr.FileMgr.Autostart = false;
                            else
                            {
                                writer.Log(LogType.Error, "wrong arguments");
                                return;
                            }
                            writer.EditConfig("autostart = " + reminderMgr.FileMgr.Autostart);
                            break;
                        case "upcomingreminderstime":
                        case "upcomingRemindersTime":
                        case "time":
                            int days = int.Parse(tokens[2]);
                            if (days >= -1)
                            {
                                reminderMgr.FileMgr.UpcomingDays = days; 
                                writer.EditConfig("upcomingRemindersTime = " + days);
                            }
                            else
                                writer.Log(LogType.Error, "wrong arguments");
                            break;
                        case "devmode":
                        case "devMode":
                            if (Validator.StringEqualsTrue(tokens[2]))
                                writer.Devmode = true;
                            else if (Validator.StringEqualsFalse(tokens[2]))
                                writer.Devmode = false;
                            else
                            {
                                writer.Log(LogType.Error, "wrong arguments");
                                return;
                            }
                            writer.EditConfig("devmode = " + writer.Devmode);
                            reminderMgr.FileMgr.SaveConfig();
                            break;
                        case "notification":
                            if (Validator.StringEqualsTrue(tokens[2]))
                                reminderMgr.FileMgr.Notification = true;
                            else if (Validator.StringEqualsFalse(tokens[2]))
                                reminderMgr.FileMgr.Notification = false;
                            else
                            {
                                writer.Log(LogType.Error, "wrong arguments");
                                return;
                            }
                            writer.EditConfig("notification = " + reminderMgr.FileMgr.Notification);
                            break;
                        case "quickedit":
                        case "quickEdit":
                            if (Validator.StringEqualsTrue(tokens[2]))
                                reminderMgr.FileMgr.Quickedit = true;
                            else if (Validator.StringEqualsFalse(tokens[2]))
                                reminderMgr.FileMgr.Quickedit = false;
                            else
                            {
                                writer.Log(LogType.Error, "wrong arguments");
                                return;
                            }
                            writer.EditConfig("quickedit = " + reminderMgr.FileMgr.Quickedit);
                            break;
                        default:
                            writer.Log(LogType.Error, "wrong arguments");
                            return;
                    }
                }
                else
                {
                    writer.Log(LogType.Error, "wrong arguments");
                }
            }
            catch (Exception ex)
            {
                writer.Log(LogType.Error, "changing config failed, wrong id format");
                writer.Log(LogType.ErrorEx, ex.Message);
            }
        }

        //command: exit
        private void CmdExit()
        {
            Environment.Exit(0);
        }
    }
}
