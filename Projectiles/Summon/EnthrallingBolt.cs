using Aequus.Buffs.Debuffs;
using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Projectiles.Summon
{
    public class EnthrallingBolt : NecromancerBolt
    {
        public override string Texture => AequusHelpers.GetPath<NecromancerBolt>();

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(100, 225, 255, 255 - Projectile.alpha);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            EnthrallingDebuff.ApplyDebuff(target, 3600, Projectile.owner, Projectile.ai[0]);
        }
    }
}