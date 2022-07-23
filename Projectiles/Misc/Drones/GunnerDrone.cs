using Aequus.Items.Consumables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.Drones
{
    public class GunnerDrone : TownDroneBase
    {
        public float gotoVelocityX;
        public float gotoVelocityY;
        public int gotoVelocityXResetTimer;
        public int gotoVelocityYResetTimer;

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.npcProj = true;
        }

        public override void AI()
        {
            base.AI();

            int tileHeight = 30;
            int tileX = ((int)Projectile.position.X + Projectile.width / 2) / 16;
            int tileY = ((int)Projectile.position.Y + Projectile.height / 2) / 16;
            for (int i = 0; i < 30; i++)
            {
                if (WorldGen.InWorld(tileX, tileY + i, 10) && AequusHelpers.IsSolid(Main.tile[tileX, tileY + i]))
                {
                    tileHeight = i + 1;
                    break;
                }
            }

            var target = Projectile.FindTargetWithinRange(800f, checkCanHit: false);
            if (target == null)
            {
                int t = Projectile.FindTargetWithLineOfSight(1600f);
                if (t != -1)
                    target = Main.npc[t];
            }

            float targetDistance = 1600f;
            float minDistance = 600f;
            if (target != null)
            {
                if (Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, target.position, target.width, target.height))
                {
                    Projectile.ai[0]++;
                    if (Projectile.ai[0] > 15f)
                    {
                        var shootPosition = Projectile.Center + new Vector2(0f, 12f);
                        if (Main.myPlayer == Projectile.owner)
                        {
                            var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), shootPosition, Vector2.Normalize(target.Center - shootPosition).RotatedBy(Main.rand.NextFloat(-0.04f, 0.04f)) * 10f, ProjectileID.Bullet,
                                Projectile.damage, Projectile.knockBack, Projectile.owner);
                            p.ArmorPenetration += Projectile.damage / 2;
                        }
                        SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
                        Projectile.ai[0] = 0f;
                    }
                }
                else
                {
                    minDistance = 100f;
                }
                targetDistance = Projectile.Distance(target.Center);
            }

            if (tileHeight >= 30)
            {
                gotoVelocityY = 3f;
            }
            else if (tileHeight >= 20)
            {
                gotoVelocityY = 1f;
            }
            else if (tileHeight >= 10)
            {
                gotoVelocityY = 0.5f;
            }
            else if (tileHeight < 2)
            {
                gotoVelocityY = -0.5f;
            }
            else if (target == null || targetDistance < minDistance)
            {
                if (gotoVelocityYResetTimer <= 0)
                {
                    gotoVelocityY = Main.rand.NextFloat(-1.5f, 0.1f);
                    gotoVelocityYResetTimer = Main.rand.Next(20, 300);
                    Projectile.netUpdate = true;
                }
                else
                {
                    gotoVelocityYResetTimer--;
                }
            }

            var pylonWorld = pylonSpot.ToWorldCoordinates();
            float yLerp = 0.02f;
            if (!Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                if (pylonWorld.Y < Projectile.position.Y || Main.rand.NextBool(30))
                {
                    for (int i = 0; i > -4; i--)
                    {
                        if (WorldGen.InWorld(tileX, tileY + i, 10) && AequusHelpers.IsSolid(Main.tile[tileX, tileY + i]))
                        {
                            //AequusHelpers.dustDebug(tileX, tileY + i);
                            gotoVelocityY = Math.Abs(gotoVelocityY);
                            yLerp = 0.125f;
                            Projectile.netUpdate = true;
                            break;
                        }
                    }
                }
            }

            if (target == null || targetDistance < minDistance)
            {
                if (gotoVelocityXResetTimer <= 0)
                {
                    gotoVelocityX = Main.rand.NextFloat(-2f, 2f);
                    gotoVelocityXResetTimer = Main.rand.Next(60, 500);
                    Projectile.netUpdate = true;
                }
                else
                {
                    gotoVelocityXResetTimer--;
                }

                if (Projectile.wet)
                {
                    gotoVelocityY = -gotoVelocityY.Abs();
                }
                var diffFromPylon = pylonSpot.ToWorldCoordinates() - Projectile.Center;
                if (diffFromPylon.Length() > 1000f)
                {
                    var gotoVector = Vector2.Normalize(diffFromPylon) * (float)Math.Sqrt(gotoVelocityX * gotoVelocityX + gotoVelocityY * gotoVelocityY);
                    gotoVelocityX = gotoVector.X;
                    gotoVelocityY = gotoVector.Y;
                }
                Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, gotoVelocityX, 0.02f);
                Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, gotoVelocityY, yLerp);
                if (target != null && Projectile.velocity.Length() > 2f)
                {
                    Projectile.velocity *= 0.95f;
                }
            }
            else
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(target.Center - Projectile.Center + new Vector2(0f, tileHeight < 10 ? -100f : 0f)) * 6f, 0.01f);
            }
            if (target != null)
            {
                Projectile.ai[1] = target.whoAmI + 1;
            }
            else
            {
                Projectile.ai[1] = 0f;
            }

            Projectile.rotation = Projectile.velocity.X * 0.1f;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.CollideWithOthers(0.1f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                gotoVelocityX = -gotoVelocityX;
                Projectile.velocity.X = Projectile.oldVelocity.X * 0.8f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                gotoVelocityY = -gotoVelocityY;
                Projectile.velocity.Y = Projectile.oldVelocity.Y * 0.8f;
            }
            Projectile.netUpdate = true;
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if ((int)Projectile.localAI[0] == 0 && Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextFloat() < 0.8f)
            {
                Item.NewItem(Projectile.GetSource_Death(), Projectile.getRect(), ModContent.ItemType<GunnerDronePack>());
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var texture, out var off, out var frame, out var origin, out int _);
            var gunTexture = ModContent.Request<Texture2D>(Texture + "Gun");

            var color = GetDrawColor();
            var lightingColor = AequusHelpers.GetColor(Projectile.Center, color);
            float turretRotation = AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f, -1f, 1f);
            int npcTarget = (int)Projectile.ai[1] - 1;
            if (npcTarget > -1)
            {
                turretRotation = (Main.npc[npcTarget].Center - Projectile.Center).ToRotation() + MathHelper.PiOver2;
            }
            Main.EntitySpriteDraw(gunTexture.Value, Projectile.position + off +
                (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * texture.Height / 2f - Main.screenPosition + new Vector2(1f, 0f), null, lightingColor,
                turretRotation - MathHelper.PiOver2, new Vector2(gunTexture.Value.Width / 2f, 4f), Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, Projectile.position + off - Main.screenPosition, frame, lightingColor,
                Projectile.rotation, origin, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            return false;
        }
    }
}