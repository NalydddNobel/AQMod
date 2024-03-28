using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using Terraria.Utilities;

namespace Aequus.DataSets.Json;

[JsonConverter(typeof(JsonRangeConverter))]
public record struct JsonRange(int Min, int Max) {
    public JsonRange(int Quantity) : this(Quantity, Quantity) { }
}

public class JsonRangeConverter : JsonConverter<JsonRange> {
    public override void WriteJson(JsonWriter writer, [AllowNull] JsonRange value, JsonSerializer serializer) {
        if (value.Min == value.Max) {
            writer.WriteValue(value.Min);
        }
        else {
            writer.WriteValue(value.Min + "-" + value.Max);
        }
    }

    public override JsonRange ReadJson(JsonReader reader, Type objectType, [AllowNull] JsonRange existingValue, bool hasExistingValue, JsonSerializer serializer) {
        string value = (string)reader.Value;

        if (value.Contains('-')) {
            var split = value.Split('-');
            if (split.Length == 2 && Helper.TryReadInt(split[0].Trim(), out int min) && Helper.TryReadInt(split[1].Trim(), out int max)) {
                return new(min, max);
            }
        }

        if (!Helper.TryReadInt(value, out int rawValue)) {
            return new(rawValue);
        }

        return existingValue;
    }
}

public static class JsonRangeExtensions {
    public static int Next(this UnifiedRandom random, JsonRange range) {
        return random.Next(range.Min, range.Max + 1);
    }
}