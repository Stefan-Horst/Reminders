using System;
using System.Collections.Generic;
using System.Text;

namespace Reminders.src
{
    class TextProvider : ITextProvider
    {
        private IOutputWriter outputWriter;
        private ReminderManager reminderManager;

        public TextProvider(IOutputWriter outputWriter)
        {
            this.outputWriter = outputWriter;
        }

        public void Run()
        {
            while (true)
            {
                reminderManager.GetDueReminders(DateTime.Now).ForEach(i => outputWriter.AddText(i.ToString()));

                //System.Threading.Thread.Sleep(sleepTime);
            }
        }
    }
}
