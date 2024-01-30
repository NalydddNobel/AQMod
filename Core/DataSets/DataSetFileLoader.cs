using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Aequus.Core.DataSets;

public sealed class DataSetFileLoader {
    private readonly DataSet _dataSet;
    public readonly String FilePath;
    public readonly String ModFileStreamPath;
    public readonly String FileData;

    public DataSetFileLoader(DataSet dataSet) {
        _dataSet = dataSet;
        FilePath = dataSet.FilePath;
        ModFileStreamPath = $"{FilePath}.json"[(dataSet.Mod.Name.Length + 1)..];
        if (dataSet.Mod.FileExists(ModFileStreamPath)) {
            using var stream = dataSet.Mod.GetFileStream(ModFileStreamPath, newFileStream: true);
            using var streamReader = new StreamReader(stream);
            FileData = streamReader.ReadToEnd();
        }
    }

    public void ApplyToDataSet() {
        try {
            if (!String.IsNullOrEmpty(FileData)) {
                JsonConvert.DeserializeObject(FileData, _dataSet.GetType());
            }
        }
        catch (Exception ex) {
            _dataSet.Mod.Logger.Error(ex);
        }
    }

    [Conditional("DEBUG")]
    internal void CreateTempFile() {
        String fileLocation = Path.Join(Aequus.DebugPath, "ModSources", FilePath.Replace($"Content/DataSets/", "Assets/Metadata/") + ".json").Replace('/', Path.DirectorySeparatorChar);
        _dataSet.Mod.Logger.Debug(fileLocation);
        try {
            // Only attempt to create the file if this Directory even exists.
            if (!Directory.Exists(Path.GetDirectoryName(fileLocation))) {
                return;
            }

            JsonSerializerSettings settings = new JsonSerializerSettings {
                Formatting = Formatting.Indented,
            };
            String jsonData = JsonConvert.SerializeObject(_dataSet, settings);
            if (jsonData != null) {
                Byte[] buffer = Encoding.UTF8.GetBytes(jsonData);
                using FileStream file = File.Create(fileLocation, buffer.Length);
                file.Write(buffer, 0, buffer.Length);
            }
        }
        catch (Exception ex) {
            _dataSet.Mod.Logger.Error(ex);
        }
        finally {
        }
    }
}