using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Fishing {
    public abstract class FishingPoleItem : ModItem {
        public virtual bool PreDrawFishingLine(Projectile bobber) {
            return true;
        }
        public virtual void ModifyDrawnFishingLine(Projectile bobber, ref Vector2 lineOriginOffset, ref Color lineColor) {
        }
    }
}