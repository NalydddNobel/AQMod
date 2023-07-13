using Newtonsoft.Json;
using ReLogic.Reflection;
using System.Collections.Generic;
using System.IO;
using Terraria.ModLoader;

namespace Aequus.Common.IO {
    public class JsonContentFile {
        public readonly string Name;
        public readonly Dictionary<string, List<string>> contentArray;
        public readonly IdDictionary idSet;

        internal JsonContentFile(Mod mod, string file, IdDictionary idSet) {
            this.idSet = idSet;
            Name = file;
            using var stream = mod.GetFileStream($"Content/{file}.json", newFileStream: true);
            using var streamReader = new StreamReader(stream);
            contentArray = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(streamReader.ReadToEnd());
        }

        internal JsonContentFile(string file, IdDictionary idSet) : this(Aequus.Instance, file, idSet) {
        }

        public void AddToIntCollection(string name, ICollection<int> set) {
            var l = ReadIntList(name);
            foreach (int entry in l) {
                set.Add(entry);
            }
        }

        public List<int> ReadIntList(string name) {
            var resultList = new List<int>();
            if (!contentArray.TryGetValue(name, out var list))
                return resultList;

            foreach (var s in list) {
                string itemName = s.Replace(';', '/');
                if (idSet.TryGetId(itemName, out int id)) {
                    resultList.Add(id);
                }
            }
            return resultList;
        }
    }
}