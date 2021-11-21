using System;
using System.Collections.Generic;
using System.Text;

namespace Reminder.src
{
    //taking the commands from the cmd and delegating logic to remindermanager and output to outputtextwriter
    // problem: remindermanager has some standalone tasks like the welcome message at program start, should it be called from here (not fitting) or separately from main
    class CommandExecutor
    {
        private OutputTextWriter writer;
        private ReminderManager reminderMgr;

        public CommandExecutor(OutputTextWriter outputTextWriter, ReminderManager reminderManager)
        {
            writer = outputTextWriter;
            reminderMgr = reminderManager;
        }

        public void Execute(String cmd)
        {
            switch (cmd) {
                case "t":
                    break;
                case "":
                    break;
                default:
                    writer.ShowError(0);
                    break;
            }
        }
    }
}
