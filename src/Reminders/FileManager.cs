using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Reminders.src
{
    class FileManager
    {
        private const string ConfigFile = "config.txt";
        private const int NumConfigArgs = 3;
        private const string ConfigText = "path=default;\n" +
                                          "autostart=false;\n" +
                                          "upcomingreminderstime=3;";
        private const int NumRmdrParams = 3;

        private OutputTextWriter writer;

        private string appPath = AppDomain.CurrentDomain.BaseDirectory;
        private string dataPath;
        private string dataFilename = "data.rmdr";
        private string fileRaw; //first used for loading config, then only for data file (.rmdr) REPLACE WITH LOCAL VARIABLE???
        private int upcomingDays;
        private bool autostart;
        private Reminder[] reminders;

        public FileManager(OutputTextWriter outputTextWriter)
        {
            writer = outputTextWriter;

            Init();
        }

        private void Init()
        {
            if (! LoadConfig())
                writer.ShowError(2, "");
            //give option to reset config

            if (! LoadData())
                writer.ShowError(3, "");
        }

        // loads config file and applies the settings
        public bool LoadConfig()
        {
            // structure of config file: parameter=value;[newline]... (= and ; characters not used in names or values)

            if (! LoadFile(appPath, ConfigFile))
            {
                //send error to textwriter
                
                return false;
            }

            fileRaw = Regex.Replace(fileRaw, @"\t|\n|\r", ""); //remove all newlines and tabs
            string[] lines = fileRaw.Split(';');

            if (lines.Length != NumConfigArgs + 1) //+1 for the extra line which is removed next
            {
                //send error to textwriter

                return false;
            }

            try
            {
                Array.Resize(ref lines, lines.Length - 1); //solves problem of empty string in array as last element which was created because of the last ; in the file

                string[] values = new string[NumConfigArgs];

                for (int i = 0; i < lines.Length; i++) //no fixed length to "provoke" exception
                {
                    values[i] = lines[i].Split('=')[1];
                }

                ApplyConfig(values);
            }
            catch (Exception e)
            {
                //send error to textwriter: config file is corrupted, wrong syntax used
                Console.WriteLine(e.StackTrace);
                return false;
            }

            return true;
        }

        private void ApplyConfig(string[] values) //change to bool if arg with possibility of foreseeable errors is added
        {
            // param: path; allowed values: "default" or any file path
            if (values[0] == "default") //path of application
                dataPath = AppDomain.CurrentDomain.BaseDirectory;
            else
                dataPath = values[0];

            // param: autostart; allowed values: "true" or "false"
            if (values[1] == "true")
            {   //add to autostart
                autostart = true;

                string startupDir = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

                ShortcutCreator shortcutCreator = new ShortcutCreator();
                shortcutCreator.CreateShortcut(startupDir);
            }
            else if (values[1] == "false")
            {   //remove from autostart
                autostart = false;

                string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "Reminder.lnk");

                if (File.Exists(shortcutPath))
                    File.Delete(shortcutPath);
                else
                {
                    //send note to textwriter
                    //no return false needed because situation is weird but no negative impact on program
                }
            }
            //no else block, errors will be caught in catch block where this method is called from

            // param: upcomingreminderstime; allowed values: "-1", "0", or every whole number above zero
            if (values[2] == "-1")
                upcomingDays = -1;
            else if (values[2] == "0")
                upcomingDays = 0;
            else if (int.Parse(values[2]) > 0)
                upcomingDays = int.Parse(values[2]);
            //no else block or tryparse, errors will be caught in catch block where this method is called from
        }

        // overrides config.txt with default text (settings) or creates new config.txt if file was deleted
        public bool RestoreConfigToDefault()
        {
            return SaveFile(appPath, ConfigFile, ConfigText);
        }

        public bool SaveConfig()
        {
            string configText = "path=" + appPath + ";\n" +
                                "autostart=" + autostart + ";\n" +
                                "upcomingreminderstime=" + upcomingDays + ";";

            SaveFile(appPath, ConfigFile, configText);

            return true;
        }

        public bool LoadData()
        {
            // structure of data file: YYYYMMDDhhmm;[-1..n <- repeat];[content];[newline]...

            if (! LoadFile(appPath, dataFilename))
            {
                //send error to textwriter

                return false;
            }

            fileRaw = Regex.Replace(fileRaw, @"\t|\n|\r", ""); //remove all newlines and tabs

            List<string> l = new List<string>();

            StringBuilder currentSegment = new StringBuilder();

            for (int i = 0; i < fileRaw.Length; i++)
            {
                char c = fileRaw[i];

                if (c == ';')
                {
                    l.Add(currentSegment.ToString());
                    currentSegment.Clear();
                }
                else if (c == '|')
                {
                    currentSegment.Append(fileRaw[i + 1]);

                    i++;
                }
                else
                {
                    currentSegment.Append(c);
                }
            }

            string[] values = l.ToArray();

            if ((values.Length ) % NumRmdrParams != 0) //+1 for the extra line which is removed next
            {
                //send error to textwriter

                return false;
            }

            try
            {
                Reminder[] rmdrs = new Reminder[values.Length / NumRmdrParams];

                for (int i = 0; i < values.Length; i += NumRmdrParams) //build reminders, they always consist of 3 (might change) values
                {
                    Reminder r = new Reminder(values[i], int.Parse(values[i + 1]), values[i + 2]);

                    rmdrs[i / 3] = r;
                }

                reminders = rmdrs;
            }
            catch (Exception e)
            {
                //send error to textwriter: config file is corrupted, wrong syntax used
                Console.WriteLine(e.StackTrace);
                return false;
            }

            return true;
        }

        public bool SaveData()
        {
            // structure of data file: YYYYMMDDhhmm;[0/1 <- repeat];[content];[newline]...

            string dataText = "";

            foreach (Reminder r in reminders) //assumes reminders do not contain any errors
            {
                dataText += r.Date.ToString("yyyyMMddHHmm") + ";" +
                            r.Repeat + ";";

                string content = r.Content;
                StringBuilder sb = new StringBuilder(content.Length);

                for (int i = 0; i < content.Length; i++) //insert escape characters, char to escape: ";", escape char: "|"
                {
                    char c = content[i];

                    if (c == ';' || c == '|')
                        sb.Append("|");

                    sb.Append(c);
                }

                dataText += sb.ToString() + ";\n";
            }

            return SaveFile(dataPath, dataFilename, dataText);
        }

        public bool ClearData()
        {
            return SaveFile(dataPath, dataFilename, "");
        }

        // puts input of file in string; returns false if unsuccessful
        private bool LoadFile(string path, string name)
        {
            try
            {
                fileRaw = File.ReadAllText(Path.Combine(path, name));
            }
            catch (Exception e)
            {
                fileRaw = "ERROR";
                Console.WriteLine(e.StackTrace);

                return false;
            }

            return true;
        }

        // saves text in a file (overrides old file or creates new file if does not exist yet), retuns false if unsuccesful
        private bool SaveFile(string path, string name, string content)
        {
            try
            {
                File.WriteAllText(Path.Combine(path, name), content, Encoding.Unicode);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);

                return false;
            }

            return true;
        }

        // converts the List to a string, separating all entries by commas
        public string GetAllEntries()   
        {
            string s = "";
            if (reminders.Length > 0)
                reminders[0].ToString();

            if (reminders.Length > 1)
            {
                for (int i = 1; i < reminders.Length; i++)
                {
                    s += ", " + reminders[i].ToString();
                }
            }

            return s;
        }

        public string Filename
        {
            get { return dataFilename; }
            set // very late feature, absolutely not needed at beginning
            {
                // users can enter filename without extension
                if (!value.EndsWith(".rmdr"))
                    value += ".rmdr";

                dataFilename = value;
            }
        }

        /*public string DataPath
        {
            get { return dataPath; }
        }*/

        public Reminder[] Reminders
        {
            get => reminders; //loaddata in get so program is always uptodate with file? or not necessary cuz file should only be changed with program?
            set
            {
                if (value != reminders)
                {
                    reminders = value;
                    SaveData();
                }
            }
        }

        public int UpcomingDays
        {
            get => upcomingDays;
            set
            {
                if (value != upcomingDays)
                {
                    upcomingDays = value;
                    SaveConfig();
                }
            }
        }
        
        public bool Autostart
        {
            get => autostart;
            set
            {
                if (value != autostart)
                {
                    autostart = value;
                    SaveConfig();
                }
            }
        }
    }
}
