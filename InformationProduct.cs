using System;
using System.Collections.Generic;

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


    /// <summary>
    /// Собственная реализация класса KeyValuePair
    /// </summary>
    /// <typeparam name="T">Тип первого элемента пары</typeparam>
    /// <typeparam name="K">Тип второго элемента пары</typeparam>
    class Pair<T, K>{
		public T First { get; set; }
		public K Second { get; set; }
	}
    /*Класс Версия продукта. Объекты данного класса хранят соответствующую информацию о версии продукта
        (прописанной в ТЗ) с возможностью обновления каких-либо данных о версии */
    class Version {
        public string NameProduct { get; private set; }  /*Правильнее будет ProductName*/
        public string NumberVersion { get; private set; } /*Правильнее будет ProductVersion*/
        public string ShortDescription { get; private set; }
        public string LongDescription { get; private set; }
        public string Changes { get; private set; }
        public Pair<string,string> DistributedFile { get; private set; } /*Что понимается под DisributedFile? По-русски понимается как распространенный файл. Может DownloadableFile ? */

        private Version() { }

        /*
         * Имена параметров методов пишутся в camelCase (https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-parameters). 
         * А подчеркивание обычно используется только для именования приватных членов класса. Т.е. если бы ты вводил не свойство ShortDescription, а
         * приватный член для хранения этого значения, то вот его можно было бы объявить как private string _shortDescription. А так - подчеркивания 
         * лучше не использовать. 
         */

        public Version (string _NameProduct, string _NumberVersion, string _ShortDescription,
                        string _LongDescription, string _Changes, Pair<string,string> _DistributedFile) {
            NameProduct = _NameProduct;
            NumberVersion = _NumberVersion;
            ShortDescription = _ShortDescription;
            LongDescription = _LongDescription;
            Changes = _Changes;
            DistributedFile = _DistributedFile;
        }

        public void Update(Version UpdateVersion) {
            NameProduct = UpdateVersion.NameProduct;
            NumberVersion = UpdateVersion.NumberVersion;
            ShortDescription = UpdateVersion.ShortDescription;
            LongDescription = UpdateVersion.LongDescription;
            Changes = UpdateVersion.Changes;
            DistributedFile = UpdateVersion.DistributedFile;
        }
    }
    /*Класс Продукт. Объекты данного класса хранят соответствующую информацию о Продукте
        (прописанной в ТЗ), с возможностью получения необходимых данных Продукта, хранения всех версий данного Продукта,
         добавления, обновления, удаления и, при необходимости, изменения последней версии Продукта*/
    class Product {

        /*
         * К сожалению, не нашел этого в том руководству по именованию, на которое давал ссылку, но приватные члены класса обычно именуются 
         * в camelCase или _camelCase. А вот публичные и protected - в PascalCase.
         */

        private List<Version> AllVersions;
        private Version LatestVersion;

        /*
         * Пустые конструкторы без параметров в C# не нужны - они создаются компилятором автоматически, если не объявлен иной конструктор.
         * Если же объявлен, то пустой создается только тогда, когда он нужен. Но в твоем случае он все-равно не используется.
         */

        private Product() { }
        
        public Product (Version _LatestVersion) {
            AllVersions = new List<Version>();
            this.LatestVersion = _LatestVersion;
            AllVersions.Add(_LatestVersion);
        }

        public string GetNameProduct() {
            return LatestVersion.NameProduct;
        }

        public string GetNumberLatestVersion() {
            return LatestVersion.NumberVersion;
        }

        public string GetShortDescription() {
            return LatestVersion.ShortDescription;
        }

        public List<Version> GetAllVersions() {
            return AllVersions;
        }

        private void UpdateLatestVersion() {
            if (AllVersions.Count > 0) {
                LatestVersion = AllVersions[AllVersions.Count - 1];
            }

            /*
             * Как вариант для таких условий можно использовать Linq to Objects. Тогда код выше будет выглядеть так: 
             * if (AllVersions.Any()) {
             *   LatestVersion = AllVersions.Last() 
             * }
             * 
             * По смыслу будет то же самое, но читается более "литературно". Не думаешь, что значит Count-1, а просто видишь, что в списке
             * берется последний элемент. 
             * 
             * И еще одно замечание: что если метод UpdateLatestVersion вызывается из RemoveVersion, но удаляется последняя версия из имеющихся? 
             * Получится, что список версий пуст, а в LatestVersion будет записана уже удаленная версия. А если сократить код до одной строчки
             * 
             * LatestVersion = AllVersions.LastOrDefault()
             * 
             * то он еще и будет выставлять LatestVersion в null в случае пустого списка. 
             * 
             */
        }

        public void AddVersion (Version AddedVersion) {
            AllVersions.Add(AddedVersion);
            UpdateLatestVersion();
        }

        public void RemoveVersion (Version RemovedVersion) {
            AllVersions.Remove(RemovedVersion);
            UpdateLatestVersion();
        }

        //Проверка добавляемой версии на то, является ли она старее последней
        public bool CheckAddedVersion(Version AddedVersion) {
            if (AllVersions.Count > 0) {
                string[] Latest = AllVersions[AllVersions.Count - 1].NumberVersion.Split('.');
                string[] Added = AddedVersion.NumberVersion.Split('.');

                if (Convert.ToInt32(Latest[0]) < Convert.ToInt32(Added[0])) return true;
                else if (Convert.ToInt32(Latest[0]) == Convert.ToInt32(Added[0])) {
                    if (Convert.ToInt32(Latest[1]) < Convert.ToInt32(Added[1])) return true;
                    else if (Convert.ToInt32(Latest[1]) == Convert.ToInt32(Added[1])) {
                        if (Convert.ToInt32(Latest[2]) < Convert.ToInt32(Added[2])) return true;
                    }
                }


                /*
                 * Немного тяжело читается этот код по сравнивани версий. Более того, если будет не 3 фрагмента в версии, а 4 или больше, то не факт,
                 * что он будет работать корректно. Ты мог бы подумать, как эту логику заменить на функцию, которая при рекурсивном вызове будет выполять ту же 
                 * логику? Такой код будет более читаемым. 
                 */


                return false;
            }
            return true;
        }

    }
    
}