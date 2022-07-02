using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Projectiles.Monster
{
    public class HyperStariteProj : EnemyAttachedProjBase
    {
        public override string Texture => Aequus.BlankTexture;

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.hide = true;
        }

        protected override bool CheckAttachmentConditions(NPC npc)
        {
            return (int)npc.ai[0] != -1 && npc.ModNPC is NPCs.Boss.OmegaStarite;
        }

        protected override void AIAttached(NPC npc)
        {
            Projectile.position += (npc.rotation + MathHelper.TwoPi / 5f * Projectile.ai[1] - MathHelper.PiOver2).ToRotationVector2() * (npc.height * npc.scale + npc.ai[3] + 18f);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.townNPC || target.life < 5)
                damage = (int)(damage * 0.1f);
        }
    }
}