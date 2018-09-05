using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using repository;
using Moq;

namespace repository.UnitTests {

    /// <summary>
    /// Тестовый класс для тестирования публичных методов класса MemoryProductRepository
    /// </summary>
    [Ignore("Системные тесты с внешней зависимостью в виде базы данных")]
    [TestFixture]
    public class MemoryProductRepositorySystemTest {
        private string _lastMessageFromLog;
        public ProductRepository CurrentProductRepository { get; set; }
        public Mock<ILogger> MockLogger { get; set; }

        [SetUp]
        public void SetUp() {
            MockLogger = new Mock<ILogger>();
            MockLogger.Setup(m => m.LogRecord(It.IsAny<string>())).Callback<string>((message) => _lastMessageFromLog = message);
            CurrentProductRepository = new MemoryProductRepository(MockLogger.Object);
        }

        [Test]
        public void AddVersion_WithIncorrectVersionNumber_ReturnsFalseAndWriteThisToLog() {

            /*
             * Интересно! Когда стал переименовывать метод, то уперся в то, что же написать в третьем фрагменте. Ведь при добавлении версии с 
             * некорректным номером система не просто должна писать в лог, но и выполнять какое-то действие, либо информировать о том, что выполнить его не может.
             * В данном случае варианта, наверное, 2. Первый - выкинуть исключение ArgumentException. Второй - просто вернуть null, вместо ссылки на вновь добавленную версию.
             * По идее, второй вариант получше, но метод войдовый, поэтому такой вариант здесь не подойдет. Но в любом случае вызов метода на корректных данных и на некорректных должен
             * чем-то отличаться. Я пока написал в название метода Fail, а какой конкретно Fail ты сделаешь - вибирай) И, естественно, нужна проверка в секции Assert, что Fail был. 
             */

            /*
             * ПРАВКИ:
             * 1) Теперь методы добавления, обновления и удаления возвращают булево значение в зависимости от результата.
             * 2) Добавил проверку в секции Assert на то, что добавляемая версия с некорректным номером не добавлена в репозиторий после вызова метода AddVersion().
             */
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
            string expectedMessage = "Обновляемого продукта не существует!";
            string actualMessage = _lastMessageFromLog;

            Assert.AreEqual(expectedMessage, actualMessage);
            Assert.False(IsUpdatableVersionUpdatedInRepository);         
        }

        [Test]
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
        public void RemoveVersion_RemovableVersionNotFoundInRepository_ReturnsFalseAndWriteToLogAboutNonExistentRemovableVersion() {
            //Arrange
            string productName = "productName";
            Version removableVersion = CreateNewVersion("1.1.1");

            //Act
            bool IsRemovableVersionRemovedFromRepository = CurrentProductRepository
                                                        .RemoveVersion(productName, removableVersion.ProductVersion);

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

            bool isRemovedVersionExistInRepository = IsThereVersion(productName, removableVersion);
            bool isRemovedProductExistInRepository = IsThereProduct(potentiallyRemovedProduct);

            string expectedMessage = $"Продукт {productName} успешно удален";
            string actualMessage = _lastMessageFromLog;

            Assert.False(isRemovedVersionExistInRepository);
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


        /// <summary>
        /// Метод для создания объектов класса Version
        /// </summary>
        private Version CreateNewVersion(string productVersion) {
            return new Version(productVersion,"shortDesc", "longDesc", "changes", "fileName", "fileUrl");
        }


        /// <summary>
        /// Метод для проверки существования версии в репозитории
        /// </summary>
        private bool IsThereVersion(string productName, Version version) {
            List<Version> versions = CurrentProductRepository.GetProductVersions(productName);

            return versions == null ? false : versions.Contains(version);
        }

        /// <summary>
        /// Метод для проверки существования продукта в репозитории
        /// </summary>
        private bool IsThereProduct(Product product) {
            List<Product> products = CurrentProductRepository.GetProducts();

            return products == null ? false : products.Contains(product);
        }
        
    }
}