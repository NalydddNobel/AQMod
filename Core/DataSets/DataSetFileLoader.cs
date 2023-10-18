using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using Terraria;

namespace Aequus.Core.DataSets;

public sealed class DataSetFileLoader {
    private DataSet _dataSet;
    public readonly string FilePath;
    public readonly string ModFileStreamPath;
    public readonly string FileData;

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
            if (!string.IsNullOrEmpty(FileData)) {
                JsonConvert.PopulateObject(FileData, _dataSet);
            }
        }
        catch (Exception ex) {
            _dataSet.Mod.Logger.Error(ex);
        }
    }

    public void CreateTempFile() {
        string createFile = $"{Main.SavePath}/ModSources/{FilePath}.Temp.json";
        _dataSet.Mod.Logger.Debug(createFile);
        try {
            var settings = new JsonSerializerSettings {
                Formatting = Formatting.Indented,
            };
            var jsonData = JsonConvert.SerializeObject(_dataSet, settings);
            if (jsonData != null) {
                var buffer = Encoding.UTF8.GetBytes(jsonData);
                using var file = File.Create(createFile, buffer.Length);
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