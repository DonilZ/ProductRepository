using System;
using System.Collections.Generic;

namespace repository {
    //Собственная реализация класса KeyValuePair
    class Pair<T, K>{
		public T First { get; set; }
		public K Second { get; set; }
	}
    /*Класс Версия продукта. Объекты данного класса хранят соответствующую информацию о версии продукта
        (прописанной в ТЗ) с возможностью обновления каких-либо данных о версии */
    class Version {
        public string NameProduct { get; private set; }
        public string NumberVersion { get; private set; }
        public string ShortDescription { get; private set; }
        public string LongDescription { get; private set; }
        public string Changes { get; private set; }
        public Pair<string,string> DistributedFile { get; private set; }

        private Version() { }

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
        private List<Version> AllVersions;
        private Version LatestVersion;

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

                return false;
            }
            return true;
        }

    }
    
}