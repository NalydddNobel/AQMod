using Aequus.DataSets.Structures;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Aequus.DataSets.Json;

internal class EntryConverter : JsonConverter<IIDEntry> {
    public override void WriteJson(JsonWriter writer, [AllowNull] IIDEntry value, JsonSerializer serializer) {
        writer.WriteValue(value.Name);
        //if (value.ValidEntry && value.VanillaEntry) {
        //    writer.WriteComment(value.Id.ToString());
        //}
    }

    public override IIDEntry ReadJson(JsonReader reader, Type objectType, [AllowNull] IIDEntry existingValue, bool hasExistingValue, JsonSerializer serializer) {
        string name = (string)reader.Value;
        IIDEntry entry = (IIDEntry)Activator.CreateInstance(objectType);

        entry.SetName(name);

        return entry;
    }
}