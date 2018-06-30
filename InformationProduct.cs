using System;
using System.Collections.Generic;
using System.Linq;

namespace repository {

    /*
     * Комментарии к классам и публичным методам нужно писать с использованием triple slash comment. Это такая спец. форма комментариев перед некоторыми элементами 
     * кода (классами, методами, свойствами). От обычных (double slash или slash + asterisk) они отличаются тем, что поддерживаются системами генерации документации.
     * Т.е. есть такие утилиты, которые парсят код и собирают перечень классов, их свойств и методов, а также комментариев к ним, и формируют html-сайт с описанием 
     * этих классов. Почти везде API документируется таким образом. Наверняка ты с такими сайтами встречался.
     * Кроме того, triple slash comments поддерживаются IDE и появляются в качестве подсказок при работе с классами и методами.
     * 
     * Пример triple slash comments я привел ниже для класса Pair. Просьба остальные комментарии к классами и методам переделать в такой вид.
          */


    /* Как я понимаю, объекты класса Pair<,> используются у тебя для хранения пары "имя файла - URL". Почему бы тогда не ввести класс
     * FileInfo с полями FileName и FileUrl? Могу допустить, что ты "заложил" это на какие-то возможные будущие варианты использования, 
     * но, во-первых, такие классы наверняка есть в базовых библиотеках .net, а во-вторых, читаемость код уменьшается: там где ты мог писать
     * .FileName, ты вынужден писать .First, что уже не отражает смысл того, что хранится в этом свойстве. 
     */


<<<<<<< HEAD
     /*
      * ПРАВКИ:
      * 1) Переделал комментарии на triple slash comment.
      * 2) Переименовал класс Pair (теперь это класс FileInfo) и его члены (First на FileName; Second на FileInfo);
     */


    /// <summary>
    /// Класс, содержащий информацию о файле (имя файла, URL файла)
    /// </summary>
    /// <typeparam name="T">Тип имени файла</typeparam>
    /// <typeparam name="K">Тип URL файла</typeparam>
    class FileInfo<T, K>{
		public T FileName { get; set; }
		public K FileUrl { get; set; }
=======
    /// <summary>
    /// Собственная реализация класса KeyValuePair
    /// </summary>
    /// <typeparam name="T">Тип первого элемента пары</typeparam>
    /// <typeparam name="K">Тип второго элемента пары</typeparam>
    class Pair<T, K>{
		public T First { get; set; }
		public K Second { get; set; }
>>>>>>> 638cfd6cf65319457d5878763338303644681dec
	}

    /// <summary>
    /// Класс Версия продукта. Объекты данного класса хранят соответствующую информацию о версии продукта(прописанной в ТЗ) с возможностью обновления каких-либо данных о версии.
    /// </summary>
 
    class Version {
<<<<<<< HEAD
        public string ProductName { get; private set; }  /*Правильнее будет ProductName.*/
        public string ProductVersion { get; private set; } /*Правильнее будет ProductVersion.*/
        public string ShortDescription { get; private set; }
        public string LongDescription { get; private set; }
        public string Changes { get; private set; }
        public FileInfo<string,string> DownloadableFile { get; private set; } /*Что понимается под DisributedFile? По-русски понимается как распространенный файл. Может DownloadableFile ? */

        /*
         * ПРАВКИ:
         * 1) Поменял названия свойств NameProduct, NumberVersion и DistributedFile на ProductName, ProductVersion и DownloadableFile соответственно.

        */
=======
        public string NameProduct { get; private set; }  /*Правильнее будет ProductName*/
        public string NumberVersion { get; private set; } /*Правильнее будет ProductVersion*/
        public string ShortDescription { get; private set; }
        public string LongDescription { get; private set; }
        public string Changes { get; private set; }
        public Pair<string,string> DistributedFile { get; private set; } /*Что понимается под DisributedFile? По-русски понимается как распространенный файл. Может DownloadableFile ? */

        private Version() { }
>>>>>>> 638cfd6cf65319457d5878763338303644681dec

        /*
         * Имена параметров методов пишутся в camelCase (https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-parameters). 
         * А подчеркивание обычно используется только для именования приватных членов класса. Т.е. если бы ты вводил не свойство ShortDescription, а
         * приватный член для хранения этого значения, то вот его можно было бы объявить как private string _shortDescription. А так - подчеркивания 
         * лучше не использовать. 
         */

<<<<<<< HEAD
        /*
         * ПРАВКИ:
         * 1) Переименовал параметры конструктора клаcса Version в соответствии с camelCase 
         */

        public Version (string productName, string productVersion, string shortDescription,
                        string longDescription, string changes, FileInfo<string,string> downloadableFile) {
            ProductName = productName;
            ProductVersion = productVersion;
            ShortDescription = shortDescription;
            LongDescription = longDescription;
            Changes = changes;
            DownloadableFile = downloadableFile;
=======
        public Version (string _NameProduct, string _NumberVersion, string _ShortDescription,
                        string _LongDescription, string _Changes, Pair<string,string> _DistributedFile) {
            NameProduct = _NameProduct;
            NumberVersion = _NumberVersion;
            ShortDescription = _ShortDescription;
            LongDescription = _LongDescription;
            Changes = _Changes;
            DistributedFile = _DistributedFile;
>>>>>>> 638cfd6cf65319457d5878763338303644681dec
        }

        /// <summary>
        /// Метод для обновления информации о версии.
        /// </summary>
        public void Update(Version UpdateVersion) {
            ProductName = UpdateVersion.ProductName;
            ProductVersion = UpdateVersion.ProductVersion;
            ShortDescription = UpdateVersion.ShortDescription;
            LongDescription = UpdateVersion.LongDescription;
            Changes = UpdateVersion.Changes;
            DownloadableFile = UpdateVersion.DownloadableFile;
        }
    }
    
    ///<summary>
    ///Класс Продукт. Объекты данного класса хранят соответствующую информацию о Продукте
    ///(прописанной в ТЗ), с возможностью получения необходимых данных Продукта, хранения всех версий данного Продукта,
    ///добавления, обновления, удаления и, при необходимости, изменения последней версии Продукта   
    ///</summary>

    class Product {
<<<<<<< HEAD

        /*
         * К сожалению, не нашел этого в том руководству по именованию, на которое давал ссылку, но приватные члены класса обычно именуются 
         * в camelCase или _camelCase. А вот публичные и protected - в PascalCase.
         */

        /*
         * ПРАВКИ:
         * 1) Переименовал приватные члены в соответствии с _camelCase
         */

        private List<Version> _allVersions;
        private Version _latestVersion;
        private string _nameProduct;

        /*
=======

        /*
         * К сожалению, не нашел этого в том руководству по именованию, на которое давал ссылку, но приватные члены класса обычно именуются 
         * в camelCase или _camelCase. А вот публичные и protected - в PascalCase.
         */

        private List<Version> AllVersions;
        private Version LatestVersion;

        /*
>>>>>>> 638cfd6cf65319457d5878763338303644681dec
         * Пустые конструкторы без параметров в C# не нужны - они создаются компилятором автоматически, если не объявлен иной конструктор.
         * Если же объявлен, то пустой создается только тогда, когда он нужен. Но в твоем случае он все-равно не используется.
         */

<<<<<<< HEAD
        /*
         * ПРАВКИ:
         * 1) Убрал конструкторы без параметров. 
         */
=======
        private Product() { }
>>>>>>> 638cfd6cf65319457d5878763338303644681dec
        
        public Product (Version _LatestVersion) {
            _allVersions = new List<Version>();
            this._latestVersion = _LatestVersion;
            _allVersions.Add(_LatestVersion);
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
<<<<<<< HEAD
            _latestVersion = _allVersions.LastOrDefault();

            /*
             * Как вариант для таких условий можно использовать Linq to Objects. Тогда код выше будет выглядеть так: 
             * if (_allVersions.Any()) {
             *   _latestVersion = _allVersions.Last() 
=======
            if (AllVersions.Count > 0) {
                LatestVersion = AllVersions[AllVersions.Count - 1];
            }

            /*
             * Как вариант для таких условий можно использовать Linq to Objects. Тогда код выше будет выглядеть так: 
             * if (AllVersions.Any()) {
             *   LatestVersion = AllVersions.Last() 
>>>>>>> 638cfd6cf65319457d5878763338303644681dec
             * }
             * 
             * По смыслу будет то же самое, но читается более "литературно". Не думаешь, что значит Count-1, а просто видишь, что в списке
             * берется последний элемент. 
             * 
             * И еще одно замечание: что если метод UpdateLatestVersion вызывается из RemoveVersion, но удаляется последняя версия из имеющихся? 
<<<<<<< HEAD
             * Получится, что список версий пуст, а в _latestVersion будет записана уже удаленная версия. А если сократить код до одной строчки
             * 
             * _latestVersion = _allVersions.LastOrDefault()
             * 
             * то он еще и будет выставлять _latestVersion в null в случае пустого списка. 
             * 
             */

             

             /*
              * ПРАВКИ:
              * 1) Заменил свою конструкцию на _latestVersion = _allVersions.LastOrDefault().
              */
=======
             * Получится, что список версий пуст, а в LatestVersion будет записана уже удаленная версия. А если сократить код до одной строчки
             * 
             * LatestVersion = AllVersions.LastOrDefault()
             * 
             * то он еще и будет выставлять LatestVersion в null в случае пустого списка. 
             * 
             */
>>>>>>> 638cfd6cf65319457d5878763338303644681dec
        }

        /// <summary>
        /// Метод для добавления версии в общий список версий продукта и, как следствие, обновления последней версии продукта.
        /// </summary>
        public void AddVersion (Version AddedVersion) {
            _allVersions.Add(AddedVersion);
            UpdateLatestVersion();
        }

        /// <summary>
        /// Метод для удаления версии из общего списка версий продукта и, как следствие, обновления последней версии продукта (при необходимости).
        /// </summary>
        public void RemoveVersion (Version RemovedVersion) {
            _allVersions.Remove(RemovedVersion);
            UpdateLatestVersion();
        }

        /// <summary>
        /// Метод для проверк добавляемой версии на то, является ли она старее последней.
        /// </summary>
        public bool CheckAddedVersion(Version AddedVersion) {
            /*if (_allVersions.Count > 0) {
                string[] Latest = _allVersions[_allVersions.Count - 1].ProductVersion.Split('.');
                string[] Added = AddedVersion.ProductVersion.Split('.');

                if (Convert.ToInt32(Latest[0]) < Convert.ToInt32(Added[0])) return true;
                else if (Convert.ToInt32(Latest[0]) == Convert.ToInt32(Added[0])) {
                    if (Convert.ToInt32(Latest[1]) < Convert.ToInt32(Added[1])) return true;
                    else if (Convert.ToInt32(Latest[1]) == Convert.ToInt32(Added[1])) {
                        if (Convert.ToInt32(Latest[2]) < Convert.ToInt32(Added[2])) return true;
                    }
                }*/


                /*
                 * Немного тяжело читается этот код по сравнивани версий. Более того, если будет не 3 фрагмента в версии, а 4 или больше, то не факт,
                 * что он будет работать корректно. Ты мог бы подумать, как эту логику заменить на функцию, которая при рекурсивном вызове будет выполять ту же 
                 * логику? Такой код будет более читаемым. 
                 */

                /*
                 * ПРАВКИ:
                 * 1) Добавил рекурсивную функцию IsNewVersion(), проверяющая актуальность добавляемой версии. Каждая цифра в номере добавляемой версии
                 * проверяется с соответствующей цифрой последней версией текущего продукта (соответствие поддерживается с помощью index).
                 */


<<<<<<< HEAD
              /*  return false;
            }
            return true; */

            return _allVersions.Count < 0 || IsNewVersion(0, AddedVersion) ?  true : false;
        }

        private bool IsNewVersion (int index, Version AddedVersion) {

            string[] latest = _latestVersion.ProductVersion.Split('.');
            string[] added = AddedVersion.ProductVersion.Split('.');

            if (index == added.Count()) return false;

            if (Convert.ToInt32(added[index]) > Convert.ToInt32(latest[index])) {
                return true;
            }
            else if (Convert.ToInt32(added[index]) == Convert.ToInt32(latest[index])) {
                return IsNewVersion(++index, AddedVersion);
            }
            else {
=======

                /*
                 * Немного тяжело читается этот код по сравнивани версий. Более того, если будет не 3 фрагмента в версии, а 4 или больше, то не факт,
                 * что он будет работать корректно. Ты мог бы подумать, как эту логику заменить на функцию, которая при рекурсивном вызове будет выполять ту же 
                 * логику? Такой код будет более читаемым. 
                 */


>>>>>>> 638cfd6cf65319457d5878763338303644681dec
                return false;
            }
        }

    }
    
}