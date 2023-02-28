using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.Bobbers
{
    public class NimrodBobber : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BobberWooden);
            DrawOriginOffsetY = 8;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void PostAI()
        {
            if (Projectile.wet)
            {
                Projectile.extraUpdates = 0;
            }
            else
            {
                Projectile.extraUpdates = 1;
            }

            int mainBobber = -1;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].bobber && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].type == ModContent.ProjectileType<NimrodCloudBobber>())
                {
                    mainBobber = i;
                    break;
                }
            }

            Projectile.rotation = 0f;
            if ((int)Projectile.ai[0] < 1)
            {
                if (mainBobber == -1)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.Center = Main.player[Projectile.owner].Center;
                }
            }
            else if ((int)Projectile.ai[0] == 1)
            {
                if (mainBobber != -1)
                {
                    var diff = Main.projectile[mainBobber].Center - Projectile.Center;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(diff).UnNaN() * 12f, 0.1f);
                    Projectile.rotation = diff.ToRotation() + MathHelper.PiOver2;
                    Projectile.CollideWithOthers(2f);
                    (Main.projectile[mainBobber].ModProjectile as NimrodCloudBobber).gotoPosition = new Vector2(-2f);
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
            return true;
        }

        public override bool PreDrawExtras()
        {
            var player = Main.player[Projectile.owner];
            if (!Projectile.bobber || player.inventory[player.selectedItem].holdStyle <= 0)
                return false;
            int cloud = -1;
            if (Projectile.Aequus().HasProjectileOwner)
            {
                cloud = Projectile.Aequus().sourceProj;
            }
            else
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].bobber && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].type == ModContent.ProjectileType<NimrodCloudBobber>())
                    {
                        cloud = i;
                        break;
                    }
                }
            }
            if (cloud != -1)
            {
                Helper.DrawFishingLine(player, Projectile.position, Projectile.width / 2, Projectile.height, Projectile.velocity, Projectile.localAI[0], Main.projectile[cloud].Center + new Vector2(-8f, 0f),
                    getLighting: (v, c) => Color.Lerp(new Color(75, 120, 255, 128) * 0.5f, new Color(100, 235, 255, 180) * 0.8f, Helper.Wave(Main.GlobalTimeWrappedHourly * 5f + cloud, 0f, 1f)));
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Main.spriteBatch.Draw(texture, Projectile.Center + new Vector2(0f, DrawOriginOffsetY) - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class NimrodCloudBobber : ModProjectile
    {
        public override string Texture => Aequus.VanillaTexture + "Projectile_" + ProjectileID.RainCloudRaining;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.RainCloudRaining];
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BobberWooden);
            Projectile.width = 40;
            Projectile.height = 40;
        }

        public Vector2 gotoPosition = new Vector2(0f, 0f);

        public override bool PreAI()
        {
            if (Main.player[Projectile.owner].inventory[Main.player[Projectile.owner].selectedItem].fishingPole == 0 || Main.player[Projectile.owner].CCed || Main.player[Projectile.owner].noItems
               || Main.player[Projectile.owner].pulley || Main.player[Projectile.owner].dead || Projectile.wet)
            {
                Projectile.Kill();
            }

            if ((int)gotoPosition.X == -2f)
            {
                Projectile.tileCollide = false;
                Projectile.frame = 0;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Main.player[Projectile.owner].Center - Projectile.Center) * 12f, 0.6f);
                Projectile.timeLeft = 10;
                if ((Projectile.Center - Main.player[Projectile.owner].Center).Length() < 10f)
                {
                    Projectile.Kill();
                }
                Projectile.rotation += 0.1f;
                return false;
            }

            Projectile.CollideWithOthers(0.15f);

            if ((int)gotoPosition.X == -1f)
            {
                Projectile.frame = 1;
                Projectile.velocity *= 0.9f;
                Projectile.timeLeft = 20;
                if ((Main.player[Projectile.owner].Center - Projectile.Center).Length() > 3000f)
                {
                    Projectile.ai[0] = 1f;
                    gotoPosition.X = -2f;
                    Projectile.netUpdate = true;
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    if ((int)gotoPosition.Y != -20)
                    {
                        if (Projectile.velocity.Length() < 1f)
                        {
                            gotoPosition.Y = -20;
                            SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
                            var s = Projectile.GetSource_FromThis();
                            for (int i = -1; i <= 1; i++)
                            {
                                Projectile.NewProjectile(s, Projectile.Center + new Vector2(20f, 0f), new Vector2(i * 4f, 1f), ModContent.ProjectileType<NimrodBobber>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                            }
                        }
                    }
                    else
                    {
                        bool shouldKill = true;
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            if (Main.projectile[i].active && Main.projectile[i].bobber && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].type == ModContent.ProjectileType<NimrodBobber>())
                            {
                                shouldKill = false;
                            }
                        }
                        if (shouldKill)
                        {
                            Projectile.ai[0] = 1f;
                            gotoPosition.X = -2f;
                            Projectile.netUpdate = true;
                        }
                    }
                    if (Main.player[Projectile.owner].controlUseItem && Main.player[Projectile.owner].releaseUseItem)
                    {
                        Projectile.ai[0] = 1f;
                        gotoPosition.X = -2f;
                        Projectile.netUpdate = true;
                    }
                }
                Projectile.rotation = 0f;
                return false;
            }
            Projectile.timeLeft = 10;
            if (Main.myPlayer == Projectile.owner && gotoPosition == new Vector2(0f, 0f))
            {
                gotoPosition = Main.MouseWorld + new Vector2(0f, -20f);
                Projectile.netUpdate = true;
            }
            if ((Projectile.Center - gotoPosition).Length() < 10f)
            {
                gotoPosition = new Vector2(-1f, -1f);
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
            else
            {
                var rect = Projectile.getRect();
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && i != Projectile.whoAmI && Projectile.type == Main.projectile[i].type && Projectile.owner == Main.projectile[i].owner
                        && Projectile.Colliding(rect, Main.projectile[i].getRect()))
                    {
                        var velocityAddition = Main.projectile[i].DirectionTo(Projectile.Center).UnNaN() * 0.05f;
                        gotoPosition += velocityAddition * 50f;
                        gotoPosition.Y -= 2f;
                        Projectile.velocity += velocityAddition;
                    }
                }
                Projectile.velocity = Vector2.Normalize(gotoPosition - Projectile.Center) * Main.player[Projectile.owner].HeldItem.shootSpeed;
                Projectile.rotation += 0.1f;
            }
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(gotoPosition.X);
            writer.Write(gotoPosition.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            gotoPosition.X = reader.ReadSingle();
            gotoPosition.Y = reader.ReadSingle();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            gotoPosition = new Vector2(-2f, -2f);
            return true;
        }

        public override bool PreDrawExtras()
        {
            var player = Main.player[Projectile.owner];
            if (!Projectile.bobber || player.inventory[player.selectedItem].holdStyle <= 0)
                return false;
            float x = Main.player[Projectile.owner].direction == -1 ? -48f : 44f;
            Helper.DrawFishingLine(player, Projectile.position, Projectile.width / 2, Projectile.height, Projectile.velocity, Projectile.localAI[0], Main.player[Projectile.owner].Center + new Vector2(x, -38f),
                getLighting: (v, c) => Color.Lerp(new Color(30, 60, 200, 128) * 0.5f, new Color(70, 155, 185, 180) * 0.8f, Helper.Wave(-(Main.GlobalTimeWrappedHourly * 5f + Projectile.whoAmI), 0f, 1f)));
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var drawCoordinates = Projectile.Center - Main.screenPosition;
            if (Projectile.frame == 0)
            {
                Main.instance.LoadProjectile(ProjectileID.RainCloudMoving);
                var texture = TextureAssets.Projectile[ProjectileID.RainCloudMoving].Value;
                var frame = texture.Frame(1, 4, 0, (int)Main.GameUpdateCount / 8 % 4);
                Main.spriteBatch.Draw(texture, drawCoordinates, frame, lightColor, Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            }
            else
            {
                var texture = TextureAssets.Projectile[Type].Value;
                var frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, (int)Main.GameUpdateCount / 8 % Main.projFrames[Projectile.type]);
                Main.spriteBatch.Draw(texture, drawCoordinates, frame, lightColor, Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}