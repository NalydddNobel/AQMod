using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Core.Autoloading;

public sealed class GlowMasksLoader : GlobalItem {
    internal static readonly Dictionary<string, short> PathToGlowMaskId = new();
    internal static readonly Dictionary<int, short> ItemIdToGlowMaskId = new();

    public static bool TryGetId(string texture, out short id) {
        id = -1;
        if (ItemIdToGlowMaskId != null && PathToGlowMaskId.TryGetValue(texture, out var index)) {
            id = index;
            return true;
        }
        return false;
    }

    public static short GetId(int itemID) {
        return ItemIdToGlowMaskId != null && ItemIdToGlowMaskId.TryGetValue(itemID, out var index) ? index : (short)-1;
    }

    public static short GetId(string texture) {
        return PathToGlowMaskId != null && PathToGlowMaskId.TryGetValue(texture, out var index) ? index : (short)-1;
    }

    internal static short AddGlowmask(string texture) {
        var customTexture = ModContent.Request<Texture2D>(texture, AssetRequestMode.ImmediateLoad);
        customTexture.Value.Name = texture;

        short glowmaskId = (short)TextureAssets.GlowMask.Length;

        PathToGlowMaskId.Add(texture, glowmaskId);

        Array.Resize(ref TextureAssets.GlowMask, glowmaskId + 1);
        TextureAssets.GlowMask[glowmaskId] = customTexture;

        return glowmaskId;
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
        PathToGlowMaskId.Clear();
        ItemIdToGlowMaskId.Clear();
    }

    public override void SetDefaults(Item item) {
        if (item.type < ItemID.Count) {
            return;
        }

        short id = GetId(item.type);
        if (id > 0) {
            item.glowMask = id;
        }
    }
}