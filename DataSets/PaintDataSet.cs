using Aequu2.DataSets.Structures;
using Aequu2.DataSets.Structures.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace Aequu2.DataSets;

public class PaintDataSet : DataSet {
    [JsonProperty]
    public static Dictionary<PaintColor, IDEntry<ItemID>> Paints { get; private set; } = [];
    [JsonProperty]
    public static Dictionary<PaintColor, IDEntry<ItemID>> DeepPaints { get; private set; } = [];
    [JsonIgnore]
    public static Dictionary<PaintColor, IDEntry<ItemID>> Dyes { get; private set; } = new() {
        [PaintColor.Red] = ItemID.RedDye,
        [PaintColor.Orange] = ItemID.OrangeDye,
        [PaintColor.Yellow] = ItemID.YellowDye,
        [PaintColor.Lime] = ItemID.LimeDye,
        [PaintColor.Green] = ItemID.GreenDye,
        [PaintColor.Teal] = ItemID.TealDye,
        [PaintColor.Cyan] = ItemID.CyanDye,
        [PaintColor.SkyBlue] = ItemID.SkyBlueDye,
        [PaintColor.Blue] = ItemID.BlueDye,
        [PaintColor.Purple] = ItemID.PurpleDye,
        [PaintColor.Violet] = ItemID.VioletDye,
        [PaintColor.Pink] = ItemID.PinkDye,
        [PaintColor.Brown] = ItemID.BrownDye,
        [PaintColor.Black] = ItemID.BlackDye,
        [PaintColor.Rainbow] = ItemID.RainbowDye,
        [PaintColor.White] = ItemID.SilverDye,
    };
    [JsonProperty]
    public static Dictionary<PaintColor, IDEntry<ItemID>> YoyoStrings { get; private set; } = [];
    [JsonProperty]
    public static Dictionary<PaintColor, IDEntry<ItemID>> GolfBalls { get; private set; } = [];
    [JsonIgnore]
    public static Dictionary<PaintColor, Color> RGB { get; private set; } = new() {
        [PaintColor.Red] = Color.Red,
        [PaintColor.Orange] = Color.Orange,
        [PaintColor.Yellow] = Color.Yellow,
        [PaintColor.Lime] = Color.LawnGreen,
        [PaintColor.Green] = Color.ForestGreen,
        [PaintColor.Teal] = Color.Teal,
        [PaintColor.Cyan] = Color.Cyan,
        [PaintColor.SkyBlue] = Color.SkyBlue,
        [PaintColor.Blue] = Color.Blue,
        [PaintColor.Purple] = Color.Purple,
        [PaintColor.Violet] = Color.Violet,
        [PaintColor.Pink] = Color.HotPink,
        [PaintColor.Brown] = Color.Brown,
        [PaintColor.Black] = Color.Black,
        [PaintColor.Gray] = Color.Gray,
        [PaintColor.White] = Color.White,
    };

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