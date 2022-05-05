﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Reminders.src
{
    class SimultaneousConsoleIO
    {
        private string promptDefault = "";
        private Queue<string> outputTextQueue = new Queue<string>();

        public SimultaneousConsoleIO()
        {

        }

        public SimultaneousConsoleIO(string promptDefault)
        {
            this.promptDefault = promptDefault;
        }

        public string SimulReadLine()
        {
            return SimulReadLine(promptDefault);
        }

        public string SimulReadLine(string prompt)
        {
            StringBuilder cmdInput = new StringBuilder();

            int cursorYInit = Console.CursorTop;
            int cursorXTotal = 0; // like cursorleft but does not reset at new lines
            int cursorXOffset; // length of prompt before input

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

                PrintText(cmdInput.ToString(), cursorYInit, prompt);
            }
            Console.WriteLine();

            Console.CursorTop = cursorYInit + (cursorXOffset + cmdInput.Length) / Console.BufferWidth; // set cursor y to next line after last (full) line of input
            if ((cursorXOffset + cmdInput.Length) % Console.BufferWidth > 0) // move cursor y one more down if there is one last not full line of input
                Console.CursorTop++;

            string input = cmdInput.ToString();
            //PrintText(input);
            Console.WriteLine(input); //del later

            return input;
        }

        private void PrintText(string inputCache, int cursorYInit, string prompt) //use cmdinput,... instead
        {
            if (outputTextQueue.Count > 0)
            {
                int tempPosY = Console.CursorTop;
                Console.CursorTop = cursorYInit;
                for (int i = cursorYInit; i < tempPosY; i++)
                {
                    Console.WriteLine(new string(' ', Console.BufferWidth));
                }
                Console.CursorTop = tempPosY;

                foreach (string output in outputTextQueue)
                {
                    Console.WriteLine(output);
                }
                Console.Write(prompt + inputCache);
            }
        }

        public void SimulWriteLine(string text)
        {
            outputTextQueue.Enqueue(text);
        }
    }
}
