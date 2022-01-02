using Microsoft.Xna.Framework;
using Terraria.ID;

namespace AQMod.Projectiles.FallingStars
{
    public sealed class LifeCrystalFallingStar : FallingStarLikeProjectile
    {
        public override string Texture => "Terraria/Item_" + ItemID.LifeCrystal;
        protected override Color SparkleDustColor => new Color(255, 0, 80, 255);
        protected override Color SparkleDustColorGold => new Color(255, 200, 230, 255);
        protected override Color TrailColor => SparkleDustColor * 0.2f;
        protected override int DroppedItem => ItemID.LifeCrystal;
    }
}