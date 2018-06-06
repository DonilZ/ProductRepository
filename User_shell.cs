using System;
using System.Collections.Generic;

namespace repository {
    class User_shell_get {

        public static void get_all_products(ref Db_manager db_manager) {
            List<Product> products = db_manager.get_products_inf();
            foreach(Product product in products) {
                Console.WriteLine("| " + product.get_name_product() + " |  | " + product.get_num_last_version()
                                    + " |  | " + product.get_short_desc() + " |");
            }
        }

        public static void get_concrete_product_versions(ref Db_manager db_manager) {
            Console.WriteLine("Введите уникальное имя необходимого продукта:");
            string name_product = Console.ReadLine();

            List<Version> versions = db_manager.get_product_versions_inf(name_product);

            try {
                foreach(Version version in versions) {
                    Console.WriteLine("| " + version.number_version + " |  | " + version.short_desc + " |");
                }
            }
            catch {
                Console.WriteLine("Выбранного продукта не существует");
            }
        }

        public static void get_concrete_product_concrete_version(ref Db_manager db_manager) {
            Console.WriteLine("Введите уникальное имя необходимого продукта:");
            string name_product = Console.ReadLine();

            Console.WriteLine("Введите номер версии данного продукта:");
            string number_version = Console.ReadLine();

            Version version = db_manager.get_product_concrete_version_inf(name_product, number_version);

            try {
            Console.WriteLine("| " + version.name_product + " |  | " + version.number_version + " |  | " + 
                            version.short_desc + " |  | " + version.long_desc + " |  | " + version.changes +
                            " |  | " + version.dist_file.First + " |  | " + version.dist_file.Second + " |");
            }
            catch {
                Console.WriteLine("Выбранной версии не существует");
            }
        }

    }

    class User_shell {   
         
        public static void add_version(ref Db_manager db_manager) {
            string name_product, number_version, short_desc, long_desc, changes;
            Pair<string, string> file;

            input_values(out name_product, out number_version, out short_desc, out long_desc,
                        out changes, out file);

            Version version = new Version(name_product, number_version, short_desc, long_desc, changes, file);
            Product product = new Product(version);

            db_manager.add_version(version, product);
        }

        public static void update_version(ref Db_manager db_manager) {
            string name_product, number_version, short_desc, long_desc, changes;
            Pair<string, string> file;

            input_values(out name_product, out number_version, out short_desc, out long_desc,
                        out changes, out file);

            Version new_version = new Version(name_product, number_version, short_desc, long_desc, changes, file);

            db_manager.update_version(new_version);
        }

        public static void delete_version(ref Db_manager db_manager) {
            Console.WriteLine("Введите уникальное имя продукта:");
            string name_product = Console.ReadLine();

            Console.WriteLine("Введите номер версии продукта:");
            string number_version = Console.ReadLine();

            db_manager.delete_version(name_product, number_version);
        }

        private static void input_values(out string name_product, out string number_version, out string short_desc,
                                        out string long_desc, out string changes, out Pair<string, string> file) {
            Console.WriteLine("Введите уникальное имя продукта:");
            name_product = Console.ReadLine();

            Console.WriteLine("Введите номер версии продукта:");
            number_version = Console.ReadLine();

            Console.WriteLine("Введите краткое описание продукта:");
            short_desc = Console.ReadLine();

            Console.WriteLine("Введите подробное описание продукта:");
            long_desc = Console.ReadLine();

            Console.WriteLine("Введите список изменений, сделанных в этой версии продукта:");
            changes = Console.ReadLine();

            file = new Pair<string, string>();

            Console.WriteLine("Введите уникальное имя файла с дистрибутивом продукта:");
            file.First = Console.ReadLine();

            Console.WriteLine("Введите URL с ссылкой на файл с дистрибутивом продукта:");
            file.Second = Console.ReadLine();

        }
    }
}
