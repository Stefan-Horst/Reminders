using System;
using System.Collections.Generic;
using System.Text;

namespace Reminders.src
{
    class OutputWriter : IOutputWriter //maybe let everybody call this class and only this then calls outputtextwriter? or just combine both output classes (modularity?)?
    {
        public Queue<string> OutputTextQueue => new Queue<string>();

        private int sleepTime = 1000 * 5; // 5 seconds
        private ReminderManager reminderManager;

        public OutputWriter(ReminderManager reminderManager)
        {
            this.reminderManager = reminderManager;

            Run(); //start loop here or in program?
        }

        public void Run()
        {
            while (true)
            {
                while (OutputTextQueue.Count > 0)
                    Console.WriteLine(OutputTextQueue.Dequeue());

                //check for reminders
                reminderManager.GetDueReminders(DateTime.Now).ForEach(i => Console.WriteLine(i.ToString()));

                System.Threading.Thread.Sleep(sleepTime);
            }
        }
    }
}
