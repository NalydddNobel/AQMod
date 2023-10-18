using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Core;

public class GlowMasksHandler : GlobalItem {
    private static readonly Dictionary<string, short> TexturePathToGlowMaskID = new();
    private static readonly Dictionary<int, short> ItemIDToGlowMask = new();

    public static void AddGlowmask(string texturePath) {
        TexturePathToGlowMaskID[texturePath] = -1;
    }

    public static bool TryGetID(string texture, out short id) {
        id = -1;
        if (ItemIDToGlowMask != null && TexturePathToGlowMaskID.TryGetValue(texture, out var index)) {
            id = index;
            return true;
        }
        return false;
    }

    public static short GetID(int itemID) {
        if (ItemIDToGlowMask != null && ItemIDToGlowMask.TryGetValue(itemID, out var index)) {
            return index;
        }
        return -1;
    }
    public static short GetID(string texture) {
        if (TexturePathToGlowMaskID != null && TexturePathToGlowMaskID.TryGetValue(texture, out var index)) {
            return index;
        }
        return -1;
    }

    public override void SetStaticDefaults() {
        // Do not run on a server
        if (Main.dedServ) {
            return;
        }

        var masks = TextureAssets.GlowMask.ToList();
        // Misc glowmasks registered manually, giving them IDs for in-game
        foreach (var s in TexturePathToGlowMaskID) {
            var customTexture = ModContent.Request<Texture2D>(s.Key, AssetRequestMode.ImmediateLoad);
            customTexture.Value.Name = s.Key;
            TexturePathToGlowMaskID[s.Key] = (short)masks.Count;
            masks.Add(customTexture);
        }

        // Check all registered items in the mod
        foreach (var m in Aequus.Instance.GetContent<ModItem>()) {
            // Find GlowMask attribute from registered item
            var attr = m?.GetType().GetAttribute<AutoloadGlowMaskAttribute>();
            if (attr != null) {
                string modItemTexture = m.Texture;
                // Check if this item will be registering multiple glowmasks
                if (attr.CustomGlowmasks != null) {
                    foreach (var customGlowmaskPath in attr.CustomGlowmasks) {
                        string customTexturePath = modItemTexture + customGlowmaskPath;
                        var customTexture = ModContent.Request<Texture2D>(customTexturePath, AssetRequestMode.ImmediateLoad);
                        customTexture.Value.Name = customTexturePath;
                        TexturePathToGlowMaskID.Add(customTexturePath, (short)masks.Count);
                        masks.Add(customTexture);
                    }
                    continue;
                }

                // Otherwise, get a _Glow texture using the item's Texture property
                var texture = ModContent.Request<Texture2D>(modItemTexture + "_Glow", AssetRequestMode.ImmediateLoad); // ImmediateLoad so the asset can be given a name
                texture.Value.Name = modItemTexture;
                // Add the asset to the TextureAssets.GlowMask edit List
                masks.Add(texture);
                TexturePathToGlowMaskID.Add(modItemTexture, (short)(masks.Count - 1));

                // Assign an ItemID for this glowmask to be connected with, so it is automatically applied
                if (attr.AutoAssignItemID)
                    ItemIDToGlowMask.Add(m.Type, (short)(masks.Count - 1));
            }
        }
        // Set to modified array, so Item.glowMask works on it
        TextureAssets.GlowMask = masks.ToArray();
    }

    public override void Unload() {
        if (TextureAssets.GlowMask != null) {
            TextureAssets.GlowMask = TextureAssets.GlowMask.Where(delegate (Asset<Texture2D> tex) {
                bool? obj;
                if (tex == null) {
                    obj = null;
                }
                else {
                    string name = tex.Name;
                    obj = name != null ? new bool?(!name.StartsWith("Aequus/")) : null;
                }
                return obj ?? true;
            }).ToArray();
        }
        TexturePathToGlowMaskID.Clear();
        ItemIDToGlowMask.Clear();
    }

    public override void SetDefaults(Item item) {
        if (item.type >= ItemID.Count) {
            short id = GetID(item.type);
            if (id > 0) {
                item.glowMask = id;
            }
        }
    }
}