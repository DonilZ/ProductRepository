using System;
using System.Collections.Generic;
using System.Linq;

namespace repository {

    /*   
     *
     * ВОПРОСЫ:
     * 1) Есть ли еще какие-либо преимущества (помимо оптимальности) методов экзмепляров двух классов UserShell перед методами типа в рамках данного случая? 
     * Ведь нам теперь вместо передачи аргументов в методы необходимо создавать объекты каждого из двух классов (+ создание конструкторов с параметром в каждом классе).
     */

     /*
      * С оптимальностью - вопрос. В одном случае ты создаешь экземпляры, другом - передаешь параметр в метод, а значит он кладется в стэк. В зависимость от сценария
      * оптимальнее может оказаться и тот, и другой подходы. Но оптимальность часто не главный критерий, т.к. проблемы со скоростью могут быть совсем в другом месте и 
      * там их решить можно только до определенного предела, на фоне которого оптимизация здесь может вообще не иметь никакого практического смысла. 
      * 
      * А вот принципы слабой связаности (low coupling) в больших системах имеют очень большое значение, т.к. позволяют делать системы гибче и надежнее. И в случае работы со 
      * static-классам связаность очень сильная. Ты не можешь заменить один статик класс на другой. В тестах, или когда нужно подменить функциональность. А в случае с экземплярами
      * ты можешь и объект класса-наследника подсунуть вместо основного, и mock-и, что существенно снижает связанность. Можешь интерфейсы использовать, что еще большую гибкость добавляет.
      * Главное преимущество - в этом. 
      */


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
                Console.WriteLine($"| {currentProduct.GetProductName()} |  | {currentProduct.GetLatestVersionNumber()} |  | {currentProduct.GetShortDescription()} |");
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

            Version resultedVersion = _currentProductRepository.GetProductConcreteVersion(productName, productVersion);

            if (resultedVersion != null) {
                Console.WriteLine($"| {resultedVersion.ProductName} |  | {resultedVersion.ProductVersion} |  " + 
                                    $"| {resultedVersion.ShortDescription} |  | {resultedVersion.LongDescription} |  " + 
                                    $"| {resultedVersion.Changes} |  | {resultedVersion.DownloadableFile.FileName} |  " + 
                                    $"| {resultedVersion.DownloadableFile.FileUrl} |");
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
            Version newVersion = InputValues();

            _currentProductRepository.AddVersion(newVersion);
        }

        /// <summary>
        /// Метод для обновления конкретной версии в репозитории
        /// </summary>
        public void UpdateVersion() {
            Version inputVersion = InputValues();
            

            Version updatedVersion = inputVersion;

            _currentProductRepository.UpdateVersion(updatedVersion);
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

        /// <summary>
        /// Метод для ввода пользователем информации
        /// </summary>
        private Version InputValues() {

            Console.WriteLine("Введите уникальное имя продукта:");
            string productName;
            productName = Console.ReadLine();

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

            FileInfo file = new FileInfo();

            Console.WriteLine("Введите уникальное имя файла с дистрибутивом продукта:");
            file.FileName = Console.ReadLine();

            Console.WriteLine("Введите URL с ссылкой на файл с дистрибутивом продукта:");
            file.FileUrl = Console.ReadLine();

            return new Version(productName, productVersion, shortDescription, longDescription, changes, file);
        }
    }
}
