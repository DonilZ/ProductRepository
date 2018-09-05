using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using repository;
using Moq;

namespace repository.UnitTests {

    /// <summary>
    /// Тестовый класс для тестирования публичных методов класса PostgreSQLProductRepository
    /// </summary>
    [Ignore("Системные тесты с внешней зависимостью в виде базы данных")]
    [TestFixture]
    public class PostgreSQLProductRepositoryUnitTest {
        private string _lastMessageFromLog;
        private ConnectionData _connectionData;
        private VersionContext _database;
        public PostgreSQLProductRepository CurrentProductRepository;
        public Mock<ILogger> MockLogger { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp() {
            _connectionData = new ConnectionData("localhost", "5432", "repository_db", "donilz", "1234");

            MockLogger = new Mock<ILogger>();
            MockLogger.Setup(m => m.LogRecord(It.IsAny<string>())).Callback<string>((message) => _lastMessageFromLog = message);

            CurrentProductRepository = new PostgreSQLProductRepository(_connectionData, MockLogger.Object);

            _database = new VersionContext(_connectionData);
        }

        [TearDown]
        public void TearDown() {
            clearProducts();
            _database.SaveChanges();
        }

        private void clearProducts() {
            var products = from product in _database.Products
                            select product;
            foreach(var product in products) {
                _database.Products.Remove(product);
            }
        }

        [Test]
        public void AddVersion_WithIncorrectVersionNumber_ReturnsFalseAndWriteThisToLog() {
            //Arrange
            Version versionWithIncorrectVersionNumber = CreateNewVersion(".1.1.");
            string productName = "productName";

            //Act           
            bool IsVersionWithIncorrectVersionNumberInRepository = CurrentProductRepository.AddVersion(productName, versionWithIncorrectVersionNumber);

            //Assert
            string expectedMessage = "Номер версии введен некорректно";
            string actualMessage = _lastMessageFromLog;


            Assert.AreEqual(expectedMessage, actualMessage);
            Assert.False(IsVersionWithIncorrectVersionNumberInRepository);
        }

        [Test]
        public void AddVersion_WithLatestVersionNumberAndUniqueProductName_AddedNewProductAndNewVersionInRepository() {
            //Arrange
            Version newVersion = CreateNewVersion("1.1.1");
            string productName = "productName";

            //Act
            CurrentProductRepository.AddVersion(productName, newVersion);

            //Assert
            Product potentiallyAddedProduct = new Product(productName);
            potentiallyAddedProduct.InitializeLatestVersion(newVersion);

            bool WasProductAdded = IsThereProduct(potentiallyAddedProduct);
            bool WasVersionAdded = IsThereVersion(productName, newVersion);

            Assert.True(WasProductAdded);
            Assert.True(WasVersionAdded);        
        }

        [TestCase("1.1.0")]
        [TestCase("1.0.1")]
        [TestCase("0.1.1")]
        [TestCase("0.0.1")]
        [TestCase("0.1.0")]
        [TestCase("1.0.0")]
        [TestCase("1.1.1")]
        [TestCase("0.0.0")]
        public void AddVersion_WithOldVersionNumber_ReturtnsFalseAndWriteThisToLog(string oldVersionNumber) {
            //Arrange
            string productName = "productName";
            Version latestVersion = CreateNewVersion("1.1.1");
            Version oldVersion = CreateNewVersion(oldVersionNumber);

            CurrentProductRepository.AddVersion(productName, latestVersion);

            //Act
            bool IsOldVersionInRepository = CurrentProductRepository.AddVersion(productName, oldVersion);

            //Assert
            string expectedMessage = "Данная версия не может быть добавлена, так как не является новой";
            string actualMessage = _lastMessageFromLog;

            Assert.AreEqual(expectedMessage, actualMessage);
            Assert.False(IsOldVersionInRepository);
        }

        [TestCase("1.1.2")]
        [TestCase("1.2.1")]
        [TestCase("2.1.1")]
        [TestCase("2.2.1")]
        [TestCase("2.1.2")]
        [TestCase("1.2.2")]
        [TestCase("2.2.2")]
        [TestCase("1.1.10")]
        public void AddVersion_WithLatestVersionNumberButWithAnExistingProductName_AddedNewVersionInListOfProductVersionsAndNotDeleteOldVersion(string newVersionNumber) {
            //Arrange
            string productName = "productName";
            Version latestVersion = CreateNewVersion("1.1.1");
            Version newVersion = CreateNewVersion(newVersionNumber);

            CurrentProductRepository.AddVersion(productName, latestVersion);

            //Act
            CurrentProductRepository.AddVersion(productName, newVersion);

            //Assert
            bool isRepositoryContainLatestVersion = CurrentProductRepository.GetProductVersions(productName).Contains(latestVersion);
            bool isRepositoryContainNewVersion = CurrentProductRepository.GetProductVersions(productName).Contains(newVersion);

            Assert.True(isRepositoryContainLatestVersion);
            Assert.True(isRepositoryContainNewVersion);         
        }

        [Test]
        public void UpdateVersion_UpdatableProductNotFoundInRepository_ReturnsFalseAndWriteToLogAboutNonExistentUpdatableProduct() {
            //Arrange
            string productName = "productName";
            Version updatableVersion = CreateNewVersion("1.1.1");

            //Act
            bool IsUpdatableVersionUpdatedInRepository = CurrentProductRepository.UpdateVersion(productName, updatableVersion);

            //Assert
            string expectedMessage = "Продукта с таким именем не существует!";
            string actualMessage = _lastMessageFromLog;

            Assert.AreEqual(expectedMessage, actualMessage);
            Assert.False(IsUpdatableVersionUpdatedInRepository);         
        }

        public void UpdateVersion_UpdatableVersionNotFoundInRepository_ReturnsFalseAndWriteToLogAboutNonExistentUpdatableVersion() {
            //Arrange
            string productName = "productName";
            Version updatableVersion = CreateNewVersion("1.1.1");
            Version newVersion = CreateNewVersion("1.1.2");

            CurrentProductRepository.AddVersion(productName, updatableVersion);

            //Act
            bool IsUpdatableVersionUpdatedInRepository = CurrentProductRepository.UpdateVersion(productName, newVersion);

            //Assert
            string expectedMessage = "Обновляемой версии продукта не существует!";
            string actualMessage = _lastMessageFromLog;

            Assert.AreEqual(expectedMessage, actualMessage);
            Assert.False(IsUpdatableVersionUpdatedInRepository);         
        }

        
        [Test]
        public void UpdateVersion_UpdatableVersionExistInRepository_UpdateVersionInRepositoryAndWriteToLogThisReport() {
            //Arrange
            string productName = "productName";
            Version firstVersion = CreateNewVersion("1.1.1");

            CurrentProductRepository.AddVersion(productName, firstVersion);

            Version newVersion = new Version(firstVersion.ProductVersion, "newShortDesc", 
                                            "newLongDesc", "newChanges", "newFileName", "newFileUrl");

            //Act
            CurrentProductRepository.UpdateVersion(productName, newVersion);

            //Assert
            bool isFirstVersionExistInRepository = IsThereVersion(productName, CreateNewVersion("1.1.1"));
            bool isUpdatableVersionExistInRepository = IsThereVersion(productName, newVersion);

            string expectedMessage = $"Версия {firstVersion.ProductVersion} продукта {productName} успешно обновлена";
            string actualMessage = _lastMessageFromLog;

            Assert.False(isFirstVersionExistInRepository);
            Assert.True(isUpdatableVersionExistInRepository);
            Assert.AreEqual(expectedMessage, actualMessage);       
        }

        [Test]
        public void RemoveVersion_RemovableProductNotFoundInRepository_ReturnsFalseAndWriteToLogAboutNonExistentRemovableProduct() {
            //Arrange
            string productName = "productName";
            Version removableVersion = CreateNewVersion("1.1.1");

            //Act
            bool IsRemovableVersionRemovedFromRepository = CurrentProductRepository
                                                        .RemoveVersion(productName, removableVersion.ProductVersion);

            //Assert
            string expectedMessage = "Продукта с таким именем не существует!";
            string actualMessage = _lastMessageFromLog;

            Assert.AreEqual(expectedMessage, actualMessage);
            Assert.False(IsRemovableVersionRemovedFromRepository);
        }

        [Test]
        public void RemoveVersion_RemovableVersionNotFoundInRepository_ReturnsFalseAndWriteToLogAboutNonExistentRemovableVersion() {
            //Arrange
            string productName = "productName";
            Version removableVersion = CreateNewVersion("1.1.1");
            Version notExistentVersion = CreateNewVersion("1.1.2");

            CurrentProductRepository.AddVersion(productName, removableVersion);

            //Act
            bool IsRemovableVersionRemovedFromRepository = CurrentProductRepository
                                                        .RemoveVersion(productName, notExistentVersion.ProductVersion);

            //Assert
            string expectedMessage = "Удаляемой версии продукта не существует!";
            string actualMessage = _lastMessageFromLog;

            Assert.AreEqual(expectedMessage, actualMessage);
            Assert.False(IsRemovableVersionRemovedFromRepository);
        }


        [Test]
        public void RemoveVersion_TheProductHasOnlyOneVersion_RemovesTheVersionAndTheProductAndWritesThisToLog() {
            //Arrange
            string productName = "productName";
            Version removableVersion = CreateNewVersion("1.1.1");
            CurrentProductRepository.AddVersion(productName, removableVersion);

            //Act
            CurrentProductRepository.RemoveVersion(productName, removableVersion.ProductVersion);

            //Assert
            Product potentiallyRemovedProduct = new Product(productName);
            potentiallyRemovedProduct.InitializeLatestVersion(removableVersion);

            bool isRemovedProductExistInRepository = IsThereProduct(potentiallyRemovedProduct);

            string expectedMessage = $"Продукт {productName} успешно удален";
            string actualMessage = _lastMessageFromLog;

            Assert.False(isRemovedProductExistInRepository);
            Assert.AreEqual(expectedMessage, actualMessage);
        }


        [Test]
        public void RemoveVersion_TheProductHasMoreThanOneVersion_RemovesTheVersionAndWritesThisToLog() {
            //Arrange
            string productName = "productName";
            Version firstVersion = CreateNewVersion("1.1.1");
            Version removableVersion = CreateNewVersion("1.1.2");

            CurrentProductRepository.AddVersion(productName, firstVersion);
            CurrentProductRepository.AddVersion(productName, removableVersion);

            Product product = CurrentProductRepository.GetProducts().FirstOrDefault();

            //Act
            CurrentProductRepository.RemoveVersion(productName, removableVersion.ProductVersion);

            //Assert
            bool productStillExistsInRepository = IsThereProduct(product);
            bool isRemovedVersionExistInRepository = IsThereVersion(productName, removableVersion);

            string expectedMessage = $"Версия {removableVersion.ProductVersion} продукта {productName} успешно удалена";
            string actualMessage = _lastMessageFromLog;

            Assert.True(productStillExistsInRepository);
            Assert.False(isRemovedVersionExistInRepository);
            Assert.AreEqual(expectedMessage, actualMessage);
        }


        private Version CreateNewVersion(string productVersion) {
            return new Version(productVersion,"shortDesc", "longDesc", "changes", "fileName", "fileUrl");
        }

        private bool IsThereVersion(string productName, Version desiredVersion) {
            Product currentProduct = getProductFromDatabase(productName);

            return _database.Versions
                    .Any(version => version.ProductId == currentProduct.Id && version.ProductVersion == desiredVersion.ProductVersion
                        && version.ShortDescription == desiredVersion.ShortDescription && version.LongDescription == desiredVersion.LongDescription
                        && version.Changes == desiredVersion.Changes && version.DownloadableFileName == desiredVersion.DownloadableFileName
                        && version.DownloadableFileUrl == desiredVersion.DownloadableFileUrl);

        }

        private bool IsThereProduct(Product desiredProduct) {
            return _database.Products
                    .Any(product => product.ProductName == desiredProduct.ProductName);
        }

        private Product getProductFromDatabase(string productName) {
            Product currentProduct = _database.Products
                                    .SingleOrDefault(product => product.ProductName == productName);

            return currentProduct;
        }
    }
}
