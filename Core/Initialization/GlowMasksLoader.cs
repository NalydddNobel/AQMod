using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent;

namespace Aequus.Core.Initialization;

public sealed class GlowMasksLoader : GlobalItem {
    internal static readonly Dictionary<String, Int16> PathToGlowMaskId = new();
    internal static readonly Dictionary<Int32, Int16> ItemIdToGlowMaskId = new();

    public static Boolean TryGetId(String texture, out Int16 id) {
        id = -1;
        if (ItemIdToGlowMaskId != null && PathToGlowMaskId.TryGetValue(texture, out var index)) {
            id = index;
            return true;
        }
        return false;
    }

    public static Int16 GetId(Int32 itemID) {
        return ItemIdToGlowMaskId != null && ItemIdToGlowMaskId.TryGetValue(itemID, out var index) ? index : (Int16)(- 1);
    }

    public static Int16 GetId(String texture) {
        return PathToGlowMaskId != null && PathToGlowMaskId.TryGetValue(texture, out var index) ? index : (Int16)(-1);
    }

    internal static Int16 AddGlowmask(String texture) {
        var customTexture = ModContent.Request<Texture2D>(texture, AssetRequestMode.ImmediateLoad);
        customTexture.Value.Name = texture;

        Int16 glowmaskId = (Int16)TextureAssets.GlowMask.Length;

        PathToGlowMaskId.Add(texture, glowmaskId);

        Array.Resize(ref TextureAssets.GlowMask, glowmaskId + 1);
        TextureAssets.GlowMask[glowmaskId] = customTexture;

        return glowmaskId;
    }

    public override void Unload() {
        if (TextureAssets.GlowMask != null) {
            TextureAssets.GlowMask = TextureAssets.GlowMask.Where(delegate (Asset<Texture2D> tex) {
                Boolean? obj;
                if (tex == null) {
                    obj = null;
                }
                else {
                    String name = tex.Name;
                    obj = name != null ? new Boolean?(!name.StartsWith("Aequus/")) : null;
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

        Int16 id = GetId(item.type);
        if (id > 0) {
            item.glowMask = id;
        }
    }
}