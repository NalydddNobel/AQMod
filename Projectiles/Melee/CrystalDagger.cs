using AQMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class CrystalDagger : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = 19;
            projectile.penetrate = -1;
            projectile.alpha = 0;
            projectile.hide = true;
            projectile.ownerHitCheck = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 8;
            projectile.manualDirectionChange = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            var player = Main.player[projectile.owner];
            float speedMultiplier = 1f + (1f - player.meleeSpeed);
            if (projectile.localAI[0] == 0f)
                projectile.localAI[0] = 200f * speedMultiplier;
            var playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            projectile.direction = player.direction;
            player.heldProj = projectile.whoAmI;
            player.itemTime = player.itemAnimation;
            projectile.position.X = playerCenter.X - projectile.width / 2;
            projectile.position.Y = playerCenter.Y - projectile.height / 2;
            float lerpAmount = MathHelper.Clamp(1f - speedMultiplier, 0.1f, 1f);
            if (!player.frozen && !player.stoned)
            {
                if ((int)projectile.ai[0] == 0)
                {
                    projectile.ai[0] = 25f;
                    projectile.ai[1] = Main.rand.NextFloat(-0.3f, 0.3f);
                    if (Main.myPlayer == player.whoAmI && lerpAmount > 0f)
                        projectile.velocity = Vector2.Normalize(Main.MouseWorld - playerCenter).RotatedBy(projectile.ai[1]) * projectile.ai[0];
                    projectile.netUpdate = true;
                }
                if (player.itemAnimation < player.itemAnimationMax / 3f)
                {
                    projectile.ai[0] = MathHelper.Lerp(projectile.ai[0], 0f, MathHelper.Clamp(lerpAmount + 0.75f, 0.8f, 1f));
                }
                else
                {
                    projectile.ai[0] = MathHelper.Lerp(projectile.ai[0], projectile.localAI[0], MathHelper.Clamp(lerpAmount + 0.55f, 0.8f, 1f));
                }
            }
            if (player.itemAnimation == 0)
                projectile.Kill();
            if (Main.myPlayer == player.whoAmI && lerpAmount > 0f)
                projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(Main.MouseWorld - playerCenter).RotatedBy(projectile.ai[1]) * projectile.ai[0], lerpAmount * 0.4f);
            projectile.direction = projectile.velocity.X <= 0f ? -1 : 1;
            projectile.spriteDirection = projectile.direction;
            projectile.rotation = projectile.velocity.ToRotation() + (MathHelper.PiOver4 * 3f);
            if (projectile.spriteDirection == -1)
                projectile.rotation += -MathHelper.PiOver2;
            player.ChangeDir(projectile.direction);
            int d = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(220, 220, 220, 80), 0.8f);
            Main.dust[d].velocity *= 0.5f;
        }
    }
}
