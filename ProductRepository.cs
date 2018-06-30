using System;
using System.Collections.Generic;
using System.Linq;

namespace repository {

    /*
     * У тебя класс DatabaseManager получился почти реалиазией шаблона Репозиторий, т.к. он инкапсулирует в себе запросную логику и предоставляет наружу более 
     * простой интерфейс для работы с данными. Это хорошо! Иногда такие объекты называют репозиториями (в твоем случае - ProductRepository), иногда - сервисами. Сервис, конечно,
     * немного более широкое понятие, чем репозиторий; репозиторий может быть лишь частью сервиса. Но можно и сервисом назвать - ProductService. Предлагаю выбрать
     * одно из этих двух названий, т.к. DatabaseManager - это что-то, что управляет БД, в смысле создания, резервного копирования и т.п. 
     */

    /*
     * ПРАВКИ:
     * 1) Поменял название класса DatabaseManager на ProductRepository 
    */

    ///<summary>
    ///Класс, имитирующий работу с базой данных (хранение версий и продуктов, получение необходимой информации о версиях и продуктах, добавление, обновление и удаление версий и продуктов) 
    ///</summary>
    class ProductRepository {
        private static ProductRepository _instanceDatabase;
        private List<Version> _versions;
        private List<Product> _products;
        private ILogger _log;

        private ProductRepository(ILogger log) {
            _versions = new List<Version>();
            _products = new List<Product>();
            _log = log;
        }

        /// <summary>
        /// Метод для создания объекта класса ProductRepository (если объект уже был создан, то метод вернет этот объект)
        /// </summary>
        public static ProductRepository CreateDatabase(ILogger log) {
            return _instanceDatabase = _instanceDatabase ?? new ProductRepository(log); 
            /*
             * Для такой логики есть более короткий способ записи: return InstanceDatabase = InstanceDatabase ?? new ProductRepository(); 
             * Просто для общего развития погляди, плс.
             */

            /*
             * ПРАВКИ:
             * 1) Заменил свою реализацию с if на нулевой коалесцирующий оператор
             */
        }

        /*
         * Мне кажется, термин Information у тебя лишний здесь. Сама модель - это продукты и версии. Понятно, что это не сами продукты, а информация о них. Но, все в
         * информационных системах - лишь информация об объектах. Так что, в данном случае это не добавляет ничего к пониманию, а код удлинняет. К тому же, усложняет перевод на 
         * англиский. Предлагаю все методы переименовать на GetProducts(), GetProductVersions() и т.п.
         * 
         * И еще один момент - в данном случае слово Get можно тоже опустить. Потому что понятно, что если ты пишешь dbInstance.Products(), то это и есть получение всех продуктов.
         * В некоторых книгах есть такие советы. Хотя есть и те, кто строго требует использования слова Get в данном случае.
         */



         /*
          * ПРАВКИ:
          * 1) Убрал Information из названий методов.
          * 2) Все же Get мне хотелось бы оставить, так как, например, метод с названием Products мне не очень нравится.
          */

        /// <summary>
        /// Метод для получения общего списка всех продуктов, находящихся в репозитории
        /// </summary>
        public List<Product> GetProducts() {
            return _products;
        }

        /// <summary>
        /// Метод для получения общего списка всех версий конкретного продукта
        /// </summary>
        public List<Version> GetProductVersions(string productName) { 
            return _products.Where(product => product.GetProductName() == productName).SingleOrDefault()?.GetAllVersions();

            /*
             * С использованием linq этот код можно записать в одну строку:
             * 
             *  return Products.Where(product => product.GetProductName() == ProductName).SingleOrDefault()?.GetAllVersions();
             *  
             * Так как мы активно используем linq, то предлагаю немного его освоить. И еще, здесь используется оператор ?. - Это чтобы 
             * в случае отсутствия объектов вернулся null, а не вылетело null reference exception.
             */



             /*
              * ПРАВКИ:
              * 1) Сократил до одной строчки (использовав linq и тернарные операторы) места, в которых происходил проход по списку с помощью цикла foreach и выбор определенного элемента.
              * 
              * ВОПРОСЫ:
              * 1) Оператор ? (конкретно в выражении return Products.Where(product => product.GetProductName() == ProductName).SingleOrDefault()?.GetAllVersions();) 
              * после подвыражения - это просто, грубо говоря, тернарный оператор, проверяющий, не является ли подвыражение перед ним null?
              * 2) Проверьте пожалуйста, ничего ли я не упустил, вставляя linq (предшествующий код я закомментировал) в тех местах, где до этого использовался цикл foreach.
             */
        }

        /// <summary>
        /// Метод для получения конкретной версии продукта
        /// </summary>
        public Version GetProductConcreteVersion(string productName, string productVersion) {
            List<Version> currentVersions = GetProductVersions(productName);

            /* if (CurrentVersions != null) {          
                foreach (Version CurrentVersion in CurrentVersions) {
                    if (ProductVersion == CurrentVersion.ProductVersion) {
                        return CurrentVersion;
                    }
                }
            }
            
            return null; */

            return currentVersions?.Where(version => version.ProductVersion == productVersion).SingleOrDefault();
        }

        /// <summary>
        /// Метод для добавления версии (и, возможно, продукта) в репозиторий
        /// </summary>
        public void AddVersion(Version addedVersion, Product addedProduct) {
            if (CheckCorrectProductVersion(addedVersion.ProductVersion)) {

                
                if (CheckUniqueProduct(addedVersion)) {
                    _products.Add(addedProduct);
                    _log.LogRecord($"Новый продукт {addedVersion.ProductName} успешно добавлен");
                }
                else {
                    Product productFromDatabase = GetProduct(addedProduct.GetProductName());
                    
                    if (!productFromDatabase.CheckAddedVersion(addedVersion)) {
                       _log.LogRecord("Данная версия не может быть добавлена, так как не является новой");
                        return;
                    }

                    AddVersionToProduct(addedVersion);

                }

                _versions.Add(addedVersion);
                
                _log.LogRecord($"Версия {addedVersion.ProductVersion} продукта {addedVersion.ProductName} успешно добавлена");
            }
            else {
                _log.LogRecord("Номер версии введен некорректно");
            }
        }

        /// <summary>
        /// Метод для проверки наличия в репозитории конкретного продукта
        /// </summary>
        private bool CheckUniqueProduct(Version addedVersion) {
            /* foreach (Product CurrentProduct in Products) {
                if (CurrentProduct.GetProductName() == AddedVersion.ProductName) {
                    return false;
                }
            }
            return true; */

            return _products.Where(product => product.GetProductName() == addedVersion.ProductName).SingleOrDefault() == null ? true : false;
        }

        /// <summary>
        /// Метод для добавления версии в общий список всех версий конкретного продукта
        /// </summary>
        private void AddVersionToProduct(Version addedVersion) {
            /* foreach (Product CurrentProduct in Products) {
                if (CurrentProduct.GetProductName() == AddedVersion.ProductName) {
                    CurrentProduct.AddVersion(AddedVersion);
                    return;
                }
            } */

            _products.Where(product => product.GetProductName() == addedVersion.ProductName).SingleOrDefault()?.AddVersion(addedVersion); 
        }

        /// <summary>
        /// Метод для получения конкретного продукта из репозитория по имени этого продукта
        /// </summary>
        private Product GetProduct(string productName) {
            /*for(int i = 0; i < Products.Count; ++i) {
                if (Products[i].GetProductName() == ProductName) {
                    return Products[i];
                }
            }

            return null;*/

            return _products.Where(product => product.GetProductName() == productName).SingleOrDefault();
        }

        /// <summary>
        /// Метод для обновления конкретной версии в репозитории
        /// </summary>
        public void UpdateVersion(Version updatedVersion) {
            /* foreach (Version CurrentVersion in Versions) {
                if (UpdatedVersion.ProductName == CurrentVersion.ProductName && UpdatedVersion.ProductVersion == CurrentVersion.ProductVersion) { 
                    CurrentVersion.Update(UpdatedVersion);

                    _log.LogRecord($"Версия {CurrentVersion.ProductVersion} продукта {CurrentVersion.ProductName} успешно обновлена");
                    return;
                }
            }
            
            _log.LogRecord("Обновляемой версии продукта не существует"); */

            Version currentVersion = _versions.Where(version => version.ProductName == updatedVersion.ProductName && 
                                                    version.ProductVersion == updatedVersion.ProductVersion).SingleOrDefault();
            if (currentVersion != null) {
                currentVersion.Update(updatedVersion);
                _log.LogRecord($"Версия {currentVersion.ProductVersion} продукта {currentVersion.ProductName} успешно обновлена");
                return;
            }

            _log.LogRecord("Обновляемой версии продукта не существует");

        }

        /*
         * Обычно в web-приложениях такие сервисы - это часть бэкэнда. А он может не быть консольным приложение, так что консоли там может просто не оказаться. Но 
         * сама идея записывать информацию об изменениях - правильная. Просто ее нужно писать в лог. Я тебе предлагаю при создании класса передать ему объект, реализцющий интерфейс 
         * ILogger с методом Log(), и в качестве реализации использовать обертку над консолью. Что-то вроде: 
         * class ConsoleLogger: ILogger
         * {
         *    public void Log(string logText) {
         *      Console.WriteLine(logText);
         *    }
         * }
         * 
         * Такой подход позволит тебе потом вместо консольного логгера использовать файловый логгер, например. 
         */



        /*
         * ПРАВКИ:
         * 1) Добавил интерфейс ILogger и реализующий его класс ConsoleLogger. В класс ProductRepository теперь дополнительно передается один параметр
         * типа ILogger, и сообщения об изменениях теперь записываются в лог с помощью функции интерфейса LogRecord(), а не напрямую в консоль.
        */

        /// <summary>
        /// Метод для удаления конкретной версии (и, возможно, продукта) из репозитория
        /// </summary>
        public void RemoveVersion(string productName, string productVersion) {
            Version removedVersion = GetProductConcreteVersion(productName, productVersion);

            if (removedVersion != null && _versions.Contains(removedVersion)) {
                    _versions.Remove(removedVersion);
                    _log.LogRecord($"Версия {productVersion} продукта {productName} успешно удалена");
                    RemoveProduct(removedVersion);
            }
            else {
                _log.LogRecord("Удаляемой версии продукта не существует!");
            }
            
        }

        /// <summary>
        /// Метод для удаления конкретного продукта из репозитория
        /// </summary>
        private void RemoveProduct(Version removedVersion) {
            /* foreach (Product CurrentProduct in Products) {
                if (CurrentProduct.GetAllVersions().Contains(RemovedVersion)) {
                    CurrentProduct.RemoveVersion(RemovedVersion);
                    if (CurrentProduct.GetAllVersions().Count == 0) {
                        Products.Remove(CurrentProduct);
                        _log.LogRecord($"Продукт {CurrentProduct.GetProductName()} успешно удален");
                        break;
                    }
                }
            } */

            Product currentProduct = _products.Where(product => product.GetAllVersions().Contains(removedVersion)).SingleOrDefault();
            currentProduct.RemoveVersion(removedVersion);
            
            if (!currentProduct.GetAllVersions().Any()) {
                _products.Remove(currentProduct);
                _log.LogRecord($"Продукт {currentProduct.GetProductName()} успешно удален");
            }
        }
        
        /// <summary>
        /// Метод для проверки корректности введенного номера версии
        /// </summary>
        private bool CheckCorrectProductVersion(string productVersion) {
            try {
                string[] splitedVersion = productVersion.Split('.');

                if (splitedVersion.Length != 3) return false;

                int summedNumbersVersion = 0;

                for (int i = 0; i < 3; ++i) {
                    summedNumbersVersion += Convert.ToInt32(splitedVersion[i]);
                }

                return true;
            }
            catch {
                return false;
            }
        }

    }
    
}
