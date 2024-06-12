using Aequus.Common.NPCs.Bestiary;
using Aequus.Content.Biomes.PollutedOcean;
using Aequus.Content.CrossMod;
using Aequus.Content.Dedicated.Baguette;
using Aequus.Content.Enemies.PollutedOcean.BreadOfCthulhu.Items;
using Aequus.Content.Items.Tools.Keys;
using Aequus.Core.ContentGeneration;
using Aequus.DataSets;
using System;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Content.Enemies.PollutedOcean.BreadOfCthulhu;

[AutoloadBanner]
[BestiaryBiome<PollutedOceanBiomeSurface>()]
[BestiaryBiome<PollutedOceanBiomeUnderground>()]
public class BreadOfCthulhu : ModNPC {
    public override void SetStaticDefaults() {
        Main.npcFrameCount[NPC.type] = 5;
        ItemSets.KillsToBanner[BannerItem] = 10;
        NPCDataSet.NoDropElementInheritence.Add(Type);
    }

    public override void SetDefaults() {
        NPC.width = 24;
        NPC.height = 24;
        NPC.aiStyle = -1;
        NPC.damage = 40;
        NPC.defense = 10;
        NPC.lifeMax = 175;
        NPC.HitSound = SoundID.NPCHit16;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = Item.buyPrice(silver: 80);
        NPC.knockBackResist = 0.4f;
        NPC.rarity = 1;
        NPC.alpha = 250;
        NPC.waterMovementSpeed = 1f;
        NPC.npcSlots = 4f;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry)
            .QuickUnlock();
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BreadOfCthulhuMask>(), chanceDenominator: 7));

        // Drop alt evil chunk
        int evilChunkMinStack = 1;
        int evilChunkMaxStack = 3;
        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsCorruption(), ItemID.Vertebrae, minimumDropped: evilChunkMinStack, maximumDropped: evilChunkMaxStack));
        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsCrimson(), ItemID.RottenChunk, minimumDropped: evilChunkMinStack, maximumDropped: evilChunkMaxStack));
        if (Avalon.Enabled && Avalon.TryFind("YuckyBit", out ModItem avalon_YuckyBit)) {
            npcLoot.Add(ItemDropRule.ByCondition(new Avalon.ItemDropConditions.IsNotContagion(), avalon_YuckyBit.Type, minimumDropped: evilChunkMinStack, maximumDropped: evilChunkMaxStack));
        }

        npcLoot.Add(ItemDropRule.OneFromOptions(chanceDenominator: 1, ItemID.OldShoe, ItemID.TinCan, ItemID.FishingSeaweed));
        npcLoot.Add(ItemDropRule.Common(ItemID.TrifoldMap, chanceDenominator: 10));
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Baguette>(), chanceDenominator: 10));

        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CopperKey>(), chanceDenominator: CopperKey.DropRate));
    }

    public override void HitEffect(NPC.HitInfo hit) {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        if (NPC.life <= 0) {
            for (int i = 0; i < 30; i++) {
                Terraria.Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood, hit.HitDirection * 2);
            }

            for (int i = 0; i < 30; i++) {
                var d = Terraria.Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.FoodPiece,
                    newColor: new Color(Main.rand.Next(20, 100), 200, 20, 200));
                d.velocity = new Vector2(Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-3f, -6f));
            }

            NPC.NewGore(AequusTextures.BreadOfCthulhu_0, NPC.position, NPC.velocity);
            NPC.NewGore(AequusTextures.BreadOfCthulhu_1, NPC.TopLeft, NPC.velocity);
            NPC.NewGore(AequusTextures.BreadOfCthulhu_2, NPC.Center, NPC.velocity);
        }
        Terraria.Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood, hit.HitDirection * 2);
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot) {
        return NPC.alpha <= 0;
    }

    public override void AI() {
        if (NPC.justHit) {
            NPC.ai[1] = 0f;
            NPC.ai[2] = 0f;
        }

        if (NPC.alpha > 0) {
            if (NPC.ai[0] == 0) {
                if (!NPC.HasValidTarget) {
                    NPC.TargetClosest();
                }
                else {
                    NPC.FaceTarget();
                }
                NPC.velocity.Y = -5f;
                NPC.velocity.X = 3f * NPC.direction;
                NPC.ai[0] = 1f;
                int waterDust = Terraria.Dust.dustWater();
                for (int i = 0; i < 50; i++) {
                    var d = Terraria.Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, waterDust);
                    d.velocity = new Vector2(Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-3f, -6f));
                }
            }
            if (Main.rand.NextBool(10)) {
                var d = Terraria.Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, Terraria.Dust.dustWater());
                d.velocity *= 0.2f;
                d.velocity -= NPC.velocity * 0.2f;
            }
            NPC.alpha -= 5;
            if (NPC.alpha < 0) {
                NPC.alpha = 0;
            }
            else {
                return;
            }
        }

        if (NPC.velocity.Y == 0) {
            int jumpTime = (int)(NPC.ai[0] % 1000f);
            int jumpType = (int)(NPC.ai[0] / 1000f);

            NPC.localAI[0] = 0f;
            NPC.ai[0]++;
            NPC.velocity.X *= 0.9f;

            if (jumpType == 2) {
                if (jumpTime > 75) {
                    if (NPC.ai[1] < 2) {
                        if (Math.Abs(NPC.ai[2] - NPC.position.X) < 10f) {
                            NPC.ai[1]++;
                            if ((int)NPC.ai[1] >= 2) {
                                NPC.direction = -NPC.direction;
                            }
                            else {
                                NPC.TargetClosest();
                            }
                        }
                        else {
                            NPC.TargetClosest();
                        }
                    }
                    else {
                        NPC.ai[1]++;
                        if ((int)NPC.ai[1] >= 5) {
                            NPC.ai[1] = 0f;
                        }
                    }

                    NPC.velocity.X = 10f * NPC.direction;
                    NPC.velocity.Y = -2f;
                    if (Main.player[NPC.target].position.Y + 160 < NPC.position.Y) {
                        NPC.velocity.Y -= 2f;
                    }
                    if (Main.player[NPC.target].position.Y + 320 < NPC.position.Y) {
                        if (Math.Abs(Main.player[NPC.target].position.X - NPC.position.X) < 320f) {
                            NPC.velocity.Y -= 10f;
                            NPC.velocity.X *= 0.4f;
                        }
                        else {
                            NPC.velocity.Y -= 4f;
                            NPC.velocity.X *= 0.7f;
                        }
                    }
                    NPC.ai[0] = 0f;
                    NPC.ai[2] = NPC.position.X;
                    NPC.ai[3] = NPC.velocity.X;
                }
            }
            else if (jumpTime > 50) {
                if (NPC.ai[1] < 2) {
                    if (Math.Abs(NPC.ai[2] - NPC.position.X) < 10f) {
                        NPC.ai[1]++;
                        if ((int)NPC.ai[1] >= 2) {
                            NPC.direction = -NPC.direction;
                        }
                        else {
                            NPC.TargetClosest();
                        }
                    }
                    else {
                        NPC.TargetClosest();
                    }
                }
                else {
                    NPC.ai[1]++;
                    if ((int)NPC.ai[1] >= 5) {
                        NPC.ai[1] = 0f;
                    }
                }

                NPC.velocity.X = 7f * NPC.direction;
                NPC.velocity.Y = -4f;
                if (Main.player[NPC.target].position.Y + 160 < NPC.position.Y) {
                    NPC.velocity.Y -= 2f;
                }
                if (Main.player[NPC.target].position.Y + 320 < NPC.position.Y) {
                    NPC.velocity.Y -= 4f;
                    NPC.velocity.X *= 0.7f;
                }
                jumpType++;
                NPC.ai[0] = jumpType * 1000f;
                NPC.ai[2] = NPC.position.X;
                NPC.ai[3] = NPC.velocity.X;
            }
        }
        else if (NPC.velocity.Y < 0f) {
            NPC.velocity.X += NPC.ai[3] / 20f;
            if (NPC.ai[3] > 0f) {
                if (NPC.velocity.X > NPC.ai[3]) {
                    NPC.velocity.X = NPC.ai[3];
                }
            }
            else {
                if (NPC.velocity.X < NPC.ai[3]) {
                    NPC.velocity.X = NPC.ai[3];
                }
            }

            if (!NPC.HasValidTarget) {
                NPC.TargetClosest(faceTarget: false);
            }

            if (Main.expertMode && NPC.wet && NPC.localAI[0] < 500f && NPC.velocity.Y < 0f && NPC.HasValidTarget && Main.player[NPC.target].position.Y < NPC.position.Y) {
                NPC.localAI[0]++;
                NPC.velocity.Y = Math.Max(NPC.velocity.Y - 0.5f, -8f);
            }
        }
        NPC.spriteDirection = NPC.direction;
    }

    public override void FindFrame(int frameHeight) {
        if (NPC.IsABestiaryIconDummy) {
            NPC.alpha = 0;
        }
        if (NPC.velocity.Y != 0) {
            NPC.frame.Y = frameHeight * 4;
        }
        else {
            NPC.frameCounter++;
            if (NPC.ai[0] % 1000f > 40f) {
                NPC.frameCounter++;
            }
            if (NPC.frameCounter > 4) {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
            }
            if (NPC.frame.Y >= frameHeight * 3) {
                NPC.frame.Y = 0;
            }
        }
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo) {
        if (Main.rand.NextBool(Main.expertMode ? 1 : 3)) {
            target.AddBuff(BuffID.Confused, 120);
        }
    }

    public override bool? CanFallThroughPlatforms() {
        return NPC.HasValidTarget && Main.player[NPC.target].position.Y > NPC.Bottom.Y;
    }

    public static int GetFishingChance(in FishingAttempt attempt) {
        int chance = 50;
        if (!Main.dayTime) {
            chance = 25;

            if (Main.bloodMoon) {
                chance = 6;
                if (attempt.playerFishingConditions.PoleItemType == ItemID.BloodFishingRod) {
                    chance = 3;
                }
            }
        }
        return chance;
    }
}