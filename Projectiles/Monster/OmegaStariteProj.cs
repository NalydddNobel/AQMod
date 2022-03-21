using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Monster
{
    public class OmegaStariteProj : ModProjectile
    {
        public override string Texture => AQMod.TextureNone;

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.tileCollide = false;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.hide = true;
        }

        public override bool CanDamage()
        {
            return projectile.ai[1] > 0;
        }

        public override void AI()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                var omegaStarite = Main.npc[(int)projectile.ai[0]];
                if (!omegaStarite.active || omegaStarite.ai[0] == -1f || !(omegaStarite.modNPC is NPCs.Bosses.OmegaStarite))
                    return;
                projectile.ai[1] = 1f;
                projectile.timeLeft = 32;
                projectile.Center = omegaStarite.Center;
            }
            else
            {
                projectile.ai[1] = 1f;
                var omegaStarite = Main.npc[(int)projectile.ai[0]];
                if (!omegaStarite.active || omegaStarite.ai[0] == -1f)
                    return;
                projectile.timeLeft = 2;
                projectile.Center = omegaStarite.Center;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            var omegaStarite = (NPCs.Bosses.OmegaStarite)Main.npc[(int)projectile.ai[0]].modNPC;
            for (int i = 0; i < omegaStarite.rings.Length; i++)
            {
                for (int j = 0; j < omegaStarite.rings[i].amountOfSegments; j++)
                {
                    if (omegaStarite.rings[i].CachedHitboxes[j].Intersects(targetHitbox))
                        return true;
                }
            }
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.townNPC || target.life < 5)
                damage = (int)(damage * 0.1f);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            Main.npc[(int)projectile.ai[0]].modNPC.OnHitPlayer(target, damage, crit); // janky magic :trollface:
        }
    }
}