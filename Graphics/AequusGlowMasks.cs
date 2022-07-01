using Aequus.Common;
using Aequus.Items;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Graphics
{
    public class AequusGlowMasks : IPostSetupContent
    {
        private static Dictionary<string, short> texturePathToGlowMask;
        private static Dictionary<int, short> itemIDToGlowMask;

        public static short GetID(int itemID)
        {
            if (itemIDToGlowMask != null && itemIDToGlowMask.TryGetValue(itemID, out var index))
            {
                return index;
            }
            return -1;
        }
        public static short GetID(string texture)
        {
            if (texturePathToGlowMask != null && texturePathToGlowMask.TryGetValue(texture, out var index))
            {
                return index;
            }
            return -1;
        }

        void ILoadable.Load(Mod mod)
        {
        }

        void IPostSetupContent.PostSetupContent(Aequus aequus)
        {
            if (Main.dedServ)
                return;
            itemIDToGlowMask = new Dictionary<int, short>();
            texturePathToGlowMask = new Dictionary<string, short>();
            var masks = TextureAssets.GlowMask.ToList();
            foreach (var m in Aequus.Instance.GetContent<ModItem>())
            {
                if (m?.GetType().GetAttribute<GlowmaskAttribute>() != null)
                {
                    string modItemTexture = m.Texture;
                    var texture = ModContent.Request<Texture2D>(modItemTexture + "_Glow", AssetRequestMode.ImmediateLoad);
                    texture.Value.Name = modItemTexture;
                    masks.Add(texture);
                    texturePathToGlowMask.Add(modItemTexture, (short)(masks.Count - 1));
                    itemIDToGlowMask.Add(m.Type, (short)(masks.Count - 1));
                }
            }
            TextureAssets.GlowMask = masks.ToArray();
        }

        void ILoadable.Unload()
        {
            if (TextureAssets.GlowMask != null)
            {
                TextureAssets.GlowMask = TextureAssets.GlowMask.Where(delegate (Asset<Texture2D> tex)
                {
                    bool? obj;
                    if (tex == null)
                    {
                        obj = null;
                    }
                    else
                    {
                        string name = tex.Name;
                        obj = (name != null) ? new bool?(!name.StartsWith("Aequus/")) : null;
                    }
                    return obj ?? true;
                }).ToArray();
            }
            texturePathToGlowMask?.Clear();
            texturePathToGlowMask = null;
            itemIDToGlowMask?.Clear();
            itemIDToGlowMask = null;
        }
    }
}