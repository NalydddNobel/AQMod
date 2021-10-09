using AQMod.Common;
using AQMod.Content.Dusts;
using AQMod.Effects.WorldEffects;
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
            AQMod.WorldEffects.Add(new FishingPopperEffect((int)bobber.position.X, (int)bobber.position.Y, tile.liquid, ModContent.DustType<MonoDust>(), new Color(255, 220, 20, 175)));
        }
        public virtual void OnCatchEffect(Player player, AQPlayer aQPlayer, Projectile bobber, Tile tile)
        {
        }
    }
}