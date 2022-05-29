using System;

namespace Reminders.src
{
    class TextProvider : ITextProvider
    {
        private IOutputWriter outputWriter;
        private ReminderManager reminderManager;

        public TextProvider(IOutputWriter outputWriter, ReminderManager reminderManager)
        {
            SetOutputWriter(outputWriter);
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
