using System;
using System.Collections.Generic;
using System.Linq;

namespace repository {

    ///<summary>
    ///Класс, имитирующий работу с базой данных (хранение версий и продуктов, получение необходимой информации о версиях и продуктах, добавление, обновление и удаление версий и продуктов) 
    ///</summary>
    public class ProductRepository {
        private static ProductRepository _singletonInstance; /*это ведь как раз ссылка на singleton? Тогда лучше _singletonInstance*/
        private List<Version> _versions;
        private List<Product> _products;

        private ILogger _logger; 

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

        /*
         * Думаю, что более "чистым" решением здесь было бы отказаться от синглтонов вообще и сделать все через обычную инъекцию зависимостей.
         * Предлагаю так и сделать - перевести репозиторий в обычный инстанс класса, создать его при старте системы и дальше его и использовать. 
         * В процессе попробуй сам оценить, какой код проще и понятнее. И тестопригоднее. Если сложится ощущение, что стало лучше, значит выбор правильный. Если
         * нет, то обсудим.
         * 
         * А вообще в качестве другого варианта можно было бы и логгер сделать синглтоном с подменяемым инстансом (для тестов). Тогда его не нужно было бы
         * передавать в GetInstance. А метод SetLogger вообще стал бы не нужен. Но такой подход чреват тем, что количество таких синглтонов будет расти с ростом системы
         * и в коде будет появляться все больше вызовов типа SingltonService.GetInstance(). Все бы ничего, но в этом случае зависимости размазываются по коду (в каждом месте
         * получения инстанса) и для того, чтобы просто ответить себе на вопрос, от каких других классов зависит твой класс, нужно найти в его коде все такие вызовы. Для 
         * этого надо помнить все классы с синглтонами. В общем, это не есть хорошо. 
         * 
         * Можно, правда, пойти дальше и сделать один статиковый сервис локатор (это паттерн такой), через который ты получаешь инстансы нужных тебе сервисов. Этот вариант 
         * лучше, т.к. чтобы найти внешние зависимости, размазанные по коду, тебе достаточно найти все обращения к сервис-локатору. Но зависимости все-равно остаются размазанными. 
         * В отличие от этого инъекция через конструктор не требует поиска вообше. Просто смотришь на параметры конструктора и видишь все зависимости. Поэтому, мое мнение такое, 
         * что сервис-локатор можно использовать, но только для тех сервисов, которые нужны в большом количестве мест системы (например, логгер - как раз такой), но обращения к 
         * сервис-локатору должны быть сразу в конструкторе и за этим просто нужно следить. А во остальных случаях нужно использовать инъекцию через конструктор. А для этого проекта
         * сервис-локатор вообще не очень нужен, т.к. кода немного и риск того, что в конструктор будет приходить слишком много параметров, минимален. 
         * 
         * Еще, по поводу синглтонов в интернет тоже дискуссия о том, как их правильно использовать. Можешь посмотреть вот этот ответ на SO и статьи по ссылкам в нем - 
         * https://stackoverflow.com/a/228380
         */

        /// <summary>
        /// Метод для создания объекта класса ProductRepository (если объект уже был создан, то метод вернет этот объект)
        /// </summary>
        public static ProductRepository GetInstance(ILogger logger) {

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
            /* SingleOrDefault сам принимает предикат, так что можно where в данном случае вообще убрать, а предикат перенести. То же есть и ниже по коду  */
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
             * ВОПРОСЫ:
             * 1) Вы указали мне на случай с конструкцией (1). Также я инвертировал условие еще в одном месте (2), но условие получилось не очень коротким.
             * Возникает вопрос, что будет целесообразнее: вложенный if или условие с 1-2 (может быть, 3) логическими операторами?
             */

            /*
             * Invert if - это не догма, естественно. И в каких-то сучаях может быть удобвнее оставить вложенность. Но вот в данном случае даже усложнение условия 
             * в if не перевешивает получившейся понятности в коде, на мой взгляд. Более того, а нужно ли здесь это сложное условие? Нельзя ли заменить 
             * на более простую конструкцию? 
             * 
             * Например, можно эту строчку 
             *      Product productFromDatabase = GetProduct(newVersion.ProductName);
             * заменить на такую: 
             *      Product productFromDatabase = GetProduct(newVersion.ProductName) ?? new Product(newVersion);
             *      
             * А потом достаточно проверить, что добавляемая версия больше последней (а для нового продукта это всегда так, т.к. последней версии еще нет) и 
             * выйти, если версия не больше. 
             * 
             * И уже оставшийся код просто добавит все данные в списки. Правда, продукт нужно добавлять только если он вновь созданные. Но ты можешь 
             * процедуру добавления продукта сделать такой, чтобы она не добавляла продукт, если он уже есть. Возможно, в этом случае оптимальность выполнения 
             * будет пониже, но читаться код будет еще легче, с моей точки зрения.
             */

            /* (1) */
            if (!IsProductVersionCorrect(newVersion.ProductVersion)) {
                _logger.LogRecord("Номер версии введен некорректно");
                return;
            }

            Product productFromDatabase = GetProduct(newVersion.ProductName);

  /* (2) */ if (productFromDatabase != null && !productFromDatabase.NewVersionIsGreaterThenLatest(newVersion)) {
                _logger.LogRecord("Данная версия не может быть добавлена, так как не является новой");
                return;
            }

            if (productFromDatabase == null) {
                
                productFromDatabase = new Product(newVersion);

                _products.Add(productFromDatabase);
                _logger.LogRecord($"Новый продукт {newVersion.ProductName} успешно добавлен");
            }
            else {
                AddVersionToProduct(newVersion);
            }

            _versions.Add(newVersion);
                
            _logger.LogRecord($"Версия {newVersion.ProductVersion} продукта {newVersion.ProductName} успешно добавлена");
        }

        /// <summary>
        /// Метод для добавления версии в общий список всех версий конкретного продукта
        /// </summary>
        private void AddVersionToProduct(Version newVersion) {
            GetProduct(newVersion.ProductName)?.AddVersion(newVersion); 
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
        }

       
        /// <summary>
        /// Метод для удаления конкретной версии (и, возможно, продукта) из репозитория
        /// </summary>
        public void RemoveVersion(string productName, string productVersion) {
            Version versionToRemove = GetProductConcreteVersion(productName, productVersion); 

            if (versionToRemove == null || !_versions.Contains(versionToRemove)) {
                _logger.LogRecord("Удаляемой версии продукта не существует!");
                return;
            }

            _versions.Remove(versionToRemove);

            _logger.LogRecord($"Версия {productVersion} продукта {productName} успешно удалена");

            RemoveProduct(versionToRemove);

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


        /*
         * Добавлять дополнительные публичные методы к классу только для целей написания теста чаще всего излишне. Получается, что
         * сам по себе класс не позволяет его нормально протестировать, что неправильно и является поводом задуматься над тем, 
         * а все ли сделано верно. Но в твоем случае те же проверки можно выполнить через уже существующие методы. Ниже напишу, как.
         */

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

        /*
         * Два метода выше излишни, т.к. можно сделать то же через GetProduct и GetVersion. И проверить результат на null.
         */
        

        /// <summary>
        /// Метод для очистки репозитория
        /// </summary>
        public void ClearProductsAndVersions(){
            _products.Clear();
            _versions.Clear();
        }


        /*
         * Здесь лучше просто для каждой проверки создавать новый репозиторий с требуемым исходным состоянием. Твой способ очистка не гарантирует 
         * возврат репозитория в начальное состояние. Вдруг у тебя со временем появится еще что-то, что хранит состояние репозитория? Ты будешь вынужден
         * править и этот метод тоже. Каждый раз.
         */

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
         * Проверка количества версий и объектов тоже не нужно. Просто проверяй, что добавленный тобой продукт/версия появился в репозитории
         */

        /*
         ПРАВКИ:
         1) Добавил публичные методы IsThereProduct, IsThereVersion, ClearProductsAndVersions, GetCountProducts, GetCountVersions
         для вызова их в тестовых методах.
        */

        /*
         * Еще добавлю, что сами по себе некоторые из этих доп. методов, возможно и могут быть в API репозитория, но только в том случае, если
         * это обусловлено требованиями к API, а не тестами. Именно поэтому я и предлагаю их не добавлять сейчас.
         */

    }
    
}
