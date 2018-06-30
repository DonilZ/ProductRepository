using System;
using System.Collections.Generic;
using System.Linq;

namespace repository {

    /*
<<<<<<< HEAD
     * В двух классах UserShell ты хорошо сделал, что вынес внешнюю зависимость в виде ProductRepository-а наружу, т.е. применил принцип инверсии управления, 
     * буква I в известном SOLID. Это позволит тебе при тестировании этих классов заменить реальную зависимость на объекты заглушки. Но вот реализация этой инверсии
     * нестандартная, скажем так. Чаще всего зависимость инъектируют в класс через параметры конструктора при создании экземпляра объекта. Т.е. если бы ты сделал UserShell 
     * обычным классом с обычными, а не static методами, то при конструировании этого класса ты бы мог передать ему через коснтруктор экземпляр ProductRepository-а, а уже методы 
=======
     * В двух классах UserShell ты хорошо сделал, что вынес внешнюю зависимость в виде DatabaseManager-а наружу, т.е. применил принцип инверсии управления, 
     * буква I в известном SOLID. Это позволит тебе при тестировании этих классов заменить реальную зависимость на объекты заглушки. Но вот реализация этой инверсии
     * нестандартная, скажем так. Чаще всего зависимость инъектируют в класс через параметры конструктора при создании экземпляра объекта. Т.е. если бы ты сделал UserShell 
     * обычным классом с обычными, а не static методами, то при конструировании этого класса ты бы мог передать ему через коснтруктор экземпляр DatabaseManager-а, а уже методы 
>>>>>>> 638cfd6cf65319457d5878763338303644681dec
     * не требовали бы его передавать в качестве параметра. На самом деле, такой способ оптимален здесь. 
     * 
     * Но есть еще один - использование синглтона. Тогда зависимость получается не через параметры методов, а обращением к статиковому методу получения синглтона, т.е. 
     * в твоем случае - к CreateDatabase. И если ты эту зависимость хоел бы заменить на объект-заглушку, ты мог бы завести еще один статик метод - SetDatabase, который бы 
     * подставил нужный инстанс-заглушку, которая уже возвращалась бы при обращении к CreateDatabase. С таким подходом больше проблем, на самом деле, поэтому использовать его 
     * лучше только в определенных специальных случаях. Пока не буду тебя загружать этим. При желании можешь почитать книжку The Art of Unit Testing, там и об этом, и о многом
     * другом полезном написано. 
     * 
     * Выше использовал термин объект-заглушка. Возможно, он тебе не знаком. Если так, почитай про mocks и fakes в unit-тестировании. И вообще про unit-тестирование. Нам 
     * потом тоже пригодится.
     * 
     */

<<<<<<< HEAD

    /* 
     * ПРАВКИ:
     * 1) Инъектировал зависимость (ProductRepository) через параметр конструктора, вследствие чего поменял static методы на обычные и без параметров.
     *
     * ВОПРОСЫ:
     * 1) Есть ли еще какие-либо преимущества (помимо оптимальности) методов экзмепляров двух классов UserShell перед методами типа в рамках данного случая? 
     * Ведь нам теперь вместо передачи аргументов в методы необходимо создавать объекты каждого из двух классов (+ создание конструкторов с параметром в каждом классе).
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
                Console.WriteLine($"| {currentProduct.GetProductName()} |  | {currentProduct.GetNumberLatestVersion()} |  | {currentProduct.GetShortDescription()} |");
=======
    //Класс, отвечающий за предоставление пользователю необходимой информации о продуктах и их версиях из БД.
    class UserShellGet {

        public static void GetInformationProducts(DatabaseManager CurrentDatabaseManager) {
            List<Product> Products = CurrentDatabaseManager.GetInformationProducts();
            foreach(Product CurrentProduct in Products) {
                Console.WriteLine("| {0} |  | {1} |  | {2} |", CurrentProduct.GetNameProduct(), 
                                CurrentProduct.GetNumberLatestVersion(), CurrentProduct.GetShortDescription());
>>>>>>> 638cfd6cf65319457d5878763338303644681dec

                /*
                 * Попробуй string interpolation в качестве способа формирования строки
                 */

<<<<<<< HEAD
                /*
                 * Прошу прощения (вы в первых правках уже указывали на это), прочитал неполную информацию по string interpolation и сделал неправильно.
                 * Исправлено.
                 */

=======
>>>>>>> 638cfd6cf65319457d5878763338303644681dec
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

<<<<<<< HEAD
            if (resultedVersion != null) {
                Console.WriteLine($"| {resultedVersion.ProductName} |  | {resultedVersion.ProductVersion} |  " + 
                                    $"| {resultedVersion.ShortDescription} |  | {resultedVersion.LongDescription} |  " + 
                                    $"| {resultedVersion.Changes} |  | {resultedVersion.DownloadableFile.FileName} |  " + 
                                    $"| {resultedVersion.DownloadableFile.FileUrl} |");
=======
            if (ResultedVersion != null) {
                Console.WriteLine("| {0} |  | {1} |  | {2} |  | {3} |  | {4} |  | {5} |  | {6} |", 
                            ResultedVersion.NameProduct, ResultedVersion.NumberVersion, ResultedVersion.ShortDescription, 
                            ResultedVersion.LongDescription, ResultedVersion.Changes, ResultedVersion.DistributedFile.First, 
                            ResultedVersion.DistributedFile.Second);
>>>>>>> 638cfd6cf65319457d5878763338303644681dec
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
            Version inputVersion = InputValues();

            

            Version addedVersion = inputVersion;
            Product addedProduct = new Product(addedVersion);

            _currentProductRepository.AddVersion(addedVersion, addedProduct);
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

<<<<<<< HEAD
        /// <summary>
        /// Метод для ввода пользователем информации
        /// </summary>
        private Version InputValues() {
=======
        private static void InputValues(out Version InputVersion) {
>>>>>>> 638cfd6cf65319457d5878763338303644681dec

            /*
             * 1. Вместо передачи out-параметра лучше, чтобы этот метод просто возвращал объект класса Version, а на вход не принимал ничего
             * 
             * 2. Вместо того, чтобы создавать отдельные переменные, читать в них значения с консоли, а потом конструировать объект Version, лучше сконструировать 
             * его сразу, а потом в его свойства читать с консоли. Будет лаконичнее.
             * 
             */

<<<<<<< HEAD
            /*
             * ПРАВКИ:
             * 1) (1 пункт) Убрал out-параметр. Поменял тип возвращаемого значения метода (теперь он возвращает объект класса Version).
             * 2) (2 пункт) Пока договорились оставить как есть.
             */

=======
>>>>>>> 638cfd6cf65319457d5878763338303644681dec

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

            FileInfo<string,string> file = new FileInfo<string, string>();

            Console.WriteLine("Введите уникальное имя файла с дистрибутивом продукта:");
            file.FileName = Console.ReadLine();

            Console.WriteLine("Введите URL с ссылкой на файл с дистрибутивом продукта:");
            file.FileUrl = Console.ReadLine();

            Version inputVersion = new Version(productName, productVersion, shortDescription, longDescription, changes, file);

            return inputVersion;

        }
    }
}
