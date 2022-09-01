using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Reminders.ConsoleIO;
using Reminders.WinApi;
using SimultaneousConsoleIO;

namespace Reminders
{
    public class Program
    {
        //maybe let user change timeout value in config?
        private static IOutputWriter outputWriter;
        private static OutputTextWriter writer; //call this from remindermanager or commandexecuter (if even needed at all)
        private static ReminderManager reminderMgr;
        private static CommandExecutor cmdExec;
        /*private static Queue<string> outputTextQueue = new Queue<string>();
        private static StringBuilder cmdInput = new StringBuilder();*/

        public static void Main()
        {
            //Test();

            Init();

            while (true) // main program loop
            {
                string s = Console.ReadLine();
                Console.WriteLine(s);

                //cmdExec.Execute(Console.ReadLine()); TODO
            }
        }

        private static void Init()
        {
            Console.OutputEncoding = Encoding.Unicode;

            outputWriter = new OutputWriter(); //problem of circular dependency?
            SimulConsoleIO simio = new SimulConsoleIO(outputWriter);
            writer = new OutputTextWriter(simio);
            reminderMgr = new ReminderManager(writer);
            simio.TextProvider = new TextProvider(reminderMgr);
            cmdExec = new CommandExecutor(writer, reminderMgr);

            DisableQuickEdit dqe = new DisableQuickEdit();
            if (!dqe.Disable())
                writer.ShowError(0, ""); //TODO give user info that console wont get updated if he clicks anywhere in it

            writer.ShowWelcome(); //todo 2 methods first only void, add args of method to config (or data?)
            writer.ShowWelcomeReminders(reminderMgr.UpcomingDays, "");

            /* while (true)
             {
                 if (DateTime.Now == new DateTime(DateTime.Now.Year, 5, 25))
                     Console.WriteLine("g");
                 //Thread.Sleep(25);
             }*/
            
            /*Task.Run(() => {
                int i = 0;
                while (true)
                {
                    outputWriter.AddText("testtesttesttesttesttesttesttesttesttesttesttest" + i);
                    i++;
                    Thread.Sleep(5000);
                }
            });*/

            while (true)
                cmdExec.Execute(simio.ReadLine("")); //use keyavailable there so readline doesnt block and output doesnt need extra thread
        }


        private static void Test() //method only temporary
        {
            Console.OutputEncoding = Encoding.Unicode;

            DisableQuickEdit dqe = new DisableQuickEdit();
            Console.WriteLine(dqe.Disable());

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
            reminderMgr = new ReminderManager(writer);
            try {
                //reminderMgr.ReadReminder(999);
                reminderMgr.GetRemainingTime(999);
            }
            catch (Exception e) { Console.WriteLine(e.GetBaseException()); }

            DateTime d = new DateTime(1000);
            DateTime f = new DateTime(2000);

            Console.WriteLine(d.ToShortDateString());
            Console.WriteLine(d.Subtract(f).Ticks);

            //TimeSpan expected = new DateTime(2040, 12, 20).Subtract(DateTime.Now);
            TimeSpan expected = new DateTime(2040, 12, 20).Subtract(new DateTime(2040, 12, 20));
            string se = expected.ToString(@"d\.hh\:mm\:ss");
            Console.WriteLine(se);

            TimeSpan ts = new TimeSpan(-1);
            Console.WriteLine(ts.ToString());

            //Task.Run(() => CmdOutputThread());
            /*Thread.Sleep(2000);
            NotificationWindow nw = new NotificationWindow();
            nw.Display("test 1 test 2 test 3 test 4");*/
            Console.WriteLine("MSG CLOSED"); //is only displayed after msgbox is closed NOTIFY USER OF MSGBOX IN CONSOLE THEN DEL MSG IN CMD AFTER CLOSE OF MSGBOX
            //string s = Console.ReadLine();
            //Console.Write("test");Console.CursorLeft--; Console.Write(" \b");Console.ReadLine();
        }
    }
}
