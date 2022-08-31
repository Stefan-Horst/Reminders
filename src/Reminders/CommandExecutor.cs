﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reminders.util;

namespace Reminders
{
    //taking the commands from the cmd and delegating logic to remindermanager and output to outputtextwriter
    // problem: remindermanager has some standalone tasks like the welcome message at program start, should it be called from here (not fitting) or separately from main
    public class CommandExecutor
    {
        private OutputTextWriter writer;
        private ReminderManager reminderMgr;

        private string[] tokens; //current user input tokenized

        private ConverterFormatter converter = new ConverterFormatter();
        private Validator validator = new Validator();

        // todo cache reminders from last show etc command with easier access ids

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
                    writer.ShowCommands();
                    break;

                case "read": // show details of reminder e.g. complete content
                case "r":
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

                case "update":
                case "u":
                    CmdUpdate();
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
                    CmdSettings();
                    break;

                case "":
                    break;

                default:
                    writer.ShowError(0, "wrong command");
                    break;
            }
        }

        //command: read id
        //command structure: read[/r] {id}
        private void CmdRead() // show one reminder in full detail
        {
            try
            {
                bool b = int.TryParse(tokens[1], out int id);

                if (!b)
                {
                    writer.ShowError(0, "wrong id format");
                    return;
                }

                writer.ShowReminder(reminderMgr.ReadReminder(id));
            }
            catch (Exception ex)
            {
                writer.ShowError(0, ex.Message);
            }
        }

        //command: create date time repeat content
        //command structure: create[/c] {dd(.)mm(.)(yy)yy} ({hh(:[/.])mm}) ({x}min[/h/d/m/y]) {text}
        private void CmdCreate()
        {
            try
            {
                if (! validator.IsDateValid(tokens[1], out string date))
                {
                    writer.ShowError(0, "wrong date arg");
                    return;
                }

                string repeat = "0";
                int i = 2;
                
                if (validator.IsTimeValid(tokens[i], out string time))
                    i++;
                else
                    time = "0000";
                
                if (validator.IsTimespanValid(tokens[i]/*, out _*/))
                {
                    repeat = converter.StandardizeTimespan(tokens[i], out _, out _);
                    i++;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append(tokens[i]);

                for (int j = i + 1; j < tokens.Length; j++)
                {
                    sb.Append(" " + tokens[j]);
                }

                /*if (tokens.Length == 5)
                {
                    if (! validator.IsTimeValid(tokens[2], out time))
                    {
                        writer.ShowError(0, "wrong time arg");
                        return;
                    }
                    if (! validator.IsTimespanValid(tokens[3], out _))
                    {
                        writer.ShowError(0, "wrong date arg");
                        return;
                    }
                    repeat = tokens[3];
                    text = tokens[4];
                }
                else if (tokens.Length == 4)
                {
                    if (validator.IsTimeValid(tokens[2], out time))
                    {
                        repeat = "0";
                    }
                    else if (validator.IsTimespanValid(tokens[2], out _))
                    {
                        time = "0000";
                        repeat = tokens[2];
                    }
                    else
                    {
                        writer.ShowError(0, "wrong time/timespan args");
                        return;
                    }
                    text = tokens[3];
                }
                else if (tokens.Length == 3)
                {
                    time = "0000";
                    repeat = "0";
                    text = tokens[2];
                }
                else
                {
                    writer.ShowError(0, "missing args");
                    return;
                }*/
                //if (IsDateValid(date, out string date1) && IsTimeValid(time, out string time1) && IsTimespanValid(repeat, out _))
                reminderMgr.CreateReminder(date + time, repeat, sb.ToString());
            }
            catch (Exception ex)
            {
                //Console.WriteLine("SSS");
                writer.ShowError(0, ex.Message);
            }
        }

        //always keep last (list of) reminders with numbers (ids) cached so ids can be referenced in commands

        //command: delete id
        //command structure: delete[/del/d] {id}
        private void CmdDelete()
        {
            try
            {
                bool b = int.TryParse(tokens[1], out int id);

                if (!b)
                {
                    writer.ShowError(0, "wrong id format");
                    return;
                }

                writer.DeleteReminder(reminderMgr.ReadReminder(id));
                reminderMgr.DeleteReminder(id);
            }
            catch (Exception ex)
            {
                writer.ShowError(0, ex.Message);
            }
        }

        //command: update id date time repeat content
        //command structure: update[/u] {id} ({dd(.)mm(.)(yy)yy}) ({hh(:[/.])mm}) ({x}min[/h/d/m/y]) ({text})
        private void CmdUpdate()
        {
            try
            {
                Reminder r = reminderMgr.ReadReminder(int.Parse(tokens[1]));

                for (int i = 2; i < tokens.Length; i++)
                {
                    string s = tokens[i]/*.Replace(".", "").Replace(":", "")*/;

                    //if (int.TryParse(s, out _)) //maybe not needed
                    //{
                    if (validator.IsDateValid(s, out string date)) //date
                    {
                        r.DateString = date + r.Date.ToString("HHmm");
                    }
                    else if (validator.IsTimeValid(s, out string time)) //time
                    {
                        r.DateString = r.DateString.Remove(8) + time;
                    }
                    //}
                    else if (validator.IsTimespanValid(s/*, out _*/)) //repeat
                    {
                        r.Repeat = converter.StandardizeTimespan(s, out _, out _);
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

                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                writer.ShowError(0, ex.Message);
            }
        }

        //comand: search term / "term1" "term2" ...
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
                                terms.Add(tokens[i]);
                                continue;
                            }
                            if (string.Join("", tokens[i..]).Contains('"')) // make sure there is a second " in a later token
                            {
                                while (! tokens[i].EndsWith('"')) // exception possible if no second "
                                {
                                    sb.Append(tokens[i]);
                                    i++;
                                }
                                terms.Add(sb.ToString());
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
                    writer.ShowError(0, "no arguments");
                    return;
                }

                List<Reminder> rmdrs = new List<Reminder>();
                foreach (string t in terms)
                {
                    rmdrs.AddRange(reminderMgr.Reminders.FindAll(r => r.ToString().Contains(t)).Except(rmdrs));
                }
                writer.ListReminders(rmdrs);
            }
            catch (Exception ex)
            {
                writer.ShowError(0, ex.Message);
            }
        }

        //command: show unread startdate enddate / date / last timespan
        //command structure: show[/s] ([un]read[/[u/]r]) (s{dd(.)mm(.)(yy)yy)}) (e{dd(.)mm(.)(yy)yy)})  /  show[/s] ([un]read) {dd(.)mm(.)(yy)yy)}[/today[/t]/tomorrow[/to]/yesterday[/ye](last[/l])/week[/w]/month[/m]/year[/y]/{x}d/{x}w/{x}y/]
        private void CmdShow()
        {
            // show full text or only preview in list when multiple reminders are shown?
            try
            {
                if (tokens.Length == 1)
                {
                    writer.ListReminders(reminderMgr.Reminders);
                }
                else
                {
                    int i = 1;

                    int read = 2;
                    if (tokens[1] == "read")
                    {
                        read = 1;
                        i++;
                    }
                    else if (tokens[1] == "unread")
                    {
                        read = 0;
                        i++;
                    }

                    string s = tokens[i]/*.Replace(".", "")*/; //replace only necessary for dates
                    if (validator.IsDateValid(s, out string date1)) //startdate
                    {
                        string e = tokens[i + 1]/*.Replace(".", "")*/;
                        if (validator.IsDateValid(e, out string date2)) //enddate
                        {
                            if (read == 2)
                                writer.ListReminders(reminderMgr.GetRemindersDueInTimespan(converter.ConvertStringToDate(date1), converter.ConvertStringToDate(date2)));
                            else
                                writer.ListReminders(reminderMgr.GetRemindersDueInTimespan(converter.ConvertStringToDate(date1), converter.ConvertStringToDate(date2)).FindAll(r => r.Read == Convert.ToBoolean(read)));

                            return;
                        }

                        if (read == 2)
                            writer.ListReminders(reminderMgr.GetRemindersDueOnDate(converter.ConvertStringToDate(date1)));
                        else
                            writer.ListReminders(reminderMgr.GetRemindersDueOnDate(converter.ConvertStringToDate(date1)).FindAll(r => r.Read == Convert.ToBoolean(read)));

                        return;
                    }
                    if (s == "today" || s == "t")
                    {
                        if (read == 2)
                            writer.ListReminders(reminderMgr.GetRemindersDueOnDate(DateTime.Today));
                        else
                            writer.ListReminders(reminderMgr.GetRemindersDueOnDate(DateTime.Today).FindAll(r => r.Read == Convert.ToBoolean(read)));

                        return;
                    }
                    if (s == "tomorrow" || s == "to")
                    {
                        if (read == 2)
                            writer.ListReminders(reminderMgr.GetRemindersDueOnDate(DateTime.Today.AddDays(1)));
                        else
                            writer.ListReminders(reminderMgr.GetRemindersDueOnDate(DateTime.Today.AddDays(1)).FindAll(r => r.Read == Convert.ToBoolean(read)));

                        return;
                    }
                    if (s == "yesterday" || s == "ye")
                    {
                        if (read == 2)
                            writer.ListReminders(reminderMgr.GetRemindersDueOnDate(DateTime.Today.AddDays(-1)));
                        else
                            writer.ListReminders(reminderMgr.GetRemindersDueOnDate(DateTime.Today.AddDays(-1)).FindAll(r => r.Read == Convert.ToBoolean(read)));

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
                    else if (validator.IsTimespanValid(tokens[i]/*, out int time) && time != 0*/)) //timespan
                    {
                        /*DateTime date = DateTime.Today.AddMinutes(-ConvertToMinutes(tokens[i], time));

                        if (read == 2)
                        {
                            if (last)
                                writer.ShowAllReminders(reminderMgr.GetRemindersDueInTimespan(date, DateTime.Today));
                            else
                                writer.ShowAllReminders(reminderMgr.GetRemindersDueInTimespan(DateTime.Today, date));
                        }
                        else
                        {
                            if (last)
                                writer.ShowAllReminders(reminderMgr.GetRemindersDueInTimespan(date, DateTime.Today).FindAll(r => r.Read == Convert.ToBoolean(read)));
                            else
                                writer.ShowAllReminders(reminderMgr.GetRemindersDueInTimespan(DateTime.Today, date).FindAll(r => r.Read == Convert.ToBoolean(read)));
                        }*/
                        converter.StandardizeTimespan(tokens[i], out int time, out _);

                        if (time != 0)
                        {
                            if (last)
                                date = DateTime.Today.AddMinutes(-converter.ConvertToMinutes(tokens[i], time));
                            else
                                date = DateTime.Today.AddMinutes(converter.ConvertToMinutes(tokens[i], time));
                        }
                        else
                        {
                            date = DateTime.Today;
                        }
                    }
                    else
                    {
                        writer.ShowError(0, "command not valid");
                        return;
                    }

                    if (last)
                    {
                        if (read == 2)
                            writer.ListReminders(reminderMgr.GetRemindersDueInTimespan(date, DateTime.Today));
                        else
                            writer.ListReminders(reminderMgr.GetRemindersDueInTimespan(date, DateTime.Today).FindAll(r => r.Read == Convert.ToBoolean(read)));
                    }
                    else
                    {
                        if (read == 2)
                            writer.ListReminders(reminderMgr.GetRemindersDueInTimespan(DateTime.Today, date));
                        else
                            writer.ListReminders(reminderMgr.GetRemindersDueInTimespan(DateTime.Today, date).FindAll(r => r.Read == Convert.ToBoolean(read)));
                    }
                }
            }
            catch (Exception ex)
            {
                writer.ShowError(0, ex.Message);
            }
        }

        //command structure: config[/co/settings]
        private void CmdSettings()
        {

        }
    }
}
