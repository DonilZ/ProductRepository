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
        public void IsProductVersionCorrect_EnteredIncorrectProductVersion_ReturnsFalse(string incorrectProductVersion) {
            //Arrange
            

            //Act
            bool result = CurrentProductRepository.IsProductVersionCorrect(incorrectProductVersion);

            //Assert
            Assert.False(result);
        }


        [TestCase("123456.1234567.12345678")]
        [TestCase("1.1.1")]
        [TestCase("0.0.0")]
        public void IsProductVersionCorrect_EnteredCorrectProductVersion_ReturnsTrue(string productVersion) {
            //Arrange

            //Act
            bool result = CurrentProductRepository.IsProductVersionCorrect(productVersion);

            //Assert
            Assert.True(result);
        }


        [Test]
        public void AddVersion_EnteredIncorrectProductVersion_WriteToLogAboutIncorrectEnteredProductVersion() {
            //Arrange
            Version version = CreateNewVersion(".1.1.");

            //Act           
            CurrentProductRepository.AddVersion(version);

            //Assert
            string expectedMessage = "Номер версии введен некорректно";
            string factMessage = _lastMessageFromLog;

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
            bool resultWasVersionAdded = IsThereVersion(newVersion);

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

            /*
             * Не совсем понял вопрос. Но попробую просто описать схему теста, который ты, как я понял, хочешь написать. 
             * У тебя перед каждым тестом репозиторий пуст. Добавь в него продукт и его версию в секции Arrange. В секции Act добавь еще одну 
             * версию этого же продукта, но меньшую по номеру. В секции Assert проверь, что в логгер записано правильное сообщение. На первый взгляд проблем
             * в этом вроде нет. Если есть, то опиши их еще раз, плс.
             */
            
            /*
             * Сделал, как вы сказали, теперь все понял.
             */

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


        /*
         * Ниже я переименовал два тестовых метода. Сравни их, плс, и обрати внимание на то, что я убрал из названия то, 
         * что проверяется удаления версии из списка версий из списка версий продукта. С точки зрения пользователя репозитория 
         * не важно, как он хранит версии и продукты. Тут важно только то, что если удаляется версия продукта и она одна, то удаляется 
         * и весь продукт, а если не одна - то продукт остается. 
         * Можно, конечно, как-то тестировать согласованность списков, но ведь эта согласованность нужно для нормальной работы репозитория? 
         * Если нужна, то тогда она будет косвенно проверена через публичное API. А если не нужна, то тогда возникнет вопрос, зачем она вообще нужна.
         * Собственно, это и есть аргумент в пользу того, чтобы тестировать только публичное API класса)
         * 
         * И еще один момент. Такое именования тестовых методов нацелено ведь на то, чтобы они читались как чек-лист в документации. И в общем, они 
         * так и читаются. В этом случае не являются ли tripple slash - комменты к ним излишними? 
         */

        /*
         * ПРАВКИ:
         * 1) Насчет названий понял (исправлено), действительно, проверяю лишнее.
         * 2) Tripple slash комментарии у тестовых методов убрал. 
         */


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


             /*
             * Это не является реализацией фабричного метода. Ты просто вынес повторяющийся код по созданию объекта в отдельный метод. Предлагаю 
             * посмотреть описание Фабрик и Фабричных методов.
             */

             /*
              * ПРАВКИ:
              * 1) Исправил название метода CreateNewVersion() на CreateNewVersion(). Просто в книге The Art of Unit Testing
              * автор почему-то назвал похожий метод фабричным я, видимо, не так понял. Обязательно перечитаю про Фабричный метод и Фабрику
              */


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