using Aequus.Buffs.Necro;
using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Projectiles.Summon.Necro
{
    public class EnthrallingBolt : ZombieBolt
    {
        public override string Texture => Helper.GetPath<ZombieBolt>();

        public override float Tier => 100f;

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(100, 225, 255, 255 - Projectile.alpha);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            NecromancyDebuff.ReduceDamageForDebuffApplication<EnthrallingDebuff>(Tier, target, ref damage);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            NecromancyDebuff.ApplyDebuff<EnthrallingDebuff>(target, 3600, Projectile.owner);
        }
    }
}