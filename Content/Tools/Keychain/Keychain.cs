using Aequus.Common.Items.Components;
using Aequus.Core;
using Aequus.Core.Assets;
using Aequus.DataSets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Chat;
using Terraria.GameInput;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Tools.Keychain;

[WorkInProgress]
public class Keychain : ModItem, IRightClickOverrideWhenHovered, IOnConsumedInRecipe {
    public const int MAX_KEYS_ALLOWED = 32;
    public const int FRAME_COUNT = 6;
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

    public override void SetStaticDefaults() {
        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, FRAME_COUNT));
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

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        if (_keys.Count > 0) {
            string text = "Contains:";

            foreach (Item item in _sortedKeyIcons) {
                int stack = item.stack;
                item.stack = 1;
                text += $"\n{ItemTagHandler.GenerateTag(item)}{item.Name}";
                item.stack = stack;

                if (item.stack > 1) {
                    text += $" ({item.stack})";
                }
            }

            tooltips.AddTooltip(new TooltipLine(Mod, "KeyChain", text));
        }

        string smartSelectButton = PlayerInput.GenerateInputTag_ForCurrentGamemode_WithHacks(tagForGameplay: false, "SmartSelect");
        if (!smartSelectButton.Contains('[')) {
            smartSelectButton = ExtendLanguage.PrettyPrint(smartSelectButton);
        }

        foreach (TooltipLine line in tooltips) {
            if (line.Name.StartsWith("Tooltip")) {
                line.Text = line.Text.Replace("<shift>", smartSelectButton);
            }
        }
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

    public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        if (_sortedKeyIcons == null || _sortedKeyIcons.Count == 0) {
            return;
        }

        spriteBatch.End();
        spriteBatch.BeginUI(immediate: true, useScissorRectangle: true);

        DrawKeys(spriteBatch, position, drawColor, 0f, scale);

        spriteBatch.End();
        spriteBatch.BeginUI(immediate: false, useScissorRectangle: true);
    }

    public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) {
        if (_sortedKeyIcons == null || _sortedKeyIcons.Count == 0) {
            return;
        }

        Main.GetItemDrawFrame(Type, out _, out Rectangle frame);
        Vector2 drawCoordinates = ExtendItem.WorldDrawPos(Item, frame);

        spriteBatch.End();
        spriteBatch.BeginWorld(shader: true);

        DrawKeys(spriteBatch, drawCoordinates, lightColor, rotation, scale);

        spriteBatch.End();
        spriteBatch.BeginWorld(shader: false);
    }

    public void DrawKeys(SpriteBatch spriteBatch, Vector2 drawCoordinates, Color drawColor, float rotation, float scale) {
        Main.GetItemDrawFrame(Type, out Texture2D texture, out _);
        int count = Math.Min(_keys.Count + 1, FRAME_COUNT);

        AequusShaders.LuminentMultiply.Value.CurrentTechnique.Passes[0].Apply();

        drawCoordinates.Y -= 2f * scale;
        for (int i = 1; i < count; i++) {
            Rectangle frame = texture.Frame(verticalFrames: FRAME_COUNT, frameY: i);
            frame.Height -= 2;
            Color keyColor = drawColor;
            if (ItemDataSet.KeychainData.TryGetValue(_sortedKeyIcons[i - 1].type, out KeychainInfo value)) {
                keyColor = Utils.MultiplyRGBA(keyColor, value.Color);
            }
            spriteBatch.Draw(texture, drawCoordinates, frame, keyColor, rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f); ;
        }
    }

    public record struct KeychainInfo([JsonProperty] bool NonConsumable, [JsonProperty] Color Color);
}
