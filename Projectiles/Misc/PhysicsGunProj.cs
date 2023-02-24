using Aequus.Common.Utilities.Drawing;
using Aequus.Graphics.Primitives;
using Aequus.NPCs.PhysicistNPC.Shop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    public class PhysicsGunProj : ModProjectile
    {
        public static HashSet<int> TilePickupBlacklist { get; private set; }
        public static HashSet<int> TileBlocksLaser { get; private set; }

        public Vector2 mouseWorld;
        public Color mouseColor;
        public bool realBlock;

        public override void Load()
        {
            TilePickupBlacklist = new HashSet<int>()
            {
                TileID.AntiPortalBlock,
            };
            TileBlocksLaser = new HashSet<int>();
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public void PlaceTile()
        {
            if (!realBlock || Main.myPlayer != Projectile.owner)
            {
                return;
            }
            var player = Main.player[Projectile.owner];
            for (int k = 0; k < 50; k++)
            {
                if (!player.inventory[k].IsAir && player.inventory[k].ModItem is PhysicsGun)
                {
                    player.inventory[k].pick = 35;
                }
            }

            var tileCoords = Projectile.Center.ToTileCoordinates();
            if (!Main.tile[tileCoords].HasTile || WorldGen.CanKillTile(tileCoords.X, tileCoords.Y))
            {
                if (!Main.tile[tileCoords].HasTile || Main.player[Projectile.owner].HasEnoughPickPowerToHurtTile(tileCoords.X, tileCoords.Y))
                {
                    WorldGen.KillTile(tileCoords.X, tileCoords.Y);
                    if (!WorldGen.PlaceTile(tileCoords.X, tileCoords.Y, (int)Projectile.ai[0], forced: true))
                        SoundEngine.PlaySound(SoundID.Dig, tileCoords.ToWorldCoordinates());
                    Main.tile[tileCoords].Active(true);
                    Main.tile[tileCoords].TileType = (ushort)Projectile.ai[0];
                }
                WorldGen.SquareTileFrame(tileCoords.X, tileCoords.Y, resetFrame: true);
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendTileSquare(-1, tileCoords.X, tileCoords.Y);
                }
            }

            for (int k = 0; k < 50; k++)
            {
                if (!player.inventory[k].IsAir && player.inventory[k].ModItem is PhysicsGun)
                {
                    player.inventory[k].pick = 0;
                }
            }
        }

        public override void AI()
        {
            if ((int)Projectile.ai[1] == 4)
            {
                Projectile.extraUpdates = 1;
                if (Projectile.timeLeft < 60)
                {
                    Projectile.alpha += 255 / 60;
                }
                Projectile.rotation += Projectile.direction * ((0.1f + Projectile.velocity.Length() * 0.02f) / (1 + Projectile.extraUpdates));
                Projectile.scale -= 0.006f;
                if (Projectile.alpha < 100 && Main.rand.NextBool(6))
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight);
                }
                return;
            }

            var player = Main.player[Projectile.owner];
            if ((int)Projectile.ai[1] == 3)
            {
                if (CheckEmancipationGrills())
                {
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
                }
                return;
            }

            Main.player[Projectile.owner].heldProj = Projectile.whoAmI;
            Main.player[Projectile.owner].itemTime = 2;
            Main.player[Projectile.owner].itemAnimation = 2;
            Projectile.timeLeft = 2;
            Projectile.LoopingFrame(3);

            if ((int)Projectile.ai[1] == 2)
            {
                if (CheckEmancipationGrills())
                {
                    return;
                }

                Projectile.tileCollide = true;
                var localMouseWorld = mouseWorld;
                player.LimitPointToPlayerReachableArea(ref localMouseWorld);
                var difference = localMouseWorld - Projectile.Center;
                Projectile.velocity = difference * 0.3f;
                if (Main.myPlayer == Projectile.owner)
                {
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

            var checkCoords = Projectile.Center;
            for (int k = 0; k < 50; k++)
            {
                if (!player.inventory[k].IsAir && player.inventory[k].ModItem is PhysicsGun)
                {
                    player.inventory[k].pick = 35;
                }
            }

            for (int i = 0; i < 300; i++)
            {
                var old = checkCoords;
                player.LimitPointToPlayerReachableArea(ref checkCoords);
                if (old.X != checkCoords.X || old.Y != checkCoords.Y)
                    break;
                var checkTileCoords = checkCoords.ToTileCoordinates();
                bool inWorld = WorldGen.InWorld(checkTileCoords.X, checkTileCoords.Y, 10);
                if (inWorld)
                {
                    if (Main.tile[checkTileCoords].HasTile)
                    {
                        if (TileBlocksLaser.Contains(Main.tile[checkTileCoords].TileType))
                        {
                            break;
                        }
                        if (Main.tile[checkTileCoords].IsFullySolid() && !Main.tileFrameImportant[Main.tile[checkTileCoords].TileType])
                        {
                            if (TilePickupBlacklist.Contains(Main.tile[checkTileCoords].TileType) || !player.HasEnoughPickPowerToHurtTile(checkTileCoords.X, checkTileCoords.Y) || !WorldGen.CanKillTile(checkTileCoords.X, checkTileCoords.Y))
                            {
                                break;
                            }
                            Projectile.ai[0] = Main.tile[checkTileCoords].TileType;
                            Projectile.ai[1] = 2f;
                            WorldGen.KillTile(checkTileCoords.X, checkTileCoords.Y, noItem: true);
                            if (Main.netMode != NetmodeID.SinglePlayer)
                            {
                                if (Main.myPlayer == player.whoAmI)
                                {
                                    var p = Aequus.GetPacket(PacketType.PhysicsGunBlock);
                                    p.Write(player.whoAmI);
                                    p.Write(checkTileCoords.X);
                                    p.Write(checkTileCoords.Y);
                                    p.Send();
                                }
                                break;
                            }

                            realBlock = true;
                            break;
                        }
                    }
                }
                checkCoords += Projectile.velocity * 4f;
            }

            for (int k = 0; k < 50; k++)
            {
                if (!player.inventory[k].IsAir && player.inventory[k].ModItem is PhysicsGun)
                {
                    player.inventory[k].pick = 0;
                }
            }

            Projectile.Center = checkCoords;
            SetArmRotation();
            GunLight();
        }

        public bool CheckEmancipationGrills()
        {
            var diff = Projectile.Center - (Projectile.oldPosition + Projectile.Size / 2f);
            int checkLength = Math.Max((int)(diff.Length() / 4f), 2);
            var checkCoords = Projectile.oldPosition;
            var velocity = Vector2.Normalize(diff);
            for (int i = 0; i < checkLength; i++)
            {
                var checkTileCoords = checkCoords.ToTileCoordinates();
                bool inWorld = WorldGen.InWorld(checkTileCoords.X, checkTileCoords.Y, 10);
                if (inWorld)
                {
                    if (Main.tile[checkTileCoords].HasTile && !Main.tile[checkTileCoords].IsActuated && Main.tile[checkTileCoords].TileType == ModContent.TileType<EmancipationGrillTile>())
                    {
                        Projectile.ai[1] = 4f;
                        Projectile.velocity *= Projectile.extraUpdates + 1;
                        Projectile.velocity /= 4f;
                        Projectile.timeLeft = 120;
                        if (Projectile.velocity.Length() > 2f)
                        {
                            Projectile.velocity.Normalize();
                            Projectile.velocity *= 2f;
                        }
                        Projectile.netUpdate = true;
                        SoundEngine.PlaySound(SoundID.Item122.WithVolume(0.33f).WithPitch(0.8f), Projectile.Center);
                        return true;
                    }
                }
                checkCoords += velocity * 4f;
            }
            return false;
        }

        public void GunLight()
        {
            var beamColor = AequusHelpers.HueShift(mouseColor, AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 50f, -0.02f, 0.02f));
            Lighting.AddLight(Projectile.Center, beamColor.ToVector3() * 0.66f);
        }
        public void TileLight()
        {
            // TODO: make glowing tiles.. glow
        }
        public void SetArmRotation()
        {
            AequusHelpers.ShootRotation(Projectile, MathHelper.WrapAngle((Projectile.Center - Main.player[Projectile.owner].Center).ToRotation() + (float)Math.PI / 2f));
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
            var beamColor = AequusHelpers.HueShift(mouseColor, AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 50f, -0.03f, 0.03f));
            if ((int)Projectile.ai[1] < 3)
            {
                var prim = new TrailRenderer(Textures.Trail[2].Value, TrailRenderer.DefaultPass, (p) => new Vector2(4f), (p) => beamColor.UseA(60),
                drawOffset: Vector2.Zero);

                //mouseWorld = Main.player[Projectile.owner].MountedCenter - new Vector2(0f, 400f);
                var difference = Main.player[Projectile.owner].MountedCenter - mouseWorld;
                var dir = Vector2.Normalize(difference);
                var list = new List<Vector2>
                {
                    Main.player[Projectile.owner].MountedCenter + dir * -30f,
                };
                int amt = Aequus.HQ ? 20 : 7;
                if ((Projectile.Center - list[0]).Length() < 100f)
                {
                    amt = 0;
                }
                var pos = list[0];
                float distance = (Projectile.Center - list[0]).Length() / (amt);
                var segmentVector = Vector2.Normalize(mouseWorld - list[0]) * distance;
                for (int i = 0; i < amt; i++)
                {
                    pos += Vector2.Lerp(segmentVector, Vector2.Normalize(Projectile.Center - pos) * distance, i / (float)amt);
                    list.Add(pos);
                }
                list.Add(Projectile.Center);
                prim.Draw(list.ToArray());

                DrawGun();
            }

            if ((int)Projectile.ai[1] >= 2)
            {
                Main.instance.LoadTiles((int)Projectile.ai[0]);
                var t = TextureAssets.Tile[(int)Projectile.ai[0]].Value;
                var frame = new Rectangle(162, 54, 16, 16);
                var origin = frame.Size() / 2f;
                var drawColor = lightColor.MaxRGBA(128);

                var drawCoords = Projectile.Center - Main.screenPosition;

                if ((int)Projectile.ai[1] == 2 || (int)Projectile.ai[1] == 4)
                {
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin_World(shader: true);

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
                    Main.spriteBatch.Begin_World(shader: false);;
                }
                Main.EntitySpriteDraw(t, drawCoords, frame, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }

            return false;
        }

        public void DrawGun()
        {
            Projectile.GetDrawInfo(out var texture, out var _, out var frame, out var origin, out int _);
            frame.Width /= 2;
            origin = frame.Size() / 2f;

            var difference = Main.player[Projectile.owner].MountedCenter - mouseWorld;
            var dir = Vector2.Normalize(difference);
            var drawCoords = Main.player[Projectile.owner].MountedCenter + dir * -24f;
            float rotation = difference.ToRotation() + (Main.player[Projectile.owner].direction == -1 ? 0f : MathHelper.Pi);
            var spriteEffects = Main.player[Projectile.owner].direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(texture, drawCoords - Main.screenPosition, frame, AequusHelpers.GetColor(drawCoords),
                 rotation, origin, Projectile.scale, spriteEffects, 0);

            frame.X = frame.Width;
            var coloring = mouseColor;
            foreach (var v in AequusHelpers.CircularVector(4, Projectile.rotation))
            {
                Main.EntitySpriteDraw(texture, drawCoords + v * Projectile.scale * 2f - Main.screenPosition, frame, (coloring * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.2f, 0.5f)).UseA(100),
                    rotation, origin, Projectile.scale, spriteEffects, 0);
            }
            Main.EntitySpriteDraw(texture, drawCoords - Main.screenPosition, frame, coloring,
                rotation, origin, Projectile.scale, spriteEffects, 0);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(mouseWorld);
            writer.WriteRGB(mouseColor);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            mouseWorld = reader.ReadVector2();
            mouseColor = reader.ReadRGB();
        }
    }
}