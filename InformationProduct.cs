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
    class FileInfo<T, K>{
		public T FileName { get; set; }
		public K FileUrl { get; set; }
	}

    /// <summary>
    /// Класс Версия продукта. Объекты данного класса хранят соответствующую информацию о версии продукта(прописанной в ТЗ) с возможностью обновления каких-либо данных о версии.
    /// </summary>
    class Version {
        public string ProductName { get; private set; }  
        public string ProductVersion { get; private set; } 
        public string ShortDescription { get; private set; }
        public string LongDescription { get; private set; }
        public string Changes { get; private set; }
        public FileInfo<string,string> DownloadableFile { get; private set; } 

        public Version (string productName, string productVersion, string shortDescription,
                        string longDescription, string changes, FileInfo<string,string> downloadableFile) {
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
    }

    ///<summary>
    ///Класс Продукт. Объекты данного класса хранят соответствующую информацию о Продукте
    ///(прописанной в ТЗ), с возможностью получения необходимых данных Продукта, хранения всех версий данного Продукта,
    ///добавления, обновления, удаления и, при необходимости, изменения последней версии Продукта   
    ///</summary>
    class Product {
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
        public void AddVersion(Version addedVersion) {
            _allVersions.Add(addedVersion);
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

        /// <summary>
        /// Метод для проверк добавляемой версии на то, является ли она старее последней.
        /// </summary>
        public bool CheckAddedVersion(Version addedVersion) {
            
            /*
             * ПРАВКИ:
             * 1) Добавил рекурсивную функцию IsNewVersion(), проверяющая актуальность добавляемой версии. Каждая цифра в номере добавляемой версии
             * проверяется с соответствующей цифрой последней версией текущего продукта (соответствие поддерживается с помощью index).
             */


            /*
             * Да, с этой функцией лучше. Ниже по тексту некоторые комментарии к ней есть. Еще, я для примера написал другую реализацию рекурсивного подхода, с использованием
             * enumerator-ов. Посмотри в качестве альтернативы. 
             */


            return _allVersions.Count < 0 || IsNewVersion(0, addedVersion) ? true : false;
        }

        private bool IsNewVersion(int index, Version addedVersion) {

            string[] latest = _latestVersion.ProductVersion.Split('.');  /*Т.к. функция выхывается несколько раз, то получается, что разбивка строки будет происходить несколько раз*/
            string[] added = addedVersion.ProductVersion.Split('.');


            if (index == added.Count()) return false;

            if (Convert.ToInt32(added[index]) > Convert.ToInt32(latest[index])) {
                return true;
            } 
            /*
             * Если выполнилось условие и после него идет return, то выполнение дальше не пойдет, а значит, 
             * и else лишний. Достаточно просто if. А в последнем блоке просто return false;
             */
            else if (Convert.ToInt32(added[index]) == Convert.ToInt32(latest[index])) {
                return IsNewVersion(++index, addedVersion);
            }
            else {
                return false;
            }
        }

        private bool NewVersionIsGreater(string latestVersion, string newVersion)
        {
            var latestVersionEnumerator = GetVersionComponentEnumerator(latestVersion);
            var addedVersionEnumerator = GetVersionComponentEnumerator(newVersion);

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

    }
    
}