using Aequus.Common.Items.Components;
using Aequus.Core.Graphics;
using Aequus.DataSets;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Tools.Keychain;

public partial class Keychain : ModItem, IRightClickOverrideWhenHovered, IOnConsumedInRecipe {
    public const int MAX_KEYS_ALLOWED = 32;
    public const int KEYS_FRAME_COUNT = 5;
    public const string TAG_KEY = "Keychain";

    public List<Item> _keys;
    public List<Item> _sortedKeyIcons;

    public virtual bool AcceptItem(Item acceptedItem) {
        lock (_keys) {
            bool anyStacks = false;
            // Try stacking the accepted item into the keychain.
            foreach (Item item in _keys) {
                if (item.type == acceptedItem.type) {
                    ItemLoader.TryStackItems(item, acceptedItem, out _);
                    anyStacks = true;
                    if (acceptedItem.stack <= 0) {
                        return true;
                    }
                }
            }

            // If there were any stack attempts, and this key is non-consumable, return.
            if (anyStacks && ItemDataSet.KeychainData.TryGetValue(acceptedItem.type, out KeychainInfo value) && value.NonConsumable) {
                return true;
            }

            // Prevent adding new keys if the limit was reached.
            if (_keys.Count >= MAX_KEYS_ALLOWED) {
                return anyStacks;
            }

            // If there are still keys left, add them as a new stack.
            if (acceptedItem.stack > 0) {
                _keys.Add(acceptedItem.Clone());
                acceptedItem.TurnToAir();
                return true;
            }
        }

        return false;
    }

    public virtual bool ConsumeKey(Player player, int type) {
        lock (_keys) {
            foreach (Item item in _keys) {
                // Only iterate on keys which match the wanted type.
                if (item.type != type) {
                    continue;
                }

                // Return early if this key is non consumable.
                if (ItemDataSet.KeychainData.TryGetValue(item.type, out KeychainInfo value) && value.NonConsumable) {
                    return true;
                }

                // Reduce Stack.
                item.stack--;
                if (item.stack <= 0) {
                    item.TurnToAir();
                }

                return true;
            }
        }

        return false;
    }

    public void RefreshKeys() {
        lock (_keys) {
            _keys.RemoveAll((i) => i == null || i.IsAir);
            _sortedKeyIcons = new List<Item>(_keys.Select(GetKeyIconItem));
            _sortedKeyIcons.Sort((item1, item2) => item1.Name.CompareTo(item2.Name));
        }
    }

    private Item GetKeyIconItem(Item key) {
        Item resultKey = key.Clone();
        //resultKey.stack = 1;
        return resultKey;
    }

    public override void Load() {
        if (!Main.dedServ) {
            KeyTextures = new Paletter(AequusTextures.PaletteKey, AequusTextures.KeychainKeysTemplate);
            Main.QueueMainThreadAction(KeyTextures.Load);
        }
    }

    public override void Unload() {
        Main.QueueMainThreadAction(() => KeyTextures?.Dispose());
    }

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(gold: 1);
        _keys = new();
    }

    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<KeychainPlayer>().keyChain = this;
    }

    public override ModItem Clone(Item newEntity) {
        Keychain clone = (Keychain)base.Clone(newEntity);
        clone._keys = new List<Item>(_keys);
        clone.RefreshKeys();
        return clone;
    }

    public override void SaveData(TagCompound tag) {
        lock (_keys) {
            if (_keys.Count > 0) {
                tag[TAG_KEY] = _keys;
            }
        }
    }

    public override void LoadData(TagCompound tag) {
        lock (_keys) {
            if (tag.TryGet(TAG_KEY, out List<Item> keychain)) {
                _keys = new List<Item>(keychain);
            }
        }

        RefreshKeys();
    }

    public override void NetSend(BinaryWriter writer) {
        lock (_keys) {
            writer.Write(_keys.Count);
            for (int i = 0; i < _keys.Count; i++) {
                ItemIO.Send(_keys[i], writer, writeStack: true);
            }
        }
    }

    public override void NetReceive(BinaryReader reader) {
        lock (_keys) {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++) {
                ItemIO.Receive(reader, readStack: true);
            }
        }

        RefreshKeys();
    }

    bool IRightClickOverrideWhenHovered.RightClickOverrideWhenHovered(ref Item heldItem, Item[] inv, int context, int slot, Player player, AequusPlayer aequus) {
        if (Main.keyState.PressingControl() && Main.mouseRight && Main.mouseRightRelease) {
            foreach (Item key in _keys) {
                Main.LocalPlayer.QuickSpawnItemDirect(Main.LocalPlayer.GetSource_ItemUse(Item), key, key.stack);
            }

            _keys.Clear();
            return true;
        }

        if (heldItem == null || heldItem.IsAir || !ItemDataSet.KeychainData.TryGetValue(heldItem.type, out var value)) {
            return false;
        }

        if (Main.mouseRight && Main.mouseRightRelease && AcceptItem(heldItem)) {
            RefreshKeys();
            SoundEngine.PlaySound(SoundID.Grab);
            return true;
        }

        return false;
    }

    public void OnConsumedInRecipe(Item createdItem, RecipeItemCreationContext context) {
        foreach (Item key in _keys) {
            Main.LocalPlayer.QuickSpawnItemDirect(new EntitySource_Misc("Recipe"), key, key.stack);
        }

        _keys.Clear();
    }

    public record struct KeychainInfo([JsonProperty] bool NonConsumable, [JsonProperty] Color Color);
}
