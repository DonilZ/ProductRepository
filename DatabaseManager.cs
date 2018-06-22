using System;
using System.Collections.Generic;

namespace repository {
    /*Класс, имитирующий работу с базой данных (хранение версий и продуктов, получение необходимой информации
    о версиях и продуктах, добавление, обновление и удаление версий и продуктов) */
    class DatabaseManager {
        private static DatabaseManager InstanceDatabase;
        private List<Version> Versions;
        private List<Product> Products;

        private DatabaseManager() {
            Versions = new List<Version>();
            Products = new List<Product>();
        }

        public static DatabaseManager CreateDatabase() {
            if (InstanceDatabase == null)
                InstanceDatabase = new DatabaseManager();
            
            return InstanceDatabase;
        }

        public List<Product> GetInformationProducts() {
            return Products;
        }

        public List<Version> GetInformationProductVersions(string NameProduct) { 
            foreach (Product CurrentProduct in Products) {
                if (NameProduct == CurrentProduct.GetNameProduct()) {
                    return CurrentProduct.GetAllVersions();
                }
            }

            return null;
        }

        public Version GetInformationProductConcreteVersion(string NameProduct, string NumberVersion) {
            List<Version> CurrentVersions = GetInformationProductVersions(NameProduct);

            if (CurrentVersions != null) {          
                foreach (Version CurrentVersion in CurrentVersions) {
                    if (NumberVersion == CurrentVersion.NumberVersion) {
                        return CurrentVersion;
                    }
                }
            }
            
            return null;
        }

        public void AddVersion(Version AddedVersion, Product AddedProduct) {
            if (CheckCorrectNumberVersion(AddedVersion.NumberVersion)) {

                
                if (CheckUniqueProduct(AddedVersion)) {
                    Products.Add(AddedProduct);
                    Console.WriteLine("Новый продукт {0} успешно добавлен", AddedVersion.NumberVersion);
                }
                else {
                    Product ProductFromDatabase = GetProduct(AddedProduct.GetNameProduct());
                    
                    if (!ProductFromDatabase.CheckAddedVersion(AddedVersion)) {
                        Console.WriteLine("Данная версия не может быть добавлена, так как не является новой");
                        return;
                    }

                    AddVersionToProduct(AddedVersion);

                }

                Versions.Add(AddedVersion);
                
                Console.WriteLine("Версия {0} продукта {1} успешно добавлена", AddedVersion.NumberVersion, AddedVersion.NameProduct);
            }
            else {
                Console.WriteLine("Номер версии введен некорректно");
            }
        }

        //Проверка того, был ли добавлен ранее в БД такой продукт
        private bool CheckUniqueProduct(Version AddedVersion) {
            foreach (Product CurrentProduct in Products) {
                if (CurrentProduct.GetNameProduct() == AddedVersion.NameProduct) {
                    return false;
                }
            }
            return true;
        }

        //Добавление версии в общий список кокретного продукта
        private void AddVersionToProduct(Version AddedVersion) {
            foreach (Product CurrentProduct in Products) {
                if (CurrentProduct.GetNameProduct() == AddedVersion.NameProduct) {
                    CurrentProduct.AddVersion(AddedVersion);
                    return;
                }
            }
        }

        //Получение продукта из БД по его имени
        private Product GetProduct(string NameProduct) {
            for(int i = 0; i < Products.Count; ++i) {
                if (Products[i].GetNameProduct() == NameProduct) {
                    return Products[i];
                }
            }

            return null;
        }

        public void UpdateVersion(Version UpdatedVersion) {
            foreach (Version CurrentVersion in Versions) {
                if (UpdatedVersion.NameProduct == CurrentVersion.NameProduct && UpdatedVersion.NumberVersion == CurrentVersion.NumberVersion) { 
                    CurrentVersion.Update(UpdatedVersion);

                    Console.WriteLine("Версия {0} продукта {1} успешно обновлена", CurrentVersion.NumberVersion, CurrentVersion.NameProduct);
                    return;
                }
            }
            
            Console.WriteLine("Обновляемой версии продукта не существует");
        }

        public void RemoveVersion(string NameProduct, string NumberVersion) {
            Version RemovedVersion = GetInformationProductConcreteVersion(NameProduct, NumberVersion);

            if (RemovedVersion != null && Versions.Contains(RemovedVersion)) {
                    Versions.Remove(RemovedVersion);
                    Console.WriteLine("Версия {0} продукта {1} успешно удалена", NumberVersion, NameProduct);
                    RemoveProduct(RemovedVersion);
            }
            else {
                Console.WriteLine("Удаляемой версии продукта не существует!");
            }
            
        }

        private void RemoveProduct(Version RemovedVesion) {
            foreach (Product CurrentProduct in Products) {
                if (CurrentProduct.GetAllVersions().Contains(RemovedVesion)) {
                    CurrentProduct.RemoveVersion(RemovedVesion);
                    if (CurrentProduct.GetAllVersions().Count == 0) {
                        Products.Remove(CurrentProduct);
                        Console.WriteLine("Продукт {0} успешно удален", CurrentProduct.GetNameProduct());
                        break;
                    }
                }
            }
        }
        
        //Проверка корректности введеного номера версии продукта ( . . . )
        private bool CheckCorrectNumberVersion(string NumberVersion) {
            try {
                string[] SplitedVersion = NumberVersion.Split('.');

                if (SplitedVersion.Length != 3) return false;

                int SummedNumbersVersion = 0;

                for (int i = 0; i < 3; ++i) {
                    SummedNumbersVersion += Convert.ToInt32(SplitedVersion[i]);
                }

                return true;
            }
            catch {
                return false;
            }
        }

    }
    
}
