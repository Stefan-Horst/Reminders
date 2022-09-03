using System;
using System.Collections.Generic;
using SimultaneousConsoleIO;

namespace Reminders.ConsoleIO
{
    class OutputWriter : IOutputWriter
    {
        private Queue<string> outputTextQueue = new Queue<string>();
        private string startText; // text which will be at beginning of all output

        public string StartText { get => startText; set => startText = value; }

        public OutputWriter(string startText = "")
        {
            this.startText = startText;
        }
        
        public void AddText(string text) 
        { 
            outputTextQueue.Enqueue(startText + text); 
        }

        public string GetText()
        {
            string s = "";

            if (outputTextQueue.Count > 0)
            {
                while (outputTextQueue.Count > 1)
                    s += outputTextQueue.Dequeue() + Environment.NewLine;

                s += outputTextQueue.Dequeue();
            }

            return s;
        }
    }
}
