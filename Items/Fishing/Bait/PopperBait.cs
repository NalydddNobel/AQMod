using AQMod.Common;
using AQMod.Common.WorldEffects;
using AQMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Items.Fishing.Bait
{
    public abstract class PopperBait : ModItem
    {
        public abstract int GetExtraFishingPower(Player player, AQPlayer aQPlayer);
        public virtual void PopperEffects(Player player, AQPlayer aQPlayer, Projectile bobber, Tile tile)
        {
            SpriteUtils.WorldEffects.Add(new FishingPopperEffect((int)bobber.position.X, (int)bobber.position.Y, tile.liquid, ModContent.DustType<MonoDust>(), new Color(255, 220, 20, 175)));
        }
    }
}