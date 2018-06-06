using System;
using System.Collections.Generic;

namespace repository {

    class Pair<T, K>{
		public T First { get; set; }
		public K Second { get; set; }
	}

    class Version {
        public string name_product { get; private set; }
        public string number_version { get; private set; }
        public string short_desc { get; private set; }
        public string long_desc { get; private set; }
        public string changes { get; private set; }
        public Pair<string,string> dist_file { get; private set; }

        private Version() { }
        public Version (string name_product, string number_version, string short_desc,
                        string long_desc, string changes, Pair<string,string> dist_file) {
            this.name_product = name_product;
            this.number_version = number_version;
            this.short_desc = short_desc;
            this.long_desc = long_desc;
            this.changes = changes;
            this.dist_file = dist_file;
        }

        public void update(Version update_version) {
            name_product = update_version.name_product;
            number_version = update_version.number_version;
            short_desc = update_version.short_desc;
            long_desc = update_version.long_desc;
            changes = update_version.changes;
            dist_file = update_version.dist_file;
        }
    }

    class Product {
        private List<Version> all_versions;
        private Version last_version;

        private Product() { }
        public Product (Version last_version) {
            all_versions = new List<Version>();
            this.last_version = last_version;
            all_versions.Add(last_version);
        }

        public string get_name_product() {
            return last_version.name_product;
        }

        public string get_num_last_version() {
            return last_version.number_version;
        }

        public string get_short_desc() {
            return last_version.short_desc;
        }

        public List<Version> get_all_versions() {
            return all_versions;
        }

        private void check_last_version() {
            if (all_versions.Count > 0) {
                last_version = all_versions[0];
                string[] last = all_versions[0].number_version.Split('.');

                foreach(Version current_version in all_versions) {
                    string[] cur = current_version.number_version.Split('.');

                    if (Convert.ToInt32(last[0]) < Convert.ToInt32(cur[0])) last_version = current_version;
                    else if (Convert.ToInt32(last[0]) == Convert.ToInt32(cur[0])) {
                        if (Convert.ToInt32(last[1]) < Convert.ToInt32(cur[1])) last_version = current_version;
                        else if (Convert.ToInt32(last[1]) == Convert.ToInt32(cur[1])) {
                            if (Convert.ToInt32(last[2]) < Convert.ToInt32(cur[2])) last_version = current_version;
                        }
                    }

                }
            }
        }
        public void add_version (Version version) {
            all_versions.Add(version);
            check_last_version();
        }

        public void delete_version (Version version) {
            all_versions.Remove(version);
            check_last_version();
        }

    }
    
}