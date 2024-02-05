using Aequus.Core.DataSets;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace Aequus.Content.DataSets;

public class PaintSets : DataSet {
    [JsonProperty]
    public static Dictionary<PaintColor, Entry<ItemID>> Paints { get; private set; } = new();
    [JsonProperty]
    public static Dictionary<PaintColor, Entry<ItemID>> DeepPaints { get; private set; } = new();
    [JsonProperty]
    public static Dictionary<PaintColor, Entry<ItemID>> Dyes { get; private set; } = new();
    [JsonProperty]
    public static Dictionary<PaintColor, Entry<ItemID>> YoyoStrings { get; private set; } = new();
    [JsonProperty]
    public static Dictionary<PaintColor, Entry<ItemID>> GolfBalls { get; private set; } = new();

    public override void AddRecipes() {
        //AutogenerateEntries();
    }

    [Conditional("DEBUG")]
    private static void AutogenerateEntries() {
        AutogenerateColorLookup(Paints, "", "Paint");
        AutogenerateColorLookup(DeepPaints, "Deep", "Paint");
        AutogenerateColorLookup(Dyes, "", "Dye");
        AutogenerateColorLookup(YoyoStrings, "", "String");
        GolfBalls.Add(PaintColor.White, ItemID.GolfBall);
        AutogenerateColorLookup(GolfBalls, "GolfBallDyed", "");

        static void AutogenerateColorLookup(Dictionary<PaintColor, Entry<ItemID>> dictionary, string prefix, string suffix) {
            for (int i = 0; i < (int)PaintColor.Count; i++) {
                var key = (PaintColor)i;
                string name = prefix + key.ToString() + suffix;
                if (ItemID.Search.TryGetId(name, out int id)) {
                    dictionary.Add(key, id);
                }
            }
        }
    }
}