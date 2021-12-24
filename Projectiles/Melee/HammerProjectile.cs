using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public abstract class HammerProjectile : ModProjectile
    {
        protected abstract int Size { get; }
        protected abstract float LengthFromPlayer { get; }
        protected virtual float UseTime => 10f;
        protected virtual float EndTimeMultiplier => 2f;
        protected virtual int EndTimePullback => 4;
        protected virtual Vector2 spriteOrigin => new Vector2(Main.projectileTexture[projectile.type].Width / 2f, 20f);
        protected virtual Vector2 ModifyPlayerOffset(Vector2 offset)
        {
            return offset;
        }

        protected virtual void OnImpactGround()
        {
            SoundID.Item14.Play(projectile.Center);
        }

        public override void SetDefaults()
        {
            projectile.width = Size;
            projectile.height = Size;
            projectile.melee = true;
            projectile.friendly = true;
            projectile.ownerHitCheck = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.manualDirectionChange = true;
            projectile.ignoreWater = true;
            projectile.hide = true;
            projectile.netImportant = true;
        }

        public override void AI()
        {
            projectile.timeLeft = 60;
            Main.player[projectile.owner].heldProj = projectile.whoAmI;
            projectile.direction = Main.player[projectile.owner].direction;
            projectile.spriteDirection = projectile.direction;
            if (Main.myPlayer == projectile.owner)
            {
                var playerOffset = new Vector2(0f, -LengthFromPlayer);
                if ((int)projectile.ai[0] == 0)
                {
                    float scale = Main.player[projectile.owner].HeldItem.scale;
                    projectile.scale *= scale;
                    var center = projectile.Center;
                    projectile.width = (int)(projectile.width * scale);
                    projectile.height = (int)(projectile.height * scale);
                    projectile.Center = center;
                }
                projectile.ai[0]++;
                int slowSwingTime = (int)(UseTime * Main.player[projectile.owner].meleeSpeed);
                if ((int)projectile.ai[0] > slowSwingTime)
                {
                    Main.player[projectile.owner].itemTime = 2;
                    Main.player[projectile.owner].itemAnimation = 2;
                    float time = projectile.ai[0];
                    if ((int)projectile.ai[1] == 0)
                    {
                        if (Collision.SolidCollision(projectile.position, projectile.width, projectile.height + 4))
                        {
                            projectile.ai[1] = projectile.ai[0];
                            OnImpactGround();
                        }
                        else
                        {
                            projectile.ai[1] -= 0.25f;
                        }
                    }
                    else if ((int)projectile.ai[1] != -1)
                    {
                        time = projectile.ai[1];
                    }
                    int endTime = (int)(slowSwingTime * EndTimeMultiplier);
                    bool updateOffset = true;
                    if (Main.player[projectile.owner].channel)
                    {
                        if ((int)projectile.ai[0] > endTime - EndTimePullback)
                        {
                            updateOffset = false;
                            playerOffset = playerOffset.RotatedBy(MathHelper.Lerp(0f, MathHelper.PiOver2 * 1.5f - 0.1f, (EndTimePullback - ((int)projectile.ai[0] - endTime)) / (float)(EndTimePullback * EndTimePullback)));
                        }
                        if ((int)projectile.ai[0] > endTime)
                        {
                            projectile.ai[0] = 1f;
                            projectile.ai[1] = 0f;
                            if (Main.player[projectile.owner].HeldItem.UseSound != null)
                                Main.player[projectile.owner].HeldItem.UseSound.Play(Main.player[projectile.owner].Center);
                            Main.player[projectile.owner].itemTime = PlayerHooks.TotalUseTime(Main.player[projectile.owner].HeldItem.useTime, Main.player[projectile.owner], Main.player[projectile.owner].HeldItem);
                            Main.player[projectile.owner].itemAnimation = Main.player[projectile.owner].itemTime;
                        }
                    }
                    else if ((int)projectile.ai[0] > endTime)
                    {
                        projectile.Kill();
                    }
                    if (updateOffset)
                        playerOffset = playerOffset.RotatedBy(MathHelper.Lerp(MathHelper.PiOver2, MathHelper.PiOver2 * 1.5f - 0.1f, (time - UseTime) / (slowSwingTime + (time - UseTime) / EndTimeMultiplier)));
                }
                else
                {
                    playerOffset = playerOffset.RotatedBy(MathHelper.Lerp(0f, MathHelper.PiOver2, projectile.ai[0] / slowSwingTime));
                }
                playerOffset.X *= projectile.direction;
                playerOffset *= projectile.scale;
                ModifyPlayerOffset(playerOffset);
                projectile.Center = Main.player[projectile.owner].MountedCenter + playerOffset;
                projectile.netUpdate = true;
            }
            projectile.rotation = (projectile.Center - Main.player[projectile.owner].MountedCenter).ToRotation();
            if (projectile.direction == -1)
                projectile.rotation += MathHelper.PiOver4 * 3f;
            else
            {
                projectile.rotation += MathHelper.PiOver4;
            }
            projectile.hide = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            var effects = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            var origin = spriteOrigin;
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, origin, projectile.scale, effects, 0f);
            return false;
        }
    }
}