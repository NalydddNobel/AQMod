using Aequus.Common.DataSets;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aequus.Content.Systems.Keys.Keychains;

public class KeychainDatabase : MetaDatabase {
    public override string Category => "Items";

    public static string TextureDir => $"{typeof(KeychainDatabase).NamespacePath()}/Textures";

    [JsonProperty]
    public Dictionary<IDEntry<ItemID>, KeychainInfo> Values { get; private set; } = [
        //new(ItemID.GoldenKey, new() {NonConsumable=false})
    ];
}

public record struct KeychainInfo([JsonProperty] bool NonConsumable);
