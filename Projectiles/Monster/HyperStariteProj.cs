using Aequus.NPCs.Monsters.Event.Glimmer;
using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Projectiles.Monster {
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
            Projectile.penetrate = -1;
        }

        protected override bool CheckAttachmentConditions(NPC npc)
        {
            return (int)npc.ai[0] != -1 && npc.ModNPC is HyperStarite;
        }

        protected override void AIAttached(NPC npc)
        {
            Projectile.position += (npc.rotation + MathHelper.TwoPi / 5f * Projectile.ai[1] - MathHelper.PiOver2).ToRotationVector2() * (npc.height * npc.scale + npc.ai[3] + 50f);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.townNPC || target.life < 5)
                modifiers.SetMaxDamage(1);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Main.npc[AttachedNPC].ModNPC.OnHitPlayer(target, info);
        }
    }
}