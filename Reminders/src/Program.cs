using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Reminders.src
{
    class Program
    {
        private const int TimeOneSecond = 1000;
        private const int IdleTimeoutValue = 10; // value * 1s = time from last user input till console may delete unentered user input to display new output
        //maybe let user change timeout value in config?
        private static OutputTextWriter writer = new OutputTextWriter(); //call this from remindermanager or commandexecuter (if even needed at all)
        private static ReminderManager reminderMgr;
        private static CommandExecutor cmdExec;
        private static Queue<string> outputTextQueue = new Queue<string>();
        private static bool breakFlag = false;

        public static void Main()
        {
            Test();

            Init();

            Task.Run(() => CmdOutputThread());

            while (true) // main program loop
            {
                string s = Console.ReadLine();
                Console.WriteLine(s);

                //cmdExec.Execute(Console.ReadLine()); TODO
            }
        }

        public static void Init()
        {
            DisableQuickEdit dqe = new DisableQuickEdit();
            if (!dqe.Disable())
                writer.ShowError(0, ""); //TODO give user info that console wont get updated if he clicks anywhere in it

            reminderMgr = new ReminderManager(writer);
            cmdExec = new CommandExecutor(writer, reminderMgr);

            writer.ShowWelcome(); //todo 2 methods first only void, add args of method to config (or data?)
            writer.ShowWelcomeReminders(reminderMgr.UpcomingDays, "");
        }


        public static void Test() //method only temporary
        {
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

            Task.Run(() => CmdOutputThread());

            Thread.Sleep(2000);
            NotificationWindow nw = new NotificationWindow();
            nw.Display("test 1 test 2 test 3 test 4");
            Console.WriteLine("MSG CLOSED"); //is only displayed after msgbox is closed NOTIFY USER OF MSGBOX IN CONSOLE THEN DEL MSG IN CMD AFTER CLOSE OF MSGBOX

            while (true) //main loop (test)
            {               
                string s = Console.ReadLine();
                PrintText(s);

                /*if(Console.ReadKey().Key == ConsoleKey.Backspace)
                {
                    //delete last char
                }
                else
                Console.Write("1");*/
            }
        }

        // writes output text in the console, but only if user has not typed any input for a certain amount of time to not override user input
        public static void CmdOutputThread()
        {
            int timeoutCount = 0;
            int posX, posY; // cursor position in console
            bool clearFlag = false;
            
            while (true)
            {
                posY = Console.CursorTop;
                while (Console.CursorLeft != 0 || Console.CursorTop != posY) // wait until timeout after user types input
                {
                    clearFlag = true;
                    
                    posX = Console.CursorLeft;
                    
                    Thread.Sleep(TimeOneSecond); // 1s * IdleTimeoutValue = time until timeout
                    timeoutCount++;
                    
                    if (posX != Console.CursorLeft)
                    {
                        timeoutCount = 0;
                    }
                    else if (timeoutCount == IdleTimeoutValue || breakFlag == true)
                    {
                        timeoutCount = 0;
                        break;
                    }
                }
                
                // clear typed but unentered user input (takes into account multiple lines of user input)
                if (clearFlag == true || breakFlag == true)
                {
                    //record pos of cursor and remove that much when real inpout is entered next time in front of it (also maybe record backspaces then so not too much is deleted; maybe other way by remebering )

                    int n = Console.CursorTop + 1;
                    Console.CursorLeft = 0;
                    Console.CursorTop = posY;
                    for (int i = posY; i < n; i++)
                    {
                       Console.Write(new string(' ', Console.WindowWidth));
                    }
                    Console.CursorLeft = 0;
                    Console.CursorTop = posY;

                    clearFlag = false;
                    breakFlag = false;
                }

                Console.WriteLine("output xxx xxx"); //delete later
                
                while (outputTextQueue.Count > 0)
                {
                    Console.WriteLine(outputTextQueue.Dequeue()); //will reminders be printed here or also be enqueued?
                }   

                // wait 1s after each idle check followed by printing messages received in meantime -> wait time between message prints basically 1s
                Thread.Sleep(TimeOneSecond); 
            }
        }

        public static void PrintText(string text)
        {
            outputTextQueue.Enqueue(text);
            breakFlag = true;
        }
    }
}
