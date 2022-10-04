using Aequus.Content.DronePylons;
using Aequus.Items.Consumables.Drones;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.Drones
{
    public class CleanserDrone : TownDroneBase
    {
        public override int ItemDrop => ModContent.ItemType<InactivePylonCleanser>();

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
            else
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
                            gotoVelocityY = Math.Abs(gotoVelocityY);
                            yLerp = 0.125f;
                            Projectile.netUpdate = true;
                            break;
                        }
                    }
                }
            }

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

            Projectile.rotation = Projectile.velocity.X * 0.1f;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.CollideWithOthers(0.1f);

            Projectile.ai[1]++;
            if (Projectile.ai[1] < 90f)
                return;

            for (int i = 0; i < 100; i++)
            {
                var p = FindConvertibleTile();
                if (p == Point.Zero || Main.myPlayer != Projectile.owner)
                    return;

                Projectile.ai[1] = 0f;
                int pylonStyle = (int)Projectile.localAI[1] - 1;
                int tileType = Main.tile[p].TileType;
                int solution = 0;
                foreach (var drones in PylonManager.ActiveDrones)
                {
                    if (drones is CleanserDroneSlot cleanserSlot)
                    {
                        solution = cleanserSlot.GetSolutionProjectileID(tileType);
                        if (solution > 0)
                            break;
                    }
                }

                if (solution > 0)
                {
                    var spawnPosition = Projectile.Center;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), spawnPosition, Vector2.Normalize(p.ToWorldCoordinates() + new Vector2(8f) - spawnPosition) * 7.5f, solution, 0, 0, Projectile.owner);
                    return;
                }
            }
            Projectile.ai[1] = 86f;
        }

        public Point FindConvertibleTile()
        {
            var tilePos = Projectile.Center.ToTileCoordinates();
            return CleanserDroneSlot.FindConvertibleTile(tilePos);
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

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var texture, out var off, out var frame, out var origin, out int _);

            var color = GetPylonColor();
            Main.EntitySpriteDraw(texture, Projectile.position + off - Main.screenPosition, frame, lightColor,
                Projectile.rotation, origin, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, Projectile.position + off - Main.screenPosition, frame, color * SpawnInOpacity,
                Projectile.rotation, origin, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            return false;
        }
    }
}