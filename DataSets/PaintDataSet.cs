using Aequus.DataSets.Items;
using Aequus.DataSets.Structures;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace Aequus.DataSets;

public class PaintDataSet : DataSet {
    [JsonProperty]
    public static Dictionary<PaintColor, IDEntry<ItemID>> Paints { get; private set; } = new();
    [JsonProperty]
    public static Dictionary<PaintColor, IDEntry<ItemID>> DeepPaints { get; private set; } = new();
    [JsonProperty]
    public static Dictionary<PaintColor, IDEntry<ItemID>> Dyes { get; private set; } = new();
    [JsonProperty]
    public static Dictionary<PaintColor, IDEntry<ItemID>> YoyoStrings { get; private set; } = new();
    [JsonProperty]
    public static Dictionary<PaintColor, IDEntry<ItemID>> GolfBalls { get; private set; } = new();

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

        static void AutogenerateColorLookup(Dictionary<PaintColor, IDEntry<ItemID>> dictionary, string prefix, string suffix) {
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