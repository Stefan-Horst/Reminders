using System;
using System.Text;
using Reminders.ConsoleIO;
using Reminders.WinApi;
using SimultaneousConsoleIO;

namespace Reminders
{
    public static class Program
    {
        private static IOutputWriter outputWriter;
        private static OutputTextWriter writer;
        private static ReminderManager reminderMgr;
        private static CommandExecutor cmdExec;
        private static SimulConsoleIO simio;

        private const string Prompt = ">>> ";

        public static void Main()
        {
            Init();

            writer.ShowWelcome();
            if (reminderMgr.FileMgr.UpcomingDays == -1) // show all (non-read) reminders
                writer.ShowWelcomeReminders(reminderMgr.FileMgr.UpcomingDays, reminderMgr.Reminders.FindAll(r => r.Read == false));
            else
                writer.ShowWelcomeReminders(reminderMgr.FileMgr.UpcomingDays, reminderMgr.GetRemindersDueInTimespan(DateTime.Today, DateTime.Today.AddDays(reminderMgr.FileMgr.UpcomingDays)).FindAll(r => r.Read == false));
            
            while (true) // main program loop
                cmdExec.Execute(simio.ReadLine(Prompt));
        }

        private static void Init()
        {
            Console.OutputEncoding = Encoding.Unicode;

            outputWriter = new OutputWriter();
            simio = new SimulConsoleIO(outputWriter);
            writer = new OutputTextWriter(simio);
            reminderMgr = new ReminderManager(writer);
            simio.TextProvider = new TextProvider(reminderMgr);
            cmdExec = new CommandExecutor(writer, reminderMgr);
            
            if (reminderMgr.FileMgr.Quickedit == false)
            {
                if (! DisableQuickEdit.Disable())
                    writer.Log(LogType.Error, "disabling quickedit failed"); //give user info that console wont get updated if he clicks anywhere in it
                else
                    writer.Log(LogType.Info, "quickedit disabled");
            }
            else
            {
                writer.Log(LogType.Info, "quickedit enabled");
            }
        }
    }
}
