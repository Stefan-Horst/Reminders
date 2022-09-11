using System;
using System.Runtime.InteropServices;

namespace Reminders.WinApi
{
    public static class NotificationWindow
    {
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        private static extern int MessageBox(IntPtr h, string message, string title, int type);

        // shows "i" in circle (also produces beep sound)
        private const int MessageIcon = 0x00000040;         // MB_ICONINFORMATION
        // displays window in foreground
        private const int MessageForeground = 0x00010000;   // MB_SETFOREGROUND

        private const string WindowTitle = "Reminders";

        public static void Display(string message)
        {
            MessageBox((IntPtr)0, message, WindowTitle, MessageIcon | MessageForeground);
        }
    }
}
