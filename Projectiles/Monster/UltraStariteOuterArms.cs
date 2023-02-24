using Aequus.Events.Glimmer.Monsters;
using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Projectiles.Monster
{
    public class UltraStariteOuterArms : EnemyAttachedProjBase
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
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage()
        {
            return (int)Main.npc[AttachedNPC].ai[0] != UltraStarite.STATE_DEATHRAY && (int)Main.npc[AttachedNPC].ai[0] != 5;
        }

        protected override bool CheckAttachmentConditions(NPC npc)
        {
            return (int)npc.ai[0] != -1 && npc.ModNPC is UltraStarite;
        }

        protected override void AIAttached(NPC npc)
        {
            Projectile.position += (npc.rotation + MathHelper.TwoPi / 5f * Projectile.ai[1] - MathHelper.PiOver2).ToRotationVector2() * (npc.height * npc.scale + npc.ai[3] + 130f);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.townNPC || target.life < 5)
                damage = (int)(damage * 0.1f);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            Main.npc[AttachedNPC].ModNPC.OnHitPlayer(target, damage, crit); // janky magic :trollface:
        }
    }
}