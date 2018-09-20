using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace repository {

    ///<summary>
    ///Класс, осуществляющий работу с базой данных PostgreSQL (хранение версий и продуктов, получение необходимой информации о версиях и продуктах, добавление, обновление и удаление версий и продуктов) 
    ///</summary>
    public class PostgreSQLProductRepository : ProductRepository {
        private ILogger _logger;
        private VersionContext _database;

        public PostgreSQLProductRepository(ILogger logger) {
            _logger = logger;
            _database = new VersionContext();
        }

        public override List<Product> GetProducts() {
            List<Product> allProducts = _database.Products
                                        .Include(product => product.AllVersions).ToList();

            return allProducts;
        }

        public override List<Version> GetProductVersions(string productName) {
            List<Version> requiredVersions = formAListOfVersionsOfTheDesiredProduct(productName);

            return requiredVersions;                            
        }

        private List<Version> formAListOfVersionsOfTheDesiredProduct (string productName) {
            Product currentProduct = getProductFromDatabase(productName);

            return currentProduct == null ? null : currentProduct.AllVersions;
        }

        public override Version GetConcreteVersion(string productName, string productVersion) {
            Version requiredVersion = extractDesiredVersionFromProduct(productName, productVersion);

            return requiredVersion;                            
        }

        private Version extractDesiredVersionFromProduct (string productName, string productVersion) {
            Product currentProduct = getProductFromDatabase(productName);

            return currentProduct == null ? null :  currentProduct.AllVersions
                                                    .SingleOrDefault(version => version.ProductVersion == productVersion);
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
                if (!currentProduct.NewVersionIsGreaterThenLatest(newVersion)) {
                    _logger.LogRecord("Данная версия не может быть добавлена, так как не является новой");
                    return false;
                }
                
                newVersion.SetContainProduct(currentProduct);
                _database.Versions.Add(newVersion); 
            }

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

            if (!currentProduct.AllVersions.Any(version => version.ProductVersion == updatabaleVersion.ProductVersion)) {
                _logger.LogRecord("Обновляемой версии продукта не существует!");
                return false;
            }

            Version versionForUpdate = currentProduct.AllVersions
                                                    .First(version => version.ProductVersion == updatabaleVersion.ProductVersion);

            versionForUpdate.Update(updatabaleVersion);
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

            if (!currentProduct.AllVersions.Any(version => version.ProductVersion == productVersion)) {

                _logger.LogRecord("Удаляемой версии продукта не существует!");
                return false;
            }


            _database.Versions.Remove(currentProduct.AllVersions.First(version => version.ProductVersion == productVersion));
            _database.SaveChanges();

            _logger.LogRecord($"Версия {productVersion} продукта {productName} успешно удалена");


             if (!IsTheProductHaveAtLeastOneVersion(currentProduct)) {
                removeProduct(productName);
            
                _logger.LogRecord($"Продукт {productName} успешно удален");
            }

            return true;
        }

        private bool IsTheProductHaveAtLeastOneVersion(Product currentProduct) {
            return currentProduct.GetAllVersions().Any();
        }

        private void removeProduct (string nameOfTheRemovableProduct) {
             Product removableProduct = _database.Products
                                            .Single(product => product.ProductName == nameOfTheRemovableProduct);

            _database.Products.Remove(removableProduct);
            _database.SaveChanges();
        }

        private Product getProductFromDatabase(string productName) {
            Product currentProduct = _database.Products.Include(product => product.AllVersions)
                                    .SingleOrDefault(product => product.ProductName == productName);

            return currentProduct;
        }

    }

}
