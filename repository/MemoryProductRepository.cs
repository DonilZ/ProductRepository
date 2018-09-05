using System;
using System.Collections.Generic;
using System.Linq;

namespace repository {
    
    ///<summary>
    ///Класс, имитирующий работу с базой данных в памяти (хранение версий и продуктов, получение необходимой информации о версиях и продуктах, добавление, обновление и удаление версий и продуктов) 
    ///</summary>
    public class MemoryProductRepository : ProductRepository {
        private List<Version> _versions;
        private List<Product> _products;

        private ILogger _logger; 

        public MemoryProductRepository(ILogger logger) {
            _versions = new List<Version>();
            _products = new List<Product>();
            _logger = logger;
        }

        public override List<Product> GetProducts() {
            return _products;
        }

        public override List<Version> GetProductVersions(string productName) { 
            return _products.SingleOrDefault(product => product.ProductName == productName)?.GetAllVersions(); 
        }

        public override Version GetConcreteVersion(string productName, string productVersion) {
            List<Version> currentVersions = GetProductVersions(productName);

            return currentVersions?.SingleOrDefault(version => version.ProductVersion == productVersion);
        }

        public override bool AddVersion(string productName, Version newVersion) {
            if (!IsProductVersionCorrect(newVersion.ProductVersion)) {
                _logger.LogRecord("Номер версии введен некорректно");
                return false;
            }

            Product potentiallyNewProduct = GetProduct(productName);

            if (potentiallyNewProduct == null) {
                potentiallyNewProduct = new Product(productName);
                potentiallyNewProduct.InitializeLatestVersion(newVersion);
            }

            if (IsThereProduct(potentiallyNewProduct) && !potentiallyNewProduct.NewVersionIsGreaterThenLatest(newVersion)) {
                _logger.LogRecord("Данная версия не может быть добавлена, так как не является новой");
                return false;
            }

            if (!IsThereProduct(potentiallyNewProduct)) {
                
                _products.Add(potentiallyNewProduct);

                _logger.LogRecord($"Новый продукт {productName} успешно добавлен");
            }
            else {
                AddVersionToProduct(productName, newVersion);
            }

            _versions.Add(newVersion);
                
            _logger.LogRecord($"Версия {newVersion.ProductVersion} продукта {productName} успешно добавлена");

            return true;
        }

        /// <summary>
        /// Метод для проверки продукта на уникальность
        /// </summary>
        private bool IsThereProduct(Product potentiallyNewProduct) {
            string namePotentiallyNewProduct = potentiallyNewProduct.ProductName;

            return GetProduct(namePotentiallyNewProduct) != null;
        }

        /// <summary>
        /// Метод для добавления версии в общий список всех версий конкретного продукта
        /// </summary>
        private void AddVersionToProduct(string productName, Version newVersion) {
            GetProduct(productName)?.AddVersion(newVersion); 
        }

        /// <summary>
        /// Метод для получения конкретного продукта из репозитория по имени этого продукта
        /// </summary>
        private Product GetProduct(string productName) {
            return _products.SingleOrDefault(product => product.ProductName == productName);
        }

        public override bool UpdateVersion(string productName, Version updatedVersion) {
            Product currentProduct = _products.SingleOrDefault(product => product.ProductName == productName);

            if (currentProduct == null) {
                _logger.LogRecord("Обновляемого продукта не существует!");
                return false;
            }

            Version currentVersion = currentProduct.GetAllVersions().SingleOrDefault(version => version.ProductVersion == updatedVersion.ProductVersion);

            if (currentVersion == null) {
                _logger.LogRecord("Обновляемой версии продукта не существует!");
                return false;
            }
            
            currentVersion.Update(updatedVersion);
            _logger.LogRecord($"Версия {currentVersion.ProductVersion} продукта {productName} успешно обновлена");
            return true;
        }

        public override bool RemoveVersion(string productName, string productVersion) {
            Version versionToRemove = GetConcreteVersion(productName, productVersion); 

            if (versionToRemove == null || !_versions.Contains(versionToRemove)) {
                _logger.LogRecord("Удаляемой версии продукта не существует!");
                return false;
            }

            _versions.Remove(versionToRemove);

            _logger.LogRecord($"Версия {productVersion} продукта {productName} успешно удалена");

            RemoveProduct(versionToRemove);

            return true;

        }

        /// <summary>
        /// Метод для удаления конкретного продукта из репозитория
        /// </summary>
        private void RemoveProduct(Version removedVersion) {
            Product currentProduct = _products.SingleOrDefault(product => product.GetAllVersions().Contains(removedVersion));
            currentProduct.RemoveVersion(removedVersion);
            
            if (!currentProduct.GetAllVersions().Any()) {
                _products.Remove(currentProduct);
                _logger.LogRecord($"Продукт {currentProduct.ProductName} успешно удален");
            }
        }
        
    }
}