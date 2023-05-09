using Aequus.Projectiles.Monster;
using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Content.NPCs.Boss.OmegaStarite.Projectiles {
    public class OmegaStariteProj : EnemyAttachedProjBase {
        public const float HurtRadius = 84.85f / 2f;

        public override void SetDefaults() {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
        }

        public override bool? CanDamage() {
            return Projectile.ai[1] > 0;
        }

        protected override bool CheckAttachmentConditions(NPC npc) {
            return (int)npc.ai[0] != OmegaStarite.ACTION_DEAD && npc.ModNPC is OmegaStarite;
        }

        public override void AI() {
            base.AI();
            Projectile.ai[1] = 1f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            var starite = (OmegaStarite)Main.npc[AttachedNPC].ModNPC;
            for (int i = 0; i < starite.rings.Count; i++) {
                for (int j = 0; j < starite.rings[i].amountOfSegments; j++) {
                    if (starite.rings[i].CachedHitboxes[j].Intersects(targetHitbox)) {
                        return Helper.IsRectangleCollidingWithCircle(starite.rings[i].CachedHitboxes[j].Center.ToVector2(), HurtRadius, targetHitbox);
                    }
                }
            }
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
            if (target.townNPC || target.life < 5) {
                modifiers.SetMaxDamage(1);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) {
            Main.npc[AttachedNPC].ModNPC.OnHitPlayer(target, info); // janky magic :trollface:
        }
    }
}