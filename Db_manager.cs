using System;
using System.Collections.Generic;

namespace repository {

    class Db_manager {
        private static Db_manager instance_db;
        private static List<Version> versions;
        private static List<Product> products;

        private Db_manager() {
            versions = new List<Version>();
            products = new List<Product>();
        }

        public static Db_manager create_db() {
            if (instance_db == null)
                instance_db = new Db_manager();
            
            return instance_db;
        }
        public List<Product> get_products_inf() {
            return products;
        }
        public List<Version> get_product_versions_inf(string name_product) {
            try {
                foreach (Product product in products) {
                    if (name_product == product.get_name_product()) {
                        return product.get_all_versions();
                    }
                }
            }
            catch {
                return null;
            }

            return null;
        }
        public Version get_product_concrete_version_inf(string name_product, string number_version) {
            List<Version> _versions = get_product_versions_inf(name_product);           
            try {
                foreach (Version version in _versions) {
                    if (number_version == version.number_version) {
                        return version;
                    }
                }
            }
            catch {
                return null;
            }

            return null;
        }
        public void add_version(Version version, Product product) {
            if (check_correct_number_version(version.number_version)) {
                try {
                    versions.Add(version);
                    if (check_products(version)) {
                        products.Add(product);
                        Console.WriteLine("Новый продукт " + version.name_product + " успешно добавлен");
                    }
                    Console.WriteLine("Версия " + version.number_version + " продукта " + version.name_product + " успешно добавлена");
                }
                catch {
                    get_error_db();
                }
            }
            else {
                Console.WriteLine("Номер версии введен некорректно");
            }
        }
        private bool check_products(Version version) {
            foreach (Product product in products) {
                if (product.get_name_product() == version.name_product) {
                    product.add_version(version);
                    return false;
                }
            }
            return true;
        }

        public void update_version(Version new_version) {
            try {
                foreach (Version version in versions) {
                    if (new_version.name_product == version.name_product && new_version.number_version == version.number_version) { 
                        version.update(new_version);

                        Console.WriteLine("Версия " + version.number_version + " продукта " + version.name_product + " успешно обновлена");
                        return;
                    }
                }
            }
            catch {
                get_error_db();
                return;
            }
            
            Console.WriteLine("Обновляемой версии продукта не существует");
        }

        public void delete_version(string name_product, string number_version) {
            Version deletion_version = get_product_concrete_version_inf (name_product, number_version);
            try {
                if (deletion_version != null && versions.Contains(deletion_version)) {
                    versions.Remove(deletion_version);
                    Console.WriteLine("Версия " + number_version + " продукта " + name_product + " успешно удалена");
                    delete_product(deletion_version);
                }
                else {
                    Console.WriteLine("Удаляемой версии продукта не существует!");
                }
            }
            catch {
                get_error_db();
            }
        }

        private void delete_product(Version deletion_version) {
            foreach (Product product in products) {
                if (product.get_all_versions().Contains(deletion_version)) {
                    product.delete_version(deletion_version);
                    if (product.get_all_versions().Count == 0) {
                        products.Remove(product);
                        Console.WriteLine("Продукт " + product.get_name_product() + " успешно удален");
                        break;
                    }
                }
            }
        }

        private void get_error_db() {
            Console.WriteLine("База данных не инициализирована");
        }
        private bool check_correct_number_version(string number_version) {
            try {
                string[] ver = number_version.Split('.');

                if (ver.Length != 3) return false;
                int sum = 0;

                for (int i = 0; i < 3; ++i) {
                    sum += Convert.ToInt32(ver[i]);
                }

                return true;
            }
            catch {
                return false;
            }
        }

    }
    
}
