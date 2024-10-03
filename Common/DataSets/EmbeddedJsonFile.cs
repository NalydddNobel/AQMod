using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
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
            if (!string.IsNullOrEmpty(FileData)) {
                JsonConvert.DeserializeObject(FileData, _dataSet.GetType());
            }
        }
        catch (Exception ex) {
            Aequus.Instance.Logger.Error(ex);
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