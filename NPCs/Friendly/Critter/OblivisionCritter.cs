using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Friendly.Critter
{
    public class OblivisionCritter : ModNPC
    {
        public float eyeData;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 10;
            NPCID.Sets.CountsAsCritter[Type] = true;
            NPCID.Sets.TrailingMode[Type] = 7;
            NPCID.Sets.TrailCacheLength[Type] = 14;
            NPCID.Sets.DebuffImmunitySets.Add(Type, new Terraria.DataStructures.NPCDebuffImmunityData()
            {
                ImmuneToAllBuffsThatAreNotWhips = true,
            });
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry)
                .AddMainSpawn(BestiaryBuilder.Underworld);
        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 24;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 5;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.npcSlots = 0.5f;
            NPC.noGravity = true;
            //NPC.catchItem = (short)ModContent.ItemType<DwarfStarite>();
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(255, 255, 255, 255);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            int amt = NPC.life <= 0 ? 30 : 1;
            for (int i = 0; i < amt; i++)
            {
                var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Smoke, newColor: Color.Black);
                d.velocity *= 0.75f;
                d.noGravity = true;
                d.fadeIn = d.scale + 0.1f;
            }
            for (int i = 0; i < amt / 3; i++)
            {
                var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Firework_Red);
                d.position += Vector2.Normalize(d.velocity) * NPC.width / 2f;
                d.velocity *= 0.9f;
                d.noGravity = true;
                d.fadeIn = d.scale + 0.1f;
            }
        }

        public override void AI()
        {
            if ((int)NPC.ai[1] == -1)
            {
                if (NPC.ai[3] > 0f)
                    NPC.ai[3] = 0f;
                NPC.ai[3] -= 0.66f;
                if (Main.rand.NextBool())
                {
                    var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Unit() * NPC.ai[3] / 2f, ModContent.DustType<MonoDust>(), newColor: Color.Lerp(Color.Yellow.UseB(128), Color.White, Math.Min(Main.rand.NextFloat(0.5f, 1f) - NPC.ai[3] / 60f, 1f)).UseA(0));
                    d.velocity *= 0.2f;
                    d.velocity += (NPC.Center - d.position) / 16f;
                }
                if (NPC.ai[3] < -60f)
                {
                    NPC.life = -33333;
                    NPC.HitEffect();
                    NPC.checkDead();
                }
                return;
            }
            int tileHeight = 0;
            int tileX = ((int)NPC.position.X + NPC.width) / 16;
            int tileY = ((int)NPC.position.Y + NPC.height) / 16;
            for (int i = 0; i < 10; i++)
            {
                if (WorldGen.InWorld(tileX, tileY + i, 10) && Main.tile[tileX, tileY + i].IsSolid())
                {
                    tileHeight = i + 1;
                    break;
                }
            }
            if (tileHeight == 10)
            {
                NPC.ai[0] = 0.5f;
            }
            else
            {
                if ((int)NPC.ai[1] <= 0)
                {
                    NPC.ai[0] = Main.rand.NextFloat(-1f, 1f);
                    NPC.ai[1] = Main.rand.Next(20, 80);
                }
                else
                {
                    NPC.ai[1]--;
                    if (NPC.collideX)
                    {
                        NPC.ai[0] = -NPC.ai[0];
                        NPC.velocity.Y = NPC.oldVelocity.Y * 0.8f;
                    }
                }
            }
            if ((int)NPC.ai[3] <= 0)
            {
                NPC.ai[2] = Main.rand.NextFloat(-2f, 2f);
                NPC.ai[3] = Main.rand.Next(120, 600);
            }
            else
            {
                NPC.ai[3]--;
                if (NPC.collideX)
                {
                    NPC.ai[2] = -NPC.ai[2];
                    NPC.velocity.X = NPC.oldVelocity.X * 0.8f;
                }
            }

            int eyeTargetFriendlyNPC = -1;
            float eyeTargetDistance = 2000f;
            int eyeTargetPlayer = -1;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active)
                {
                    if (!Main.player[i].dead)
                    {
                        var d = NPC.Distance(Main.player[i].Center);
                        if (d < eyeTargetDistance)
                        {
                            eyeTargetDistance = d;
                            eyeTargetPlayer = i;
                        }
                    }
                    if (Main.player[i].ghost && Main.myPlayer == i && Main.rand.NextBool(600))
                    {
                        Main.NewText(AequusText.GetTextWith("OblivisonEasterEgg", new { PlayerName = Main.player[i].name }));
                    }
                }
            }

            eyeTargetDistance = 2000f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (i != NPC.whoAmI && Main.npc[i].active && Main.npc[i].friendly)
                {
                    var d = NPC.Distance(Main.npc[i].Center);
                    if (d < eyeTargetDistance)
                    {
                        eyeTargetDistance = d;
                        eyeTargetFriendlyNPC = i;
                    }
                }
            }

            if (NPC.frame.Y == 0 && Main.rand.NextBool(400))
            {
                NPC.frame.Y += NPC.frame.Height;
            }
            if (NPC.frame.Y >= NPC.frame.Height * 4 && Main.rand.NextBool(400))
            {
                NPC.frameCounter = -1.0;
            }

            if (eyeData == 0f && Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(100))
            {
                if (eyeTargetFriendlyNPC != -1 && eyeTargetPlayer != -1)
                {
                    eyeData = Main.rand.NextBool() ? -1f : 1f;
                }
                else if (eyeTargetFriendlyNPC != -1)
                {
                    eyeData = -1f;
                }
                else if (eyeTargetPlayer != -1)
                {
                    eyeData = 1f;
                }
                NPC.netUpdate = true;
            }

            if (eyeData != 0f)
            {
                if (eyeData < 0f)
                {
                    if (eyeData > -1000f && Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(600))
                    {
                        eyeData -= 1000f;
                    }
                    if (eyeTargetFriendlyNPC != -1 && eyeData > -1000f)
                    {
                        float val = -eyeData % 1000f - 1f;
                        float rotation = val % MathHelper.TwoPi;
                        int distance = (int)(val / MathHelper.TwoPi);
                        if (distance < 32)
                        {
                            distance++;
                        }
                        rotation = rotation.AngleLerp((Main.npc[eyeTargetFriendlyNPC].Center - NPC.Center).ToRotation(), 0.05f);
                        if (rotation < 0f)
                        {
                            rotation += MathHelper.TwoPi;
                        }
                        eyeData = -(1f + rotation + distance * MathHelper.TwoPi);
                    }
                    else
                    {
                        eyeData += MathHelper.TwoPi;
                        if (eyeData >= -1000f)
                        {
                            eyeData = 0f;
                        }
                    }
                }
                if (eyeData > 0f)
                {
                    if (eyeData < 1000f && Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(600))
                    {
                        eyeData += 1000f;
                    }
                    if (eyeTargetPlayer != -1 && eyeData < 1000f)
                    {
                        float val = eyeData % 1000f - 1f;
                        float rotation = val % MathHelper.TwoPi;
                        int distance = (int)(val / MathHelper.TwoPi);
                        if (distance < 32)
                        {
                            distance++;
                        }
                        rotation = rotation.AngleLerp((Main.player[eyeTargetPlayer].Center - NPC.Center).ToRotation(), 0.05f);
                        if (rotation < 0f)
                        {
                            rotation += MathHelper.TwoPi;
                        }
                        eyeData = 1f + rotation + distance * MathHelper.TwoPi;
                    }
                    else
                    {
                        eyeData -= MathHelper.TwoPi;
                        if (eyeData <= 1000f)
                        {
                            eyeData = 0f;
                        }
                    }
                }
            }

            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.ai[2] / 2f, 0.01f);
            NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, NPC.ai[0] / 4f, 0.01f);
            NPC.rotation += Main.rand.NextFloat(-0.01f, 0.01f);
            Lighting.AddLight(NPC.Center, new Vector3(0.3f, 0.01f, 0.01f));
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.frame.Y > 0)
            {
                if (NPC.frameCounter < 0.0)
                {
                    NPC.frameCounter--;
                    if (NPC.frameCounter < -2.0)
                    {
                        NPC.frame.Y -= NPC.frame.Height;
                        NPC.frameCounter = NPC.frame.Y == 0 ? 0.0 : -1.0;
                    }
                }
                else if (NPC.frame.Y < NPC.frame.Height * 4)
                {
                    NPC.frameCounter++;
                    if (NPC.frameCounter >= 2.0)
                    {
                        NPC.frameCounter = 0.0;
                        NPC.frame.Y += NPC.frame.Height;
                    }
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.Aequus().ZoneGoreNest ? 0.25f : 0f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var texture = TextureAssets.Npc[NPC.type].Value;
            var offset = new Vector2(NPC.width / 2f, NPC.height / 2f);
            var origin = NPC.frame.Size() / 2f;
            var drawPos = NPC.Center - screenPos;
            var whiteFrame = NPC.frame;
            whiteFrame.Y += 150;
            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                float p = AequusHelpers.CalcProgress(NPCID.Sets.TrailCacheLength[NPC.type], i);
                Main.spriteBatch.Draw(texture, NPC.oldPos[i] + offset - Main.screenPosition, NPC.frame, new Color(200, 200, 200, 0) * p, NPC.oldRot[i], origin, NPC.scale * p, SpriteEffects.None, 0f);
            }
            foreach (var v in AequusHelpers.CircularVector(8, NPC.rotation + Main.GlobalTimeWrappedHourly))
            {
                Main.spriteBatch.Draw(texture, drawPos + v * 8f * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 2.5f, 0f, 1f), whiteFrame, Color.Red * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.1f, 0.2f) * NPC.Opacity, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            }
            foreach (var v in AequusHelpers.CircularVector(4, NPC.rotation))
            {
                Main.spriteBatch.Draw(texture, drawPos + v * 2f, whiteFrame, new Color(255, 30, 10, 0) * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.5f, 1f) * NPC.Opacity, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            }
            var pixel = ModContent.Request<Texture2D>(Aequus.AssetsPath + "Pixel", AssetRequestMode.ImmediateLoad).Value;
            Main.spriteBatch.Draw(texture, drawPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);

            var eyeOffset = Vector2.Zero;

            int eyeFrame = NPC.frame.Y / NPC.frame.Height;
            if (eyeData != 0f)
            {
                float val = eyeData.Abs() % 1000f - 1f;
                float rotation = val % MathHelper.TwoPi;
                float distance = (int)(val / MathHelper.TwoPi);
                float div = 8f;
                distance *= 1f - eyeFrame / 5f;
                eyeOffset += rotation.ToRotationVector2() * distance / div;
            }

            Main.spriteBatch.Draw(pixel, drawPos + eyeOffset, null, NPC.GetAlpha(drawColor) * (1f - eyeFrame / 5f), NPC.rotation, pixel.Size() / 2f, NPC.scale * 4f, SpriteEffects.None, 0f);
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(eyeData);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            eyeData = reader.ReadSingle();
        }
    }
}