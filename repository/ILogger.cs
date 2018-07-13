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

    /// <summary>
    /// Класс поддельного логгера с целью его подставки для тестирования
    /// </summary>
    public class FakeLogger : ILogger {
        private string _lastLogMessage;

        /// <summary>
        /// Метод для записи последнего лога в поле _lastLogMessage
        /// </summary>
        public void LogRecord(string textLog) {
            _lastLogMessage = textLog;
        }

        /// <summary>
        /// Метод для получения последнего лога
        /// </summary>
        public string GetLastLogMessage() {
            return _lastLogMessage;
        }
    }

    
}