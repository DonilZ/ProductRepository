using System;
using System.Collections.Generic;
using System.Linq;

namespace repository {

    interface ILogger {
        void LogRecord(string textLog);
    }

    class ConsoleLogger : ILogger {
        public void LogRecord(string textLog) {
            Console.WriteLine(textLog);
        }
    }

    
}