using Aequus.Core.IO;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Core.Utilities;

public static class IOHelper {
    public static void AddContentIDsToCollection(this JsonContentFile<string> jsonContentFile, string name, ICollection<int> set) {
        var l = GetContentIDsList(jsonContentFile, name);
        foreach (int entry in l) {
            set.Add(entry);
        }
    }

    public static List<int> GetContentIDsList(this JsonContentFile<string> jsonContentFile, string name) {
        List<int> resultList = new();
        if (name.Contains('_')) {
            var nameSplit = name.Split("_");
            if (!jsonContentFile.contentArray.TryGetValue(nameSplit[0], out var orderedDictionary)) {
                return resultList;
            }

            if (!orderedDictionary.TryGetValue(nameSplit[1], out var orderedList)) {
                return resultList;
            }

            foreach (var contentName in orderedList) {
                if (jsonContentFile.idSearch.TryGetId(contentName, out int id)) {
                    resultList.Add(id);
                }
            }
            return resultList;
        }

        if (!jsonContentFile.contentArray.TryGetValue(name, out var dictionary)) {
            return resultList;
        }

        foreach (var data in dictionary) {
            string namePrefix = "";
            if (data.Key != "Vanilla") {
                if (!ModLoader.TryGetMod(data.Key, out var mod)) {
                    continue;
                }
                namePrefix = mod.Name + "/";
            }

            foreach (var contentName in data.Value) {
                if (jsonContentFile.idSearch.TryGetId(namePrefix + contentName, out int id)) {
                    resultList.Add(id);
                }
            }
        }
        return resultList;
    }

    public static T Get<T>(this TagCompound tag, string key, T defaultValue) {
        if (tag.TryGet(key, out T outputValue)) {
            return outputValue;
        }
        return defaultValue;
    }
}