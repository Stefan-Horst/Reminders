using System;
using System.Collections.Generic;
using System.Text;

namespace Reminder.src
{
    //taking the commands from the cmd and delegating logic to remindermanager and output to outputtextwriter
    // problem: remindermanager has some standalone tasks like the welcome message at program start, should it be called from here (not fitting) or separately from main
    class CommandExecuter
    {
        private OutputTextWriter outputTextWriter;

        public CommandExecuter()
        {
            outputTextWriter = new OutputTextWriter();
        }

        public void Execute(String cmd)
        {
            switch (cmd) {
                case "t":
                    break;
                default:
                    outputTextWriter.ShowError(0);
                    break;
            }
        }
    }
}
