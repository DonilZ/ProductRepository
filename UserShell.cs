using System;
using System.Collections.Generic;

namespace repository {
    //Класс, отвечающий за предоставление пользователю необходимой информации о продуктах и их версиях из БД.
    class UserShellGet {

        public static void GetInformationProducts(DatabaseManager CurrentDatabaseManager) {
            List<Product> Products = CurrentDatabaseManager.GetInformationProducts();
            foreach(Product CurrentProduct in Products) {
                Console.WriteLine("| {0} |  | {1} |  | {2} |", CurrentProduct.GetNameProduct(), 
                                CurrentProduct.GetNumberLatestVersion(), CurrentProduct.GetShortDescription());
            }
        }

        public static void GetInformationProductVersions(DatabaseManager CurrentDatabaseManager) {
            Console.WriteLine("Введите уникальное имя необходимого продукта:");
            string NameProduct = Console.ReadLine();

            List<Version> Versions = CurrentDatabaseManager.GetInformationProductVersions(NameProduct);

            if (Versions != null) {
                foreach(Version CurrentVersion in Versions) {
                    Console.WriteLine("| {0} |  | {1} |", CurrentVersion.NumberVersion, CurrentVersion.ShortDescription);
                }
            }
            else {
                Console.WriteLine("Выбранного продукта не существует");
            }
        }

        public static void GetInformationProductConcreteVersion(DatabaseManager CurrentDatabaseManager) {
            Console.WriteLine("Введите уникальное имя необходимого продукта:");
            string NameProduct = Console.ReadLine();

            Console.WriteLine("Введите номер версии данного продукта:");
            string NumberVersion = Console.ReadLine();

            Version ResultedVersion = CurrentDatabaseManager.GetInformationProductConcreteVersion(NameProduct, NumberVersion);

            if (ResultedVersion != null) {
            Console.WriteLine("| {0} |  | {1} |  | {2} |  | {3} |  | {4} |  | {5} |  | {6} |", 
                            ResultedVersion.NameProduct, ResultedVersion.NumberVersion, ResultedVersion.ShortDescription, 
                            ResultedVersion.LongDescription, ResultedVersion.Changes, ResultedVersion.DistributedFile.First, 
                            ResultedVersion.DistributedFile.Second);
            }
            else {
                Console.WriteLine("Выбранной версии не существует");
            }
        }

    }
    /*Класс, отвечающий за добавление пользователем очередной версии продукта (или нового продукта) в БД,
    обновление необходимой версии продукта и удаление необходимой версии продукта (или продукта полностью) из БД.
    */
    class UserShell {   
         
        public static void AddVersion(DatabaseManager CurrentDatabaseManager) {
            Version InputVersion;

            InputValues(out InputVersion);

            Version AddedVersion = InputVersion;
            Product AddedProduct = new Product(AddedVersion);

            CurrentDatabaseManager.AddVersion(AddedVersion, AddedProduct);
        }

        public static void UpdateVersion(DatabaseManager CurrentDatabaseManager) {
            Version InputVersion;
            InputValues(out InputVersion);

            Version UpdatedVersion = InputVersion;

            CurrentDatabaseManager.UpdateVersion(UpdatedVersion);
        }

        public static void RemoveVersion(DatabaseManager CurrentDatabaseManager) {
            Console.WriteLine("Введите уникальное имя продукта:");
            string NameProduct = Console.ReadLine();

            Console.WriteLine("Введите номер версии продукта:");
            string NumberVersion = Console.ReadLine();

            CurrentDatabaseManager.RemoveVersion(NameProduct, NumberVersion);
        }

        private static void InputValues(out Version InputVersion) {
            Console.WriteLine("Введите уникальное имя продукта:");
            string NameProduct;
            NameProduct = Console.ReadLine();

            Console.WriteLine("Введите номер версии продукта:");
            string NumberVersion;
            NumberVersion = Console.ReadLine();

            Console.WriteLine("Введите краткое описание продукта:");
            string ShortDescription;
            ShortDescription = Console.ReadLine();

            Console.WriteLine("Введите подробное описание продукта:");
            string LongDescription;
            LongDescription = Console.ReadLine();

            Console.WriteLine("Введите список изменений, сделанных в этой версии продукта:");
            string Changes;
            Changes = Console.ReadLine();

            Pair<string,string> File = new Pair<string, string>();

            Console.WriteLine("Введите уникальное имя файла с дистрибутивом продукта:");
            File.First = Console.ReadLine();

            Console.WriteLine("Введите URL с ссылкой на файл с дистрибутивом продукта:");
            File.Second = Console.ReadLine();

            InputVersion = new Version(NameProduct, NumberVersion, ShortDescription, LongDescription, Changes, File);

        }
    }
}
