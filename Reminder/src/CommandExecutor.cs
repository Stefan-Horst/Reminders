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

        private string[] tokens; //current input tokenized

        public CommandExecutor(OutputTextWriter outputTextWriter, ReminderManager reminderManager)
        {
            writer = outputTextWriter;
            reminderMgr = reminderManager;
        }

        /*
         * alle r eines zeitraums anzeigen
         * crud
         * r nach inhalt bzw stichworten suchen
         * einzelne eigenschaften eines r ändern
         * optionen ändern (zeitraum bei "startseite", [verschlüsselung])
         * hilfeseite
         */
        public void Execute(String input)
        {
            tokens = input.Split(' ');

            switch (tokens[0]) {
                case "help":
                case "h":
                    writer.ShowHelp();
                    break;

                case "read":
                case "r":
                    CmdRead();
                    break;

                case "create":
                case "c":
                    CmdCreate();
                    break;

                case "delete":
                case "d":
                    CmdDelete();
                    break;

                case "update":
                case "u":
                    CmdUpdate();
                    break;

                case "search": //search for keyword
                case "s":
                    CmdSearch();
                    break;

                case "show": // show or list reminders
                case "sh":
                    CmdShow();
                    break;

                case "settings":
                case "set": //maybe change
                    CmdSettings();
                    break;

                case "":
                    break;

                default:
                    writer.ShowError(0, "");
                    break;
            }
        }

        private void CmdRead()
        {

        }

        private void CmdCreate()
        {

        }

        private void CmdDelete()
        {

        }

        private void CmdUpdate()
        {

        }

        private void CmdSearch()
        {

        }

        private void CmdShow()
        {

        }

        private void CmdSettings()
        {

        }
    }
}
