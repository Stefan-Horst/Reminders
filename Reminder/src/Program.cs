﻿using System;

namespace Reminder.src
{
    class Program
    {
        public static OutputTextWriter writer = new OutputTextWriter(); //call this from remindermanager or commandexecuter (if even needed at all)
        public static ReminderManager reminderMgr;
        public static CommandExecutor cmdExec;

        static void Main(string[] args)
        {
            Test();


            Init();

            while (true)
            {
                cmdExec.Execute(Console.ReadLine());
            }
        }

        public static void Init()
        {
            reminderMgr = new ReminderManager(writer);
            cmdExec = new CommandExecutor(writer, reminderMgr);

            writer.ShowWelcome(); //todo 2 methods first only void, add args of method to config (or data?)
            writer.ShowWelcomeReminders(reminderMgr.UpcomingDays, "");
        }


        public static void Test() //method only temporary
        {
            //C:\Users\Stefan\Desktop\DesktopStuff\CSharp\Reminder\Reminder\bin\Debug\netcoreapp2.1\
            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.Startup));
            Console.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().Location);

            FileManager fileManager = new FileManager(writer);
            foreach (Reminder r in fileManager.Reminders)
            {
                Console.WriteLine(r.Date.ToString() + ", " + r.Repeat + ", " + r.Content);
            }
            

            Console.WriteLine("--------------------------------------------\n");
        }
    }
}
