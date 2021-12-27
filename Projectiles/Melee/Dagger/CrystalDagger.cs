using AQMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee.Dagger
{
    public class CrystalDagger : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = 19;
            Projectile.penetrate = -1;
            Projectile.alpha = 0;
            Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
            Projectile.manualDirectionChange = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            var player = Main.player[Projectile.owner];
            float speedMultiplier = 1f + (1f - player.meleeSpeed);
            if (Projectile.localAI[0] == 0f)
                Projectile.localAI[0] = 200f * speedMultiplier;
            var playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            Projectile.direction = player.direction;
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation;
            Projectile.position.X = playerCenter.X - Projectile.width / 2;
            Projectile.position.Y = playerCenter.Y - Projectile.height / 2;
            float lerpAmount = MathHelper.Clamp(1f - speedMultiplier, 0.1f, 1f);
            if (!player.frozen && !player.stoned)
            {
                if ((int)Projectile.ai[0] == 0)
                {
                    Projectile.ai[0] = 25f;
                    Projectile.ai[1] = Main.rand.NextFloat(-0.3f, 0.3f);
                    if (Main.myPlayer == player.whoAmI && lerpAmount > 0f)
                        Projectile.velocity = Vector2.Normalize(Main.MouseWorld - playerCenter).RotatedBy(Projectile.ai[1]) * Projectile.ai[0];
                    Projectile.netUpdate = true;
                }
                if (player.itemAnimation < player.itemAnimationMax / 3f)
                {
                    Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], 0f, MathHelper.Clamp(lerpAmount + 0.75f, 0.8f, 1f));
                }
                else
                {
                    Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], Projectile.localAI[0], MathHelper.Clamp(lerpAmount + 0.55f, 0.8f, 1f));
                }
            }
            if (player.itemAnimation == 0)
                Projectile.Kill();
            if (Main.myPlayer == player.whoAmI && lerpAmount > 0f)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Main.MouseWorld - playerCenter).RotatedBy(Projectile.ai[1]) * Projectile.ai[0], lerpAmount * 0.4f);
            Projectile.direction = Projectile.velocity.X <= 0f ? -1 : 1;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 * 3f;
            if (Projectile.spriteDirection == -1)
                Projectile.rotation += -MathHelper.PiOver2;
            player.ChangeDir(Projectile.direction);
            int d = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(220, 220, 220, 80), 0.8f);
            Main.dust[d].velocity *= 0.5f;
        }
    }
}
