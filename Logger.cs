using System;
using System.IO;

namespace ClassLibrary3
{
    public static class Logger
    {
        private static string logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "revit_ai_log.txt"
        );

        public static void Log(string message)
        {
            string log = $"[{DateTime.Now:HH:mm:ss}] {message}";

            // Write to Visual Studio Output
            System.Diagnostics.Debug.WriteLine(log);

            // Write to file (VERY IMPORTANT for Revit)
            File.AppendAllText(logPath, log + Environment.NewLine);
        }
    }
}