using NUnit.Framework;
using repository;

namespace repository.UnitTests {

    /// <summary>
    /// Тестовый класс для тестирования публичных методов класса ProductRepository
    /// </summary>
    [TestFixture]
    public class ProductRepositoryUnitTest {
        public ProductRepository CurrentProductRepository { get; set; }
        public FakeLogger MockLogger { get; set; }

        [SetUp]
        public void SetUp() {
            MockLogger = new FakeLogger();
            CurrentProductRepository = ProductRepository.GetInstance(MockLogger);
            
            CurrentProductRepository.SetLogger(MockLogger);
            CurrentProductRepository.ClearProductsAndVersions();
        }

        /// <summary>
        /// Тестирование метода IsProductVersionCorrect() класса ProductRepository на сценарий ввода некорректного номера версии 
        /// </summary>
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
        public void IsProductVersionCorrect_EnteredIncorrectProductVersion_ReturnsFalse(string productVersion) {
            //Arrange
            

            //Act
            bool result = CurrentProductRepository.IsProductVersionCorrect(productVersion);

            //Assert
            Assert.False(result);
        }

        /// <summary>
        /// Тестирование метода IsProductVersionCorrect() класса ProductRepository на сценарий ввода корректного номера версии 
        /// </summary>
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

        /// <summary>
        /// Тестирование метода AddVersion() класса ProductRepository на сценарий попытки добавления версии с некорректным номером
        /// </summary>
        [Test]
        public void AddVersion_EnteredIncorrectProductVersion_WriteToLogAboutIncorrectEnteredProductVersion() {
            //Arrange
            Version version = FactoryMethodCreateNewVersion(".1.1.");

            //Act
            CurrentProductRepository.AddVersion(version);

            //Assert
            Assert.AreEqual("Номер версии введен некорректно", MockLogger.GetLastLogMessage());
        }

        [Ignore("Тест не готов")]
        [Test]
        public void AddVersion_NewProductVersionIsNotNew_WriteToLogAboutNotNewProductVersion() {
            //Arrange
            Product stubProduct = new Product(FactoryMethodCreateNewVersion("1.1.1"));

            //Act
           // CurrentProductRepository.AddVersion(version);

            //Assert
            //Assert.AreEqual("Номер версии введен некорректно", stubLogger.GetLastLogMessage());

            /*
                В данном тесте я проверяю сценарий единицы работы AddVersion, при котором добавляемая версия продукта
                не является новой. То есть, мне необходимо искусственно вызывать вот эту проверку в методе AddVersion:
                "if (productFromDatabase != null && !productFromDatabase.NewVersionIsGreaterThenLatest(newVersion))"
                Но продукт же мы получаем прямо в этом методе при помощи GetProduct() и непонятно, как нам проверить
                этот сценарий.
            */

            /*
             * Не совсем понял вопрос. Но попробую просто описать схему теста, который ты, как я понял, хочешь написать. 
             * У тебя перед каждым тестом репозиторий пуст. Добавь в него продукт и его версию в секции Arrange. В секции Act добавь еще одну 
             * версию этого же продукта, но меньшую по номеру. В секции Assert проверь, что в логгер записано правильное сообщение. На первый взгляд проблем
             * в этом вроде нет. Если есть, то опиши их еще раз, плс.
             */

        }

        /// <summary>
        /// Тестирование метода AddVersion() класса ProductRepository на сценарий успешного добавления новой версии и нового продукта в репозиторий
        /// </summary>
        [Test]
        public void AddVersion_SuccessfullAddNewProductAndNewVersionInRepository_AddedNewProductAndNewVersionInRepository() {
            //Arrange
            Version version = FactoryMethodCreateNewVersion("1.1.1");

            //Act
            CurrentProductRepository.AddVersion(version);

            //Assert
            Assert.True(CurrentProductRepository.GetCountProducts() == 1);
            Assert.True(CurrentProductRepository.GetCountVersions() == 1);

            Assert.True(CurrentProductRepository.IsThereProduct(new Product(version)));
            Assert.True(CurrentProductRepository.IsThereVersion(version));        
        }

        /// <summary>
        /// Тестирование метода AddVersion() класса ProductRepository на сценарий успешного добавления новой версии уже существующего продукта в репозиторий
        /// </summary>
        [Test]
        public void AddVersion_SuccessfullAddNewVersionButNotNewProduct_AddedNewVersionInRepositoryAndNewVersionInListOfProductVersions() {
            //Arrange
            Version firstVersion = FactoryMethodCreateNewVersion("1.1.1");
            Version newVersion = FactoryMethodCreateNewVersion("1.1.2");

            //Act
            CurrentProductRepository.AddVersion(firstVersion);
            CurrentProductRepository.AddVersion(newVersion);

            //Assert
            Assert.True(CurrentProductRepository.GetCountProducts() == 1);
            Assert.True(CurrentProductRepository.GetCountVersions() == 2);

            Assert.True(CurrentProductRepository.GetProductVersions(firstVersion.ProductName).Count == 2);

            Assert.True(CurrentProductRepository.GetProductVersions(firstVersion.ProductName).Contains(firstVersion));
            Assert.True(CurrentProductRepository.GetProductVersions(firstVersion.ProductName).Contains(newVersion));         
        }

        /// <summary>
        /// Тестирование метода UpdateVersion() класса ProductRepository на сценарий попытки обновления несуществующей в репозитории версии
        /// </summary>
        [Test]
        public void UpdateVersion_UpdatableVersionNotFoundInRepository_WriteToLogAboutNonExistentUpdatableVersion() {
            //Arrange
            Version updatableVersion = FactoryMethodCreateNewVersion("1.1.1");

            //Act
            CurrentProductRepository.UpdateVersion(updatableVersion);

            //Assert
            Assert.AreEqual("Обновляемой версии продукта не существует!", MockLogger.GetLastLogMessage());         
        }
        
        /// <summary>
        /// Тестирование метода UpdateVersion() класса ProductRepository на сценарий успешного обновления продукта в репозитории
        /// </summary>
        [Test]
        public void UpdateVersion_UpdatableVersionExistInRepository_UpdateVersionInRepositoryAndWriteToLogThisReport() {
            //Arrange
            Version firstVersion = FactoryMethodCreateNewVersion("1.1.1");

            FileInfo newFileInfo = new FileInfo();
            newFileInfo.FileName = "newFileName";
            newFileInfo.FileUrl = "newFileUrl";
            Version newVersion = new Version(firstVersion.ProductName, firstVersion.ProductVersion, "newShortDesc", "newLongDesc", "newChanges", newFileInfo);

            //Act
            CurrentProductRepository.AddVersion(firstVersion);
            CurrentProductRepository.UpdateVersion(newVersion);

            //Assert
            Assert.False(CurrentProductRepository.IsThereVersion(FactoryMethodCreateNewVersion("1.1.1")));
            Assert.True(CurrentProductRepository.IsThereVersion(newVersion));
            Assert.AreEqual($"Версия {firstVersion.ProductVersion} продукта {firstVersion.ProductName} успешно обновлена", MockLogger.GetLastLogMessage());       
        }

        /// <summary>
        /// Тестирование метода RemoveVersion() класса ProductRepository на сценарий попытки удаления несуществующей в репозитории версии
        /// </summary>
        [Test]
        public void RemoveVersion_RemovableVersionNotFoundInRepository_WriteToLogAboutNonExistentRemovableVersion() {
            //Arrange
            Version removableVersion = FactoryMethodCreateNewVersion("1.1.1");

            //Act
            CurrentProductRepository.RemoveVersion(removableVersion.ProductName, removableVersion.ProductVersion);

            //Assert
            Assert.AreEqual("Удаляемой версии продукта не существует!", MockLogger.GetLastLogMessage());
        }

        /// <summary>
        /// Тестирование метода RemoveVersion() класса ProductRepository на сценарий успешного удаления версии и продукта из репозитория
        /// (версия являлась единственной версией продукта)
        /// </summary>
        [Test]
        public void RemoveVersion_RemovableVersionExistInRepositoryAndThisVersionIsTheOnlyInTheListOfProductVersions_RemoveVersionFromRepositoryAndRemoveProductFromRepositoryAndWriteToLogThisReport() {
            //Arrange
            Version removableVersion = FactoryMethodCreateNewVersion("1.1.1");

            //Act
            CurrentProductRepository.AddVersion(removableVersion);
            CurrentProductRepository.RemoveVersion(removableVersion.ProductName, removableVersion.ProductVersion);

            //Assert
            Assert.False(CurrentProductRepository.IsThereVersion(removableVersion));
            Assert.False(CurrentProductRepository.IsThereProduct(new Product(removableVersion)));
            Assert.AreEqual($"Продукт {removableVersion.ProductName} успешно удален", MockLogger.GetLastLogMessage());
        }


        /// <summary>
        /// Тестирование метода RemoveVersion() класса ProductRepository на сценарий успешного удаления версии из репозитория и из списка 
        /// версий соответствующего продукта (версия не являлась единственной версией продукта)
        /// </summary>
        [Test]
        public void RemoveVersion_RemovableVersionExistInRepositoryAndThisVersionIsTheNotOnlyInTheListOfProductVersions_RemoveVersionFromRepositoryAndRemoveThisVersionFromListOfProductVersionsAndWriteToLogThisReport() {
            //Arrange
            Version firstVersion = FactoryMethodCreateNewVersion("1.1.1");
            Version removableVersion = FactoryMethodCreateNewVersion("1.1.2");

            //Act
            CurrentProductRepository.AddVersion(firstVersion);
            CurrentProductRepository.AddVersion(removableVersion);
            Product product = CurrentProductRepository.GetProduct(firstVersion.ProductName);

            CurrentProductRepository.RemoveVersion(removableVersion.ProductName, removableVersion.ProductVersion);

            //Assert
            Assert.False(CurrentProductRepository.IsThereVersion(removableVersion));
            Assert.True(CurrentProductRepository.IsThereProduct(product));
            Assert.False(product.GetAllVersions().Contains(removableVersion));
            Assert.AreEqual($"Версия {removableVersion.ProductVersion} продукта {removableVersion.ProductName} успешно удалена", MockLogger.GetLastLogMessage());
        }

        /// <summary>
        /// Фабричный метод для создания объектов класса Version
        /// </summary>
        private Version FactoryMethodCreateNewVersion(string productVersion) {
            FileInfo fileInfo = new FileInfo();
            fileInfo.FileName = "fileName";
            fileInfo.FileUrl = "fileUrl";
            return new Version("productName", productVersion,"shortDesc", "longDesc", "changes", fileInfo);
        }




        
    }
}