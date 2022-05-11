using System;
using System.Collections.Generic;
using System.Text;

namespace Reminders.src
{
    interface IOutputWriter
    {
        Queue<string> OutputTextQueue { get; }

        public void AddText(string text) { OutputTextQueue.Enqueue(text); }
    }
}
