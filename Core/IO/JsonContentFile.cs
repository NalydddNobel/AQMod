using Newtonsoft.Json;
using ReLogic.Reflection;
using System.Collections.Generic;
using System.IO;
using Terraria.ModLoader;

namespace Aequus.Core.IO {
    public class JsonContentFile<T> {
        public readonly string FilePath;
        public readonly Dictionary<string, Dictionary<string, List<T>>> contentArray;
        public readonly IdDictionary idSearch;

        internal JsonContentFile(Mod mod, string filePath, IdDictionary idSearch) {
            this.idSearch = idSearch;
            FilePath = filePath;
            using var stream = mod.GetFileStream($"Content/{filePath}.json", newFileStream: true);
            using var streamReader = new StreamReader(stream);
            contentArray = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<T>>>>(streamReader.ReadToEnd());
        }

        internal JsonContentFile(string filePath, IdDictionary idSearch) : this(Aequus.Instance, filePath, idSearch) {
        }
    }
}