using Aequus.Core.DataSets;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aequus.Content.DataSets;

/// <summary>
/// Dataset with lists of all supported variants of an item. 
/// </summary>
public class ItemTypeVariants : DataSet {
    /// <summary><see cref="ItemID.RottenChunk"/>, <see cref="ItemID.Vertebrae"/>, Avalon/YuckyBit.</summary>
    [JsonProperty]
    public static List<ItemEntry> RottenChunk { get; private set; } = new();
    /// <summary><see cref="ItemID.DemoniteBar"/>, <see cref="ItemID.CrimtaneBar"/>, Avalon/BacciliteBar.</summary>
    [JsonProperty]
    public static List<ItemEntry> DemoniteBar { get; private set; } = new();
}
