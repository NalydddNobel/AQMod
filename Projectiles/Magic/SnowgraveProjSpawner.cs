using Aequus.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Magic
{
    public sealed class SnowgraveProjSpawner : ModProjectile
    {
        public override string Texture => Aequus.TextureNone;

        private bool _playedSound;

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 120;
            Projectile.coldDamage = true;
            Projectile.hide = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage()
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
                    AequusHelpers.PlaySound<snowgrave>(Main.player[Projectile.owner].Center);
                }
            }
            Projectile.velocity.Y = 8f;
            if (Main.myPlayer == Projectile.owner)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0f, -28f), ModContent.ProjectileType<SnowgraveProj>(), Projectile.damage / 30, Projectile.knockBack, Projectile.owner);
                Main.projectile[p].localAI[0] = Main.projectile[p].width / 6;
                Main.projectile[p].localAI[0] -= AequusHelpers.Wave(Projectile.timeLeft * 0.2f, 0f, 18f);
            }
        }
    }
}