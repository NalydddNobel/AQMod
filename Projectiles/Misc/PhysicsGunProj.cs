using Aequus.Graphics;
using Aequus.Graphics.Primitives;
using Aequus.Items.Tools;
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
        public override string Texture => Aequus.BlankTexture;

        public Vector2 mouseWorld;
        public Color mouseColor;

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
            var tileCoords = Projectile.Center.ToTileCoordinates();
            if (!Main.tile[tileCoords].HasTile || WorldGen.CanKillTile(tileCoords.X, tileCoords.Y))
            {
                if (Main.player[Projectile.owner].HasEnoughPickPowerToHurtTile(tileCoords.X, tileCoords.Y))
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
        }

        public override void AI()
        {
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
            Main.player[Projectile.owner].heldProj = Projectile.whoAmI;
            Main.player[Projectile.owner].itemTime = 2;
            Main.player[Projectile.owner].itemAnimation = 2;
            Projectile.timeLeft = 2;

            if ((int)Projectile.ai[1] == 2)
            {
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
            for (int i = 0; i < 300; i++)
            {
                var old = checkCoords;
                player.LimitPointToPlayerReachableArea(ref checkCoords);
                if (old.X != checkCoords.X || old.Y != checkCoords.Y)
                    break;
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
                        if (Main.netMode != NetmodeID.SinglePlayer)
                        {
                            NetMessage.SendTileSquare(-1, tileCoords.X, tileCoords.Y);
                        }
                        break;
                    }
                }
                checkCoords += Projectile.velocity * 4f;
            }
            Projectile.Center = checkCoords;
            SetArmRotation();
            GunLight();
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
            float value = MathHelper.WrapAngle((Projectile.Center - Main.player[Projectile.owner].Center).ToRotation() + (float)Math.PI / 2f);
            float angle = Math.Abs(value);
            int dir = Math.Sign(value);
            if (dir != Main.player[Projectile.owner].direction)
            {
                Main.player[Projectile.owner].direction = dir;
            }
            // arm angling code, thanks Split!
            int frame = (angle <= 0.6f) ? 2 : ((angle >= (MathHelper.PiOver2 - 0.1f) && angle <= MathHelper.PiOver4 * 3f) ? 3 : ((!(angle > MathHelper.Pi * 3f / 4f)) ? 3 : 4));
            Main.player[Projectile.owner].bodyFrame.Y = Main.player[Projectile.owner].bodyFrame.Height * frame;
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
            var beamColor = AequusHelpers.HueShift(mouseColor, AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 50f, -0.02f, 0.02f));
            if ((int)Projectile.ai[1] != 3)
            {
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
                Main.EntitySpriteDraw(t, drawCoords, frame, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }

        public void DrawGun()
        {
            int itemID = ModContent.ItemType<PhysicsGun>();
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

            var glowTexture = ModContent.Request<Texture2D>(ModContent.GetInstance<PhysicsGun>().Texture + "_Glow");
            var coloring = mouseColor;
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