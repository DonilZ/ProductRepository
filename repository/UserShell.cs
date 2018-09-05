using System;
using System.Collections.Generic;
using System.Linq;

namespace repository {
    ///<summary>
    ///Класс, отвечающий за предоставление пользователю необходимой информации о продуктах и их версиях из БД.
    ///</summary>
    class UserShellGet {

        private ProductRepository _currentProductRepository;

        public UserShellGet(ProductRepository currentProductRepository) {
            _currentProductRepository = currentProductRepository;
        }

        /// <summary>
        /// Метод для получения списка всех продуктов из репозитория
        /// </summary>
        public void GetProducts() {
            List<Product> products = _currentProductRepository.GetProducts();
            foreach(Product currentProduct in products) {
                Console.WriteLine($"| {currentProduct.ProductName} |  | {currentProduct.GetLatestVersionNumber()} |  | {currentProduct.GetShortDescription()} |");
            }
        }

        /// <summary>
        /// Метод для получения списка всех версий конкретного продукта
        /// </summary>
        public void GetProductVersions() {
            Console.WriteLine("Введите уникальное имя необходимого продукта:");
            string productName = Console.ReadLine();

            List<Version> versions = _currentProductRepository.GetProductVersions(productName);

            if (versions != null) {
                foreach(Version currentVersion in versions) {
                    Console.WriteLine($"| {currentVersion.ProductVersion} |  | {currentVersion.ShortDescription} |");
                }
            }
            else {
                Console.WriteLine("Выбранного продукта не существует");
            }
        }

        /// <summary>
        /// Метод для получения конкретной версии конкретного продукта
        /// </summary>
        public void GetProductConcreteVersion() {
            Console.WriteLine("Введите уникальное имя необходимого продукта:");
            string productName = Console.ReadLine();

            Console.WriteLine("Введите номер версии данного продукта:");
            string productVersion = Console.ReadLine();

            Version resultedVersion = _currentProductRepository.GetConcreteVersion(productName, productVersion);

            if (resultedVersion != null) {
                Console.WriteLine($"| {productName} |  | {resultedVersion.ProductVersion} |  " + 
                                    $"| {resultedVersion.ShortDescription} |  | {resultedVersion.LongDescription} |  " + 
                                    $"| {resultedVersion.Changes} |  | {resultedVersion.DownloadableFileName} |  " + 
                                    $"| {resultedVersion.DownloadableFileUrl} |");
            }
            else {
                Console.WriteLine("Выбранной версии не существует");
            }
        }

    }

    ///<summary>
    ///Класс, отвечающий за добавление пользователем очередной версии продукта (или нового продукта) в БД, обновление необходимой версии продукта и удаление необходимой версии продукта (или продукта полностью) из БД.
    ///</summary>
    class UserShell {  

        private ProductRepository _currentProductRepository;

        public UserShell(ProductRepository currentProductRepository) {
            _currentProductRepository = currentProductRepository;
        } 
         
        /// <summary>
        /// Метод для добавления версии (и, возможно, продукта) в репозиторий
        /// </summary>
        public void AddVersion() {
            string productName = InputProductName();
            Version newVersion = InputValues();

            _currentProductRepository.AddVersion(productName, newVersion);
        }

        /// <summary>
        /// Метод для обновления конкретной версии в репозитории
        /// </summary>
        public void UpdateVersion() {
            string productName = InputProductName();

            Version inputVersion = InputValues();
        
            Version updatedVersion = inputVersion;

            _currentProductRepository.UpdateVersion(productName, updatedVersion);
        }

        /// <summary>
        /// Метод для удаления конкретной версии (и, возможно, продукта) из репозитория
        /// </summary>
        public void RemoveVersion() {
            Console.WriteLine("Введите уникальное имя продукта:");
            string productName = Console.ReadLine();

            Console.WriteLine("Введите номер версии продукта:");
            string productVersion = Console.ReadLine();

            _currentProductRepository.RemoveVersion(productName, productVersion);
        }

        private string InputProductName() {
            Console.WriteLine("Введите уникальное имя продукта:");
            string productName;
            productName = Console.ReadLine();

            return productName;
        }

        /// <summary>
        /// Метод для ввода пользователем информации
        /// </summary>
        private Version InputValues() {
            Console.WriteLine("Введите номер версии продукта:");
            string productVersion;
            productVersion = Console.ReadLine();

            Console.WriteLine("Введите краткое описание продукта:");
            string shortDescription;
            shortDescription = Console.ReadLine();

            Console.WriteLine("Введите подробное описание продукта:");
            string longDescription;
            longDescription = Console.ReadLine();

            Console.WriteLine("Введите список изменений, сделанных в этой версии продукта:");
            string changes;
            changes = Console.ReadLine();

            Console.WriteLine("Введите уникальное имя файла с дистрибутивом продукта:");
            string fileName;
            fileName = Console.ReadLine();

            Console.WriteLine("Введите URL с ссылкой на файл с дистрибутивом продукта:");
            string fileUrl;
            fileUrl = Console.ReadLine();

            return new Version(productVersion, shortDescription, longDescription, changes, fileName, fileUrl);
        }
    }
}
