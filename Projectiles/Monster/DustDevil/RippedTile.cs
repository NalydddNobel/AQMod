using Aequus.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Monster.DustDevil
{
    public class RippedTile : ModProjectile
    {
        public static HashSet<int> TileBlacklist { get; private set; }

        public override string Texture => Aequus.BlankTexture;

        public float randomYOffset;
        public float wave;
        public float waveSpeed;

        public override void Load()
        {
            TileBlacklist = new HashSet<int>();
        }

        public override void Unload()
        {
            TileBlacklist?.Clear();
            TileBlacklist = null;
        }

        public override void SetStaticDefaults()
        {
            this.SetTrail(10);
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 500;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

        public override bool CanHitPlayer(Player target)
        {
            return (int)Projectile.ai[0] > 0f;
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 20;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }
            if ((int)Projectile.ai[1] == -1)
            {
                Projectile.velocity.Y += 0.3f;
                Projectile.tileCollide = true;
                Projectile.extraUpdates = 0;
                return;
            }
            var npc = Main.npc[(int)Projectile.ai[1]];
            if (npc.active && (int)npc.ai[0] == NPCs.Boss.DustDevil.ACTION_SUCTIONTILES)
            {
                if ((int)npc.ai[1] == 120f)
                {
                    Projectile.ai[0] = 1f;
                    var tile = Main.tile[Projectile.Center.ToTileCoordinates()];
                    if (!tile.HasTile || Main.tileFrameImportant[tile.TileType] || TileBlacklist.Contains(tile.TileType))
                    {
                        Projectile.localAI[1] = TileID.Dirt;
                    }
                    else
                    {
                        Projectile.localAI[1] = tile.TileType;
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
                    }
                    Collision.HitTiles(Projectile.position, new Vector2(0f, 0f), Projectile.width, Projectile.height);
                    wave = Main.rand.Next(1000);
                    waveSpeed = Main.rand.NextFloat(0.015f, 0.155f);
                    randomYOffset = Main.rand.NextFloat(-npc.height, npc.height);
                    Projectile.netUpdate = true;
                }
                else
                {
                    if (Projectile.timeLeft < 240)
                    {
                        if (Projectile.timeLeft == 239)
                        {
                            SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                            Projectile.velocity = Vector2.Normalize(Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)].Center - Projectile.Center) * 15.5f;
                            Projectile.extraUpdates = 0;
                        }
                        else
                        {
                            Projectile.velocity.Y += 0.01f + (240 - Projectile.timeLeft) * 0.001f;
                            Projectile.velocity.X *= 0.996f;
                        }
                    }
                    else
                    {
                        if (waveSpeed <= 0f)
                        {
                            Projectile.velocity = Vector2.Normalize(npc.Center - Projectile.Center);
                            return;
                        }
                        float orbitWidth = npc.width * 3f;
                        float y = randomYOffset + (float)Math.Sin(wave * 0.8f) * 10f;
                        NPCs.Boss.DustDevil.GetTornadoInfo(npc.height, y, out float _, out float _, out float progress);
                        var gotoPosition = npc.Center + new Vector2((float)Math.Sin(wave) * orbitWidth * progress, y);
                        Projectile.localAI[0] = (float)Math.Cos(wave);
                        var diff = gotoPosition - Projectile.Center;
                        float div = 200f;
                        float l = diff.Length();
                        if (l < 400f)
                        {
                            wave += waveSpeed * (1f - progress);
                        }
                        if (l < 40f)
                        {
                            div = 10f;
                        }
                        else if (l < 100f)
                        {
                            div /= 6f;
                        }
                        else if (l < 200f)
                        {
                            div /= 3f;
                        }
                        else if (l < 300f)
                        {
                            div /= 2f;
                        }
                        else if (l < 400f)
                        {
                            div /= 1.5f;
                        }
                        Projectile.velocity = diff / Math.Max(div * waveSpeed, 1f);
                    }
                }
            }
            else
            {
                Projectile.ai[0] = -1f;
                Projectile.ai[1] = -1f;
            }
            Projectile.rotation += Projectile.velocity.Length() * 0.1f;
        }

        public override bool ShouldUpdatePosition()
        {
            return (int)Projectile.ai[0] != 0;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if ((int)Projectile.ai[0] == 0)
            {
                AequusEffects.ProjsBehindTiles.Add(Projectile.whoAmI);
            }
            else if ((int)Projectile.ai[0] > 0)
            {
                NPCs.Boss.DustDevil.AddDraw(Projectile.whoAmI, Projectile.localAI[0]);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (AequusEffects.ProjsBehindTiles.renderingNow)
            {
                var bloom = TextureCache.Bloom[3].Value;
                var bloomFrame = new Rectangle(0, 0, bloom.Width, bloom.Height / 2);
                var bloomOrigin = new Vector2(bloomFrame.Width / 2f, bloomFrame.Height);
                var bloomColor = new Color(120, 120, 185, 0);
                float opacity = Projectile.Opacity;
                bloomColor *= opacity;
                Main.spriteBatch.Draw(bloom, Projectile.Center - Main.screenPosition, bloomFrame, bloomColor, Projectile.velocity.ToRotation() + MathHelper.PiOver2, bloomOrigin, new Vector2(0.01f, 7f * opacity), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(bloom, Projectile.Center - Main.screenPosition, bloomFrame, bloomColor * 0.6f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, bloomOrigin, new Vector2(0.01f, 7f * opacity), SpriteEffects.None, 0f);
                return false;
            }
            if ((int)Projectile.ai[0] == -1 || NPCs.Boss.DustDevil.CurrentlyDrawing(Projectile.localAI[0]))
            {
                Projectile.GetDrawInfo(out var _, out var off, out var _, out var _, out int trailLength);
                Main.instance.LoadTiles((int)Projectile.localAI[1]);
                var t = TextureAssets.Tile[(int)Projectile.localAI[1]].Value;
                var frame = new Rectangle(162, 54, 16, 16);
                var origin = frame.Size() / 2f;
                var drawColor = lightColor.MaxRGBA(128);

                Main.spriteBatch.Draw(t, Projectile.position + off - Main.screenPosition, frame, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
                for (int i = 0; i < trailLength; i++)
                {
                    var p = AequusHelpers.CalcProgress(trailLength, i);
                    Main.spriteBatch.Draw(t, Projectile.oldPos[i] + off - Main.screenPosition, frame, drawColor.UseA(100) * p * p * 0.55f,
                        Projectile.oldRot[i], origin, Projectile.scale * (0.6f + 0.4f * p), SpriteEffects.None, 0f);
                }
            }
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(wave % MathHelper.TwoPi);
            writer.Write(waveSpeed);
            writer.Write(randomYOffset);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            wave = reader.ReadSingle();
            waveSpeed = reader.ReadSingle();
            randomYOffset = reader.ReadSingle();
        }
    }
}