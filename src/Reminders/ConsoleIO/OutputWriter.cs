using System;
using System.Collections.Generic;
using SimultaneousConsoleIO;

namespace Reminders.ConsoleIO
{
    class OutputWriter : IOutputWriter //maybe let everybody call this class and only this then calls outputtextwriter? or just combine both output classes (modularity?)?
    {
        public Queue<string> OutputTextQueue = new Queue<string>();

        public void AddText(string text) 
        { 
            OutputTextQueue.Enqueue(text); 
        }

        public string GetText()
        {
            string s = "";

            if (OutputTextQueue.Count > 0)
            {
                while (OutputTextQueue.Count > 1)
                    s += OutputTextQueue.Dequeue() + Environment.NewLine;

                s += OutputTextQueue.Dequeue();
            }

            return s;
        }
    }
}
