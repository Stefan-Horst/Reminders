﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SimultaneousConsoleIO
{
    public class SimulConsoleIO
    {
        private IOutputWriter outputWriter;
        private ITextProvider textProvider;
        private string promptDefault;
        // pause time in main loop (waiting for key input or text output)
        private int sleepTime = 25; // pause as short as possible without eating cpu
        private List<string> history = new List<string>();

        public IOutputWriter OutputWriter { get => outputWriter; set => outputWriter = value; }
        public ITextProvider TextProvider { get => textProvider; set { textProvider = value; textProvider?.SetOutputWriter(outputWriter); } } // get can return null
        public string PromptDefault { get => promptDefault; set => promptDefault = value; }
        public int SleepTime { get => sleepTime; set => sleepTime = value; }

        public SimulConsoleIO(IOutputWriter outputWriter, ITextProvider textProvider = null, string promptDefault = "")
        {
            this.outputWriter = outputWriter;
            this.textProvider = textProvider;
            this.promptDefault = promptDefault;

            textProvider?.SetOutputWriter(outputWriter);
        }

        public void Write(string text)
        {
            outputWriter.AddText(text);
        }

        public void WriteLine(string text)
        {
            Write(text + Environment.NewLine);
        }

        public string ReadLine()
        {
            return ReadLine(promptDefault);
        }

        public string ReadLine(string prompt, string inputText = "")
        {
            StringBuilder cmdInput = new StringBuilder();

            int cursorYInit = Console.CursorTop;
            int cursorXTotal = 0; // like cursorleft but does not reset at new lines
            int cursorXOffset = prompt.Length; // length of prompt before input
            int index = -1; // index of history list

            Console.Write(prompt);

            if (inputText != "")
            {
                Console.Write(inputText);
                cmdInput.Append(inputText);
                cursorXTotal = cmdInput.Length;
                SetCursorEndOfInput(cursorYInit, cursorXOffset, cursorXTotal);
            }

            ConsoleKeyInfo cki = default;

            do // while (cki.Key != ConsoleKey.Enter)
            {
                if (Console.KeyAvailable)
                {
                    cki = Console.ReadKey(true);

                    // ctrl key not pressed or alt key pressed (making ctrl+alt possible which equals altgr key), prevents shortcuts like ctrl+i, but allows ones like altgr+q for @
                    if (cki.Key != ConsoleKey.Enter && ((cki.Modifiers & ConsoleModifiers.Control) == 0 || (cki.Modifiers & ConsoleModifiers.Alt) != 0))
                    {
                        if (cki.Key == ConsoleKey.Backspace)
                        {                            
                            Console.Write(cki.KeyChar);

                            Console.CursorLeft++; // counteracts console standard behaviour of moving cursor one to the left

                            // nested if so that backspace does not get added to cmdinput in else-part of statement
                            if (Console.CursorLeft > cursorXOffset || Console.CursorTop > cursorYInit)
                            {
                                bool lineFlag = (cursorXOffset + cursorXTotal) % Console.BufferWidth == 0; // signals when cursor is at first pos of line

                                // only go to line above when current line is empty (otherwise would already happen when first char in line is deleted, because cursor then is at start of line)
                                if (Console.CursorLeft == 1 && Console.CursorTop > cursorYInit && (cmdInput.Length + cursorXOffset) % Console.BufferWidth == 0)
                                {
                                    Console.CursorTop--;
                                    Console.CursorLeft = Console.BufferWidth - 1;
                                    Console.Write(" \b");
                                    Console.CursorLeft++;
                                }
                                else if (cursorXTotal != 0)
                                {
                                    Console.CursorLeft--;
                                    Console.Write(" \b");
                                }
                                else // makes it possible to backspace to start of line (cursor x = 0)
                                {
                                    Console.CursorLeft--;
                                }

                                if (cursorXTotal > 0)
                                {
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

                                        SetCursorEndOfInput(cursorYInit, cursorXOffset, cursorXTotal);
                                    }
                                }
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
                            if (history.Count > 0)
                            {
                                ClearInput(cursorYInit, cursorXOffset, cmdInput.Length);

                                if (index == -1) // jumps to first element of history at first use of up/down key
                                    index = 0;
                                else if (index > 0)
                                    index--;

                                Console.Write(history[index]);

                                cmdInput.Clear();
                                cmdInput.Append(history[index]);

                                cursorXTotal = cmdInput.Length;

                                SetCursorEndOfInput(cursorYInit, cursorXOffset, cursorXTotal);
                            }
                        }
                        else if (cki.Key == ConsoleKey.DownArrow)
                        {
                            if (history.Count > 0)
                            {
                                ClearInput(cursorYInit, cursorXOffset, cmdInput.Length);

                                if (index == -1) // jumps to last element of history at first use of up/down key
                                    index = history.Count - 1;
                                else if (index < history.Count - 1)
                                    index++;

                                Console.Write(history[index]);

                                cmdInput.Clear();
                                cmdInput.Append(history[index]);

                                cursorXTotal = cmdInput.Length;

                                SetCursorEndOfInput(cursorYInit, cursorXOffset, cursorXTotal);
                            }
                        }
                        else if (cki.Key == ConsoleKey.PageUp) //page up/down for first/last history entry
                        {
                            if (history.Count > 0)
                            {
                                ClearInput(cursorYInit, cursorXOffset, cmdInput.Length);

                                index = 0;

                                Console.Write(history[0]); // first element of history

                                cmdInput.Clear();
                                cmdInput.Append(history[0]);

                                cursorXTotal = cmdInput.Length;

                                SetCursorEndOfInput(cursorYInit, cursorXOffset, cursorXTotal);
                            }
                        }
                        else if (cki.Key == ConsoleKey.PageDown)
                        {
                            if (history.Count > 0)
                            {
                                ClearInput(cursorYInit, cursorXOffset, cmdInput.Length);

                                index = history.Count - 1;

                                Console.Write(history[^1]); // last element of history

                                cmdInput.Clear();
                                cmdInput.Append(history[^1]);

                                cursorXTotal = cmdInput.Length;

                                SetCursorEndOfInput(cursorYInit, cursorXOffset, cursorXTotal);
                            }
                        }
                        else if (cki.Key == ConsoleKey.End) //ende key
                        {
                            cursorXTotal = cmdInput.Length;

                            SetCursorEndOfInput(cursorYInit, cursorXOffset, cursorXTotal);
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
                                SetCursorEndOfInput(cursorYInit, cursorXOffset, cursorXTotal);
                            }
                        }
                        else if (cki.Key == ConsoleKey.Escape)
                        {
                            Console.CursorTop = cursorYInit;
                            Console.CursorLeft = cursorXOffset;

                            Console.Write("0"); // control char for esc to "eat"
                            Console.Write("\b" + new string(' ', cmdInput.Length + 1)); // clear area of input

                            Console.CursorTop = cursorYInit;
                            Console.CursorLeft = cursorXOffset;

                            history.Add(cmdInput.ToString()); // feature might not be desired

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
                            Console.Write(cki.KeyChar);
                            
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
                }
                textProvider?.CheckForText();
                
                PrintText(cmdInput.ToString(), cursorYInit, prompt, cursorXOffset, cursorXTotal); // write text to console "while" getting user input

                cursorYInit = Console.CursorTop - (cursorXOffset + cursorXTotal) / Console.BufferWidth; // changes value relative to changes to cursortop caused by cmd window resizing

                Thread.Sleep(sleepTime);
            }
            while (cki.Key != ConsoleKey.Enter);

            cursorYInit = Console.CursorTop - (cursorXOffset + cursorXTotal) / Console.BufferWidth; // changes value relative to changes to cursortop caused by cmd window resizing

            Console.WriteLine();

            Console.CursorTop = cursorYInit + (cursorXOffset + cmdInput.Length) / Console.BufferWidth; // set cursor y to next line after last (full) line of input
            if ((cursorXOffset + cmdInput.Length) % Console.BufferWidth > 0) // move cursor y one more down if there is one last not full line of input
                Console.CursorTop++;

            history.Add(cmdInput.ToString());

            return cmdInput.ToString();
        }

        // allows printing all all queued output; useful before stopping program etc.
        public void ForcePrintQueue()
        {
            Console.Write(outputWriter.GetText());
        }

        // sets cursor position to end of user input (behind last character)
        private void SetCursorEndOfInput(int cursorYInit, int cursorXOffset, int cursorXTotal)
        {
            Console.CursorTop = cursorYInit + (cursorXTotal + cursorXOffset) / Console.BufferWidth; // '/' discards remainder
            Console.CursorLeft = (cursorXTotal + cursorXOffset) % Console.BufferWidth;
        }

        // deletes all user input and returns cursor to starting position
        private void ClearInput(int cursorYInit, int cursorXOffset, int inputLength)
        {
            Console.CursorTop = cursorYInit;
            Console.CursorLeft = cursorXOffset;

            Console.Write(new string(' ', inputLength + 1)); // clear area of input

            Console.CursorTop = cursorYInit;
            Console.CursorLeft = cursorXOffset;
        }

        // writes all output cached in the outputwriter to the console, returns true if any text was printed, otherwise returns false
        private void PrintText(string inputCache, int cursorYInit, string prompt, int cursorXOffset, int cursorXTotal)
        {
            string output = outputWriter.GetText();

            if (output.Length > 0)
            {
                // set to cursor y pos last line of input
                int tempPosY = cursorYInit + (cursorXOffset + inputCache.Length) / Console.BufferWidth;
                if ((cursorXOffset + inputCache.Length) % Console.BufferWidth > 0)
                    tempPosY++;

                Console.CursorTop = cursorYInit;
                Console.CursorLeft = 0;
                for (int i = cursorYInit; i <= tempPosY; i++) // clear current user input
                {
                    Console.WriteLine(new string(' ', Console.BufferWidth));
                }
                Console.CursorTop = cursorYInit;

                Console.Write(output);

                tempPosY = Console.CursorTop;
                int tempPosX = Console.CursorLeft;

                Console.Write(prompt + inputCache);

                Console.CursorTop = tempPosY + (cursorXTotal + cursorXOffset) / Console.BufferWidth; // '/' discards remainder
                Console.CursorLeft = tempPosX + (cursorXTotal + cursorXOffset) % Console.BufferWidth;
            }
        }
    }
}
