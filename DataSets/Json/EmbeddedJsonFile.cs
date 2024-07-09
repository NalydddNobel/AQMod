using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AequusRemake.DataSets.Json;

public sealed class EmbeddedJsonFile {
    private readonly IJsonHolder _dataSet;
    public readonly string FilePath;
    public readonly string FullFilePath;
    public readonly string FileData;

    internal EmbeddedJsonFile(IJsonHolder holder) {
        _dataSet = holder;
        FilePath = holder.FilePath;
        FullFilePath = $"Assets/Metadata/{FilePath}.json";

        if (mod.FileExists(FullFilePath)) {
            using var stream = mod.GetFileStream(FullFilePath, newFileStream: true);
            using var streamReader = new StreamReader(stream);
            FileData = streamReader.ReadToEnd();
        }
    }

    public void Apply() {
        try {
            if (!string.IsNullOrEmpty(FileData)) {
                JsonConvert.DeserializeObject(FileData, _dataSet.GetType());
            }
        }
        catch (Exception ex) {
            Log.Error(ex);
        }
    }

    [Conditional("DEBUG")]
    internal void GenerateEmbeddedFiles() {
        string fileLocation = Path.Join(AequusRemake.DEBUG_FILES_PATH, "ModSources/AequusRemake", FullFilePath).Replace('/', Path.DirectorySeparatorChar);
        Log.Debug(fileLocation);
        try {
            // Only attempt to create the file if this Directory even exists.
            if (!Directory.Exists(Path.GetDirectoryName(fileLocation))) {
                return;
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
            Log.Error(ex);
        }
        finally {
        }
    }
}