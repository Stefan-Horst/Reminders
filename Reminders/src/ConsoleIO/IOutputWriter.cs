﻿
namespace Reminders.src
{
    interface IOutputWriter
    {
        public void AddText(string text);

        //public string GetText();

        public void UpdateTempData(string inputCache, int cursorYInit);
    }
}