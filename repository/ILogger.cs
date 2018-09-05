using System;
using System.Collections.Generic;
using System.Linq;

namespace repository {

    /// <summary>
    /// Интерфейс для возможности реализации различных логгеров
    /// </summary>
    public interface ILogger {
        /// <summary>
        /// Метод для записи логов
        /// </summary>
        void LogRecord(string textLog);
    }

    /// <summary>
    /// Класс консольного логгера для логирования в консоль
    /// </summary>
    public class ConsoleLogger : ILogger {
        /// <summary>
        /// Метод для записи логов в консоль
        /// </summary>
        public void LogRecord(string textLog) {
            Console.WriteLine(textLog);
        }
    }
    
}