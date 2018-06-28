using System;
using System.Collections.Generic;

namespace repository {

    /*
     * В двух классах UserShell ты хорошо сделал, что вынес внешнюю зависимость в виде DatabaseManager-а наружу, т.е. применил принцип инверсии управления, 
     * буква I в известном SOLID. Это позволит тебе при тестировании этих классов заменить реальную зависимость на объекты заглушки. Но вот реализация этой инверсии
     * нестандартная, скажем так. Чаще всего зависимость инъектируют в класс через параметры конструктора при создании экземпляра объекта. Т.е. если бы ты сделал UserShell 
     * обычным классом с обычными, а не static методами, то при конструировании этого класса ты бы мог передать ему через коснтруктор экземпляр DatabaseManager-а, а уже методы 
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

    //Класс, отвечающий за предоставление пользователю необходимой информации о продуктах и их версиях из БД.
    class UserShellGet {

        public static void GetInformationProducts(DatabaseManager CurrentDatabaseManager) {
            List<Product> Products = CurrentDatabaseManager.GetInformationProducts();
            foreach(Product CurrentProduct in Products) {
                Console.WriteLine("| {0} |  | {1} |  | {2} |", CurrentProduct.GetNameProduct(), 
                                CurrentProduct.GetNumberLatestVersion(), CurrentProduct.GetShortDescription());

                /*
                 * Попробуй string interpolation в качестве способа формирования строки
                 */

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

            /*
             * 1. Вместо передачи out-параметра лучше, чтобы этот метод просто возвращал объект класса Version, а на вход не принимал ничего
             * 
             * 2. Вместо того, чтобы создавать отдельные переменные, читать в них значения с консоли, а потом конструировать объект Version, лучше сконструировать 
             * его сразу, а потом в его свойства читать с консоли. Будет лаконичнее.
             * 
             */


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
