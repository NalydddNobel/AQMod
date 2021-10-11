using AQMod.Assets.Textures;
using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace AQMod.Common.ItemOverlays
{
    /// <summary>
    /// Same as <see cref="LegacyDynamicGlowmask"/> but also applies the mask in the inventory
    /// </summary>
    public class DynamicInventoryGlowmask : LegacyDynamicGlowmask
    {
        public DynamicInventoryGlowmask(GlowID glowmask, Func<Color> getColor) : base(glowmask, getColor)
        {
        }

        public override void PostDrawInventory(Player player, AQPlayer aQPlayer, Item item, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var texture = DrawUtils.LegacyTextureCache.Glows[glowmask];
            Main.spriteBatch.Draw(texture, position, frame, _getColor(), 0f, origin, scale, SpriteEffects.None, 0f);
        }
    }
}