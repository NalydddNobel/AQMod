using Aequus.Graphics;
using Aequus.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc
{
    public class PhysicsGunProj : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public void PlaceTile()
        {
            var tileCoords = Projectile.Center.ToTileCoordinates();
            if (!Main.tile[tileCoords].HasTile || WorldGen.CanKillTile(tileCoords.X, tileCoords.Y))
            {
                WorldGen.KillTile(tileCoords.X, tileCoords.Y);
                if (!WorldGen.PlaceTile(tileCoords.X, tileCoords.Y, (int)Projectile.ai[0], forced: true))
                    SoundEngine.PlaySound(SoundID.Dig, tileCoords.ToWorldCoordinates());
                Main.tile[tileCoords].Active(true);
                Main.tile[tileCoords].TileType = (ushort)Projectile.ai[0];
                WorldGen.SquareTileFrame(tileCoords.X, tileCoords.Y, resetFrame: true);
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendTileSquare(-1, tileCoords.X, tileCoords.Y);
                }
            }
        }

        public override void AI()
        {
            var player = Main.player[Projectile.owner];
            if ((int)Projectile.ai[1] == 3)
            {
                Projectile.velocity.Y += 0.03f;
                Projectile.rotation += Projectile.direction * ((0.1f + Projectile.velocity.Length() * 0.02f) / (1 + Projectile.extraUpdates));
                TileLight();
                return;
            }
            else
            {
                if (!player.channel || !player.controlUseItem)
                {
                    if (Projectile.ai[1] == 0)
                    {
                        Projectile.Kill();
                    }
                    else
                    {
                        Projectile.tileCollide = true;
                        Projectile.timeLeft = 2400;
                        Projectile.extraUpdates = 3;
                        Projectile.velocity *= 0.5f;
                        if (Projectile.velocity.Length() > 24f)
                        {
                            Projectile.velocity.Normalize();
                            Projectile.velocity *= 24f;
                        }
                        Projectile.velocity /= (1 + Projectile.extraUpdates);
                        Projectile.ai[1] = 3f;
                    }
                    return;
                }
            }
            Main.player[Projectile.owner].itemTime = 2;
            Main.player[Projectile.owner].itemAnimation = 2;
            Projectile.timeLeft = 2;

            if ((int)Projectile.ai[1] == 2)
            {
                Projectile.tileCollide = true;
                if (Main.myPlayer == Projectile.owner)
                {
                    var mouseWorld = Main.MouseWorld;
                    player.LimitPointToPlayerReachableArea(ref mouseWorld);
                    var difference = mouseWorld - Projectile.Center;
                    Projectile.velocity = difference * 0.3f;
                    if (Main.mouseRight && Main.mouseRightRelease)
                    {
                        PlaceTile();
                        Projectile.Kill();
                        SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
                    }
                }
                Projectile.rotation += player.direction * 0.1f;
                GunLight();
                TileLight();
                return;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                var oldVelocity = Projectile.velocity;
                Projectile.velocity = Vector2.Normalize(Main.MouseWorld - Main.player[Projectile.owner].Center);
                if (oldVelocity.X != Projectile.velocity.X || oldVelocity.Y != Projectile.velocity.Y)
                    Projectile.netUpdate = true;
                Projectile.Center = Main.player[Projectile.owner].Center;
            }

            var checkCoords = Projectile.Center;
            for (int i = 0; i < 200; i++)
            {
                var tileCoords = checkCoords.ToTileCoordinates();
                bool inWorld = WorldGen.InWorld(tileCoords.X, tileCoords.Y, 10);
                if (inWorld)
                {
                    if (Main.tile[tileCoords].IsFullySolid() && !Main.tileFrameImportant[Main.tile[tileCoords].TileType])
                    {
                        if (!Main.player[Projectile.owner].HasEnoughPickPowerToHurtTile(tileCoords.X, tileCoords.Y) || !WorldGen.CanKillTile(tileCoords.X, tileCoords.Y))
                        {
                            break;
                        }
                        Projectile.ai[0] = Main.tile[tileCoords].TileType;
                        Projectile.ai[1] = 2f;
                        WorldGen.KillTile(tileCoords.X, tileCoords.Y, noItem: true);
                        break;
                    }
                }
                checkCoords += Projectile.velocity * 4f;
            }
            Projectile.Center = checkCoords;
        }

        public void GunLight()
        {
            var beamColor = AequusHelpers.HueShift(Main.player[Projectile.owner].Aequus().SyncedMouseColor, AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 50f, -0.02f, 0.02f));
            Lighting.AddLight(Projectile.Center, beamColor.ToVector3() * 0.66f);
        }
        public void TileLight()
        {
            // TODO: make glowing tiles.. glow
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            return true;
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if ((int)Projectile.ai[1] == 3)
            {
                var tileCoords = Projectile.Center.ToTileCoordinates();
                if (WorldGen.InWorld(tileCoords.X, tileCoords.Y, 10))
                {
                    PlaceTile();
                    Projectile.Kill();
                    return true;
                }
            }
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return (int)Projectile.ai[1] != 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var beamColor = AequusHelpers.HueShift(Main.player[Projectile.owner].Aequus().SyncedMouseColor, AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 50f, -0.02f, 0.02f));
            if ((int)Projectile.ai[1] != 3)
            {
                var prim = new TrailRenderer(TextureCache.Trail[2].Value, TrailRenderer.DefaultPass, (p) => new Vector2(4f), (p) => beamColor.UseA(60),
                drawOffset: Vector2.Zero);

                var list = new List<Vector2>
                {
                    Main.player[Projectile.owner].Center,
                    Projectile.Center
                };
                prim.Draw(list.ToArray());
            }

            if ((int)Projectile.ai[1] >= 2)
            {
                Main.instance.LoadTiles((int)Projectile.ai[0]);
                var t = TextureAssets.Tile[(int)Projectile.ai[0]].Value;
                var frame = new Rectangle(162, 54, 16, 16);
                var origin = frame.Size() / 2f;
                var drawColor = lightColor.MaxRGBA(128);

                var drawCoords = Projectile.Center - Main.screenPosition;

                if ((int)Projectile.ai[1] == 2)
                {
                    Main.spriteBatch.End();
                    Begin.GeneralEntities.BeginShader(Main.spriteBatch);
                    var s = GameShaders.Armor.GetSecondaryShader(ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex, Main.LocalPlayer);
                    var dd = new DrawData(t, Projectile.Center - Main.screenPosition, frame, beamColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
                    foreach (var c in AequusHelpers.CircularVector(4))
                    {
                        dd.position = drawCoords + c * 2f;
                        s.Apply(null, dd);
                        dd.Draw(Main.spriteBatch);
                    }

                    Main.spriteBatch.End();
                    Begin.GeneralEntities.Begin(Main.spriteBatch);
                }
                Main.spriteBatch.Draw(t, drawCoords, frame, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}