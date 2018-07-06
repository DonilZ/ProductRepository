using System;
using System.Collections.Generic;
using System.Linq;

namespace repository {

    ///<summary>
    ///Класс, имитирующий работу с базой данных (хранение версий и продуктов, получение необходимой информации о версиях и продуктах, добавление, обновление и удаление версий и продуктов) 
    ///</summary>
    class ProductRepository {
        private static ProductRepository _instanceDatabase; /*это ведь как раз ссылка на singleton? Тогда лучше _singletonInstance*/
        private List<Version> _versions;
        private List<Product> _products;

        /*
         * Никак не могу понять, зачем здесь отдельный список версий? Ведь они внутри продуктов есть! Понятно, что это могут быть ссылки на
         * одни и те же объекты, но ведь их нужно держать в согласованном состоянии. В случае с EntityFramework например, это можно делать,
         * так как он сам за согласованностью следит, но в случае со спиками - зачем так сложно? Я пока просто прокомментарил отдельные фрагменты
         * в коде этого класса, но на этот момент внимания не обращал. Можешь или сам попробовать поправить, или оставить до следующей итерации.
         */


        private ILogger _log; /*логгер и лог - это разные вещи. здесь - логгер*/

        private ProductRepository(ILogger log) {
            _versions = new List<Version>();
            _products = new List<Product>();
            _log = log;
        }

        /// <summary>
        /// Метод для создания объекта класса ProductRepository (если объект уже был создан, то метод вернет этот объект)
        /// </summary>
        public static ProductRepository CreateDatabase(ILogger log) {

            /*Лучше назвать мето GetInstance() или Instance(). Во-первых, тут не всегда создается инстанс. Только если его нет. Во-вторых, снаружи
             код может и не знать что так есть БД, она создается и т.п. Есть репозиторий и он возвращает данные.*/

            return _instanceDatabase = _instanceDatabase ?? new ProductRepository(log); 
        }

         /*
          * ПРАВКИ:
          * 1) Убрал Information из названий методов.
          * 2) Все же Get мне хотелось бы оставить, так как, например, метод с названием Products мне не очень нравится.
          * 
          * КОММЕНТАРИИ
          * 2) В данном проекте - возражений нет.
          * 
          */

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
            return _products.Where(product => product.GetProductName() == productName).SingleOrDefault()?.GetAllVersions();

             /*
              * ВОПРОСЫ:
              * 1) Оператор ? (конкретно в выражении return Products.Where(product => product.GetProductName() == ProductName).SingleOrDefault()?.GetAllVersions();) 
              * после подвыражения - это просто, грубо говоря, тернарный оператор, проверяющий, не является ли подвыражение перед ним null?
              * 2) Проверьте пожалуйста, ничего ли я не упустил, вставляя linq (предшествующий код я закомментировал) в тех местах, где до этого использовался цикл foreach.
             */


            /*
             * 1) Именно так. Это синтаксический сахар. Но очень удобный, т.к. такие проверки в случае сложной структуры объектов в модели нужны очень часто.
             * 2) Хорошо. Прокомментрую конкретно в тех местах.
             */
        }

        /// <summary>
        /// Метод для получения конкретной версии продукта
        /// </summary>
        public Version GetProductConcreteVersion(string productName, string productVersion) {
            List<Version> currentVersions = GetProductVersions(productName);

            /* if (CurrentVersions != null) {          
                foreach (Version CurrentVersion in CurrentVersions) {
                    if (ProductVersion == CurrentVersion.ProductVersion) {
                        return CurrentVersion;
                    }
                }
            }
            
            return null; */

            /*
             * Строго говоря, цикл у тебя искал FirstOrDefault(). Его отличие от SingleOrDefault() в том, что если будет два удовлетворяющих объекта, 
             * то он выломается с исключением. А FirstOrDefault() - нет. Но с точки зрения логики здесь правильнее SingleOrDefault().
             */

            return currentVersions?.Where(version => version.ProductVersion == productVersion).SingleOrDefault();
        }

        /// <summary>
        /// Метод для добавления версии (и, возможно, продукта) в репозиторий
        /// </summary>
        public void AddVersion(Version addedVersion, Product addedProduct) {
            /*
             * Мне не очень понятно, зачем в API репозитория есть единственный метод Add с такой сигнатурой. Получается, что для добавления новой версии
             * уже существующего продукта пользователь API все-равно должен создать экземпляр Product. А в итоге это бесполезное действие. Я бы здесь 
             * лучше подавал только Version, а Product при необходимость создавал прямо здесь. Ну или можно сделать два разных метода в API.
             */


            /*
             * Попробуй в коде этого метода применить рефакторинг "invert if". В случае с первым if-ом нужно просто проверять отрицание того же условия,  
             * а потом писать в лог и делать return. В этом случае код в ветке else можно уже в нее не оборачивать, а писать на том же уровне. Код будет менее 
             * вложенным.
             *
             */

            if (CheckCorrectProductVersion(addedVersion.ProductVersion)) {

                
                if (CheckUniqueProduct(addedVersion)) { 
                    /* У тебя в условии, и далее в GetProduct происходит один и тот же перебор продуктов. Нельзя ли 
                     * просто получить продукт через GetProduct, а если вернулся null, создать его здесь же добавить. В любом из двух вариантов
                     * у тебя на руках будет продукт, к которому нужно просто добавить версию. Код будет короче. 
                     */
                    _products.Add(addedProduct);
                    _log.LogRecord($"Новый продукт {addedVersion.ProductName} успешно добавлен");
                }
                else {
                    Product productFromDatabase = GetProduct(addedProduct.GetProductName());
                    
                    if (!productFromDatabase.CheckAddedVersion(addedVersion)) {
                       _log.LogRecord("Данная версия не может быть добавлена, так как не является новой");
                        return;
                    }

                    AddVersionToProduct(addedVersion);

                }

                _versions.Add(addedVersion);
                
                _log.LogRecord($"Версия {addedVersion.ProductVersion} продукта {addedVersion.ProductName} успешно добавлена");
            }
            else {
                _log.LogRecord("Номер версии введен некорректно");
            }
        }

        /// <summary>
        /// Метод для проверки наличия в репозитории конкретного продукта
        /// </summary>
        private bool CheckUniqueProduct(Version addedVersion) {
            /* foreach (Product CurrentProduct in Products) {
                if (CurrentProduct.GetProductName() == AddedVersion.ProductName) {
                    return false;
                }
            }
            return true; */

            return _products.Where(product => product.GetProductName() == addedVersion.ProductName).SingleOrDefault() == null ? true : false;

            /*
             * Можно проще:
             * return !_products.Any(product => product.GetProductName() == addedVersion.ProductName);
             * 
             * И еще, метод лучше переименовать в IsProductUnique()
             */
        }

        /// <summary>
        /// Метод для добавления версии в общий список всех версий конкретного продукта
        /// </summary>
        private void AddVersionToProduct(Version addedVersion) {
            /* foreach (Product CurrentProduct in Products) {
                if (CurrentProduct.GetProductName() == AddedVersion.ProductName) {
                    CurrentProduct.AddVersion(AddedVersion);
                    return;
                }
            } */

            _products.Where(product => product.GetProductName() == addedVersion.ProductName).SingleOrDefault()?.AddVersion(addedVersion); 

            /*
             * Тут лучше использовать метод GetProduct, а не копипастить его код. 
             */
        }

        /// <summary>
        /// Метод для получения конкретного продукта из репозитория по имени этого продукта
        /// </summary>
        private Product GetProduct(string productName) {
            /*for(int i = 0; i < Products.Count; ++i) {
                if (Products[i].GetProductName() == ProductName) {
                    return Products[i];
                }
            }

            return null;*/

            return _products.Where(product => product.GetProductName() == productName).SingleOrDefault();
        }

        /// <summary>
        /// Метод для обновления конкретной версии в репозитории
        /// </summary>
        public void UpdateVersion(Version updatedVersion) {
            /* foreach (Version CurrentVersion in Versions) {
                if (UpdatedVersion.ProductName == CurrentVersion.ProductName && UpdatedVersion.ProductVersion == CurrentVersion.ProductVersion) { 
                    CurrentVersion.Update(UpdatedVersion);

                    _log.LogRecord($"Версия {CurrentVersion.ProductVersion} продукта {CurrentVersion.ProductName} успешно обновлена");
                    return;
                }
            }
            
            _log.LogRecord("Обновляемой версии продукта не существует"); */

            Version currentVersion = _versions.Where(version => version.ProductName == updatedVersion.ProductName && 
                                                    version.ProductVersion == updatedVersion.ProductVersion).SingleOrDefault();
            if (currentVersion != null) {
                currentVersion.Update(updatedVersion);
                _log.LogRecord($"Версия {currentVersion.ProductVersion} продукта {currentVersion.ProductName} успешно обновлена");
                return;
            }

            _log.LogRecord("Обновляемой версии продукта не существует");

            /*Здесь тоже можно было бы немного "упростить" код, если сначала проверить currentVersion == null. Тогда бы внутри if было только две строки кода, а не 3*/

        }

       
        /// <summary>
        /// Метод для удаления конкретной версии (и, возможно, продукта) из репозитория
        /// </summary>
        public void RemoveVersion(string productName, string productVersion) {
            Version removedVersion = GetProductConcreteVersion(productName, productVersion); /*лучше переменную назвать versionToRemove*/

            if (removedVersion != null && _versions.Contains(removedVersion)) {
                    _versions.Remove(removedVersion);
                    _log.LogRecord($"Версия {productVersion} продукта {productName} успешно удалена");
                    RemoveProduct(removedVersion);
            }
            else {
                _log.LogRecord("Удаляемой версии продукта не существует!");
            }
            
        }

        /// <summary>
        /// Метод для удаления конкретного продукта из репозитория
        /// </summary>
        private void RemoveProduct(Version removedVersion) {
            /* foreach (Product CurrentProduct in Products) {
                if (CurrentProduct.GetAllVersions().Contains(RemovedVersion)) {
                    CurrentProduct.RemoveVersion(RemovedVersion);
                    if (CurrentProduct.GetAllVersions().Count == 0) {
                        Products.Remove(CurrentProduct);
                        _log.LogRecord($"Продукт {CurrentProduct.GetProductName()} успешно удален");
                        break;
                    }
                }
            } */

            Product currentProduct = _products.Where(product => product.GetAllVersions().Contains(removedVersion)).SingleOrDefault();
            currentProduct.RemoveVersion(removedVersion);
            
            if (!currentProduct.GetAllVersions().Any()) {
                _products.Remove(currentProduct);
                _log.LogRecord($"Продукт {currentProduct.GetProductName()} успешно удален");
            }
        }
        
        /*
         * CheckCorrectProductVersion переводится как "проверить корректную версию продукта", а ты хочешь проверить версию на корректность. Так что, 
         * лучше IsProductVersionCorrect(string productVersion);
         */

        /// <summary>
        /// Метод для проверки корректности введенного номера версии
        /// </summary>
        private bool CheckCorrectProductVersion(string productVersion) {
            try {
                string[] splitedVersion = productVersion.Split('.');

                if (splitedVersion.Length != 3) return false;

                int summedNumbersVersion = 0;

                for (int i = 0; i < 3; ++i) {
                    summedNumbersVersion += Convert.ToInt32(splitedVersion[i]);
                }

                return true;
            }
            catch {
                return false;
            }
        }

    }
    
}
