using Aequus.Buffs.Debuffs.Necro;
using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Projectiles.Summon.Necro
{
    public class EnthrallingBolt : ZombieBolt
    {
        public override string Texture => AequusHelpers.GetPath<ZombieBolt>();

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(100, 225, 255, 255 - Projectile.alpha);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            NecromancyDebuff.ApplyDebuff<EnthrallingDebuff>(target, 3600, Projectile.owner);
        }
    }
}