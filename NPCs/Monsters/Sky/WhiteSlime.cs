using Aequus.Biomes;
using Aequus.Common.Catalogues;
using Aequus.Common.ItemDrops;
using Aequus.Content.Necromancy;
using Aequus.Graphics;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Misc.Dyes;
using Aequus.Items.Misc.Energies;
using Aequus.Items.Placeable.Banners;
using Aequus.Items.Weapons.Magic;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters.Sky
{
    public class WhiteSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 20;
            NPCID.Sets.TrailingMode[Type] = 7;
            NPCID.Sets.TrailCacheLength[Type] = 10;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(0f, 16f),
                PortraitPositionYOverride = 36f,
            });

            HeatDamageTypes.HeatNPC.Add(Type);
            NecromancyDatabase.NPCs.Add(Type, GhostInfo.Two);
        }

        public override void SetDefaults()
        {
            NPC.width = 38;
            NPC.height = 26;
            NPC.aiStyle = -1;
            NPC.damage = 60;
            NPC.defense = 10;
            NPC.lifeMax = 210;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.alpha = 155;
            NPC.lavaImmune = true;
            NPC.trapImmune = true;
            NPC.noGravity = true;
            NPC.value = Item.buyPrice(silver: 10);
            NPC.knockBackResist = 0.35f;
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[BuffID.OnFire] = true;
            NPC.buffImmune[BuffID.CursedInferno] = true;
            NPC.buffImmune[BuffID.ShadowFlame] = true;
            NPC.buffImmune[BuffID.Confused] = false;

            NPC.SetLiquidSpeeds(water: 1f, lava: 1f);

            Banner = NPC.type;
            BannerItem = ModContent.ItemType<WhiteSlimeBanner>();

            this.SetBiome<GaleStreamsInvasion>();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f);
            if (AequusWorld.HardmodeTier)
            {
                NPC.lifeMax *= 2;
                NPC.knockBackResist = 0f;
            }
            //if (AQMod.calamityMod.IsActive)
            //{
            //    NPC.lifeMax = (int)(NPC.lifeMax * 2.5f);
            //    NPC.damage = (int)(NPC.damage * 1.5f);
            //    NPC.defense *= 2;
            //}
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateGaleStreamsEntry(database, bestiaryEntry);
        }

        public override void AI()
        {
            var dustRect = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);

            if ((int)NPC.ai[1] == 1)
            {
                NPC.noTileCollide = false;
                if (NPC.velocity.Y == 0f)
                {
                    NPC.velocity.X *= 0.8f;
                    dustRect.Y += dustRect.Height / 3 * 2;
                    dustRect.Height /= 3;
                    if (NPC.localAI[0] > 21f)
                    {
                        if (NPC.localAI[0] > 120f)
                        {
                            NPC.localAI[0]++;
                            if (NPC.localAI[0] > 147f)
                            {
                                NPC.localAI[0] = 0f;
                                NPC.ai[1] = -1f;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            NPC.localAI[0] += Main.rand.Next(5);
                        }
                    }
                    else
                    {
                        if (NPC.velocity.X.Abs() <= 3f)
                        {
                            if (NPC.localAI[0] == 0 && Main.netMode != NetmodeID.Server)
                            {
                                AequusHelpers.PlaySound(SoundType.Sound, "boowomp", NPC.Center);
                            }
                            NPC.localAI[0]++;
                        }
                    }
                }
            }
            else
            {
                if (NPC.velocity.Y < -0.1f || (NPC.HasValidTarget && NPC.position.Y + NPC.height - 2 < Main.player[NPC.target].position.Y + Main.player[NPC.target].height - 10))
                    NPC.noTileCollide = true;
                else
                    NPC.noTileCollide = false;

                int jumpTime = (int)(NPC.ai[0] % 1000f);
                if (NPC.velocity.Y == 0f)
                {
                    NPC.velocity.X *= 0.8f;
                    bool incrementTimer = true;
                    if (jumpTime > 100f)
                    {
                        NPC.TargetClosest();
                        NPC.ai[1] = 0f;
                        NPC.ai[0] += 900f;
                        bool close = false;
                        if (NPC.HasValidTarget)
                        {
                            close = Vector2.Distance(NPC.Center, Main.player[NPC.target].Center) < 360f;
                        }
                        if (NPC.ai[0] > 5000f)
                        {
                            NPC.ai[0] = -80f;
                            NPC.ai[1] = 1f;
                            if (close)
                            {
                                NPC.velocity.Y += -12f;
                                NPC.velocity.X += 8f * NPC.direction;
                            }
                            else
                            {
                                NPC.velocity.Y += -18f;
                                NPC.velocity.X += 6f * NPC.direction;
                            }
                        }
                        else if (NPC.ai[0] > 3000f && NPC.ai[0] < 4000f)
                        {
                            if (close)
                            {
                                NPC.velocity.Y += -4f;
                                NPC.velocity.X += 10f * NPC.direction;
                            }
                            else
                            {
                                NPC.velocity.Y += -9.5f;
                                NPC.velocity.X += 9f * NPC.direction;
                            }
                        }
                        else
                        {
                            if (close)
                            {
                                NPC.velocity.Y += -8.5f;
                                NPC.velocity.X += 7.5f * NPC.direction;
                            }
                            else
                            {
                                NPC.velocity.Y += -13.5f;
                                NPC.velocity.X += 6f * NPC.direction;
                            }
                        }
                        NPC.noTileCollide = true;
                        NPC.netUpdate = true;
                        incrementTimer = false;
                    }
                    if (incrementTimer)
                    {
                        NPC.ai[0] += 1.3f;
                        if (Main.expertMode)
                        {
                            NPC.ai[0] += 2f;
                        }
                    }
                }
            }

            int dustRate = 12;
            if (NPC.velocity.Length() > 4f)
            {
                dustRate -= 8;
            }

            if (Main.GameUpdateCount % dustRate == 0)
            {
                if (dustRate >= 11 || Main.rand.NextBool(12 - dustRate))
                {
                    var d = Dust.NewDustDirect(dustRect.TopLeft(), dustRect.Width, dustRect.Height, ModContent.DustType<MonoSparkleDust>(), 0f, 0f, 0, new Color(215, 190, 160, 0));
                    d.velocity *= 0.1f;
                    d.velocity -= NPC.velocity * 0.15f;
                    d.velocity.Y -= 1f;
                    d.scale = Main.rand.NextFloat(1.125f, 1.85f);
                    d.fadeIn = 1.35f * d.scale;
                }
                else
                {
                    var d = Dust.NewDustDirect(dustRect.TopLeft(), dustRect.Width, dustRect.Height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(215, 190 - Main.rand.Next(10, 40), 160, 0));
                    d.velocity *= 0.2f;
                    d.velocity -= NPC.velocity * 0.2f;
                }
            }

            NPC.velocity.Y += 0.5f;
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.Y != 0)
            {
                if (NPC.velocity.Y < 0f)
                {
                    NPC.frame.Y = frameHeight;
                }
                else
                {
                    NPC.frame.Y = frameHeight * 2;
                }
            }
            else if ((int)NPC.ai[1] == 1)
            {
                if (NPC.localAI[0] > 21f)
                {
                    if (NPC.localAI[0] > 120f)
                    {
                        NPC.frame.Y = frameHeight * 10 + frameHeight * (((int)NPC.localAI[0] - 120) / 3);
                    }
                    else
                    {
                        NPC.frame.Y = frameHeight * 10;
                    }
                }
                else
                {
                    NPC.frame.Y = frameHeight * ((int)NPC.localAI[0] / 3 + 3);
                }
            }
            else
            {
                int jumpTime = (int)(NPC.ai[0] % 1000f);
                NPC.frameCounter += 1.0d;
                if (NPC.frameCounter >= 6.0d)
                {
                    NPC.frameCounter = 0.0d;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > frameHeight)
                        NPC.frame.Y = 0;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var texture = TextureAssets.Npc[Type].Value;
            var offset = new Vector2(NPC.width / 2f, NPC.height / 2f - 1.5f);
            var orig = new Vector2(NPC.frame.Width / 2f, NPC.frame.Height / 2f);

            int trailLength = NPCID.Sets.TrailCacheLength[Type];
            for (int i = 0; i < trailLength; i++)
            {
                if (i < trailLength - 1 && (NPC.oldPos[i] - NPC.oldPos[i + 1]).Length() < 1f)
                {
                    continue;
                }
                float progress = 1f / trailLength * i;
                spriteBatch.Draw(texture, NPC.oldPos[i] + offset - screenPos, NPC.frame, new Color(255, 255, 255, 255 - NPC.alpha) * (1f - progress) * 0.35f, NPC.rotation, orig, NPC.scale, SpriteEffects.None, 0f);
            }

            if (!NecromancyScreenRenderer.RenderingNow)
            {
                float brightness = (float)Math.Sin(Main.GlobalTimeWrappedHourly);
                foreach (var v in AequusHelpers.CircularVector(3, Main.GlobalTimeWrappedHourly * 2f))
                {
                    spriteBatch.Draw(texture, NPC.position + offset - screenPos + v * (brightness * 2f + 2f), NPC.frame, new Color(255, 255, 255, 255 - NPC.alpha) * (1f - brightness * 0.8f), NPC.rotation, orig, NPC.scale, SpriteEffects.None, 0f);
                }
            }

            spriteBatch.Draw(texture, NPC.position + offset - screenPos, NPC.frame, new Color(255, 255, 255, 255 - NPC.alpha), NPC.rotation, orig, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            int count = 1;
            if (NPC.life <= 0)
                count = 20;
            for (int i = 0; i < count; i++)
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(120, 120, 120, 0));
                Main.dust[d].velocity *= 0.1f;
                Main.dust[d].scale = Main.rand.NextFloat(1f, 1.5f);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            this.CreateLoot(npcLoot)
                .Add<Umystick>(new HardmodeTierCondition(), chance: 15, stack: 1)
                .Add(ItemID.SlimeStaff, chance: 100, stack: 1)
                .Add<AtmosphericEnergy>(chance: 20, stack: 1)
                .Add(ItemID.Gel, chance: 1, stack: (5, 15))
                .Add<CensorDye>(chance: 10, stack: 1)
                .Add<CinnamonRoll>(chance: 15, stack: 1);
        }

        //public override void NPCLoot()
        //{
        //    if (Main.rand.NextBool(8))
        //    {
        //        Item.NewItem(NPC.getRect(), ModContent.ItemType<CinnamonRoll>());
        //    }
        //}
    }
}