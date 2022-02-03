using AQMod.Assets;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Magic
{
    public sealed class SnowgraveProjectileSpawner : ModProjectile
    {
        public override string Texture => TexturePaths.Empty;

        private bool _playedSound;

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.timeLeft = 120;
            projectile.coldDamage = true;
            projectile.hide = true;
            projectile.tileCollide = false;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                if (!_playedSound)
                {
                    _playedSound = true;
                    AQSound.Play(SoundType.Item, "snowgrave", Main.player[projectile.owner].Center, 0.75f, 0f);
                }
            }
            if (Main.myPlayer == projectile.owner)
            {
                int p = Projectile.NewProjectile(projectile.Center, new Vector2(0f, -28f), ModContent.ProjectileType<SnowgraveProjectile>(), projectile.damage / 30, projectile.knockBack, projectile.owner);
                Main.projectile[p].localAI[0] = Main.projectile[p].width / 6;
                Main.projectile[p].localAI[0] -= AQUtils.Wave(projectile.timeLeft * 0.2f, 0f, 18f);
            }
        }
    }
}