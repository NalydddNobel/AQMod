using Newtonsoft.Json;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Aequus.Common.DataSets;

public sealed class EmbeddedJsonFile {
    private readonly IJsonHolder _dataSet;
    public readonly string FileName;
    public readonly string FullFilePath;
    public readonly string? FileData;

    internal EmbeddedJsonFile(IJsonHolder holder) {
        _dataSet = holder;
        FileName = holder.FilePath;
        FullFilePath = $"Assets/Metadata/{FileName}.json";

        if (Aequus.Instance.FileExists(FullFilePath)) {
            using var stream = Aequus.Instance.GetFileStream(FullFilePath, newFileStream: true);
            using var streamReader = new StreamReader(stream);
            FileData = streamReader.ReadToEnd();
        }
        else {
            FileData = null;
        }
    }

    public void Apply() {
        try {
            if (string.IsNullOrEmpty(FileData)) {
                return;
            }

            DeserializeTo(FileData, _dataSet);
        }
        catch (Exception ex) {
            Aequus.Instance.Logger.Error(ex);
        }
    }

    static void DeserializeTo(string data, object instance) {
        Type myType = instance.GetType();

        object? obj = JsonConvert.DeserializeObject(data, myType);
        Type? objType = obj?.GetType();

        if (obj == null || objType == null || !objType.Equals(myType)) {
            return;
        }

        foreach (FieldInfo f in objType.GetFields(BindingFlags.Public | BindingFlags.Instance).Where(f => f.GetCustomAttribute<JsonPropertyAttribute>() != null)) {
            object? to = f.GetValue(instance);
            object? from = f.GetValue(obj);

            if (from == null) {
                return;
            }

            if (to == null) {
                f.SetValue(obj, from);
            }
            else if (to is IList toList && from is IList fromList) {
                foreach (object fromItem in fromList) {
                    toList.Add(fromItem);
                }
            }
            else if (to is IDictionary toDict && from is IDictionary fromDict) {
                var enumerator = fromDict.GetEnumerator();

                while (enumerator.MoveNext()) {
                    toDict.Add(enumerator.Key, enumerator.Value);
                }
            }
        }
    }

    [Conditional("GEN")]
    internal void GenerateEmbeddedFiles() {
        string assetLocation = Path.Join(Aequus.ModSources, "Assets", "Metadata");
        string fileLocation = Path.Join(assetLocation, $"{FileName}.json");
        Aequus.Instance.Logger.Debug(fileLocation);
        try {
            // Only attempt to create the file if the metadata directory even exists.
            if (!Directory.Exists(assetLocation)) {
                return;
            }

            // Create subdirectories for the full file location.
            string? fileSubLocation = Path.GetDirectoryName(fileLocation);
            if (!Directory.Exists(fileSubLocation)) {
                Directory.CreateDirectory(fileSubLocation!);
            }

            JsonSerializerSettings settings = new JsonSerializerSettings {
                Formatting = Formatting.Indented,
            };
            string jsonData = JsonConvert.SerializeObject(_dataSet, settings);
            if (jsonData != null) {
                byte[] buffer = Encoding.UTF8.GetBytes(jsonData);
                using FileStream file = File.Create(fileLocation, buffer.Length);
                file.Write(buffer, 0, buffer.Length);
            }
        }
        catch (Exception ex) {
            Aequus.Instance.Logger.Error(ex);
        }
    }
}