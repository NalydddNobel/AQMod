using Aequus.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Magic
{
    public class UmystickProj : ModProjectile
    {
        private float _gfxOffY;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if ((int)Projectile.ai[0] == -1)
                return;
            var player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;
            Vector2 rotatedRelativePoint = player.RotatedRelativePoint(player.MountedCenter);
            bool ignoreChannel = false;
            if ((int)Projectile.ai[1] < 0)
            {
                ignoreChannel = true;
                int timer = (int)Projectile.ai[1] % 5;
                if (Projectile.frame < Main.projFrames[Projectile.type] - 1)
                {
                    Projectile.frame++;
                }
                if (timer == 0)
                {
                    _gfxOffY = 14f;
                    SoundHelper.Play(SoundType.Sound, "Umystick/shoot" + Main.rand.Next(3), Projectile.Center, 0.6f);
                    if (Main.myPlayer == Projectile.owner)
                    {
                        var shootPosition = Projectile.Center;
                        if (Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, player.position, player.width, player.height))
                        {
                            shootPosition = player.Center;
                        }
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), shootPosition, Projectile.velocity * 27.5f, ModContent.ProjectileType<UmystickMoon>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
                else
                {
                    _gfxOffY *= 0.94f;
                }
                if (Projectile.ai[1] <= -15)
                {
                    if (Projectile.ai[0] == 0)
                    {
                        Projectile.ai[0]++;
                    }
                    else if (!player.CheckMana(player.inventory[player.selectedItem], pay: true))
                    {
                        Projectile.ai[0] = -1f;
                        return;
                    }
                    Projectile.ai[1] = 24f;
                }
            }
            else
            {
                if (Projectile.frame > 0)
                {
                    Projectile.frame--;
                }
                _gfxOffY *= 0.94f;
            }
            Projectile.ai[1]--;
            if (Main.myPlayer == Projectile.owner)
            {
                if (ignoreChannel || (player.channel && !player.noItems && !player.CCed))
                {
                    var difference = Main.MouseWorld - player.Center;
                    Projectile.rotation = difference.ToRotation();
                    Projectile.velocity = Vector2.Normalize(difference);
                    Projectile.Center = player.Center + Projectile.velocity * 32f;
                }
                else
                {
                    Projectile.ai[0] = -1f;
                    Projectile.timeLeft = player.itemTime;
                }
                Projectile.netUpdate = true;
            }
            player.ChangeDir(Math.Sign(Projectile.velocity.X));
            Projectile.hide = false;
            if (player.itemTime <= 2)
            {
                player.itemTime = 2;
                if (Projectile.ai[1] > 2)
                {
                    player.itemTime = (int)Projectile.ai[1];
                }
            }
            else
            {
                Projectile.friendly = false;
            }
            if (player.itemAnimation <= 2)
            {
                player.itemAnimation = 2;
                if (Projectile.ai[1] > 2)
                {
                    player.itemTime = (int)Projectile.ai[1];
                }
            }
            Projectile.timeLeft = 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            var origin = frame.Size() / 2f;
            var center = Projectile.Center;
            var effects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                effects = SpriteEffects.FlipHorizontally;
            var player = Main.player[Projectile.owner];
            origin.X += Projectile.spriteDirection * 4f;
            center += Projectile.velocity * -_gfxOffY;
            Main.spriteBatch.Draw(texture, center - Main.screenPosition, frame, lightColor, Projectile.rotation, origin, Projectile.scale, effects, 0f);
            return false;
        }
    }
}