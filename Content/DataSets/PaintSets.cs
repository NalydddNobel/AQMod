using Aequus.Core.DataSets;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace Aequus.Content.DataSets;

public class PaintSets : DataSet {
    [JsonProperty]
    public static Dictionary<PaintColor, ItemEntry> Paints { get; private set; } = new();
    [JsonProperty]
    public static Dictionary<PaintColor, ItemEntry> DeepPaints { get; private set; } = new();
    [JsonProperty]
    public static Dictionary<PaintColor, ItemEntry> Dyes { get; private set; } = new();
    [JsonProperty]
    public static Dictionary<PaintColor, ItemEntry> YoyoStrings { get; private set; } = new();
    [JsonProperty]
    public static Dictionary<PaintColor, ItemEntry> GolfBalls { get; private set; } = new();

    public override void AddRecipes() {
        //AutogenerateEntries();
    }

    [Conditional("DEBUG")]
    private static void AutogenerateEntries() {
        AutogenerateColorLookup(Paints, "", "Paint");
        AutogenerateColorLookup(DeepPaints, "Deep", "Paint");
        AutogenerateColorLookup(Dyes, "", "Dye");
        AutogenerateColorLookup(YoyoStrings, "", "String");
        GolfBalls.Add(PaintColor.White, (ItemEntry)ItemID.GolfBall);
        AutogenerateColorLookup(GolfBalls, "GolfBallDyed", "");

        static void AutogenerateColorLookup(Dictionary<PaintColor, ItemEntry> dictionary, System.String prefix, System.String suffix) {
            for (System.Int32 i = 0; i < (System.Int32)PaintColor.Count; i++) {
                var key = (PaintColor)i;
                System.String name = prefix + key.ToString() + suffix;
                if (ItemID.Search.TryGetId(name, out System.Int32 id)) {
                    dictionary.Add(key, (ItemEntry)id);
                }
            }
        }
    }
}