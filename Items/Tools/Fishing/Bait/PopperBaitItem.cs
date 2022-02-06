using AQMod.Content.Players;
using AQMod.Dusts;
using AQMod.Effects.WorldEffects;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Items.Tools.Fishing.Bait
{
    public abstract class PopperBaitItem : ModItem
    {
        public abstract int GetExtraFishingPower(Player player, PlayerFishing fishing);
        public virtual void OnEnterWater(Player player, PlayerFishing fishing, Projectile bobber, Tile tile)
        {
            AQMod.WorldEffects.Add(new FishingPopperEffect((int)bobber.position.X, (int)bobber.position.Y, tile.liquid, ModContent.DustType<MonoDust>(), new Color(255, 220, 20, 175)));
        }
        public virtual void OnCatchEffect(Player player, PlayerFishing fishing, Projectile bobber, Tile tile)
        {
        }
    }
}