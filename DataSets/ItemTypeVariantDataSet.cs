using Aequus.DataSets.Structures;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aequus.DataSets;

/// <summary>
/// Dataset with lists of all supported variants of an item. 
/// </summary>
public class ItemTypeVariantDataSet : DataSet {
    /// <summary><see cref="ItemID.RottenChunk"/>, <see cref="ItemID.Vertebrae"/>, Avalon/YuckyBit.</summary>
    [JsonProperty]
    public static List<IDEntry<ItemID>> RottenChunk { get; private set; } = new();
    /// <summary><see cref="ItemID.DemoniteBar"/>, <see cref="ItemID.CrimtaneBar"/>, Avalon/BacciliteBar.</summary>
    [JsonProperty]
    public static List<IDEntry<ItemID>> DemoniteBar { get; private set; } = new();
}
