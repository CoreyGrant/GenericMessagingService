using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.IntegrationTests
{
    public interface ILogger
    {
        void Log(string message, int indent = 0);
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string message, int indent = 0)
        {
            var prefixString = string.Join("\t", Enumerable.Range(0, indent).Select(x => ""));
            Console.WriteLine(prefixString + message);
        }
    }

    public class FileLogger : ILogger
    {
        private readonly FileStream logFile;

        public FileLogger(string fileName)
        {
            var directoryPath = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            this.logFile = File.OpenWrite(fileName);
            this.logFile.Position = this.logFile.Length;
        }

        public void Log(string message, int indent = 0) 
        {
            this.logFile.Write(Encoding.UTF8.GetBytes(message + "\n"));
            this.logFile.Flush();
        }
    }

    public class ComboLogger: ILogger
    {
        private readonly ILogger[] loggers;

        public ComboLogger(params ILogger[] loggers)
        {
            this.loggers = loggers;
        }

        public void Log(string message, int indent = 0)
        {
            var timestamp = DateTime.Now.ToString("yyyy/MM/dd-HH:mm.sss");
            var fullMessage = timestamp + ": " + message;
            foreach(var logger in loggers) { logger.Log(fullMessage, indent); }
        }
    }
}
