using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Aequus.Common.DataSets;

internal class EntryConverter : JsonConverter<IIDEntry> {
    public override void WriteJson(JsonWriter writer, [AllowNull] IIDEntry value, JsonSerializer serializer) {
        if (value == null || string.IsNullOrEmpty(value.Name)) {
            return;
        }

        writer.WriteValue(value.Name);
        //if (value.ValidEntry && value.VanillaEntry) {
        //    writer.WriteComment(value.Id.ToString());
        //}
    }

    public override IIDEntry ReadJson(JsonReader reader, Type objectType, [AllowNull] IIDEntry existingValue, bool hasExistingValue, JsonSerializer serializer) {
        string? name = reader.Value as string;

        if (Activator.CreateInstance(objectType) is not IIDEntry entry || string.IsNullOrEmpty(name)) {
            return default!;
        }

        entry.SetName(name);

        return entry;
    }
}