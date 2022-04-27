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
            Console.OutputEncoding = Encoding.Unicode;

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

            while (true) //main loop (test)
            {
                int cursorYInit = Console.CursorTop;
                int cursorXTotal = 0; // like cursorleft but does not reset at new lines
                int cursorXOffset; // length of prompt before input

                string prompt = "> test: ";
                Console.Write(prompt);
                cursorXOffset = prompt.Length;

                ConsoleKeyInfo cki = Console.ReadKey(true);
                while (cki.Key != ConsoleKey.Enter)
                {
                    // ctrl key not pressed or alt key pressed (making ctrl+alt possible which equals altgr key)
                    if ((cki.Modifiers & ConsoleModifiers.Control) == 0 || (cki.Modifiers & ConsoleModifiers.Alt) != 0) // prevents shortcuts like ctrl+i, but allows ones like altgr+q for @
                    {
                        Console.Write(cki.KeyChar);

                        if (cki.Key == ConsoleKey.Backspace)
                        { 
                            // nested if so that backspace does not get added to cmdinput in else-part of statement
                            if (Console.CursorLeft > cursorXOffset - 1 || Console.CursorTop > cursorYInit)
                            {
                                bool lineFlag = (cursorXOffset + cursorXTotal) % Console.BufferWidth == 0; // signals when cursor is at first pos of line

                                // only go to line above when current line is empty (otherwise would already happen when first char in line is deleted, because cursor then is at start of line)
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

                                // move text after backspace one to the left if there is text after cursor
                                if (cursorXTotal < cmdInput.Length) 
                                {
                                    int tempPosY = Console.CursorTop;

                                    if (lineFlag == true) // if cursor x at start of line
                                    {
                                        Console.CursorTop = tempPosY - 1;
                                        Console.CursorLeft = Console.BufferWidth - 1;
                                    }
                                    Console.Write(cmdInput.ToString(cursorXTotal, cmdInput.Length - cursorXTotal) + " \b");

                                    Console.CursorTop = cursorYInit + (cursorXTotal + cursorXOffset) / Console.BufferWidth; // '/' discards remainder
                                    Console.CursorLeft = (cursorXTotal + cursorXOffset) % Console.BufferWidth;
                                }
                            }
                            // counteracts console standard behaviour of moving cursor one to the left
                            else if (Console.CursorLeft != 0)
                            {
                                Console.CursorLeft++;
                            }
                        }
                        else if (cki.Key == ConsoleKey.LeftArrow)
                        {
                            if (Console.CursorLeft == 0 && Console.CursorTop > cursorYInit) // if cursor x at start of line but not start of input
                            {
                                Console.CursorTop--;
                                Console.CursorLeft = Console.BufferWidth - 1;
                                cursorXTotal--;
                            }
                            else if (Console.CursorLeft > cursorXOffset || Console.CursorTop > cursorYInit) // if cursor x not at start of input
                            {
                                Console.CursorLeft--;
                                cursorXTotal--;
                            }
                        }
                        else if (cki.Key == ConsoleKey.RightArrow)
                        {
                            // nested if so that right arrow does not get added to cmdinput in else-part
                            if (Console.CursorLeft - cursorXOffset < cmdInput.Length - (Console.CursorTop - cursorYInit) * Console.BufferWidth) // cursor can not exeed length of input
                            {
                                if (Console.CursorLeft == Console.BufferWidth - 1) // if cursor x at end of line
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
                        }
                        else if (cki.Key == ConsoleKey.UpArrow) //up and down for history
                        {

                        }
                        else if (cki.Key == ConsoleKey.DownArrow)
                        {

                        }
                        else if (cki.Key == ConsoleKey.PageUp) //page up/down for first/last history entry
                        {

                        }
                        else if (cki.Key == ConsoleKey.PageDown)
                        {

                        }
                        else if (cki.Key == ConsoleKey.End) //ende key
                        {
                            Console.CursorTop = cursorYInit + (cursorXOffset + cmdInput.Length) / Console.BufferWidth;
                            Console.CursorLeft = (cursorXOffset + cmdInput.Length) % Console.BufferWidth;

                            cursorXTotal = cmdInput.Length;
                        }
                        else if (cki.Key == ConsoleKey.Home) //pos1 key
                        {
                            Console.CursorTop = cursorYInit;
                            Console.CursorLeft = cursorXOffset;

                            cursorXTotal = 0;
                        }
                        else if (cki.Key == ConsoleKey.Delete) //entf key
                        {
                            // nested if so that del key does not get added to cmdinput in else-part
                            if (Console.CursorLeft - cursorXOffset < cmdInput.Length - (Console.CursorTop - cursorYInit) * Console.BufferWidth) // if not at end of input
                            {
                                cmdInput.Remove(cursorXTotal, 1);

                                if (Console.CursorLeft == Console.BufferWidth - 1) // if cursor x at end of line
                                {
                                    Console.Write(cmdInput.ToString(cursorXTotal, cmdInput.Length - cursorXTotal) + " \b");
                                }
                                else
                                {
                                    Console.Write(" \b" + cmdInput.ToString(cursorXTotal, cmdInput.Length - cursorXTotal) + " \b");
                                }
                                Console.CursorTop = cursorYInit + (cursorXOffset + cursorXTotal) / Console.BufferWidth;
                                Console.CursorLeft = (cursorXOffset + cursorXTotal) % Console.BufferWidth;
                            }
                        }
                        else if (cki.Key == ConsoleKey.Escape)
                        {
                            //add cmd to history before?
                            Console.CursorTop = cursorYInit;
                            Console.CursorLeft = cursorXOffset;

                            Console.Write("0"); // control char for esc to "eat"
                            Console.Write("\b" + new string(' ', cmdInput.Length + 1)); // clear area of input

                            Console.CursorTop = cursorYInit;
                            Console.CursorLeft = cursorXOffset;

                            cmdInput.Clear();
                            cursorXTotal = 0;
                        }
                        else if (cki.Key == ConsoleKey.Tab)
                        {
                            //just ignore for now / prevent user tabbing
                        }
                        else if (cki.Key == ConsoleKey.Insert)
                        {
                            //just ignore for now / mode switching not really necessary
                        }
                        else // handle normal key input
                        {
                            if (cursorXTotal >= cmdInput.Length) // if cursor is at end of input
                            {
                                cmdInput.Append(cki.KeyChar);

                                if ((cursorXOffset + cursorXTotal) % Console.BufferWidth == Console.BufferWidth - 1) // if cursor x at end of line
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

                                if ((cursorXOffset + cursorXTotal) % Console.BufferWidth == Console.BufferWidth - 1) // if cursor x at end of line
                                {
                                    Console.CursorTop = tempPosY + 1;
                                    Console.CursorLeft = 0;
                                }
                                else
                                {
                                    Console.CursorTop = tempPosY;
                                    Console.CursorLeft = (cursorXOffset + cursorXTotal) % Console.BufferWidth + 1;
                                }
                            }
                            cursorXTotal++;
                        }
                    }
                    cki = Console.ReadKey(true);

                    cursorYInit = Console.CursorTop - (cursorXOffset + cursorXTotal) / Console.BufferWidth; // changes value relative to changes to cursortop caused by cmd window resizing
                }
                Console.WriteLine();
                
                Console.CursorTop = cursorYInit + (cursorXOffset + cmdInput.Length) / Console.BufferWidth; // set cursor y to next line after last (full) line of input
                if ((cursorXOffset + cmdInput.Length) % Console.BufferWidth > 0) // move cursor y one more down if there is one last not full line of input
                    Console.CursorTop++;

                PrintText(cmdInput.ToString());
                Console.WriteLine(cmdInput.ToString()); //del later
                cmdInput.Clear();
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
