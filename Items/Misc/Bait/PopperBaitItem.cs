using AQMod.Content.Players;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Items.Misc.Bait
{
    public abstract class PopperBaitItem : ModItem
    {
        public abstract int GetExtraFishingPower(Player player, PlayerFishing fishing);
        public virtual void OnEnterWater(Player player, PlayerFishing fishing, Projectile bobber, Tile tile)
        {
        }
        public virtual void OnCatchEffect(Player player, PlayerFishing fishing, Projectile bobber, Tile tile)
        {
        }
    }
}