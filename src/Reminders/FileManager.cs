using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Reminders.WinApi;

namespace Reminders
{
    public class FileManager
    {
        private const string ConfigFile = "config.txt";
        private const int NumConfigArgs = 6;
        private const string ConfigText = "path=default;\n" +
                                          "autostart=false;\n" +
                                          "upcomingreminderstime=3;\n" +
                                          "devmode=false\n" +
                                          "notification=true\n" +
                                          "quickedit=false";
        private const int NumRmdrParams = 4;
        private const string AutostartFilename = "Reminders";

        private OutputTextWriter writer;

        private readonly string appPath = AppDomain.CurrentDomain.BaseDirectory; // config.txt is always saved here
        private string dataPath; // path where data.rmdr is saved
        private string dataFilename = "data.rmdr";
        private int upcomingDays;
        private bool autostart;
        private bool notification;
        private bool quickedit;
        private Reminder[] reminders;

        public FileManager(OutputTextWriter outputTextWriter)
        {
            writer = outputTextWriter;

            Init();
        }

        private void Init()
        {
            dataPath = appPath;
            
            if (! LoadConfig())
            {
                 writer.Log(LogType.Problem, "loading config failed, proceeding with default settings");
                            
                 if (! RestoreConfigToDefault())
                     writer.Log(LogType.ErrorCritical, "restoring config failed");
            }

            if (! LoadData())
            {
                writer.Log(LogType.Error, "loading data failed");

                if (! File.Exists(appPath + dataFilename))
                {
                    if (! SaveData()) // creates new empty data file
                        writer.Log(LogType.ErrorCritical, "creating data file failed");
                    
                    if (! LoadData())
                        writer.Log(LogType.ErrorCritical, "loading data failed");
                }
                else
                {
                    writer.Log(LogType.ErrorCritical, "loading data file failed");
                }
            }
        }

        // loads config file and applies settings
        private bool LoadConfig()
        {
            // structure of config file: parameter=value;[newline]... (= and ; characters not used in names or values)

            if (! LoadFile(appPath, ConfigFile, out string fileRaw))
            {
                writer.Log(LogType.Error, "loading config file failed");
                return false;
            }

            fileRaw = Regex.Replace(fileRaw, @"\t|\n|\r", ""); //remove all newlines and tabs
            string[] lines = fileRaw.Split(';');

            if (lines.Length != NumConfigArgs + 1) //+1 for the extra line which is removed next
            {
                writer.Log(LogType.Error, "config file incomplete");
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
            catch (Exception ex)
            {
                writer.Log(LogType.Error, ex.Message);
                return false;
            }

            return true;
        }

        private void ApplyConfig(string[] values)
        {
            bool error = false;
            
            // param: devmode; allowed values: "true" or "false"
            if (values[3] == "true" || values[3] == "True")
                writer.Devmode = true;
            else if (values[3] == "false" || values[3] == "False")
                writer.Devmode = false;
            else
            {
                writer.Log(LogType.Error, "config devmode param is wrong");
                error = true;
            }

            writer.Log(LogType.Info, "file loaded successfully: " + ConfigFile); // first info log only possible after devmode init
            writer.Log(LogType.Info, "devmode: " + writer.Devmode);
            
            // param: path; allowed values: "default" or any file path
            if (values[0] == "default") //path of application
                dataPath = appPath;
            else
                dataPath = values[0];
            
            writer.Log(LogType.Info, "datapath: " + dataPath);

            // param: autostart; allowed values: "true" or "false"
            if (values[1] == "true" || values[1] == "True")
            {   //add to autostart
                autostart = true;

                string startupDir = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

                if (! File.Exists(Path.Combine(startupDir, AutostartFilename + ".lnk")))
                {
                    ShortcutCreator shortcutCreator = new ShortcutCreator();
                    shortcutCreator.CreateShortcut(startupDir, AutostartFilename);
    
                    if (! File.Exists(Path.Combine(startupDir, AutostartFilename + ".lnk")))
                    {
                        writer.Log(LogType.Error, "creating autostart shortcut failed"); 
                        error = true;
                    }
                    else
                        writer.Log(LogType.Info, "autostart shortcut created: " + startupDir + "\\" + AutostartFilename + ".lnk");
                }
                else
                    writer.Log(LogType.Info, "autostart shortcut already exists");
            }
            else if (values[1] == "false" || values[1] == "False")
            {   //remove from autostart
                autostart = false;

                string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), AutostartFilename + ".lnk");

                if (File.Exists(shortcutPath))
                { 
                    File.Delete(shortcutPath);

                    if (File.Exists(shortcutPath))
                    { 
                        writer.Log(LogType.Error, "deleting autostart shortcut failed");
                        error = true;
                    }
                    writer.Log(LogType.Info, "autostart shortcut deleted");
                }
                else
                    writer.Log(LogType.Info, "autostart shortcut does not exist");
            }
            else
            {
                writer.Log(LogType.Error, "config autostart param is wrong");
                error = true;
            }
            
            // param: upcomingreminderstime; allowed values: "-1", "0", or every whole number above zero
            if (values[2] == "-1")
                upcomingDays = -1;
            else if (values[2] == "0")
                upcomingDays = 0;
            else if (int.Parse(values[2]) > 0)
                upcomingDays = int.Parse(values[2]);
            else
            {
                writer.Log(LogType.Error, "config upcomingRemindersTime param is wrong");
                error = true;
            }
            
            writer.Log(LogType.Info, "upcomingDays: " + upcomingDays);
            
            // param: notification; allowed values: "true" or "false"
            if (values[4] == "true" || values[4] == "True")
                notification = true;
            else if (values[4] == "false" || values[4] == "False")
                notification = false;
            else
            {
                writer.Log(LogType.Error, "config notification param is wrong");
                error = true;
            }
            
            writer.Log(LogType.Info, "notification: " + notification);

            // param: quickedit; allowed values: "true" or "false"
            if (values[5] == "true" || values[5] == "True")
                quickedit = true;
            else if (values[5] == "false" || values[5] == "False")
                quickedit = false;
            else
            {
                writer.Log(LogType.Error, "config quickedit param is wrong");
                error = true;
            }
            
            writer.Log(LogType.Info, "quickedit: " + quickedit);

            if (! error)
                writer.Log(LogType.Info, "config loaded and applied successfully");
        }

        // overrides config.txt with default text (settings) or creates new config.txt if file was deleted
        public bool RestoreConfigToDefault()
        {
            bool b = SaveFile(appPath, ConfigFile, ConfigText);

            if (b)
                writer.Log(LogType.Info, "file saved successfully: " + ConfigFile);
            
            return b;
        }

        public void SaveConfig()
        {
            string configText = "path=" + dataPath + ";\n" +
                                "autostart=" + autostart + ";\n" +
                                "upcomingreminderstime=" + upcomingDays + ";\n" +
                                "devmode=" + writer.Devmode + ";\n" + 
                                "notification=" + notification + ";\n" +
                                "quickedit=" + quickedit + ";";
            
            ApplyConfig(new string[] {dataPath, "" + autostart, "" + upcomingDays, "" + writer.Devmode, "" + notification, "" + quickedit});

            if (! SaveFile(appPath, ConfigFile, configText))
                writer.Log(LogType.Error, "saving config failed");
            else
                writer.Log(LogType.Info, "file saved successfully: " + ConfigFile);
        }

        private bool LoadData()
        {
            // structure of data file: YYYYMMDDhhmm;[-1..n <- repeat];[content];[newline]...

            if (! LoadFile(dataPath, dataFilename, out string fileRaw))
            {
                writer.Log(LogType.Error, "loading data failed");
                return false;
            }
            writer.Log(LogType.Info, "data file loaded successfully: " + dataFilename);

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

            if (values.Length % NumRmdrParams != 0) //+1 for the extra line which is removed next
            {
                writer.Log(LogType.ErrorCritical, "wrong amount of params in data");
                return false;
            }

            try
            {
                Reminder[] rmdrs = new Reminder[values.Length / NumRmdrParams];

                for (int i = 0; i < values.Length; i += NumRmdrParams) //build reminders, they always consist of 4 (might change) values
                {
                    Reminder r = new Reminder(values[i], values[i + 1], bool.Parse(values[i + 2]), values[i + 3]);

                    rmdrs[i / NumRmdrParams] = r;
                }

                reminders = rmdrs;
            }
            catch (Exception ex)
            {
                writer.Log(LogType.ErrorEx, ex.Message);
                writer.Log(LogType.ErrorCritical, "creation of reminders from data failed");
                return false;
            }
            writer.Log(LogType.Info, "data loaded successfully");

            return true;
        }

        private bool SaveData()
        {
            // structure of data file: YYYYMMDDhhmm;[repeat];[read];[content];[newline]...

            string dataText = "";

            foreach (Reminder r in reminders) //assumes reminders do not contain any errors
            {
                dataText += r.Date.ToString("ddMMyyyyHHmm") + ";" +
                            r.Repeat + ";" +
                            r.Read + ";";

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

            bool b = SaveFile(dataPath, dataFilename, dataText);

            if (b)
                writer.Log(LogType.Info, "file saved successfully: " + dataFilename);

            return b;
        }

        public bool ClearData()
        {
            return SaveFile(dataPath, dataFilename, "");
        }

        // puts input of file in string; returns false if unsuccessful
        private bool LoadFile(string path, string name, out string fileRaw)
        {
            fileRaw = "";
            
            try
            {
                fileRaw = File.ReadAllText(Path.Combine(path, name));
            }
            catch (Exception ex)
            {
                fileRaw = "ERROR";
                writer.Log(LogType.ErrorEx, ex.Message);
                return false;
            }

            return true;
        }

        // saves text in a file (overrides old file or creates new file if does not exist yet), returns false if unsuccessful
        private bool SaveFile(string path, string name, string content)
        {
            try
            {
                File.WriteAllText(Path.Combine(path, name), content, Encoding.Unicode);
            }
            catch (Exception ex)
            {
                writer.Log(LogType.ErrorEx, ex.Message);
                return false;
            }

            return true;
        }

        public string Filename
        {
            get => dataFilename;
            set // very late feature, absolutely not needed at beginning
            {
                // users can enter filename without extension
                if (!value.EndsWith(".rmdr"))
                    value += ".rmdr";

                dataFilename = value;
            }
        }

        public string DataPath
        {
            get => dataPath;
            set
            {
                if (value != dataPath)
                {
                    if (!SaveData())
                    {
                        writer.Log(LogType.Error, "saving data failed");
                        return;
                    }

                    string oldPath = dataPath;
                    
                    if (value == "default")
                        dataPath = appPath;
                    else
                        dataPath = value;
                    
                    SaveConfig();

                    reminders = Array.Empty<Reminder>();
                    Init(); // load data from new file or create new data file if none exists

                    writer.FileChange(oldPath, dataPath);

                    //if (! LoadData())
                    //    writer.Log(LogType.Error, "loading data failed");
                }
            }
        }

        public Reminder[] Reminders
        {
            get => reminders; //loaddata in get so program is always uptodate with file? or not necessary cuz file should only be changed with program?
            set
            {
                if (value != reminders)
                {
                    reminders = value;
                    
                    if (! SaveData())
                        writer.Log(LogType.Error, "saving data failed");
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
        
        public bool Notification
        {
            get => notification;
            set
            {
                if (value != notification)
                {
                    notification = value;
                    SaveConfig();
                }
            }
        }
        
        public bool Quickedit
        {
            get => quickedit;
            set
            {
                if (value != quickedit)
                {
                    quickedit = value;
                    SaveConfig();
                }
            }
        }
    }
}
