using Microsoft.Xna.Framework;
using Terraria.ID;

namespace AQMod.Projectiles.FallingStars
{
    public sealed class ManaCrystalFallingStar : FallingStarLikeProjectile
    {
        public override string Texture => "Terraria/Item_" + ItemID.ManaCrystal;
        protected override Color SparkleDustColorGold => new Color(200, 200, 255, 255);
        protected override int DroppedItem => ItemID.ManaCrystal;
    }
}