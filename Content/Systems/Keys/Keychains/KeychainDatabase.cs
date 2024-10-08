using Aequus.Common.DataSets;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aequus.Content.Systems.Keys.Keychains;

public class KeychainDatabase : MetaDatabase {
    public override string Category => "Items";

    public static string TextureDir => $"{typeof(KeychainDatabase).NamespacePath()}/Textures";

    [JsonProperty]
    public readonly Dictionary<IDEntry<ItemID>, KeychainInfo> Values = [];

    public readonly Dictionary<IDEntry<ItemID>, string> TexturePaths = [];

    [ModCall("AddKey")]
    public static void AddKeychainInfo(int itemId, string texturePath, bool nonConsumable = false) {
        var db = Instance<KeychainDatabase>();

        db.Values[itemId] = new() {
            NonConsumable = nonConsumable,
        };

        db.TexturePaths[itemId] = texturePath;
    }
}

public record struct KeychainInfo([JsonProperty] bool NonConsumable);
