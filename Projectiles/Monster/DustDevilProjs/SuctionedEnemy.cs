using Aequus.NPCs.Boss;
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

namespace Aequus.Projectiles.Monster.DustDevilProjs
{
    public class SuctionedEnemy : ModProjectile
    {
        public static List<int> SelectableEnemies { get; private set; }

        public override string Texture => Aequus.BlankTexture;

        public float randomYOffset;
        public float wave;
        public float waveSpeed;

        public NPC _drawNPC;

        public override void Load()
        {
            SelectableEnemies = new List<int>()
            {
                NPCID.BlueSlime,
                NPCID.GreenSlime,
                NPCID.PurpleSlime,
                NPCID.Harpy,
                NPCID.CorruptBunny,
                NPCID.CrimsonBunny,
                NPCID.Mummy,
                NPCID.SandSlime,
                NPCID.JungleBat,
                NPCID.Derpling,
                NPCID.SpikedJungleSlime,
                NPCID.SpikedIceSlime,
                NPCID.IceSlime,
                NPCID.ToxicSludge,
            };
        }

        public override void Unload()
        {
            SelectableEnemies?.Clear();
            SelectableEnemies = null;
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
            Projectile.timeLeft = 240;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void AI()
        {
            if ((int)Projectile.ai[1] == -1)
            {
                Projectile.velocity.Y += 0.3f;
                Projectile.timeLeft = Math.Min(Projectile.timeLeft, 120);
                Projectile.extraUpdates = 0;
                return;
            }
            var npc = Main.npc[(int)Projectile.ai[1]];
            if (npc.active && (int)npc.ai[0] == NPCs.Boss.DustDevil.ACTION_SUCTIONENEMIES)
            {
                if ((int)Projectile.ai[0] == 0)
                {
                    Projectile.ai[0] = SelectableEnemies[Main.rand.Next(SelectableEnemies.Count)];

                    SoundEngine.PlaySound(SoundID.Item1);
                    wave = Main.rand.Next(1000);
                    waveSpeed = Main.rand.NextFloat(0.015f, 0.155f);
                    randomYOffset = Main.rand.NextFloat(-npc.height, npc.height);
                    Projectile.netUpdate = true;
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
                    DustDevil.GetLegacyTornadoInfo(npc.height, y, out float _, out float _, out float progress);
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
            else
            {
                Projectile.ai[1] = -1f;
            }
            Projectile.rotation += Projectile.velocity.Length() * 0.025f;
        }

        public override void Kill(int timeLeft)
        {
            return;
            if ((int)Projectile.ai[0] == 0)
                return;
            SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
            var n = NPC.NewNPCDirect(Projectile.GetSource_FromThis(), Projectile.Center, (int)Projectile.ai[0]);
            n.velocity = Vector2.Normalize(Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)].Center - Projectile.Center) * 10f;
            n.netUpdate = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if ((int)Projectile.ai[0] != 0)
            {
                NPCs.Boss.DustDevil.AddLegacyDraw(Projectile.whoAmI, Projectile.localAI[0]);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if ((int)Projectile.ai[0] == -1 || NPCs.Boss.DustDevil.CurrentlyDrawing(Projectile.localAI[0]))
            {
                Projectile.GetDrawInfo(out var _, out var off, out var _, out var _, out int trailLength);
                int npcTexture = NPCID.FromNetId((int)Projectile.ai[0]);
                Main.instance.LoadNPC(npcTexture);
                var t = TextureAssets.Npc[npcTexture].Value;
                if (_drawNPC == null)
                {
                    _drawNPC = new NPC();
                    _drawNPC.SetDefaults((int)Projectile.ai[0]);
                    _drawNPC.hide = false;
                    _drawNPC.FindFrame();
                }
                var frame = _drawNPC.frame;
                var origin = frame.Size() / 2f;
                var drawColor = lightColor.MaxRGBA(128);

                Main.spriteBatch.Draw(t, Projectile.position + off - Main.screenPosition, frame, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
                if (_drawNPC.color != Color.Transparent)
                {
                    Main.spriteBatch.Draw(t, Projectile.position + off - Main.screenPosition, frame, _drawNPC.color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
                }
                for (int i = 0; i < trailLength; i++)
                {
                    var p = AequusHelpers.CalcProgress(trailLength, i);
                    Main.spriteBatch.Draw(t, Projectile.oldPos[i] + off - Main.screenPosition, frame, drawColor.UseA(100) * p * p * 0.55f,
                        Projectile.oldRot[i], origin, Projectile.scale * (0.6f + 0.4f * p), SpriteEffects.None, 0f);
                    if (_drawNPC.color != Color.Transparent)
                    {
                        Main.spriteBatch.Draw(t, Projectile.oldPos[i] + off - Main.screenPosition, frame, _drawNPC.color.UseA(100) * p * p * 0.55f,
                            Projectile.oldRot[i], origin, Projectile.scale * (0.6f + 0.4f * p), SpriteEffects.None, 0f);
                    }
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