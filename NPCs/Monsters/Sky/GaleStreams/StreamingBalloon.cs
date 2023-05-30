using Aequus.Content.Events;
using Aequus.Content.World.Seeds;
using Aequus.Items.Weapons.Ranged.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters.Sky.GaleStreams {
    public class StreamingBalloon : ModNPC
    {
        private bool _setupFrame = false;
        private int _balloonFrame;
        public const int FramesX = 8;

        private NPC _bestiaryRenderNPC;
        public NPC SlaveNPC => Main.npc[(int)NPC.ai[0]];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(0f, -14f),
                PortraitPositionYOverride = -20f,
                Scale = 0.7f,
            });
            NPCID.Sets.TrailingMode[NPC.type] = 7;
            NPCID.Sets.TrailCacheLength[NPC.type] = 20;
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData()
            {
                ImmuneToAllBuffsThatAreNotWhips = true,
                ImmuneToWhips = true,
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 26;
            NPC.height = 26;
            NPC.lifeMax = 1;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.DeathSound = SoundID.Item111;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.Aequus().noGravityDrops = true;

            this.SetBiome<GaleStreamsBiomeManager>();
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateGaleStreamsEntry(database, bestiaryEntry);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            //Gore.NewGore(new Vector2(NPC.position.X + NPC.width / 2, NPC.position.Y + 30),
            //    new Vector2(3f, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi)),
            //    ModGore.GetGoreSlot("AQMod/Gores/GaleStreams/BalloonTextureCopy"));
            if (Main.netMode != NetmodeID.MultiplayerClient && (int)NPC.ai[0] > -1 && (int)NPC.ai[0] < Main.maxNPCs)
            {
                int oldLife = Main.npc[(int)NPC.ai[0]].life;
                Main.npc[(int)NPC.ai[0]].SetDefaults((int)NPC.ai[2]);
                Main.npc[(int)NPC.ai[0]].life = oldLife;
                Main.npc[(int)NPC.ai[0]].target = NPC.target;
                Main.npc[(int)NPC.ai[0]].ai[1] = NPC.ai[1];
            }
        }

        public override void AI()
        {
            if ((int)NPC.ai[0] == -1)
            {
                NPC.velocity.Y -= 0.1f;
                if (NPC.timeLeft > 30)
                {
                    NPC.timeLeft = 30;
                }
                return;
            }
            if ((int)NPC.ai[2] == 0)
            {
                NPC.TargetClosest(faceTarget: false);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (NPC.HasValidTarget)
                    {
                        /*if (AprilFools.CheckAprilFools())
                        {
                            NPC.ai[2] = NPCID.LavaSlime;
                        }
                        else*/ if (ChristmasSeedSystem.Active || (Main.xMas && Main.rand.NextBool()))
                        {
                            NPC.ai[2] = Main.rand.NextBool() ? NPCID.IceSlime : NPCID.SpikedIceSlime;
                        }
                        else
                        {
                            var selectableEnemies = EnemyList();
                            NPC.ai[2] = selectableEnemies[Main.rand.Next(selectableEnemies.Count)];
                        }
                        int n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + NPC.width / 2, (int)NPC.position.Y + 66, (int)NPC.ai[2]);
                        Main.npc[n].Aequus().noVisible = true;
                        Main.npc[n].noTileCollide = true;
                        Main.npc[n].knockBackResist = 0f;
                        if ((int)NPC.ai[2] == NPCID.BlueSlime ||
                            (int)NPC.ai[2] < 0)
                        {
                            List<int> selectableLoot = null;
                            if (Main.rand.NextBool())
                            {
                                selectableLoot = new List<int>()
                                {
                                    ItemID.ShinyRedBalloon,
                                    ItemID.Starfury,
                                    ItemID.LuckyHorseshoe,
                                    ItemID.CreativeWings,
                                    ModContent.ItemType<Slingshot>(),
                                    //ModContent.ItemType<CinnamonRoll>(),
                                    //ModContent.ItemType<TemperatureHairDye>(),
                                };
                            }
                            else if (Main.rand.NextBool(4))
                            {
                                selectableLoot = new List<int>()
                                {
                                    ItemID.KiteBlue,
                                    ItemID.KiteBlueAndYellow,
                                    ItemID.KiteRed,
                                    ItemID.KiteRedAndYellow,
                                    ItemID.KiteYellow,
                                    ItemID.KiteBunny,
                                    ItemID.PaperAirplaneA,
                                    ItemID.PaperAirplaneB,
                                };
                            }

                            if (selectableLoot != null)
                                NPC.ai[1] = selectableLoot[Main.rand.Next(selectableLoot.Count)];
                        }
                        NPC.ai[0] = n;
                    }
                    else
                    {
                        NPC.life = 0;
                        NPC.active = false;
                        NPC.netUpdate = true;
                    }
                    NPC.netUpdate = true;
                }
            }
            if (!Main.npc[(int)NPC.ai[0]].active)
            {
                // confetti :D
                NPC.ai[0] = -1f;
                NPC.netUpdate = true;
                return;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Main.npc[(int)NPC.ai[0]].netID != (int)NPC.ai[2])
                {
                    NPC.ai[0] = -1f;
                    NPC.netUpdate = true;
                    return;
                }
            }

            if (NPC.direction == 0)
            {
                if (Main.player[NPC.target].position.X + Main.player[NPC.target].width / 2f > NPC.position.X + NPC.width / 2f)
                {
                    NPC.direction = 1;
                }
                else
                {
                    NPC.direction = -1;
                }
            }

            if (NPC.direction == -1)
            {
                if (NPC.velocity.X > -7f)
                    NPC.velocity.X -= 0.06f;
            }
            else
            {
                if (NPC.velocity.X < 7f)
                    NPC.velocity.X += 0.06f;
            }

            float distance = NPC.Distance(Main.player[NPC.target].Center);
            if (distance > 1650f)
            {
                NPC.life = 0;
                if (Main.netMode != NetmodeID.Server)
                {
                    SoundEngine.PlaySound(SoundID.Item111.WithVolume(1.5f).WithPitch(0.9f), NPC.Center);
                }
                NPC.HitEffect();
                NPC.active = false;
            }

            if (Collision.SolidCollision(NPC.position, NPC.width, 100))
            {
                if (distance < 40f)
                {
                    NPC.life = 0;
                    if (Main.netMode != NetmodeID.Server)
                    {
                        SoundEngine.PlaySound(SoundID.Item111.WithVolume(1.5f).WithPitch(0.9f), NPC.Center);
                    }
                    NPC.HitEffect();
                    NPC.active = false;
                }
                if (NPC.position.Y > Main.player[NPC.target].position.Y)
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y *= 0.92f;
                    if (NPC.velocity.Y > -7f)
                        NPC.velocity.Y -= 0.1f;
                }
                else
                {
                    if (NPC.velocity.Y < 0f)
                        NPC.velocity.Y *= 0.92f;
                    if (NPC.velocity.Y < 7f)
                        NPC.velocity.Y += 0.1f;
                }
            }
            else
            {
                if (distance < 150f)
                {
                    NPC.life = 0;
                    if (Main.netMode != NetmodeID.Server)
                    {
                        SoundEngine.PlaySound(SoundID.Item111.WithVolume(1.5f).WithPitch(0.9f), NPC.Center);
                    }
                    NPC.HitEffect();
                    NPC.active = false;
                }
                if (NPC.position.Y > Main.player[NPC.target].position.Y - 100f + Math.Sin(Main.time / 60f) * 30f)
                {
                    if (NPC.velocity.Y > -7f)
                        NPC.velocity.Y -= 0.1f;
                }
                else
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y *= 0.92f;
                    if (NPC.velocity.Y < 12f)
                        NPC.velocity.Y += 0.05f;
                    if ((NPC.position.X + NPC.width / 2f - Main.player[NPC.target].position.X + Main.player[NPC.target].width / 2f).Abs() < 100f)
                    {
                        NPC.life = 0;
                        if (Main.netMode != NetmodeID.Server)
                        {
                            SoundEngine.PlaySound(SoundID.Item111.WithVolume(1.5f).WithPitch(0.9f), NPC.Center);
                        }
                        NPC.HitEffect();
                        NPC.active = false;
                    }
                }
            }

            NPC.rotation = NPC.velocity.X * 0.01f;

            if (NPC.active)
            {
                var npc = SlaveNPC;
                npc.velocity = NPC.velocity;
                npc.position.X = NPC.position.X + NPC.width / 2f - Main.npc[(int)NPC.ai[0]].width / 2f;
                npc.position.Y = NPC.position.Y + 66 - Main.npc[(int)NPC.ai[0]].height / 2;
                npc.position -= NPC.velocity;
                npc.ai[0] = 0f;
                npc.Aequus().noVisible = true;
                npc.noTileCollide = true;
                npc.knockBackResist = 0f;
                npc.Aequus().noGravityDrops = true;
            }
            if (NPC.ai[1] > 0f)
            {
                SlaveNPC.ai[1] = NPC.ai[1];
            }
        }
        private List<int> EnemyList()
        {
            var selectableEnemies = new List<int>()
            {
                NPCID.RedSlime,
                NPCID.PurpleSlime,
                NPCID.BlackSlime,
                NPCID.JungleSlime,
                NPCID.IceSlime,
                NPCID.SpikedJungleSlime,
                NPCID.SpikedIceSlime,
            };
            if (Aequus.HardmodeTier)
            {
                selectableEnemies.Add(NPCID.ToxicSludge);
            }
            if (Main.hardMode && Main.player[NPC.target].ZoneHallow)
            {
                selectableEnemies.Add(NPCID.IlluminantSlime);
            }
            return selectableEnemies;
        }

        public override void FindFrame(int frameHeight)
        {
            if (!_setupFrame)
            {
                _setupFrame = true;
                NPC.frame.Width /= FramesX;
                _balloonFrame = Main.rand.Next(FramesX - 1);
            }
        }

        public class StreamingBalloonSlimeInsideDropRule : IItemDropRule, IItemDropRuleCondition, IProvideItemConditionDescription
        {
            public int Item;
            public int Chance;

            public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; }

            public StreamingBalloonSlimeInsideDropRule(int item, int chance = 2)
            {
                Item = item;
                Chance = chance;
                ChainedRules = new List<IItemDropRuleChainAttempt>();
            }

            public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
            {
                float num = 1f / Chance;
                float dropRate = num * ratesInfo.parentDroprateChance;
                ratesInfo.conditions = new List<IItemDropRuleCondition>() { this, };
                drops.Add(new DropRateInfo(Item, 1, 1, dropRate, ratesInfo.conditions));
                Chains.ReportDroprates(ChainedRules, num, drops, ratesInfo);
            }

            public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
            {
                var result = default(ItemDropAttemptResult);
                result.State = ItemDropAttemptResultState.DidNotRunCode;
                return result;
            }

            public bool CanDrop(DropAttemptInfo info)
            {
                return false;
            }

            public bool CanShowItemDropInUI()
            {
                return true;
            }

            public string GetConditionDescription()
            {
                return null;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            int goodItemsChance = 2 * 5;
            int randomItemChance = 4 * 8;

            this.CreateLoot(npcLoot)
                .Add(new StreamingBalloonSlimeInsideDropRule(ItemID.LuckyHorseshoe, goodItemsChance))
                .Add(new StreamingBalloonSlimeInsideDropRule(ItemID.Starfury, goodItemsChance))
                .Add(new StreamingBalloonSlimeInsideDropRule(ItemID.ShinyRedBalloon, goodItemsChance))
                .Add(new StreamingBalloonSlimeInsideDropRule(ItemID.CreativeWings, goodItemsChance))
                .Add(new StreamingBalloonSlimeInsideDropRule(ModContent.ItemType<Slingshot>(), goodItemsChance))
                .Add(new StreamingBalloonSlimeInsideDropRule(ItemID.KiteBlue, randomItemChance))
                .Add(new StreamingBalloonSlimeInsideDropRule(ItemID.KiteBlueAndYellow, randomItemChance))
                .Add(new StreamingBalloonSlimeInsideDropRule(ItemID.KiteRed, randomItemChance))
                .Add(new StreamingBalloonSlimeInsideDropRule(ItemID.KiteRedAndYellow, randomItemChance))
                .Add(new StreamingBalloonSlimeInsideDropRule(ItemID.KiteYellow, randomItemChance))
                .Add(new StreamingBalloonSlimeInsideDropRule(ItemID.KiteBunny, randomItemChance))
                .Add(new StreamingBalloonSlimeInsideDropRule(ItemID.PaperAirplaneA, randomItemChance))
                .Add(new StreamingBalloonSlimeInsideDropRule(ItemID.PaperAirplaneB, randomItemChance));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                _balloonFrame = Helper.TimedBasedOn((int)Main.GameUpdateCount, 90, FramesX - 1);
            }
            var texture = TextureAssets.Npc[Type].Value;
            var drawPosition = NPC.Center;
            var origin = new Vector2(NPC.frame.Width / 2f, NPC.frame.Height / 4f);
            Main.spriteBatch.Draw(texture, drawPosition - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition - screenPos, new Rectangle(NPC.frame.X + NPC.frame.Width * (_balloonFrame + 1), NPC.frame.Y, NPC.frame.Width, NPC.frame.Height), NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);

            NPC renderNPC;
            if (NPC.IsABestiaryIconDummy)
            {
                var spawnableEnemies = EnemyList();
                int index = Helper.TimedBasedOn((int)Main.GameUpdateCount, 30, spawnableEnemies.Count - 1);
                _bestiaryRenderNPC = new NPC();
                _bestiaryRenderNPC.SetDefaults(spawnableEnemies[index]);
                renderNPC = _bestiaryRenderNPC;
            }
            else if ((int)NPC.ai[0] == -1)
            {
                return false;
            }
            else
            {
                renderNPC = SlaveNPC;
                renderNPC.Aequus().noVisible = true;
            }

            int frameX = NPCToFrame(renderNPC.type);
            var alpha = renderNPC.GetAlpha(drawColor);
            float scale = renderNPC.scale;
            var npcClr = renderNPC.color;
            if (npcClr != default(Color))
            {
                npcClr = renderNPC.GetColor(drawColor);
            }

            if (scale != 1f)
            {
                float y = 70f - origin.Y;
                drawPosition.Y += y - y * scale; // keeps the slime's top on the same Y coordinate
            }

            if ((int)NPC.ai[1] > 0)
            {
                int itemType = (int)NPC.ai[1];
                float num199 = 1f;
                float num200 = 20f * scale * NPC.scale;
                float num201 = 18f * scale * NPC.scale;
                Main.instance.LoadItem(itemType);
                var itemTexture = TextureAssets.Item[itemType].Value;
                float num202 = itemTexture.Width;
                float num203 = itemTexture.Height;
                if (num202 > num200)
                {
                    num199 *= num200 / num202;
                    num203 *= num199;
                }
                if (num203 > num201)
                {
                    num199 *= num201 / num203;
                }
                float num205 = -1f;
                float num206 = 1f;
                int somethingelse = 13;
                num206 -= 13f;
                float itemscale = 0.2f;
                itemscale -= 0.3f * somethingelse;
                spriteBatch.Draw(itemTexture, new Vector2(drawPosition.X + num205 - screenPos.X, drawPosition.Y + num206 - screenPos.Y + 66f), null, drawColor, itemscale, itemTexture.Size() / 2f, num199, SpriteEffects.None, 0f);
            }

            if (!NPC.IsABestiaryIconDummy)
            {
                if ((int)NPC.ai[2] == NPCID.IlluminantSlime)
                {
                    for (int i = 1; i < NPC.oldPos.Length; i++)
                    {
                        spriteBatch.Draw(texture, new Vector2(NPC.oldPos[i].X - screenPos.X + NPC.width / 2,
                            NPC.oldPos[i].Y - screenPos.Y + NPC.height / 2f),
                            new Rectangle(NPC.frame.X + NPC.frame.Width * frameX, NPC.frame.Y + NPC.frame.Height, NPC.frame.Width, NPC.frame.Height),
                            new Color(150 * (10 - i) / 15, 100 * (10 - i) / 15, 150 * (10 - i) / 15, 50 * (10 - i) / 15), NPC.rotation, origin, NPC.scale * scale, SpriteEffects.None, 0f);
                    }
                }
            }

            Main.spriteBatch.Draw(texture, drawPosition - screenPos, new Rectangle(NPC.frame.X + NPC.frame.Width * frameX, NPC.frame.Y + NPC.frame.Height, NPC.frame.Width, NPC.frame.Height), NPC.GetNPCColorTintedByBuffs(alpha), NPC.rotation, origin, NPC.scale * scale, SpriteEffects.None, 0f);
            if (npcClr != default(Color))
            {
                Main.spriteBatch.Draw(texture, drawPosition - screenPos, new Rectangle(NPC.frame.X + NPC.frame.Width * frameX, NPC.frame.Y + NPC.frame.Height, NPC.frame.Width, NPC.frame.Height), NPC.GetNPCColorTintedByBuffs(npcClr), NPC.rotation, origin, NPC.scale * scale, SpriteEffects.None, 0f);
            }
            return false;
        }
        private int NPCToFrame(int type)
        {
            switch (type)
            {
                case NPCID.BlueSlime:
                    return 1;
                case NPCID.LavaSlime:
                    return 2;
                case NPCID.ToxicSludge:
                    return 3;
                case NPCID.SpikedJungleSlime:
                    return 4;
                case NPCID.IceSlime:
                    return 5;
                case NPCID.SpikedIceSlime:
                    return 6;
                case NPCID.IlluminantSlime:
                    return 7;
            }
            return -1;
        }
    }
}