using System;
using System.Collections.Generic;
using System.Linq;

namespace repository {

    ///<summary>
    ///Класс, имитирующий работу с базой данных (хранение версий и продуктов, получение необходимой информации о версиях и продуктах, добавление, обновление и удаление версий и продуктов) 
    ///</summary>
    public class ProductRepository {
        private List<Version> _versions;
        private List<Product> _products;

        private ILogger _logger; 

        public ProductRepository(ILogger logger) {
            _versions = new List<Version>();
            _products = new List<Product>();
            _logger = logger;
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
            return _products.SingleOrDefault(product => product.GetProductName() == productName)?.GetAllVersions(); 
        }

        /// <summary>
        /// Метод для получения конкретной версии продукта
        /// </summary>
        public Version GetProductConcreteVersion(string productName, string productVersion) {
            List<Version> currentVersions = GetProductVersions(productName);

            return currentVersions?.SingleOrDefault(version => version.ProductVersion == productVersion);
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
             * И уже оставшийся код просто добавит все данные в списки. Правда, продукт нужно добавлять только если он вновь созданный. Но ты можешь 
             * процедуру добавления продукта сделать такой, чтобы она не добавляла продукт, если он уже есть. Возможно, в этом случае оптимальность выполнения 
             * будет пониже, но читаться код будет еще легче, с моей точки зрения.
             */

            /*
             * ПРАВКИ:
             * 1) Добавил метод IsThereProduct() для проверки продукта на уникальность
             *
             * ВОПРОСЫ:
             * 1) Вы сказали, что у нового продукта еще нет последней версии, но при создании нового объекта Product
             * в конструктор мы передаем версию (единственную на момент создания) этого продукта, которая сразу записывается как
             * последняя версия только что созданного продукта. Мне кажется это правильным, потому что не может продукта, который сразу
             * после создания совсем не содержит никаких версий, да и вообще не может быть продукта без версий. 
             * Поэтому, в моем случае, сокращение проверки не получится. Нужно ли этот момент исправить?
             */

            if (!IsProductVersionCorrect(newVersion.ProductVersion)) {
                _logger.LogRecord("Номер версии введен некорректно");
                return;
            }

            Product potentiallyNewProduct = GetProduct(newVersion.ProductName) ?? new Product(newVersion);

            if (IsThereProduct(potentiallyNewProduct) && !potentiallyNewProduct.NewVersionIsGreaterThenLatest(newVersion)) {
                _logger.LogRecord("Данная версия не может быть добавлена, так как не является новой");
                return;
            }

            if (!IsThereProduct(potentiallyNewProduct)) {
                
                _products.Add(potentiallyNewProduct);

                _logger.LogRecord($"Новый продукт {newVersion.ProductName} успешно добавлен");
            }
            else {
                AddVersionToProduct(newVersion);
            }

            _versions.Add(newVersion);
                
            _logger.LogRecord($"Версия {newVersion.ProductVersion} продукта {newVersion.ProductName} успешно добавлена");
        }

        /// <summary>
        /// Метод для проверки продукта на уникальность
        /// </summary>
        public bool IsThereProduct(Product potentiallyNewProduct) {
            string namePotentiallyNewProduct = potentiallyNewProduct.GetProductName();

            return GetProduct(namePotentiallyNewProduct) != null;
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
            return _products.SingleOrDefault(product => product.GetProductName() == productName);
        }

        /// <summary>
        /// Метод для обновления конкретной версии в репозитории
        /// </summary>
        public void UpdateVersion(Version updatedVersion) {
            Version currentVersion = _versions.SingleOrDefault(version => version.ProductName == updatedVersion.ProductName && 
                                                                version.ProductVersion == updatedVersion.ProductVersion);

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
            Product currentProduct = _products.SingleOrDefault(product => product.GetAllVersions().Contains(removedVersion));
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
                int countOfNumbersInVersion = 3;

                if (splitedVersion.Length != countOfNumbersInVersion) return false;

                int summedNumbersVersion = 0;

                for (int i = 0; i < countOfNumbersInVersion; ++i) {
                    summedNumbersVersion += Convert.ToInt32(splitedVersion[i]);
                }

                return true;
            }
            catch {
                return false;
            }
        }


        /*
         * ПРАВКИ:
         * 2) Метод IsThereVersion переместил в тестовый класс. Но реализовать проверку на наличие версии в репозитори при помощи GetVersion не получится,
         * так как мы получаем версию по имени продукта и номеру версии, а, например, при тестировании метода обновления важны абсолютно
         * все данные о версии.
         * 
         * А почему нужно сравнивать именно все данные о версии? Не достаточно разве проверить, вернул репозиторий указанную версию продукта или нет? 
         * 
        */
        


    }
    
}
