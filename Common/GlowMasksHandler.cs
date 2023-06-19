using Aequus;
using Aequus.Items;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common {
    public class GlowMasksHandler : GlobalItem {
        private static Dictionary<string, short> texturePathToGlowMaskID;
        private static Dictionary<int, short> itemIDToGlowMask;

        public static void AddGlowmask(string texturePath) {
            texturePathToGlowMaskID.Add(texturePath, -1);
        }

        public static bool TryGetID(string texture, out short id) {
            id = -1;
            if (itemIDToGlowMask != null && texturePathToGlowMaskID.TryGetValue(texture, out var index)) {
                id = index;
                return true;
            }
            return false;
        }

        public static short GetID(int itemID) {
            if (itemIDToGlowMask != null && itemIDToGlowMask.TryGetValue(itemID, out var index)) {
                return index;
            }
            return -1;
        }
        public static short GetID(string texture) {
            if (texturePathToGlowMaskID != null && texturePathToGlowMaskID.TryGetValue(texture, out var index)) {
                return index;
            }
            return -1;
        }

        public override void Load() {
            if (Main.dedServ)
                return;
            itemIDToGlowMask = new Dictionary<int, short>();
            texturePathToGlowMaskID = new Dictionary<string, short>();
        }

        public override void SetStaticDefaults() {
            // Do not run on a server
            if (Main.dedServ)
                return;

            var masks = TextureAssets.GlowMask.ToList();
            // Misc glowmasks registered manually, giving them IDs for in-game
            foreach (var s in texturePathToGlowMaskID) {
                var customTexture = ModContent.Request<Texture2D>(s.Key, AssetRequestMode.ImmediateLoad);
                customTexture.Value.Name = s.Key;
                texturePathToGlowMaskID[s.Key] = (short)masks.Count;
                masks.Add(customTexture);
            }

            // Check all registered items in the mod
            foreach (var m in Aequus.Instance.GetContent<ModItem>()) {
                // Find GlowMask attribute from registered item
                var attr = m?.GetType().GetAttribute<AutoloadGlowMaskAttribute>();
                if (attr != null) {
                    // Check if this item will be registering multiple glowmasks
                    if (attr.CustomGlowmasks != null) {
                        foreach (var c in attr.CustomGlowmasks) {
                            var customTexture = ModContent.Request<Texture2D>(c, AssetRequestMode.ImmediateLoad);
                            customTexture.Value.Name = c;
                            texturePathToGlowMaskID.Add(c, (short)masks.Count);
                            masks.Add(customTexture);
                        }
                        continue;
                    }

                    // Otherwise, get a _Glow texture using the item's Texture property
                    string modItemTexture = m.Texture;
                    var texture = ModContent.Request<Texture2D>(modItemTexture + "_Glow", AssetRequestMode.ImmediateLoad); // ImmediateLoad so the asset can be given a name
                    texture.Value.Name = modItemTexture;
                    // Add the asset to the TextureAssets.GlowMask edit List
                    masks.Add(texture);
                    texturePathToGlowMaskID.Add(modItemTexture, (short)(masks.Count - 1));

                    // Assign an ItemID for this glowmask to be connected with, so it is automatically applied
                    if (attr.AutoAssignItemID)
                        itemIDToGlowMask.Add(m.Type, (short)(masks.Count - 1));
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
            texturePathToGlowMaskID?.Clear();
            texturePathToGlowMaskID = null;
            itemIDToGlowMask?.Clear();
            itemIDToGlowMask = null;
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
}

namespace Aequus.Items {
    [AttributeUsage(AttributeTargets.Class)]
    internal class AutoloadGlowMaskAttribute : Attribute {
        public readonly string[] CustomGlowmasks;
        public readonly bool AutoAssignItemID;

        public AutoloadGlowMaskAttribute() {
            AutoAssignItemID = true;
            CustomGlowmasks = null;
        }

        public AutoloadGlowMaskAttribute(params string[] glowmasks) {
            AutoAssignItemID = false;
            CustomGlowmasks = glowmasks;
        }
    }
}