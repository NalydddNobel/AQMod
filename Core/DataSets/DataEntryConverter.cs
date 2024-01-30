using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Aequus.Core.DataSets;

public class DataEntryConverter<TEntry, T> : JsonConverter<IDataEntry<T>> where TEntry : IDataEntry<T>, new() {
    public override void WriteJson(JsonWriter writer, [AllowNull] IDataEntry<T> value, JsonSerializer serializer) {
        value.Initialize();
        writer.WriteValue(value.Name);
        if (value.ValidEntry && value.VanillaEntry) {
            writer.WriteComment(value.Id.ToString());
        }
    }

    public override IDataEntry<T> ReadJson(JsonReader reader, Type objectType, [AllowNull] IDataEntry<T> existingValue, Boolean hasExistingValue, JsonSerializer serializer) {
        var entry = new TEntry {
            Name = (String)reader.Value
        };
        entry.Initialize();
        return entry;
    }
}