using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reminders
{
    //taking the commands from the cmd and delegating logic to remindermanager and output to outputtextwriter
    // problem: remindermanager has some standalone tasks like the welcome message at program start, should it be called from here (not fitting) or separately from main
    class CommandExecutor
    {
        private OutputTextWriter writer;
        private ReminderManager reminderMgr;

        private string[] tokens; //current user input tokenized

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
                if (! IsDateValid(tokens[1], out string date))
                {
                    writer.ShowError(0, "wrong date arg");
                    return;
                }

                string text;
                string time;
                string repeat;

                if (tokens.Length == 5)
                {
                    if (! IsTimeValid(tokens[2], out time))
                    {
                        writer.ShowError(0, "wrong time arg");
                        return;
                    }
                    if (! IsTimespanValid(tokens[3], out _))
                    {
                        writer.ShowError(0, "wrong date arg");
                        return;
                    }
                    repeat = tokens[3];
                    text = tokens[4];
                }
                else if (tokens.Length == 4)
                {
                    if (IsTimeValid(tokens[2], out time))
                    {
                        repeat = "0";
                    }
                    else if (IsTimespanValid(tokens[2], out _))
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
                }
                //if (IsDateValid(date, out string date1) && IsTimeValid(time, out string time1) && IsTimespanValid(repeat, out _))
                reminderMgr.CreateReminder(date + time, repeat, text);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SSS");
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

                    if (int.TryParse(s, out _)) //maybe not needed
                    {
                        if (IsDateValid(s, out string date)) //date
                        {
                            r.DateString = date + r.Date.ToString("HHmm");
                        }
                        else if (IsTimeValid(s, out string time)) //time
                        {
                            r.DateString = r.DateString.Remove(6) + time;
                        }
                    }
                    else if (IsTimespanValid(s, out _)) //repeat
                    {
                        r.Repeat = s;
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
                            else if (string.Join("", tokens[i..]).Contains('"')) // make sure there is a second " in a later token
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
        //command structure: show[/s] ([un]read[/[u/]r]) (s{dd(.)mm(.)(yy)yy)}) (e{dd(.)mm(.)(yy)yy)})  /  show[/s] ([un]read) {dd(.)mm(.)(yy)yy)}[/today[/t]/tomorrow[/to]/yesterday[/y](last[/l])/week[/w]/month[/m]/year[/y]/{x}d/{x}w/{x}y/]
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
                    if (IsDateValid(s, out string date1)) //startdate
                    {
                        string e = tokens[i + 1]/*.Replace(".", "")*/;
                        if (IsDateValid(e, out string date2)) //enddate
                        {
                            if (read == 2)
                                writer.ListReminders(reminderMgr.GetRemindersDueInTimespan(ConvertStringToDate(date1), ConvertStringToDate(date2)));
                            else
                                writer.ListReminders(reminderMgr.GetRemindersDueInTimespan(ConvertStringToDate(date1), ConvertStringToDate(date2)).FindAll(r => r.Read == Convert.ToBoolean(read)));

                            return;
                        }

                        if (read == 2)
                            writer.ListReminders(reminderMgr.GetRemindersDueOnDate(ConvertStringToDate(date1)));
                        else
                            writer.ListReminders(reminderMgr.GetRemindersDueOnDate(ConvertStringToDate(date1)).FindAll(r => r.Read == Convert.ToBoolean(read)));

                        return;
                    }
                    else if (s == "today" || s == "t")
                    {
                        if (read == 2)
                            writer.ListReminders(reminderMgr.GetRemindersDueOnDate(DateTime.Today));
                        else
                            writer.ListReminders(reminderMgr.GetRemindersDueOnDate(DateTime.Today).FindAll(r => r.Read == Convert.ToBoolean(read)));

                        return;
                    }
                    else if (s == "tomorrow" || s == "to")
                    {
                        if (read == 2)
                            writer.ListReminders(reminderMgr.GetRemindersDueOnDate(DateTime.Today.AddDays(1)));
                        else
                            writer.ListReminders(reminderMgr.GetRemindersDueOnDate(DateTime.Today.AddDays(1)).FindAll(r => r.Read == Convert.ToBoolean(read)));

                        return;
                    }
                    else if (s == "yesterday" || s == "ye")
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
                    else if (IsTimespanValid(tokens[i], out int time) && time != 0) //timespan
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

                        if (last)
                            date = DateTime.Today.AddMinutes(-ConvertToMinutes(tokens[i], time));
                        else
                            date = DateTime.Today.AddMinutes(ConvertToMinutes(tokens[i], time));
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

        private int ConvertToMinutes(string raw, int time)
        {
            if (raw.Contains("min"))
            {
                return time;
            }
            if (raw.Contains("h"))
            {
                return time *= 60;
            }
            else if (raw.Contains("d"))
            {
                return time *= 60 * 24;
            }
            else if (raw.Contains("m"))
            {
                return time *= 60 * 24 * 30;
            }
            else if (raw.Contains("y"))
            {
                return time *= 60 * 24 * 30 * 12;
            }
            else 
            {
                return -1;
            }
        }

        /*private bool IsDateValid(string date)
        {
            try
            {
                int day = -1;
                int month = -1;
                int year = -1;

                if (date.Length == 8)
                {
                    day = int.Parse(date[..2]);
                    month = int.Parse(date.Substring(2, 2));
                    year = int.Parse(date.Substring(4, 4));
                }
                else if (date.Length == 6)
                {
                    day = int.Parse(date[..2]);
                    month = int.Parse(date.Substring(2, 2));
                    year = int.Parse(DateTime.Now.Year.ToString().Remove(2) + date.Substring(4, 2));
                }
                DateTime dt = new DateTime(year, month, day);

                return true;
            }
            catch
            {
                return false;
            }
        }*/

        private bool IsDateValid(string date, out string normalizedDate)
        {
            normalizedDate = null;

            try
            {
                date = date.Replace(".", "");

                if (date.Length == 6)
                {
                    date = date[..4] + DateTime.Now.Year.ToString().Remove(2) + date.Substring(4, 2);
                }
                else if (date.Length != 8)
                {
                    return false;
                }
                string dateFormat = "ddMMyyyy";

                bool b = DateTime.TryParseExact(date, dateFormat, System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None, out _);

                if (b)
                    normalizedDate = date;

                return b;
            }
            catch
            {
                return false;
            }
        }

        /*private bool IsTimeValid(string time)
        {
            try
            {
                int hour = -1;
                int minute = -1;

                if (time.Length == 4)
                {
                    hour = int.Parse(time[..2]);
                    minute = int.Parse(time.Substring(2, 2));

                    if (hour >= 0 && hour <= 23 && minute >= 0 && minute <= 60)
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }*/

        private bool IsTimeValid(string time, out string normalizedTime)
        {
            normalizedTime = null;

            try
            {
                time = time.Replace(".", "").Replace(":", "");

                if (time.Length == 3)
                {
                    time = "0" + time;
                }
                else if (time.Length != 4)
                {
                    return false;
                }
                string timeFormat = "HHmm";

                bool b = DateTime.TryParseExact(time, timeFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out _);

                if (b)
                    normalizedTime = time;

                return b;
            }
            catch
            {
                return false;
            }
        }

        private bool IsTimespanValid(string timespan, out int time) // also used for repeat
        {
            time = -1;

            if (timespan == "0")
            {
                time = 0;
                return true;
            }

            try
            {
                if (! (timespan.Contains("h") || timespan.Contains("d") || timespan.Contains("m") || timespan.Contains("y") || timespan.Contains("n")))
                    return false;

                bool b = int.TryParse(timespan.Remove(timespan.Length - 1).Replace("mi", ""), out int t);

                if (b && time >= 0) // 0d, 0w, etc. are acceptable timespans which just result in 0
                {
                    time = t;
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private DateTime ConvertStringToDate(string dateString)
        {
            int day = int.Parse(dateString[..2]);
            int month = int.Parse(dateString.Substring(2, 2));
            int year = int.Parse(dateString.Substring(4, 4));

            return new DateTime(year, month, day);
        }
    }
}
