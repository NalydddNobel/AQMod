using Aequus;
using Aequus.Common.Primitives;
using Aequus.Common.Utilities;
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

namespace Aequus.NPCs.Boss.DustDevil.Projectiles {
    public class DustDevilTileProj : ModProjectile {
        public const int STATE_INIT = 0;
        public const int STATE_ORBITING = 1;
        public const int STATE_BEINGPULLED = 2;
        public const int STATE_HELD = 3;
        public const int STATE_THROWN = 4;

        public static HashSet<int> TileTextureBlacklist { get; private set; }

        public override string Texture => Aequus.BlankTexture;

        public int State { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }
        public ushort tileTexture;
        public Vector3 ThreeDeeBabeyy {
            get => new Vector3(Projectile.position.X, Projectile.position.Y, Z);
            set {
                Projectile.position.X = value.X;
                Projectile.position.Y = value.Y;
                Z = value.Z;
            }
        }
        public float Z { get => (int)Projectile.localAI[0]; set => Projectile.localAI[0] = value; }
        public int npcOwner => (int)Projectile.ai[1];
        public NPC NPCOwner => Main.npc[npcOwner];

        public override void Load() {
            TileTextureBlacklist = new HashSet<int>();
        }

        public override void Unload() {
            TileTextureBlacklist?.Clear();
            TileTextureBlacklist = null;
        }

        public override void SetStaticDefaults() {
            this.SetTrail(10);
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 4000;
        }

        public override void SetDefaults() {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 500;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

        public override bool CanHitPlayer(Player target) {
            return (int)Projectile.ai[0] > 0f && Projectile.alpha <= 0;
        }

        public override void AI() {
            var npc = NPCOwner;
            Projectile.CollideWithOthers(1f);
            if (!npc.active) {
                State = STATE_THROWN;
                Projectile.hostile = false;
                Projectile.timeLeft = 240;
            }
            if (Projectile.alpha > 0) {
                Projectile.alpha -= 8;
                if (Projectile.alpha < 0) {
                    Projectile.alpha = 0;
                }
            }
            switch (State) {
                case STATE_INIT: {
                        CaptureTileId();
                        State = STATE_ORBITING;
                    }
                    break;

                case STATE_ORBITING: {
                        float orbitingPosition = 1500f;
                        if (npc.life * 2 < npc.lifeMax) {
                            orbitingPosition -= 500f * (1f - npc.life * 2f / npc.lifeMax);
                        }
                        var v = npc.Center + Vector2.Normalize(Projectile.Center - npc.Center).RotatedBy(Main.rand.NextFloat(0f, 0.06f)) * orbitingPosition - Projectile.Center;
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, (v / 10f).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)), 0.006f);
                        Projectile.timeLeft = 2;
                        Z = (float)Math.Sin(Projectile.identity + Main.GlobalTimeWrappedHourly * 1f) * 40f;
                    }
                    break;

                case STATE_BEINGPULLED: {
                        var v = npc.Center - Projectile.Center;
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, (v / 100f).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)), 0.006f);
                        Projectile.timeLeft = 2;
                        Z *= 0.99f;
                        Projectile.localAI[1]++;
                        if (npc.ai[0] != DustDevil.ACTION_SUCTIONTILES) {
                            Projectile.netUpdate = true;
                            State = STATE_HELD;
                        }
                    }
                    break;

                case STATE_HELD: {
                        Projectile.localAI[1]++;
                        Z *= 0.99f;

                        if (Projectile.localAI[1] > 500f) {
                            Projectile.netUpdate = true;
                            if (Projectile.localAI[1] > 520f) {
                                State = STATE_THROWN;
                                SoundEngine.PlaySound(SoundID.Item18, Projectile.position);
                            }
                            Projectile.timeLeft = 240;
                            Projectile.velocity += Vector2.Normalize(Main.player[npc.target].Center - Projectile.Center);
                        }
                        else {
                            var v = npc.Center - Projectile.Center;
                            Projectile.velocity = Vector2.Lerp(Projectile.velocity, (v / 30f).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)), 0.1f);
                            Projectile.timeLeft = 2;
                        }
                    }
                    break;

                case STATE_THROWN: {
                        Projectile.velocity.Y += 0.3f;
                        Projectile.tileCollide = true;
                        Projectile.extraUpdates = 0;
                    }
                    break;
            }
            if ((int)Projectile.ai[1] == -1) {
                return;
            }
            Projectile.rotation += Projectile.velocity.Length() * 0.1f;
        }
        public void CaptureTileId() {
            var coords = Projectile.Center.ToTileCoordinates();
            if (!WorldGen.InWorld(coords.X, coords.Y, 10)) {
                return;
            }
            var tile = Main.tile[coords];
            if (!tile.HasTile || Main.tileFrameImportant[tile.TileType] || TileTextureBlacklist.Contains(tile.TileType)) {
                tileTexture = TileID.Dirt;
            }
            else {
                tileTexture = tile.TileType;
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
            Projectile.hide = false;
            if (State == STATE_ORBITING || ThreeDeeBabeyy.Z < 0f) {
                Projectile.hide = true;
                behindNPCsAndTiles.Add(index);
            }
            if (State == STATE_BEINGPULLED) {
                Projectile.hide = true;
                behindNPCs.Add(index);
            }
        }
        public override bool PreDraw(ref Color lightColor) {
            if (State == STATE_BEINGPULLED || State == STATE_HELD) {
                var npc = NPCOwner;
                if (!npc.active)
                    return false;

                int maxLength = 40;
                int length = maxLength - (int)Math.Max(maxLength - Projectile.localAI[1], 0f);
                if (length > 2) {
                    DrawWindLine(npc, length, maxLength);
                }
            }

            Projectile.GetDrawInfo(out var _, out var off, out var _, out var _, out int trailLength);
            Main.instance.LoadTiles(tileTexture);
            var t = TextureAssets.Tile[tileTexture].Value;
            var frame = new Rectangle(162, 54, 16, 16);
            var origin = frame.Size() / 2f;
            var drawCoords = ViewHelper.GetViewPoint(Projectile.position + off, Z * 0.05f);
            var drawScale = ViewHelper.GetViewScale(Projectile.scale, Z * 0.05f);

            Main.spriteBatch.Draw(t, drawCoords - Main.screenPosition, frame, Helper.GetColor(drawCoords, lightColor) * Projectile.Opacity, Projectile.rotation, origin, drawScale * 1.1f, SpriteEffects.None, 0f);
            return false;
        }

        public void DrawWindLine(NPC npc, int length, int maxLength) {
            var d = new Vector2[length];
            var diff = npc.Center - Projectile.Center;
            var v = diff / maxLength;
            for (int i = 0; i < length - 1; i++) {
                d[i] = Projectile.Center + v.RotatedBy(Math.Sin(i * 0.8f + Main.GlobalTimeWrappedHourly) * 0.002f) * i;
            }
            if (length == maxLength)
                d[^1] = npc.Center;

            var prim = new ForceCoordTrailRenderer(TrailTextures.Trail[3].Value, TrailRenderer.DefaultPass, (p) => new Vector2(12f + (float)Math.Sin(p * 4f + Main.GlobalTimeWrappedHourly * 4f) * 4f) * p,
                (p) => {
                    float op = 1f;
                    float dist = Vector2.Distance(d[0] + diff * p, d[^1]);
                    if (dist < 20f) {
                        op = (dist - 20f) / 20f;
                    }
                    if (p > 0.9f) {
                        op *= (1f - 0.9f) / 0.9f;
                    }
                    return Color.White * 0.4f * ((float)Math.Sin(p * 4f + Main.GlobalTimeWrappedHourly) * 0.2f + 0.9f) * op;
                }) {
                coord1 = 1f,
                coord2 = 0f
            };

            prim.Draw(d, Projectile.identity - Main.GlobalTimeWrappedHourly * 0.25f);

            prim.GetWidth = (p) => new Vector2(12f + (float)Math.Sin(p * 4f + Main.GlobalTimeWrappedHourly * 4f) * 4f) * 3f * p;
            prim.GetColor = (p) => {
                float op = 1f;
                float dist = Vector2.Distance(d[0] + diff * p, d[^1]);
                if (dist < 20f) {
                    op = (dist - 20f) / 20f;
                }
                if (p > 0.9f) {
                    op *= (1f - 0.9f) / 0.9f;
                }
                return Color.White * 0.2f * ((float)Math.Sin(p * 4f + Main.GlobalTimeWrappedHourly) * 0.2f + 0.9f) * op;
            };
            prim.Draw(d, Projectile.identity - Main.GlobalTimeWrappedHourly * 0.5f);
        }

        public override void SendExtraAI(BinaryWriter writer) {
            writer.Write(tileTexture);
            writer.Write(Z);
        }

        public override void ReceiveExtraAI(BinaryReader reader) {
            tileTexture = reader.ReadUInt16();
            Z = reader.ReadSingle();
        }
    }
}