using System;
using System.Collections.Generic;
using System.Linq;

namespace repository {

    ///<summary>
    ///Класс, осуществляющий работу с базой данных PostgreSQL (хранение версий и продуктов, получение необходимой информации о версиях и продуктах, добавление, обновление и удаление версий и продуктов) 
    ///</summary>
    public class PostgreSQLProductRepository : ProductRepository {
        private ConnectionData _connectionData;
        private ILogger _logger;
        private VersionContext _database;

        public PostgreSQLProductRepository(ConnectionData connectionData, ILogger logger) {
            _connectionData = connectionData;
            _logger = logger;
            _database = new VersionContext(_connectionData);
        }

        public override List<Product> GetProducts() {
            List<Version> latestVersionsOfAllProducts = getTheLatestVersionsOfAllProducts();

            List<Product> allProducts = formAListOfAllProducts(latestVersionsOfAllProducts);

            return allProducts;
        }

        private List<Version> getTheLatestVersionsOfAllProducts() {
            return _database.Versions
                    .OrderBy(version => version.ProductVersion)
                    .GroupBy(version => version.ProductId)
                    .Select(x => x.Last()).ToList();
        }

        private List<Product> formAListOfAllProducts(List<Version> latestVersionsOfAllProducts) {
            List<Product> allProducts = new List<Product>();

            foreach(var latestVersion in latestVersionsOfAllProducts) {
                Product currentProduct = _database.Products
                                        .Single(product => product.Id == latestVersion.ProductId);

                currentProduct.InitializeLatestVersion(latestVersion);

                allProducts.Add(currentProduct);
            }

            return allProducts;
        }

        public override List<Version> GetProductVersions(string productName) {
            List<Version> requiredVersions = formAListOfVersionsOfTheDesiredProduct(productName);

            return requiredVersions;                            
        }

        private List<Version> formAListOfVersionsOfTheDesiredProduct (string productName) {
            Product currentProduct = getProductFromDatabase(productName);

            return currentProduct == null ? null : _database.Versions
                                                    .Where(version => version.ProductId == currentProduct.Id)
                                                    .OrderBy(version => version.ProductVersion).ToList();
        }

        public override Version GetConcreteVersion(string productName, string productVersion) {
            Product currentProduct = getProductFromDatabase(productName);

            Version requiredVersion = currentProduct == null ? null : 
                                    _database.Versions
                                    .SingleOrDefault(version => version.ProductId == currentProduct.Id
                                                    && version.ProductVersion == productVersion);

            return requiredVersion;                            
        }

        public override bool AddVersion(string productName, Version newVersion) {

            if (!IsProductVersionCorrect(newVersion.ProductVersion)) {
                _logger.LogRecord("Номер версии введен некорректно");
                return false;
            }

            Product currentProduct = getProductFromDatabase(productName);

            if (currentProduct == null) {
                currentProduct = new Product(productName);
                currentProduct.InitializeLatestVersion(newVersion);

                addProduct(currentProduct);

                _logger.LogRecord($"Новый продукт {productName} успешно добавлен");
            }
            else {
                Version latestVersionOfCurrentProduct = _database.Versions
                                                        .OrderBy(version => version.ProductVersion)
                                                        .Last(version => version.ProductId == currentProduct.Id);

                currentProduct.InitializeLatestVersion(latestVersionOfCurrentProduct);
                
                if (!currentProduct.NewVersionIsGreaterThenLatest(newVersion)) {
                    _logger.LogRecord("Данная версия не может быть добавлена, так как не является новой");
                    return false;
                } 
            }

            newVersion.SetProductId(currentProduct.Id);

            _database.Versions.Add(newVersion);
            _database.SaveChanges();
                
            _logger.LogRecord($"Версия {newVersion.ProductVersion} продукта {productName} успешно добавлена");

            return true;                                                     
        }

        private void addProduct(Product newProduct) {
            _database.Products.Add(newProduct);
            _database.SaveChanges();
        }

        public override bool UpdateVersion(string productName, Version updatabaleVersion) {

            Product currentProduct = getProductFromDatabase(productName);

            if (currentProduct == null) {
                _logger.LogRecord("Продукта с таким именем не существует!");
                return false;
            }

            if (!_database.Versions.Any(version => version.ProductId == currentProduct.Id 
                                        && version.ProductVersion == updatabaleVersion.ProductVersion)) {

                _logger.LogRecord("Обновляемой версии продукта не существует!");
                return false;
            }

            Version versionForUpdateFromRepository = _database.Versions
                                                    .Single(version => version.ProductId == currentProduct.Id 
                                                            && version.ProductVersion == updatabaleVersion.ProductVersion);

            versionForUpdateFromRepository.Update(updatabaleVersion);
            _database.SaveChanges();

            _logger.LogRecord($"Версия {updatabaleVersion.ProductVersion} продукта {productName} успешно обновлена");
            
            return true;
        }

        public override bool RemoveVersion(string productName, string productVersion) {

            Product currentProduct = getProductFromDatabase(productName);

            if (currentProduct == null) {
                _logger.LogRecord("Продукта с таким именем не существует!");
                return false;
            }

            if (!_database.Versions.Any(version => version.ProductId == currentProduct.Id 
                                            && version.ProductVersion == productVersion)) {

                _logger.LogRecord("Удаляемой версии продукта не существует!");
                return false;
            }


            _database.Versions.Remove(_database.Versions.First((version => version.ProductId == currentProduct.Id 
                                                                    && version.ProductVersion == productVersion)));
            _database.SaveChanges();

            _logger.LogRecord($"Версия {productVersion} продукта {productName} успешно удалена");


             if (!IsTheProductHaveAtLeastOneVersion(currentProduct)) {
                removeProduct(productName);
            
                _logger.LogRecord($"Продукт {productName} успешно удален");
            }

            return true;
        }

        private bool IsTheProductHaveAtLeastOneVersion(Product currentProduct) {
            return _database.Versions.Any(version => version.ProductId == currentProduct.Id);
        }

        private void removeProduct (string nameOfTheRemovableProduct) {
             Product removableProduct = _database.Products
                                            .Single(product => product.ProductName == nameOfTheRemovableProduct);

            _database.Products.Remove(removableProduct);
            _database.SaveChanges();
        }

        private Product getProductFromDatabase(string productName) {
            Product currentProduct = _database.Products
                                    .SingleOrDefault(product => product.ProductName == productName);

            return currentProduct;
        }

    }

}
