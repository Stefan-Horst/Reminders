﻿using System;
using System.Runtime.InteropServices;

namespace Reminders.WinApi
{
    public static class DisableQuickEdit
    {
        private const uint EnableQuickEdit = 0x0040;

        // STD_INPUT_HANDLE (DWORD): -10 is the standard input device.
        private const int StdInputHandle = -10;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        public static bool Disable()
        {
            IntPtr consoleHandle = GetStdHandle(StdInputHandle);

            // get current console mode
            if (! GetConsoleMode(consoleHandle, out uint consoleMode))
            {
                // ERROR: Unable to get console mode.
                return false;
            }

            // Clear the quick edit bit in the mode flags
            consoleMode &= ~EnableQuickEdit; // bitwise not operator to disable quickedit

            // set the new mode
            if (! SetConsoleMode(consoleHandle, consoleMode))
            {
                // ERROR: Unable to set console mode
                return false;
            }

            return true;
        }
    }
}
