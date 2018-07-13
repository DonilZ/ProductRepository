using System;
using System.Collections.Generic;
using System.Linq;

namespace repository {

    /// <summary>
    /// Класс, содержащий информацию о файле (имя файла, URL файла)
    /// </summary>
    /// <typeparam name="T">Тип имени файла</typeparam>
    /// <typeparam name="K">Тип URL файла</typeparam>
    /*
     * А зачем здесь параметры T и K теперь? Они же всегда string?
     */

    /*
     * ПРАВКИ:
     * 1) Убрал параметры-типы из класса FileInfo
    */
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
            if (obj == null) return false;

            Version version = obj as Version;

            if (version as Version == null) return false;

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


        /*
         * В комментарии ты пишешь, какая проверка версии происходит, а название этого не отражает. Нужно переименовать метод в NewVersionIsGreaterThenLatest().
         * Тогда и место его вызова будет читаться гораздо легче
         */

         /*
          * И еще, предлагаю везде addedVersion заменить на newVersion. По сути версия-то новая (new), а не добавленная (added). Она ведь только добавляется на этот момент.
          */

         /*
         * ПРАВКИ:
         * 1) Изменил название метода с CheckAddedVersion() на NewVersionIsGreaterThenLatest()
         * 2) Изменил имя переменной-параметра addedVersion на newVersion
         */

        /// <summary>
        /// Метод для проверки добавляемой версии на то, является ли она старее последней.
        /// </summary>
        private bool NewVersionIsGreater(Version newVersion) {
            
            /*
             * ПРАВКИ:
             * 1) Добавил рекурсивную функцию IsNewVersion(), проверяющая актуальность добавляемой версии. Каждая цифра в номере добавляемой версии
             * проверяется с соответствующей цифрой последней версией текущего продукта (соответствие поддерживается с помощью index).
             */


            /*
             * Да, с этой функцией лучше. Ниже по тексту некоторые комментарии к ней есть. Еще, я для примера написал другую реализацию рекурсивного подхода, с использованием
             * enumerator-ов. Посмотри в качестве альтернативы. 
             */

            /*
             * ПРАВКИ:
             * 1) С вашего позволения оставлю вашу реализацию этого рекурсивного метода. Полностью в ней разобрался, намного лучше и понятнее и, главное, полезно.
             * В моей реализации мне не нравится момент с index в качестве параметра и с его инкрементацией (выглядит как реализация из олимпиадного программирования)
             */

            string[] latest = _latestVersion.ProductVersion.Split('.');  /*Т.к. функция выхывается несколько раз, то получается, что разбивка строки будет происходить несколько раз*/
            string[] added = newVersion.ProductVersion.Split('.');

            return _allVersions.Count < 0 || IsNewVersion(0, latest, added) ? true : false;
        }

        private bool IsNewVersion(int index, /*Version newVersion,*/ string[] latest, string[] added) {

            // string[] latest = _latestVersion.ProductVersion.Split('.');  /*Т.к. функция выхывается несколько раз, то получается, что разбивка строки будет происходить несколько раз*/
           // string[] added = newVersion.ProductVersion.Split('.'); 

            if (index == added.Count()) return false;

            if (Convert.ToInt32(added[index]) > Convert.ToInt32(latest[index])) {
                return true;
            } 

            /*
             * Если выполнилось условие и после него идет return, то выполнение дальше не пойдет, а значит, 
             * и else лишний. Достаточно просто if. А в последнем блоке просто return false;
             */

             /*
              * ПРАВКИ:
              * 1) Убрал лишние else.
              * 2) Перенес разбиение строк с номерами добавляемой и последней версий из рекурсивной функции IsNewVersion() в метод NewVersionIsGreaterThenLatest()
              * теперь они разбиваются только один раз и массивы с разбитыми по частям версиями передаются в качестве параметров в функцию IsNewVersion().
              * И, как следствие, параметр (Version newVersion) теперь не нужен в функции IsNewVersion(). 
              */

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

/*
 ПРАВКИ:
 1) Добавил перегрузку методов Equals в класса Version и Product
*/