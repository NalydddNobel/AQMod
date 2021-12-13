using AQMod.Assets;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Magic
{
    public class Umystick : ModProjectile
    {
        private float _gfxOffY;

        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.magic = true;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if ((int)projectile.ai[0] == -1)
                return;
            var player = Main.player[projectile.owner];
            Vector2 rotatedRelativePoint = player.RotatedRelativePoint(player.MountedCenter);
            bool ignoreChannel = false;
            if ((int)projectile.ai[1] < 0)
            {
                ignoreChannel = true;
                int timer = (int)projectile.ai[1] % 5;
                if (projectile.frame < Main.projFrames[projectile.type] - 1)
                {
                    projectile.frame++;
                }
                if (timer == 0)
                {
                    _gfxOffY = 14f;
                    if (Main.myPlayer == projectile.owner)
                    {
                        var shootPosition = projectile.Center;
                        if (Collision.CanHitLine(projectile.position, projectile.width, projectile.height, player.position, player.width, player.height))
                        {
                            shootPosition = player.Center;
                        }
                        AQSound.Play(SoundType.Item, AQSound.Paths.MysticUmbrellaShoot, 0.45f, 0.4f);
                        Projectile.NewProjectile(shootPosition, projectile.velocity * 27.5f, ModContent.ProjectileType<UmystickMoon>(), projectile.damage, projectile.knockBack, projectile.owner);
                    }
                }
                else
                {
                    _gfxOffY *= 0.94f;
                }
                if (projectile.ai[1] <= -15)
                {
                    if (projectile.ai[0] == 0)
                    {
                        projectile.ai[0]++;
                    }
                    else if (!player.CheckMana(player.inventory[player.selectedItem], pay: true))
                    {
                        projectile.ai[0] = -1f;
                        return;
                    }
                    projectile.ai[1] = 24f;
                }
            }
            else
            {
                if (projectile.frame > 0)
                {
                    projectile.frame--;
                }
                _gfxOffY *= 0.94f;
            }
            projectile.ai[1]--;
            if (Main.myPlayer == projectile.owner)
            {
                if (ignoreChannel || (player.channel && !player.noItems && !player.CCed))
                {
                    AQProjectile.UpdateHeldProjDoVelocity(player, rotatedRelativePoint, projectile);
                }
                else
                {
                    projectile.ai[0] = -1f;
                    projectile.timeLeft = player.itemTime;
                    projectile.netUpdate = true;
                }
            }
            projectile.hide = false;
            if (player.itemTime <= 2)
            {
                player.itemTime = 2;
                if (projectile.ai[1] > 2)
                {
                    player.itemTime = (int)projectile.ai[1];
                }
            }
            else
            {
                projectile.friendly = false;
            }
            if (player.itemAnimation <= 2)
            {
                player.itemAnimation = 2;
                if (projectile.ai[1] > 2)
                {
                    player.itemTime = (int)projectile.ai[1];
                }
            }
            projectile.timeLeft = 2;
            AQProjectile.UpdateHeldProj(player, rotatedRelativePoint, 36f, projectile);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = this.GetTexture();
            var frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            var origin = frame.Size() / 2f;
            var center = projectile.Center;
            var effects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                effects = SpriteEffects.FlipHorizontally;
            var player = Main.player[projectile.owner];
            origin.X += projectile.spriteDirection * 4f;
            center += projectile.velocity * -_gfxOffY;
            Main.spriteBatch.Draw(texture, center - Main.screenPosition, frame, lightColor, projectile.rotation, origin, projectile.scale, effects, 0f);
            return false;
        }
    }
}