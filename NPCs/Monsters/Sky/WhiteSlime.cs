using Aequus.Common;
using Aequus.Items.Misc.Dyes;
using Aequus.Items.Misc.Energies;
using Aequus.Particles.Dusts;
using Aequus.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters.Sky
{
    public class WhiteSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 20;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(0f, 16f),
                PortraitPositionYOverride = 36f,
            });
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.WindyDay,
                new FlavorTextBestiaryInfoElement("Mods.Aequus.Bestiary.WhiteSlime")
            });
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
            NPC.knockBackResist = 0.2f;
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[BuffID.OnFire] = true;
            NPC.buffImmune[BuffID.CursedInferno] = true;
            NPC.buffImmune[BuffID.ShadowFlame] = true;
            NPC.buffImmune[BuffID.Confused] = false;

            NPC.SetLiquidSpeeds(water: 1f, lava: 1f);

            //banner = NPC.type;
            //bannerItem = ModContent.ItemType<Items.Placeable.Banners.WhiteSlimeBanner>();

            //NPC.GetGlobalNPC<AQNPC>().temperature = 40;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f);
            if (WorldFlags.HardmodeTier)
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
                                SoundHelper.Play(SoundType.Sound, "boowomp", NPC.Center, 0.9f);
                            }
                            NPC.localAI[0]++;
                        }
                    }
                }
            }
            else
            {
                if (NPC.velocity.Y < -0.1f || NPC.HasValidTarget && NPC.position.Y + NPC.height - 2 < Main.player[NPC.target].position.Y + Main.player[NPC.target].height - 10)
                    NPC.noTileCollide = true;
                else
                    NPC.noTileCollide = false;

                int jumpTime = (int)(NPC.ai[0] % 1000f);
                if (NPC.velocity.Y == 0f)
                {
                    NPC.velocity.X *= 0.8f;
                    bool incrementTimer = true;
                    if (jumpTime > 40f)
                    {
                        NPC.TargetClosest();
                        NPC.ai[1] = 0f;
                        NPC.ai[0] += 960f;
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

            NPC.velocity.Y += 0.5f;

            int d = Dust.NewDust(dustRect.TopLeft(), dustRect.Width, dustRect.Height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(120, 120, 120, 0));
            Main.dust[d].velocity *= 0.1f;
            Main.dust[d].scale = Main.rand.NextFloat(1f, 1.5f);
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
            var drawPosition = new Vector2(NPC.position.X + NPC.width / 2f, NPC.position.Y + NPC.height / 2f);
            drawPosition.Y -= 1.5f;
            var orig = new Vector2(NPC.frame.Width / 2f, NPC.frame.Height / 2f);

            Main.spriteBatch.Draw(texture, drawPosition - screenPos, NPC.frame, new Color(255, 255, 255, 255 - NPC.alpha), NPC.rotation, orig, NPC.scale, SpriteEffects.None, 0f);
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
            var rule = new LeadingConditionRule(new Conditions.IsHardmode());
            rule.OnSuccess(npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AtmosphericEnergy>(), 20)));
            npcLoot.Add(ItemDropRule.Common(ItemID.Gel, 1, 5, 15));
            npcLoot.Add(ItemDropRule.Common(ItemID.SlimeStaff, 250));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CensorDye>(), 10));
        }

        //public override void NPCLoot()
        //{
        //    if (WorldDefeats.SudoHardmode && Main.rand.NextBool(15))
        //    {
        //        Item.NewItem(NPC.getRect(), ModContent.ItemType<Items.Weapons.Magic.Umystick>());
        //    }
        //    if (Main.rand.NextBool(8))
        //    {
        //        Item.NewItem(NPC.getRect(), ModContent.ItemType<CinnamonRoll>());
        //    }
        //}
    }
}