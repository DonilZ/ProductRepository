using NUnit.Framework;
using System.Collections.Generic;
using repository;
using Moq;

namespace repository.UnitTests {

    /// <summary>
    /// Тестовый класс для тестирования публичных методов класса ProductRepository
    /// </summary>
    [TestFixture]
    public class ProductRepositoryUnitTest {
        private string _lastMessageFromLog;
        public ProductRepository CurrentProductRepository { get; set; }
        public Mock<ILogger> MockLogger { get; set; }

        [SetUp]
        public void SetUp() {
            MockLogger = new Mock<ILogger>();
            MockLogger.Setup(m => m.LogRecord(It.IsAny<string>())).Callback<string>((message) => _lastMessageFromLog = message);
            CurrentProductRepository = new ProductRepository(MockLogger.Object);
        }


        [TestCase("0")]
        [TestCase("123456")]
        [TestCase("1.")]
        [TestCase(".1.")]
        [TestCase("1.1.")]
        [TestCase("1.1.1.")]
        [TestCase(".")]
        [TestCase(".1.1.1")]
        [TestCase("1.1.1.1")]
        [TestCase("1.fd.1")]
        [TestCase("a.1.q")]
        public void IsProductVersionCorrect_ForIncorrectProductVersion_ReturnsFalse(string incorrectProductVersion) {
            //Arrange
            

            //Act
            bool result = CurrentProductRepository.IsProductVersionCorrect(incorrectProductVersion);

            //Assert
            Assert.False(result);
        }


        [TestCase("123456.1234567.12345678")]
        [TestCase("1.1.1")]
        [TestCase("0.0.0")]
        public void IsProductVersionCorrect_ForCorrectProductVersion_ReturnsTrue(string productVersion) {
            //Arrange

            //Act
            bool result = CurrentProductRepository.IsProductVersionCorrect(productVersion);

            //Assert
            Assert.True(result);
        }


        [Test]
        public void AddVersion_WithIncorrectVersionNumber_FailsAndWriteThisToLog() {

            /*
             * Интересно! Когда стал переименовывать метод, то уперся в то, что же написать в третьем фрагменте. Ведь при добавлении версии с 
             * некорректным номером система не просто должна писать в лог, но и выполнять какое-то действие, либо информировать о том, что выполнить его не может.
             * В данном случае варианта, наверное, 2. Первый - выкинуть исключение ArgumentException. Второй - просто вернуть null, вместо ссылки на вновь добавленную версию.
             * По идее, второй вариант получше, но метод войдовый, поэтому такой вариант здесь не подойдет. Но в любом случае вызов метода на корректных данных и на некорректных должен
             * чем-то отличаться. Я пока написал в название метода Fail, а какой конкретно Fail ты сделаешь - вибирай) И, естественно, нужна проверка в секции Assert, что Fail был. 
             */
            //Arrange
            Version version = CreateNewVersion(".1.1.");

            //Act           
            CurrentProductRepository.AddVersion(version);

            //Assert
            string expectedMessage = "Номер версии введен некорректно";
            string factMessage = _lastMessageFromLog;                        /*->actualMessage. Fact как прилагательное не используется*/

            Assert.AreEqual(expectedMessage, factMessage);
        }


        [Test]
        public void AddVersion_SuccessfullAddNewProductAndNewVersionInRepository_AddedNewProductAndNewVersionInRepository() {
            //Arrange
            Version newVersion = CreateNewVersion("1.1.1");

            //Act
            CurrentProductRepository.AddVersion(newVersion);

            //Assert
            bool resultWasProductAdded = CurrentProductRepository.IsThereProduct(new Product(newVersion));
            bool resultWasVersionAdded = IsThereVersion(newVersion);  /*Мне кажется, префикс result здесь лишний*/

            Assert.True(resultWasProductAdded);
            Assert.True(resultWasVersionAdded);        
        }


        [TestCase("1.1.0")]
        [TestCase("1.0.1")]
        [TestCase("0.1.1")]
        [TestCase("0.0.1")]
        [TestCase("0.1.0")]
        [TestCase("1.0.0")]
        [TestCase("1.1.1")]
        [TestCase("0.0.0")]
        public void AddVersion_UnsuccessfulAdditionOfVersionBecauseItIsNotNew_WriteToLogAboutNotNewVersion(string oldVersionNumber) {
            //Arrange
            Version latestVersion = CreateNewVersion("1.1.1");
            Version oldVersion = CreateNewVersion(oldVersionNumber);

            CurrentProductRepository.AddVersion(latestVersion);

            //Act
            CurrentProductRepository.AddVersion(oldVersion);

            //Assert
            string expectedMessage = "Данная версия не может быть добавлена, так как не является новой";
            string factMessage = _lastMessageFromLog;

            Assert.AreEqual(expectedMessage, factMessage);
        }


        [TestCase("1.1.2")]
        [TestCase("1.2.1")]
        [TestCase("2.1.1")]
        [TestCase("2.2.1")]
        [TestCase("2.1.2")]
        [TestCase("1.2.2")]
        [TestCase("2.2.2")]
        [TestCase("1.1.10")]
        public void AddVersion_SuccessfullAdditionNewVersionInListOfAlreadyExistingProduct_AddedNewVersionInListOfProductVersionsAndNotDeleteOldVersion(string newVersionNumber) {
            //Arrange
            Version latestVersion = CreateNewVersion("1.1.1");
            Version newVersion = CreateNewVersion(newVersionNumber);

            CurrentProductRepository.AddVersion(latestVersion);

            //Act
            CurrentProductRepository.AddVersion(newVersion);

            //Assert
            bool isRepositoryContainLatestVersion = CurrentProductRepository.GetProductVersions(latestVersion.ProductName).Contains(latestVersion);
            bool isRepositoryContainNewVersion = CurrentProductRepository.GetProductVersions(latestVersion.ProductName).Contains(newVersion);

            Assert.True(isRepositoryContainLatestVersion);
            Assert.True(isRepositoryContainNewVersion);         
        }


        [Test]
        public void UpdateVersion_UpdatableVersionNotFoundInRepository_WriteToLogAboutNonExistentUpdatableVersion() {
            //Arrange
            Version updatableVersion = CreateNewVersion("1.1.1");

            //Act
            CurrentProductRepository.UpdateVersion(updatableVersion);

            //Assert
            string expectedMessage = "Обновляемой версии продукта не существует!";
            string factMessage = _lastMessageFromLog;

            Assert.AreEqual(expectedMessage, factMessage);         
        }

        
        [Test]
        public void UpdateVersion_UpdatableVersionExistInRepository_UpdateVersionInRepositoryAndWriteToLogThisReport() {
            //Arrange
            Version firstVersion = CreateNewVersion("1.1.1");

            CurrentProductRepository.AddVersion(firstVersion);

            FileInfo newFileInfo = new FileInfo();
            newFileInfo.FileName = "newFileName";
            newFileInfo.FileUrl = "newFileUrl";

            Version newVersion = new Version(firstVersion.ProductName, firstVersion.ProductVersion, "newShortDesc", "newLongDesc", "newChanges", newFileInfo);

            //Act
            CurrentProductRepository.UpdateVersion(newVersion);

            //Assert
            bool isFirstVersionExistInRepository = IsThereVersion(CreateNewVersion("1.1.1"));
            bool isUpdatableVersionExistInRepository = IsThereVersion(newVersion);

            string expectedMessage = $"Версия {firstVersion.ProductVersion} продукта {firstVersion.ProductName} успешно обновлена";
            string factMessage = _lastMessageFromLog;

            Assert.False(isFirstVersionExistInRepository);
            Assert.True(isUpdatableVersionExistInRepository);
            Assert.AreEqual(expectedMessage, factMessage);       
        }


        [Test]
        public void RemoveVersion_RemovableVersionNotFoundInRepository_WriteToLogAboutNonExistentRemovableVersion() {
            //Arrange
            Version removableVersion = CreateNewVersion("1.1.1");

            //Act
            CurrentProductRepository.RemoveVersion(removableVersion.ProductName, removableVersion.ProductVersion);

            //Assert
            string expectedMessage = "Удаляемой версии продукта не существует!";
            string factMessage = _lastMessageFromLog;

            Assert.AreEqual(expectedMessage, factMessage);
        }


        [Test]
        public void RemoveVersion_TheProductHasOnlyOneVersion_RemovesTheVersionAndTheProductAndWritesThisToLog() {
            //Arrange
            Version removableVersion = CreateNewVersion("1.1.1");
            CurrentProductRepository.AddVersion(removableVersion);

            //Act
            CurrentProductRepository.RemoveVersion(removableVersion.ProductName, removableVersion.ProductVersion);

            //Assert
            bool isRemovedVersionExistInRepository = IsThereVersion(removableVersion);
            bool isRemovedProductExistInRepository = CurrentProductRepository.IsThereProduct(new Product(removableVersion));

            string expectedMessage = $"Продукт {removableVersion.ProductName} успешно удален";
            string factMessage = _lastMessageFromLog;

            Assert.False(isRemovedVersionExistInRepository);
            Assert.False(isRemovedProductExistInRepository);
            Assert.AreEqual(expectedMessage, factMessage);
        }


        [Test]
        public void RemoveVersion_TheProductHasMoreThanOneVersion_RemovesTheVersionAndWritesThisToLog() {
            //Arrange
            Version firstVersion = CreateNewVersion("1.1.1");
            Version removableVersion = CreateNewVersion("1.1.2");

            CurrentProductRepository.AddVersion(firstVersion);
            CurrentProductRepository.AddVersion(removableVersion);

            Product product = CurrentProductRepository.GetProduct(firstVersion.ProductName);

            //Act
            CurrentProductRepository.RemoveVersion(removableVersion.ProductName, removableVersion.ProductVersion);

            //Assert
            bool productStillExistsInRepository = CurrentProductRepository.IsThereProduct(product);
            bool isRemovedVersionExistInRepository = IsThereVersion(removableVersion);

            string expectedMessage = $"Версия {removableVersion.ProductVersion} продукта {removableVersion.ProductName} успешно удалена";
            string factMessage = _lastMessageFromLog;

            Assert.True(productStillExistsInRepository);
            Assert.False(isRemovedVersionExistInRepository);
            Assert.AreEqual(expectedMessage, factMessage);
        }


        /// <summary>
        /// Метод для создания объектов класса Version
        /// </summary>
        private Version CreateNewVersion(string productVersion) {
            FileInfo fileInfo = new FileInfo();
            fileInfo.FileName = "fileName";
            fileInfo.FileUrl = "fileUrl";

            return new Version("productName", productVersion,"shortDesc", "longDesc", "changes", fileInfo);
        }


        /// <summary>
        /// Метод для проверки существования версии в репозитории
        /// </summary>
        private bool IsThereVersion(Version version) {
            List<Version> versions = CurrentProductRepository.GetProductVersions(version.ProductName);

            return versions == null ? false : versions.Contains(version);
        }




        
    }
}