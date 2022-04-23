using System;
using System.Collections.Generic;
using System.Text;
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
        private static StringBuilder cmdInput = new StringBuilder();

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

            //Task.Run(() => CmdOutputThread());
            /*Thread.Sleep(2000);
            NotificationWindow nw = new NotificationWindow();
            nw.Display("test 1 test 2 test 3 test 4");*/
            Console.WriteLine("MSG CLOSED"); //is only displayed after msgbox is closed NOTIFY USER OF MSGBOX IN CONSOLE THEN DEL MSG IN CMD AFTER CLOSE OF MSGBOX
            //string s = Console.ReadLine();
            while (true) //main loop (test)
            {
                int cursorYInit = Console.CursorTop;
                int cursorXTotal = 0; // like cursorleft but does not reset at new lines SET TO XOFFSET TO MITIGATE PROBLEMS??? instead prob use total + offset in code below
                int cursorXOffset = Console.CursorLeft; // normally 0, but not if there is a prompt before input
                bool lineFlag = false; // signals when cursor is at first pos of line

                ConsoleKeyInfo cki = Console.ReadKey();
                while (cki.Key != ConsoleKey.Enter)
                {
                    int X = Console.CursorLeft;
                    int Y = Console.CursorTop;
                    int l = cmdInput.Length; //3 vars only for debug del later
                    if (cki.Key == ConsoleKey.Backspace) // nested if so that backspace does not get added to cmdinput in else-part of statement
                    {
                        if (cmdInput.Length > 0)
                        {
                            // only go to row above when current row is empty (otherwise would already happen when first char in row is deleted, because cursor is at posx=0 after deletion)
                            if (Console.CursorLeft == 0 && Console.CursorTop > cursorYInit && (cmdInput.Length + cursorXOffset) % Console.BufferWidth == 0)
                            {
                                Console.CursorTop--;
                                Console.CursorLeft = Console.BufferWidth - 1;
                                Console.Write(" \b");
                                Console.CursorLeft++;
                            }
                            else
                            {
                                Console.Write(" \b");
                            }
                            cmdInput.Remove(cursorXTotal - 1, 1);
                            cursorXTotal--;

                            if (cursorXTotal < cmdInput.Length) // move text after backspace one to the left
                            {
                                int tempPosY = Console.CursorTop;
                                /*Console.Write(cmdInput.ToString(cursorPos, cmdInput.Length - cursorPos)+" \b");*/
                                /*Console.CursorTop = tempPosY;*/

                                if (lineFlag == true && Console.CursorLeft == 0)
                                {
                                    Console.CursorTop = tempPosY - 1;
                                    /*Console.CursorTop--;*/
                                    Console.CursorLeft = Console.BufferWidth - 1;
                                    //Console.Write(cmdInput.ToString(cursorPos, cmdInput.Length - cursorPos) + " \b");
                                }
                                //else
                                //{
                                    Console.Write(cmdInput.ToString(cursorXTotal, cmdInput.Length - cursorXTotal) + " \b");
                                //Console.CursorTop = tempPosY;
                                    Console.CursorTop = cursorYInit + cursorXTotal / Console.BufferWidth; // '/' discards remainder
                                    Console.CursorLeft = cursorXTotal % Console.BufferWidth;
                                //}

                            }

                            /*if (Console.CursorLeft == 0 *//*|| Console.CursorLeft == Console.BufferWidth - 1*//*)
                                lineFlag = true;
                            else
                                lineFlag = false;*/
                        }
                    }
                    else if (cki.Key == ConsoleKey.LeftArrow) //add if input goes over multiple rows
                    {
                        if (Console.CursorLeft == 0 && Console.CursorTop > cursorYInit)
                        {
                            Console.CursorTop--;
                            Console.CursorLeft = Console.BufferWidth - 1;
                            cursorXTotal--;
                        }
                        else if (Console.CursorLeft > cursorXOffset)
                        {
                            Console.CursorLeft--;
                            cursorXTotal--;
                        }

                       /* if (Console.CursorLeft == 0)
                            lineFlag = true;
                        else
                            lineFlag = false;*/
                    }
                    else if (cki.Key == ConsoleKey.RightArrow) // nested if so that right arrow does not get added to cmdinput in else-part
                    {
                        if (Console.CursorLeft < cmdInput.Length - (Console.CursorTop - cursorYInit) * Console.BufferWidth) // cursor can not exeed length of input
                        {
                            if (Console.CursorLeft == Console.BufferWidth - 1)
                            {
                                Console.CursorTop++;
                                Console.CursorLeft = 0;
                            }
                            else
                            {
                                Console.CursorLeft++;
                            }
                            cursorXTotal++;
                        }

                      /*  if (Console.CursorLeft == 0)
                            lineFlag = true;
                        else
                            lineFlag = false;*/
                    }
                    /*else if (cki.Key == ConsoleKey.UpArrow) //up and down for history
                    {

                    }
                    else if (cki.Key == ConsoleKey.DownArrow)
                    {

                    }
                    else if (cki.Key == ConsoleKey.End) //ende key
                    {

                    }
                    else if (cki.Key == ConsoleKey.Home) //pos1 key
                    {

                    }
                    else if (cki.Key == ConsoleKey.Delete) //entf key
                    {

                    }
                    else if (cki.Key == ConsoleKey.Escape) //entf key
                    {
                        //del all input
                    }
                    else if (cki.Key == ConsoleKey.Tab)
                    {
                        //just ignore for now / prevent user tabbing
                    }*/
                    else
                    {
                        if (cursorXTotal >= cmdInput.Length)
                        {
                            cmdInput.Append(cki.KeyChar);

                            if (cursorXTotal % Console.BufferWidth == Console.BufferWidth -1)
                            {
                                Console.CursorTop++;
                                Console.CursorLeft = 0;
                            }
                      
                        }
                        else
                        {
                            cmdInput.Insert(cursorXTotal, cki.KeyChar);

                            int tempPosY = Console.CursorTop;
                            Console.Write(cmdInput.ToString(cursorXTotal + 1, cmdInput.Length - cursorXTotal - 1)); // move text after insertion one to the right
                            
                            if (/*cursorXTotal + 1 == Console.BufferWidth*/cursorXTotal % Console.BufferWidth == Console.BufferWidth - 1)
                            {
                                Console.CursorTop = tempPosY + 1;
                                Console.CursorLeft = 0;
                                lineFlag = true;
                            }
                            else
                            {
                                Console.CursorTop = tempPosY;
                                Console.CursorLeft = cursorXTotal % Console.BufferWidth + 1;
                            }
                            
                        }
                        cursorXTotal++;
                    }
                    if (Console.CursorLeft == 0)
                        lineFlag = true;
                    else
                        lineFlag = false;

                    cki = Console.ReadKey();
                }
                Console.WriteLine();

                Console.CursorTop = cursorYInit + (cursorXOffset + cmdInput.Length) / Console.BufferWidth; // set cursor y to last line of input
                if ((cursorXOffset + cmdInput.Length) % Console.BufferWidth > 0)
                    Console.CursorTop++;

                PrintText(cmdInput.ToString());
                Console.WriteLine(cmdInput.ToString()+"@ "+cmdInput.Length+" "+cmdInput.ToString().Length); //del later
                cmdInput.Clear();

                /*string s = Console.ReadLine();

                PrintText(s);*/
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
                while (/*Console.CursorLeft != 0*/ cmdInput.Length != 0 /*|| Console.CursorTop != posY*/) // wait until timeout after user types input
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
                       Console.Write(new string(' ', Console.BufferWidth));
                    }
                    Console.CursorLeft = 0;
                    Console.CursorTop = posY;
                    
                    Console.Write(cmdInput.ToString());

                    clearFlag = false;
                    breakFlag = false;
                }
                
                //Console.WriteLine("output xxx xxx"); //delete later
                
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
