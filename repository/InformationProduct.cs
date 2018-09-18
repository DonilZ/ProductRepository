using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace repository {

    /// <summary>
    /// Класс, содержащий информацию о файле (имя файла, URL файла)
    /// </summary>
    public class FileInfo{
		public string FileName { get; set; }
		public string FileUrl { get; set; }
	}

    /// <summary>
    /// Класс Версия продукта. Объекты данного класса хранят соответствующую информацию о версии продукта(прописанной в ТЗ) с возможностью обновления каких-либо данных о версии.
    /// </summary>
    public class Version {
        public int Id { get; private set; } 
        public string ProductVersion { get; private set; } 
        public string ShortDescription { get; private set; }
        public string LongDescription { get; private set; }
        public string Changes { get; private set; }
        public string DownloadableFileName { get; private set; } 
        public string DownloadableFileUrl { get; private set; }
        public Product ContainProduct  { get; private set; }

        public Version (string productVersion, string shortDescription,
                        string longDescription, string changes, string downloadableFileName, string downloadableFileUrl) {
            ProductVersion = productVersion;
            ShortDescription = shortDescription;
            LongDescription = longDescription;
            Changes = changes;
            DownloadableFileName = downloadableFileName;
            DownloadableFileUrl = downloadableFileUrl;
        }

        public void SetContainProduct (Product containProduct) {
            ContainProduct = containProduct;
        }

        /// <summary>
        /// Метод для обновления информации о версии.
        /// </summary>
        public void Update(Version updateVersion) {
            ProductVersion = updateVersion.ProductVersion;
            ShortDescription = updateVersion.ShortDescription;
            LongDescription = updateVersion.LongDescription;
            Changes = updateVersion.Changes;
            DownloadableFileName = updateVersion.DownloadableFileName;
            DownloadableFileUrl = updateVersion.DownloadableFileUrl;
        }

        /// <summary>
        /// Перегрузка метода Equals для объектов класса Version
        /// </summary>
        public override bool Equals(object obj) {
            var version = obj as Version;
            if (version == null) return false;

            return  this.ProductVersion == version.ProductVersion
                    && this.ShortDescription == version.ShortDescription && this.LongDescription == version.LongDescription
                    && this.Changes == version.Changes && this.DownloadableFileName == version.DownloadableFileName
                    && this.DownloadableFileUrl == version.DownloadableFileUrl;
        }
    }

    ///<summary>
    ///Класс Продукт. Объекты данного класса хранят соответствующую информацию о Продукте
    ///(прописанной в ТЗ), с возможностью получения необходимых данных Продукта, хранения всех версий данного Продукта,
    ///добавления, обновления, удаления и, при необходимости, изменения последней версии Продукта   
    ///</summary>
    public class Product {
        [Key]
        public int Id { get; private set; }
        public string ProductName { get; private set; }
        public List<Version> AllVersions { get; private set; }

        public Product (string productName) {
            ProductName = productName;
        }
        public void InitializeLatestVersion(Version latestVersion) {
            this.AllVersions = new List<Version>();
            this.AllVersions.Add(latestVersion);
        }

        /// <summary>
        /// Метод для получения номера последней версии продукта.
        /// </summary>
        public string GetLatestVersionNumber() {
            return AllVersions.Last().ProductVersion;
        }

        /// <summary>
        /// Метод для получения краткого описания последней версии продукта.
        /// </summary>
        public string GetShortDescription() {
            return AllVersions.Last().ShortDescription;
        }

        /// <summary>
        /// Метод для получения общего списка всех версий продукта.
        /// </summary>
        public List<Version> GetAllVersions() {
            return AllVersions;
        }

        /// <summary>
        /// Метод для добавления версии в общий список версий продукта и, как следствие, обновления последней версии продукта.
        /// </summary>
        public void AddVersion(Version newVersion) {
            AllVersions.Add(newVersion);
        }

        /// <summary>
        /// Метод для удаления версии из общего списка версий продукта и, как следствие, обновления последней версии продукта (при необходимости).
        /// </summary>
        public void RemoveVersion(Version removedVersion) {
            AllVersions.Remove(removedVersion);
        }

        /// <summary>
        /// Метод для опеределения того, является ли добавляемая версия новее последней существующей версии продукта
        /// </summary>
        public bool NewVersionIsGreaterThenLatest(Version newVersion)
        {
            var latestVersionEnumerator = GetVersionComponentEnumerator(AllVersions.Last().ProductVersion);
            var addedVersionEnumerator = GetVersionComponentEnumerator(newVersion.ProductVersion);

            return GreaterThen(latestVersionEnumerator, addedVersionEnumerator);
        }

        private bool GreaterThen(IEnumerator<int> latestVersionEnumerator, IEnumerator<int> addedVersionEnumerator)
        {
            if (!addedVersionEnumerator.MoveNext()) return false;

            int latestVersionCurrentComponent = latestVersionEnumerator.MoveNext() ? latestVersionEnumerator.Current : 0;
            int addedVersionCurrrentComponent = addedVersionEnumerator.Current;

            if (addedVersionCurrrentComponent > latestVersionCurrentComponent) return true;
            if (addedVersionCurrrentComponent < latestVersionCurrentComponent) return false;

            return GreaterThen(latestVersionEnumerator, addedVersionEnumerator);
        }

        private IEnumerator<Int32> GetVersionComponentEnumerator(string version)
        {
            return version.Split('.').Select(stringFragment => Convert.ToInt32(stringFragment)).GetEnumerator();
        }

        /// <summary>
        /// Перегрузка метода Equals для объектов класса Product
        /// </summary>
        public override bool Equals(object obj) {

            var product = obj as Product;
            if (product == null) return false;

            return this.ProductName == product.ProductName && this.AllVersions.Last() == product.GetAllVersions().Last()
                    && this.AllVersions.SequenceEqual(product.AllVersions);
        }

    }
    
}

