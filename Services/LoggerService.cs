using System;
using System.IO;

namespace AnnuaireEntreprise.Services
{
    public static class LoggerService
    {
        public static void Log(string message)
        {
            try
            {
                string filePath = Path.Combine(FileSystem.AppDataDirectory, "logs.txt");
                File.AppendAllText(filePath, $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] {message}{Environment.NewLine}");
            }
            catch { }
        }
    }
}