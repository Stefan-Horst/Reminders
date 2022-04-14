using System;
using System.Runtime.InteropServices;

namespace Reminders.src
{
    class NotificationWindow
    {
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr h, string message, string title, int type);

        // shows "i" in circle (also produces beep sound)
        private const int MessageIcon = 0x00000040;         // MB_ICONINFORMATION
        // displays window in foreground
        private const int MessageForeground = 0x00010000;   // MB_SETFOREGROUND

        private string windowTitle = "Reminders";

        public NotificationWindow() { }

        public void Display(string message)
        {
            MessageBox((IntPtr)0, message, windowTitle, MessageIcon | MessageForeground); //should window be in foreground or is it annoying while gaming etc?
        }
    }
}
