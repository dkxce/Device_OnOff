//
// Author: https://github.com/dkxce
// .Net 2.0+
// WinXP+
// 

using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace device_onoff
{
    internal class Conso1e
    {
        private static Thread inputThread;
        private static AutoResetEvent getInput, gotInput;
        private static string input;

        static Conso1e()
        {
            getInput = new AutoResetEvent(false);
            gotInput = new AutoResetEvent(false);
            inputThread = new Thread(reader);
            inputThread.IsBackground = true;
            inputThread.Start();
        }

        private static void reader()
        {
            while (true)
            {
                getInput.WaitOne();
                //input = Console.ReadLine(); // normally

                /* FOR ANY abracadabra */
                ConsoleKeyInfo key = Console.ReadKey();
                if (!char.IsControl(key.KeyChar))
                    input += key.KeyChar.ToString();
                /* FOR ANY abracadabra */

                gotInput.Set();
            }
        }

        public static string ReadLine()
        {
            return Console.ReadLine();
        }

        public static string ReadLine(int timeOutMillisecs)
        {
            getInput.Set();
            bool success = gotInput.WaitOne(timeOutMillisecs);
            if (success)
                return input;
            else
                return "";
        }
    }

    internal static class ProcWindows
    {
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        public static void ShowLaunched()
        {
            string currProcName = System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location);
            Process[] procs = Process.GetProcessesByName(currProcName);
            foreach (Process proc in procs)
                if ((proc.MainWindowHandle != IntPtr.Zero) && (proc.MainWindowHandle != Process.GetCurrentProcess().MainWindowHandle))
                {
                    ShowWindow(proc.MainWindowHandle, 5);
                    SetForegroundWindow(proc.MainWindowHandle);
                    break;
                };
        }

        public static void Kill()
        {
            string currProcName = System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location);
            Process[] procs = Process.GetProcessesByName(currProcName);
            foreach (Process proc in procs)
                if ((proc.MainWindowHandle != IntPtr.Zero) && (proc.MainWindowHandle != Process.GetCurrentProcess().MainWindowHandle))
                {
                    try { TerminateProcess(proc.Handle, 0); } catch {};
                    try { proc.Kill(); } catch { };
                    break;
                };
        }
    }
}
