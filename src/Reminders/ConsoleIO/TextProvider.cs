using System;
using SimultaneousConsoleIO;

namespace Reminders.ConsoleIO
{
    class TextProvider : ITextProvider
    {
        private IOutputWriter outputWriter;
        private ReminderManager reminderManager;

        public TextProvider(ReminderManager reminderManager)
        {
            this.reminderManager = reminderManager;
        }

        public void SetOutputWriter(IOutputWriter outputWriter)
        {
            this.outputWriter = outputWriter;
        }

        public void CheckForText()
        {
            reminderManager.GetDueReminders(DateTime.Now).ForEach(i => outputWriter.AddText(i.ToString()));
        }
    }
}
