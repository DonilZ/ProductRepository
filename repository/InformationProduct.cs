using System;
using System.Collections.Generic;
using System.Linq;

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
        public string ProductName { get; private set; }  
        public string ProductVersion { get; private set; } 
        public string ShortDescription { get; private set; }
        public string LongDescription { get; private set; }
        public string Changes { get; private set; }
        public FileInfo DownloadableFile { get; private set; } 

        public Version (string productName, string productVersion, string shortDescription,
                        string longDescription, string changes, FileInfo downloadableFile) {
            ProductName = productName;
            ProductVersion = productVersion;
            ShortDescription = shortDescription;
            LongDescription = longDescription;
            Changes = changes;
            DownloadableFile = downloadableFile;
        }

        /// <summary>
        /// Метод для обновления информации о версии.
        /// </summary>
        public void Update(Version updateVersion) {
            ProductName = updateVersion.ProductName;
            ProductVersion = updateVersion.ProductVersion;
            ShortDescription = updateVersion.ShortDescription;
            LongDescription = updateVersion.LongDescription;
            Changes = updateVersion.Changes;
            DownloadableFile = updateVersion.DownloadableFile;
        }

        /// <summary>
        /// Перегрузка метода Equals для объектов класса Version
        /// </summary>
        public override bool Equals(object obj) {

            /*
             * Здесь у тебя проверка на null и приведение типа как-то не очень понятно сделано. Можно так: 
             *      var version = obj as Version;
             *      if (version == null) return false;
             *      
             *  И еще, в C# 7, если не ошибаюсь, появился синтаксис pattern matching, который вообще в одну строку позволяет это записать.
             *  Но я лично этот синтаксис не могу запомнить, а кроме того, некоторым он потом непонятен при чтении кода. Поэтому можешь посмотреть,
             *  а использовать - по желанию.
             *  
             *  
             *  И еще, обращаю внимание на то, что ты перегруженное равенство в Linq запросах к СУБД использовать уже не сможешь, т.к. они
             *  выполняются в среде SQL-сервера, а не в среде исполнения .net, как сейчас в случае со списками. Но это просто к сведению на будущее.
             */

            /*
             * ПРАВКИ:
             * 1) Исправил свою реализацию проверки на null.
             * 
             * ВОПРОСЫ:
             * 1) А что необходимо предпринять в таком случае? Просто без этой перегрузки некорректно сравниваются объекты.
             */

            var version = obj as Version;
            if (version == null) return false;

            return this.ProductName == version.ProductName && this.ProductVersion == version.ProductVersion
                    && this.ShortDescription == version.ShortDescription && this.LongDescription == version.LongDescription
                    && this.Changes == version.Changes && this.DownloadableFile.FileName == version.DownloadableFile.FileName
                    && this.DownloadableFile.FileUrl == version.DownloadableFile.FileUrl;
        }
    }

    ///<summary>
    ///Класс Продукт. Объекты данного класса хранят соответствующую информацию о Продукте
    ///(прописанной в ТЗ), с возможностью получения необходимых данных Продукта, хранения всех версий данного Продукта,
    ///добавления, обновления, удаления и, при необходимости, изменения последней версии Продукта   
    ///</summary>
    public class Product {
        private List<Version> _allVersions;
        private Version _latestVersion;
        private string _nameProduct;

        public Product(Version latestVersion) {
            _allVersions = new List<Version>();
            _latestVersion = latestVersion;
            _allVersions.Add(latestVersion);
            _nameProduct = _latestVersion.ProductName;
        }

        /// <summary>
        /// Метод для получения названия продукта.
        /// </summary>
        public string GetProductName() {
            return _nameProduct;
        }

        /// <summary>
        /// Метод для получения номера последней версии продукта.
        /// </summary>
        public string GetNumberLatestVersion() {
            return _latestVersion.ProductVersion;
        }

        /// <summary>
        /// Метод для получения краткого описания последней версии продукта.
        /// </summary>
        public string GetShortDescription() {
            return _latestVersion.ShortDescription;
        }

        /// <summary>
        /// Метод для получения общего списка всех версий продукта.
        /// </summary>
        public List<Version> GetAllVersions() {
            return _allVersions;
        }

        /// <summary>
        /// Метод для обновления последней версии продукта.
        /// </summary>
        private void UpdateLatestVersion() {
            _latestVersion = _allVersions.LastOrDefault();
        }

        /// <summary>
        /// Метод для добавления версии в общий список версий продукта и, как следствие, обновления последней версии продукта.
        /// </summary>
        public void AddVersion(Version newVersion) {
            _allVersions.Add(newVersion);
            UpdateLatestVersion();
        }

        /// <summary>
        /// Метод для удаления версии из общего списка версий продукта и, как следствие, обновления последней версии продукта (при необходимости).
        /// </summary>
        public void RemoveVersion(Version removedVersion) {
            _allVersions.Remove(removedVersion);
            UpdateLatestVersion();
        }


        /// <summary>
        /// Метод для проверки добавляемой версии на то, является ли она старее последней.
        /// </summary>
        private bool NewVersionIsGreater(Version newVersion) {
           
            string[] latest = _latestVersion.ProductVersion.Split('.');  
            string[] added = newVersion.ProductVersion.Split('.');

            return _allVersions.Count < 0 || IsNewVersion(0, latest, added) ? true : false;
        }

        private bool IsNewVersion(int index, /*Version newVersion,*/ string[] latest, string[] added) {


            if (index == added.Count()) return false;

            if (Convert.ToInt32(added[index]) > Convert.ToInt32(latest[index])) {
                return true;
            } 

            if (Convert.ToInt32(added[index]) == Convert.ToInt32(latest[index])) {
                return IsNewVersion(++index, latest, added);
            }

            return false;
        }

        /// <summary>
        /// Метод для опеределения того, является ли добавляемая версия новее последней существующей версии продукта
        /// </summary>
        public bool NewVersionIsGreaterThenLatest(Version newVersion)
        {
            var latestVersionEnumerator = GetVersionComponentEnumerator(_latestVersion.ProductVersion);
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
            if (obj == null) return false;

            Product product = obj as Product;

            if (product as Product == null) return false;

            return this._nameProduct == product._nameProduct && this._latestVersion == product._latestVersion
                    && this._allVersions.SequenceEqual(product._allVersions);
        }

    }
    
}

