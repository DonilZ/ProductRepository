using System;
using System.Collections.Generic;
using System.Linq;

namespace repository {

    /*
     * У тебя класс DatabaseManager получился почти реалиазией шаблона Репозиторий, т.к. он инкапсулирует в себе запросную логику и предоставляет наружу более 
     * простой интерфейс для работы с данными. Это хорошо! Иногда такие объекты называют репозиториями (в твоем случае - ProductRepository), иногда - сервисами. Сервис, конечно,
     * немного более широкое понятие, чем репозиторий; репозиторий может быть лишь частью сервиса. Но можно и сервисом назвать - ProductService. Предлагаю выбрать
     * одно из этих двух названий, т.к. DatabaseManager - это что-то, что управляет БД, в смысле создания, резервного копирования и т.п. 
     */

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

            /*
             * Для такой логики есть более короткий способ записи: return InstanceDatabase = InstanceDatabase ?? new DatabaseManager(); 
             * Просто для общего развития погляди, плс.
             */
        }

        /*
         * Мне кажется, термин Information у тебя лишний здесь. Сама модель - это продукты и версии. Понятно, что это не сами продукты, а информация о них. Но, все в
         * информационных системах - лишь информация об объектах. Так что, в данном случае это не добавляет ничего к пониманию, а код удлинняет. К тому же, усложняет перевод на 
         * англиский. Предлагаю все методы переименовать на GetProducts(), GetProductVersions() и т.п.
         * 
         * И еще один момент - в данном случае слово Get можно тоже опустить. Потому что понятно, что если ты пишешь dbInstance.Products(), то это и есть получение всех продуктов.
         * В некоторых книгах есть такие советы. Хотя есть и те, кто строго требует использования слова Get в данном случае.
         */

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

            /*
             * С использованием linq этот код можно записать в одну строку:
             * 
             *  return Products.Where(product => product.GetNameProduct() == NameProduct).SingleOrDefault()?.GetAllVersions();
             *  
             * Так как мы активно используем linq, то предлагаю немного его освоить. И еще, здесь используется оператор ?. - Это чтобы 
             * в случае отсутствия объектов вернулся null, а не вылетело null reference exception.
             */
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

        /*
         * Обычно в web-приложениях такие сервисы - это часть бэкэнда. А он может не быть консольным приложение, так что консоли там может просто не оказаться. Но 
         * сама идея записывать информацию об изменениях - правильная. Просто ее нужно писать в лог. Я тебе предлагаю при создании класса передать ему объект, реализцющий интерфейс 
         * ILogger с методом Log(), и в качестве реализации использовать обертку над консолью. Что-то вроде: 
         * class ConsoleLogger: ILogger
         * {
         *    public void Log(string logText) {
         *      Console.WriteLine(logText);
         *    }
         * }
         * 
         * Такой подход позволит тебе потом вместо консольного логгера использовать файловый логгер, например. 
         */


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
