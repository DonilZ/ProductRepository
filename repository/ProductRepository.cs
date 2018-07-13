using System;
using System.Collections.Generic;
using System.Linq;

namespace repository {

    ///<summary>
    ///Класс, имитирующий работу с базой данных (хранение версий и продуктов, получение необходимой информации о версиях и продуктах, добавление, обновление и удаление версий и продуктов) 
    ///</summary>
    public class ProductRepository {
        private static ProductRepository _singletonInstance; /*это ведь как раз ссылка на singleton? Тогда лучше _singletonInstance*/
        /*
         * ПРАВКИ:
         * 1) Поменял имя static-поля на _singletonInstance
         */
        private List<Version> _versions;
        private List<Product> _products;

        /*
         * Никак не могу понять, зачем здесь отдельный список версий? Ведь они внутри продуктов есть! Понятно, что это могут быть ссылки на
         * одни и те же объекты, но ведь их нужно держать в согласованном состоянии. В случае с EntityFramework например, это можно делать,
         * так как он сам за согласованностью следит, но в случае со спиками - зачем так сложно? Я пока просто прокомментарил отдельные фрагменты
         * в коде этого класса, но на этот момент внимания не обращал. Можешь или сам попробовать поправить, или оставить до следующей итерации.
         */

        /*
         * ПРАВКИ:
         * 1) Решили оставить как есть. 
         */


        private ILogger _logger; /*логгер и лог - это разные вещи. здесь - логгер*/

        /*ПРАВКИ:
         * 1) Исправил названия поля _log на _logger. Аналогично с переменными-параметрами и локальными переменными.
         */

        private ProductRepository(ILogger logger) {
            _versions = new List<Version>();
            _products = new List<Product>();
            _logger = logger;
        }

        /// <summary>
        /// Метод для установки значения поля _logger
        /// </summary>
        public void SetLogger (ILogger logger) {
            _logger = logger;
        }

        /*
         ВОПРОС:
         1) Столкнулся с небольшой проблемой, касающейся тестирования сообщений в логе. Я создал подделку FakeLogger,
         где запоминается последнее полученное сообщение. Но так как в классе ProductRepository мы реализовали шаблон
         Singleton, то я не могу создать еще один объект класса ProducRepository, передав туда в качестве параметра
         Логгер-подделку. Поэтому я создал новый метод SetLogger, благодаря которому я смогу заменить консольный логгер
         на логгер-подделку. Является ли это правильным решением? 
        */

        /// <summary>
        /// Метод для создания объекта класса ProductRepository (если объект уже был создан, то метод вернет этот объект)
        /// </summary>
        public static ProductRepository GetInstance(ILogger logger) {

            /*Лучше назвать мето GetInstance() или Instance(). Во-первых, тут не всегда создается инстанс. Только если его нет. Во-вторых, снаружи
             код может и не знать что так есть БД, она создается и т.п. Есть репозиторий и он возвращает данные.*/

            /*
             * ПРАВКИ: 
             * 1) Изменил название метода CreateDatabase() на GetInstance()
             */

            return _singletonInstance = _singletonInstance ?? new ProductRepository(logger); 
        }

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
        }

        /// <summary>
        /// Метод для получения конкретной версии продукта
        /// </summary>
        public Version GetProductConcreteVersion(string productName, string productVersion) {
            List<Version> currentVersions = GetProductVersions(productName);

            return currentVersions?.Where(version => version.ProductVersion == productVersion).SingleOrDefault();
        }

        /// <summary>
        /// Метод для добавления версии (и, возможно, продукта) в репозиторий
        /// </summary>
        public void AddVersion(Version newVersion) {
            /*
             * Мне не очень понятно, зачем в API репозитория есть единственный метод Add с такой сигнатурой. Получается, что для добавления новой версии
             * уже существующего продукта пользователь API все-равно должен создать экземпляр Product. А в итоге это бесполезное действие. Я бы здесь 
             * лучше подавал только Version, а Product при необходимость создавал прямо здесь. Ну или можно сделать два разных метода в API.
             */

            /*
             * ПРАВКИ:
             * 1) Убрал второй параметр типа Product из метода AddVersion. Теперь при необходимости новый продукт создается внутри метода
             * AddVersion класса ProductRepository. И, как следствие, метод AddVersion в классе UserShell стал короче 
             * (т.к. больше нет необходимости создавать новый продукт там).
             */


            /*
             * Попробуй в коде этого метода применить рефакторинг "invert if". В случае с первым if-ом нужно просто проверять отрицание того же условия,  
             * а потом писать в лог и делать return. В этом случае код в ветке else можно уже в нее не оборачивать, а писать на том же уровне. Код будет менее 
             * вложенным.
             */

            /*
             * ПРАВКИ:
             * 1) Применил рефакторинг "invert if", сократил в двух местах вложенность кода. Остался небольшой вопрос, поэтому предшествующий код пока не удалил
             *
             * ВОПРОСЫ:
             * 1) Вы указали мне на случай с конструкцией (1). Также я инвертировал условие еще в одном месте (2), но условие получилось не очень коротким.
             * Возникает вопрос, что будет целесообразнее: вложенный if или условие с 1-2 (может быть, 3) логическими операторами?
             */

  /* (1) */ if (!IsProductVersionCorrect(newVersion.ProductVersion)) {
                _logger.LogRecord("Номер версии введен некорректно");
                return;
            }

            Product productFromDatabase = GetProduct(newVersion.ProductName);

  /* (2) */ if (productFromDatabase != null && !productFromDatabase.NewVersionIsGreaterThenLatest(newVersion)) {
                _logger.LogRecord("Данная версия не может быть добавлена, так как не является новой");
                return;
            }

            if (/*CheckUniqueProduct(newVersion)*/ productFromDatabase == null) {
                /* У тебя в условии, и далее в GetProduct происходит один и тот же перебор продуктов. Нельзя ли 
                * просто получить продукт через GetProduct, а если вернулся null, создать его здесь же добавить. В любом из двух вариантов
                * у тебя на руках будет продукт, к которому нужно просто добавить версию. Код будет короче. 
                */

                /*
                 * ПРАВКИ:
                 * 1) Действительно, глупо получилось. Изменил условие на простую проверку полученного продукта на null.
                 * В итоге оказалось, что метод CheckUniqueProduct() просто не нужен. Закомментировал его.
                */

                productFromDatabase = new Product(newVersion);

                _products.Add(productFromDatabase);
                _logger.LogRecord($"Новый продукт {newVersion.ProductName} успешно добавлен");
            }
            else {
                AddVersionToProduct(newVersion);
            }

            _versions.Add(newVersion);
                
            _logger.LogRecord($"Версия {newVersion.ProductVersion} продукта {newVersion.ProductName} успешно добавлена");

           /* if (CheckCorrectProductVersion(newVersion.ProductVersion)) {

                
                if (CheckUniqueProduct(newVersion)) { 
                    * У тебя в условии, и далее в GetProduct происходит один и тот же перебор продуктов. Нельзя ли 
                     * просто получить продукт через GetProduct, а если вернулся null, создать его здесь же добавить. В любом из двух вариантов
                     * у тебя на руках будет продукт, к которому нужно просто добавить версию. Код будет короче. 
                     *

                    Product newProduct = new Product(newVersion);

                    _products.Add(newProduct);
                    _logger.LogRecord($"Новый продукт {newVersion.ProductName} успешно добавлен");
                }
                else {
                    Product productFromDatabase = GetProduct(newVersion.ProductName);
                    
                    if (!productFromDatabase.NewVersionIsGreaterThenLatest(newVersion)) {
                       _logger.LogRecord("Данная версия не может быть добавлена, так как не является новой");
                        return;
                    }

                    AddVersionToProduct(newVersion);

                }

                _versions.Add(newVersion);
                
                _logger.LogRecord($"Версия {newVersion.ProductVersion} продукта {newVersion.ProductName} успешно добавлена");
            }
            else {
                _logger.LogRecord("Номер версии введен некорректно");
            } */ 
        }

      /*  /// <summary>
        /// Метод для проверки наличия в репозитории конкретного продукта
        /// </summary>
        private bool CheckUniqueProduct(Version newVersion) {
            /* foreach (Product CurrentProduct in Products) {
                if (CurrentProduct.GetProductName() == newVersion.ProductName) {
                    return false;
                }
            }
            return true; /

            return !_products.Any(product => product.GetProductName() == newVersion.ProductName);
            
            
             * Можно проще:
             * return !_products.Any(product => product.GetProductName() == newVersion.ProductName);
             * 
             * И еще, метод лучше переименовать в IsProductUnique()
             
            
            
             * ПРАВКИ:
             * 1) Исправил свою реализацию на !_products.Any(product => product.GetProductName() == newVersion.ProductName);
             * 2) Данный метод оказался ненужным, так как проверить уникальность продукта можно при помощи метода GetProduct(), поэтому пока закомментировал этот метод
            
    } */

        /// <summary>
        /// Метод для добавления версии в общий список всех версий конкретного продукта
        /// </summary>
        private void AddVersionToProduct(Version newVersion) {

            GetProduct(newVersion.ProductName)?.AddVersion(newVersion); 

            /*
             * Тут лучше использовать метод GetProduct, а не копипастить его код. 
             */
            
            /*
             * ПРАВКИ:
             * 1) Убрал копипаст. Использовал уже реализованный метод GetProduct(); 
             */
        }

        /// <summary>
        /// Метод для получения конкретного продукта из репозитория по имени этого продукта
        /// </summary>
        public Product GetProduct(string productName) {
            return _products.Where(product => product.GetProductName() == productName).SingleOrDefault();
        }

        /// <summary>
        /// Метод для обновления конкретной версии в репозитории
        /// </summary>
        public void UpdateVersion(Version updatedVersion) {


            Version currentVersion = _versions.Where(version => version.ProductName == updatedVersion.ProductName && 
                                                    version.ProductVersion == updatedVersion.ProductVersion).SingleOrDefault();

            if (currentVersion == null) {
                _logger.LogRecord("Обновляемой версии продукта не существует!");
                return;
            }
            
            currentVersion.Update(updatedVersion);
            _logger.LogRecord($"Версия {currentVersion.ProductVersion} продукта {currentVersion.ProductName} успешно обновлена");

            /*Здесь тоже можно было бы немного "упростить" код, если сначала проверить currentVersion == null. Тогда бы внутри if было только две строки кода, а не 3*/

            /*
             * ПРАВКИ:
             * 1) Использовал "invert if" и сократил тело условного оператора. 
             */
        }

       
        /// <summary>
        /// Метод для удаления конкретной версии (и, возможно, продукта) из репозитория
        /// </summary>
        public void RemoveVersion(string productName, string productVersion) {
            Version versionToRemove = GetProductConcreteVersion(productName, productVersion); /*лучше переменную назвать versionToRemove*/

            

            if (versionToRemove == null || !_versions.Contains(versionToRemove)) {
                _logger.LogRecord("Удаляемой версии продукта не существует!");
                return;
            }

            _versions.Remove(versionToRemove);

            _logger.LogRecord($"Версия {productVersion} продукта {productName} успешно удалена");

            RemoveProduct(versionToRemove);

            /*
             * ПРАВКИ:
             * 1) Изменил название локальной переменной removedVersion на versionToRemove
             * 2) "Invert if". Предшествующий код закомментировал.
             */

           /* if (versionToRemove != null && _versions.Contains(versionToRemove)) {
                    _versions.Remove(versionToRemove);
                    _logger.LogRecord($"Версия {productVersion} продукта {productName} успешно удалена");
                    RemoveProduct(versionToRemove);
            }
            else {
                _logger.LogRecord("Удаляемой версии продукта не существует!");
            }*/
            
        }

        /// <summary>
        /// Метод для удаления конкретного продукта из репозитория
        /// </summary>
        private void RemoveProduct(Version removedVersion) {
            Product currentProduct = _products.Where(product => product.GetAllVersions().Contains(removedVersion)).SingleOrDefault();
            currentProduct.RemoveVersion(removedVersion);
            
            if (!currentProduct.GetAllVersions().Any()) {
                _products.Remove(currentProduct);
                _logger.LogRecord($"Продукт {currentProduct.GetProductName()} успешно удален");
            }
        }
        
        /*
         * CheckCorrectProductVersion переводится как "проверить корректную версию продукта", а ты хочешь проверить версию на корректность. Так что, 
         * лучше IsProductVersionCorrect(string productVersion);
         */

        /*
         * ПРАВКИ:
         * 1) Изменил название метода CheckCorrectProductVersion на IsProductVersionCorrect
        */

        

        /// <summary>
        /// Метод для проверки корректности введенного номера версии
        /// </summary>
        public bool IsProductVersionCorrect(string productVersion) {
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

        /// <summary>
        /// Метод для проверки того, находится ли текущий продукт в репозитории
        /// </summary>
        public bool IsThereProduct(Product product) {
            return _products.Contains(product);
        }

        /// <summary>
        /// Метод для проверки того, находится ли текущая версия в репозитории
        /// </summary>
        public bool IsThereVersion(Version version) {
            return _versions.Contains(version);
        }

        /// <summary>
        /// Метод для очистки репозитория
        /// </summary>
        public void ClearProductsAndVersions(){
            _products.Clear();
            _versions.Clear();
        }

        /// <summary>
        /// Метод для получения количества уникальных продуктов в репозиотрии
        /// </summary>
        public int GetCountProducts() {
            return _products.Count;
        }

        /// <summary>
        /// Метод для получения количества версий в репозитории
        /// </summary>
        public int GetCountVersions() {
            return _versions.Count;
        }

        /*
         ПРАВКИ:
         1) Добавил публичные методы IsThereProduct, IsThereVersion, ClearProductsAndVersions, GetCountProducts, GetCountVersions
         для вызова их в тестовых методах.
        */

    }
    
}
