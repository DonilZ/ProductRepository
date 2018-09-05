using System;
using System.Collections.Generic;
using System.Linq;

namespace repository {

    public abstract class ProductRepository {

        /// <summary>
        /// Метод для получения общего списка всех продуктов, находящихся в репозитории
        /// </summary>
        public abstract List<Product> GetProducts();

        /// <summary>
        /// Метод для получения общего списка всех версий конкретного продукта
        /// </summary>
        public abstract List<Version> GetProductVersions(string productName);

        /// <summary>
        /// Метод для получения конкретной версии продукта
        /// </summary>
        public abstract Version GetConcreteVersion(string productName, string productVersion);

        /// <summary>
        /// Метод для добавления версии (и, возможно, продукта) в репозиторий
        /// </summary>
        public abstract bool AddVersion(string productName, Version newVersion);

        /// <summary>
        /// Метод для обновления конкретной версии в репозитории
        /// </summary>
        public abstract bool UpdateVersion(string productName, Version updatedVersion);

        /// <summary>
        /// Метод для удаления конкретной версии (и, возможно, продукта) из репозитория
        /// </summary>
        public abstract bool RemoveVersion(string productName, string productVersion);

        /// <summary>
        /// Метод для проверки корректности введенного номера версии
        /// </summary>
        protected bool IsProductVersionCorrect(string productVersion) {
            try {
                string[] splitedVersion = productVersion.Split('.');
                int countOfNumbersInVersion = 3;

                if (splitedVersion.Length != countOfNumbersInVersion) return false;

                int summedNumbersVersion = 0;

                for (int i = 0; i < countOfNumbersInVersion; ++i) {
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
