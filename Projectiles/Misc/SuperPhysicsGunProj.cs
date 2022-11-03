using Aequus.Graphics;
using Aequus.Graphics.Primitives;
using Aequus.Items.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc
{
    public class SuperPhysicsGunProj : PhysicsGunProj
    {
        public override string Texture => Aequus.BlankTexture;

        public Point frameImportant;

        public override void Load()
        {
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public void PlaceTileForced()
        {
            var player = Main.player[Projectile.owner];

            var tileCoords = Projectile.Center.ToTileCoordinates();
            if (!Main.tile[tileCoords].HasTile)
            {
                if (!Main.tile[tileCoords].HasTile)
                {
                    WorldGen.KillTile(tileCoords.X, tileCoords.Y);
                    if (!WorldGen.PlaceTile(tileCoords.X, tileCoords.Y, (int)Projectile.ai[0], forced: true))
                    {
                        SoundEngine.PlaySound(SoundID.Dig, tileCoords.ToWorldCoordinates());
                    }
                }
                Main.tile[tileCoords].Active(true);
                Main.tile[tileCoords].TileType = (ushort)Projectile.ai[0];
                if (frameImportant.X != -1)
                {
                    Main.tile[tileCoords].TileFrameX = (short)frameImportant.X;
                    Main.tile[tileCoords].TileFrameY = (short)frameImportant.Y;
                }
                else
                {
                    WorldGen.SquareTileFrame(tileCoords.X, tileCoords.Y, resetFrame: true);
                }
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
                if ((int)Projectile.ai[0] < 0)
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.velocity.Y += 0.03f;
                Projectile.rotation += Projectile.direction * ((0.1f + Projectile.velocity.Length() * 0.02f) / (1 + Projectile.extraUpdates));
                TileLight();
                return;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                var oldMouseWorld = mouseWorld;
                mouseWorld = Main.MouseWorld;
                if (mouseWorld.X != oldMouseWorld.X || mouseWorld.Y != oldMouseWorld.Y)
                {
                    Projectile.netUpdate = true;
                }
                mouseColor = Main.mouseColor;
            }
            if (mouseColor == Color.Transparent)
            {
                if (Main.myPlayer == Projectile.owner)
                    Projectile.netUpdate = true;

                mouseColor = Color.White;
            }

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
                    int tileID = (int)Projectile.ai[0];
                    if (tileID < 0)
                    {
                        if (tileID <= -2000)
                        {
                            int projIdentity = AequusHelpers.FindProjectileIdentity(-((int)Projectile.ai[1] + 100), -(tileID + 2000));
                            if (projIdentity == -1)
                            {
                                Projectile.Kill();
                                return;
                            }
                            Main.projectile[projIdentity].velocity = Projectile.velocity * (1 + Projectile.extraUpdates);
                        }
                        else if (tileID <= -1000)
                        {
                            int npcIndex = -(tileID + 1000);
                            Main.npc[npcIndex].velocity = Projectile.velocity * (1 + Projectile.extraUpdates);
                        }
                    }
                }
                return;
            }

            Main.player[Projectile.owner].heldProj = Projectile.whoAmI;
            Main.player[Projectile.owner].itemTime = 2;
            Main.player[Projectile.owner].itemAnimation = 2;
            Projectile.timeLeft = 2;

            if ((int)Projectile.ai[1] == 2 || (int)Projectile.ai[1] <= -100)
            {
                Projectile.tileCollide = false;
                var localMouseWorld = mouseWorld;
                var difference = localMouseWorld - Projectile.Center;
                Projectile.velocity = difference * 0.3f;
                int tileID = (int)Projectile.ai[0];
                if (tileID < 0)
                {
                    if (Main.mouseRight && Main.mouseRightRelease)
                    {
                        Projectile.Kill();
                        SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
                        return;
                    }
                    if (tileID <= -2000)
                    {
                        int projIdentity = AequusHelpers.FindProjectileIdentity(-((int)Projectile.ai[1] + 100), -(tileID + 2000));
                        if (projIdentity == -1)
                        {
                            Projectile.Kill();
                            return;
                        }
                        Main.projectile[projIdentity].Center = Projectile.Center;
                        Main.projectile[projIdentity].velocity = Projectile.velocity;
                    }
                    else if (tileID <= -1000)
                    {
                        int npcIndex = -(tileID + 1000);
                        Main.npc[npcIndex].Center = Projectile.Center;
                        Main.npc[npcIndex].velocity = Projectile.velocity;
                    }
                }
                else
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        if (Main.mouseRight && Main.mouseRightRelease)
                        {
                            PlaceTileForced();
                            Projectile.Kill();
                            SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
                        }
                    }
                }
                Projectile.rotation += player.direction * 0.1f;
                GunLight();
                TileLight();
                SetArmRotation();
                return;
            }

            var oldVelocity = Projectile.velocity;
            Projectile.velocity = Vector2.Normalize(mouseWorld - Main.player[Projectile.owner].Center);
            if (Main.myPlayer == Projectile.owner)
            {
                if (oldVelocity.X != Projectile.velocity.X || oldVelocity.Y != Projectile.velocity.Y)
                    Projectile.netUpdate = true;
                Projectile.Center = Main.player[Projectile.owner].Center;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.Center = Main.MouseWorld;
                var checkTileCoords = Main.MouseWorld.ToTileCoordinates();
                Projectile.netUpdate = true;
                var myRect = Projectile.getRect();
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].getRect().Intersects(myRect))
                    {
                        Projectile.ai[0] = -1000 - Main.npc[i].whoAmI;
                        Projectile.ai[1] = 2f;
                    }
                }
                if ((int)Projectile.ai[1] == 0)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].active && i != Projectile.whoAmI && Main.projectile[i].Colliding(Main.projectile[i].getRect(), myRect))
                        {
                            Projectile.ai[0] = -2000 - Main.projectile[i].identity;
                            Projectile.ai[1] = -100f - Main.projectile[i].owner;
                        }
                    }
                }
                if ((int)Projectile.ai[1] == 0 && Main.tile[checkTileCoords].HasTile)
                {
                    if (Main.tileFrameImportant[Main.tile[checkTileCoords].TileType])
                    {
                        frameImportant.X = Main.tile[checkTileCoords].TileFrameX;
                        frameImportant.Y = Main.tile[checkTileCoords].TileFrameY;
                    }
                    else
                    {
                        frameImportant = new Point(-1, -1);
                    }
                    Projectile.ai[0] = Main.tile[checkTileCoords].TileType;
                    Projectile.ai[1] = 2f;
                    Main.tile[checkTileCoords].Active(value: false);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        NetMessage.SendTileSquare(-1, checkTileCoords.X, checkTileCoords.Y);
                    }
                }
            }

            SetArmRotation();
            GunLight();
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
                    PlaceTileForced();
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
            var beamColor = AequusHelpers.HueShift(mouseColor, AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 50f, -0.02f, 0.02f)) * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 25f, 1f, 1.25f);
            if ((int)Projectile.ai[1] < 3)
            {
                DrawSuperGun();

                var prim = new TrailRenderer(TextureCache.Trail[2].Value, TrailRenderer.DefaultPass, (p) => new Vector2(4f), (p) => beamColor.UseA(60),
                drawOffset: Vector2.Zero);

                var difference = Main.player[Projectile.owner].MountedCenter - mouseWorld;
                var dir = Vector2.Normalize(difference);
                var list = new List<Vector2>
                {
                    Main.player[Projectile.owner].MountedCenter + dir * -30f,
                };
                int amt = Aequus.HQ ? 20 : 7;
                if (difference.Length() < 300f)
                {
                    amt = 0;
                }
                var segmentVector = Vector2.Normalize(mouseWorld - list[0]) * 10f;
                var pos = list[0];
                for (int i = 0; i < amt * 2; i++)
                {
                    var toBlockVector = Vector2.Normalize(Projectile.Center - pos);
                    var p = AequusHelpers.CalcProgress(amt * 3, i);
                    float lerpAmt = p;
                    float length = (pos - Projectile.Center).Length();
                    if (length <= segmentVector.Length())
                        break;
                    if (length <= 400f)
                        lerpAmt += (float)Math.Pow((1f - length / 400f), 2f) * 2f;

                    pos += segmentVector;
                    segmentVector = Vector2.Normalize(Vector2.Lerp(segmentVector, toBlockVector, Math.Max(lerpAmt * 0.44f, 0.01f))) * segmentVector.Length();
                    list.Add(pos);
                }
                list.Add(Projectile.Center);
                prim.Draw(list.ToArray());
            }

            if ((int)Projectile.ai[1] >= 2)
            {
                int tileID = (int)Projectile.ai[0];
                if (tileID < 0)
                {
                    if (tileID <= -1000)
                    {
                        int npcIndex = -(tileID + 1000);
                        return false;
                    }
                }
                Main.instance.LoadTiles((int)Projectile.ai[0]);
                var t = TextureAssets.Tile[(int)Projectile.ai[0]].Value;
                var frame = new Rectangle(162, 54, 16, 16);
                if (frameImportant.X != -1)
                {
                    frame.X = frameImportant.X;
                    frame.Y = frameImportant.Y;
                }
                var origin = frame.Size() / 2f;
                var drawColor = lightColor.MaxRGBA(128);

                var drawCoords = Projectile.Center - Main.screenPosition;

                if ((int)Projectile.ai[1] == 2 || (int)Projectile.ai[1] == 4)
                {
                    Main.spriteBatch.End();
                    Begin.GeneralEntities.BeginShader(Main.spriteBatch);

                    var s = GameShaders.Armor.GetSecondaryShader(ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex, Main.LocalPlayer);
                    var dd = new DrawData(t, Projectile.Center - Main.screenPosition, frame, beamColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
                    if ((int)Projectile.ai[1] == 4)
                    {
                        drawColor = Color.Black * Projectile.Opacity;
                        dd.color = Color.White * Projectile.Opacity * Projectile.Opacity * Projectile.Opacity;
                    }
                    foreach (var c in AequusHelpers.CircularVector(4))
                    {
                        dd.position = drawCoords + c * 2f;
                        s.Apply(null, dd);
                        dd.Draw(Main.spriteBatch);
                    }

                    Main.spriteBatch.End();
                    Begin.GeneralEntities.Begin(Main.spriteBatch);
                }
                Main.EntitySpriteDraw(t, drawCoords, frame, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }

            return false;
        }

        public void DrawSuperGun()
        {
            int itemID = ModContent.ItemType<PhysicsGun2>();
            Main.instance.LoadItem(itemID);
            var texture = TextureAssets.Item[itemID];

            var difference = Main.player[Projectile.owner].MountedCenter - mouseWorld;
            var dir = Vector2.Normalize(difference);
            var drawCoords = Main.player[Projectile.owner].MountedCenter + dir * -24f;
            float rotation = difference.ToRotation() + (Main.player[Projectile.owner].direction == -1 ? 0f : MathHelper.Pi);
            var origin = texture.Value.Size() / 2f;
            var spriteEffects = Main.player[Projectile.owner].direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(texture.Value, drawCoords - Main.screenPosition, null, AequusHelpers.GetColor(drawCoords),
                 rotation, origin, Projectile.scale, spriteEffects, 0);

            var glowTexture = ModContent.Request<Texture2D>(ModContent.GetInstance<PhysicsGun2>().Texture + "_Glow", AssetRequestMode.ImmediateLoad);
            var coloring = mouseColor;
            foreach (var v in AequusHelpers.CircularVector(8, Main.GlobalTimeWrappedHourly + Projectile.rotation))
            {
                Main.EntitySpriteDraw(glowTexture.Value, drawCoords + v * Projectile.scale * 2f - Main.screenPosition, null, (coloring * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.05f, 0.2f)).UseA(20),
                    rotation, origin, Projectile.scale, spriteEffects, 0);
            }
            foreach (var v in AequusHelpers.CircularVector(4, Projectile.rotation))
            {
                Main.EntitySpriteDraw(glowTexture.Value, drawCoords + v * Projectile.scale * 2f - Main.screenPosition, null, (coloring * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.2f, 0.5f)).UseA(100),
                    rotation, origin, Projectile.scale, spriteEffects, 0);
            }
            Main.EntitySpriteDraw(glowTexture.Value, drawCoords - Main.screenPosition, null, coloring,
                rotation, origin, Projectile.scale, spriteEffects, 0);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(frameImportant.X);
            writer.Write(frameImportant.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            frameImportant.X = reader.ReadInt32();
            frameImportant.Y = reader.ReadInt32();
        }
    }
}