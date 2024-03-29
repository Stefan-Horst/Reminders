﻿using System;
using SimultaneousConsoleIO;

namespace Reminders.ConsoleIO
{
    public class TextProvider : ITextProvider
    {
        private IOutputWriter outputWriter;
        private ReminderManager reminderManager;

        public TextProvider(ReminderManager reminderManager)
        {
            this.reminderManager = reminderManager;
        }

        public void SetOutputWriter(IOutputWriter newOutputWriter)
        {
            outputWriter = newOutputWriter;
        }

        public void CheckForText()
        {
            string output = reminderManager.GetDueRemindersOutput();
            if (output != "")
                outputWriter.AddText(output + Environment.NewLine);
        }
    }
}
