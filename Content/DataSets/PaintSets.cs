using Aequus.Core.DataSets;
using Newtonsoft.Json;
using System.Collections.Generic;

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
        //#if DEBUG
        //AutogenerateColorLookup(Paints, "", "Paint");
        //AutogenerateColorLookup(DeepPaints, "Deep", "Paint");
        //AutogenerateColorLookup(Dyes, "", "Dye");
        //AutogenerateColorLookup(YoyoStrings, "", "String");
        //GolfBalls.Add(PaintColor.White, (ItemEntry)ItemID.GolfBall);
        //AutogenerateColorLookup(GolfBalls, "GolfBallDyed", "");

        //static void AutogenerateColorLookup(Dictionary<PaintColor, ItemEntry> dictionary, string prefix, string suffix) {
        //    for (int i = 0; i < (int)PaintColor.Count; i++) {
        //        var key = (PaintColor)i;
        //        string name = prefix + key.ToString() + suffix;
        //        if (ItemID.Search.TryGetId(name, out int id)) {
        //            dictionary.Add(key, (ItemEntry)id);
        //        }
        //    }
        //}
        //#endif
    }
}