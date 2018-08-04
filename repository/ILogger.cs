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


    /* Фэйковый логгер хороший. И в данном случае можно было бы ограничиться и им, но бывет, что создать подобный фэйк для более-менее сложного класса
     * само по себе не просто. Поэтому обычно используют какие-нибудь готовые фрэймворки, которые позволяют создавать моки через декларативное описание. 
     * Предлагаю следующим шагом посмотреть библиотеку Moq и попробовать FakeLogger сделать с ее помощью.
     */

     /*
      * ПРАВКИ:
      * 1) Реализовал FakeLogger при помощи средств библиотеки Moq, а собственную реализацию FakeLogger убрал 
      */


    
}