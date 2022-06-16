using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon
{
    public abstract class MinionBase : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public Vector2 DefaultIdlePosition()
        {
            return IdlePosition(Main.player[Projectile.owner], Projectile.whoAmI, 0, 1);
        }
        public virtual Vector2 IdlePosition(Player player, int leader, int minionPos, int count)
        {
            return player.Center;
        }

        public override bool? CanCutTiles() => false;

        protected void TurnTo(Vector2 to, float amount)
        {
            TurnTo(to, amount, Projectile.velocity.Length());
        }
        protected void TurnTo(Vector2 to, float amount, float speed)
        {
            Projectile.velocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity, to - Projectile.Center, amount)) * speed;
        }
        protected void SnapTo(Vector2 to, float amount)
        {
            var lerpPosition = Vector2.Lerp(Projectile.Center, to, amount);
            var difference = lerpPosition - Projectile.Center;
            Projectile.velocity = difference;
            if (difference.X < 0f)
            {
                Projectile.direction = -1;
            }
            else
            {
                Projectile.direction = 1;
            }
        }
    }
}
