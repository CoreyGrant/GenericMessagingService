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
        private readonly string fileName;

        public FileLogger(string fileName)
        {
            this.fileName = fileName;
        }

        public void Log(string message, int indent = 0) { }
    }
}
