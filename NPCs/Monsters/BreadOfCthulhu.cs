using Aequus.Content.Necromancy;
using Aequus.Items.Vanity.Masks;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Fishing.Misc;
using Aequus.Items.Placeable.Banners;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters
{
    public class BreadOfCthulhu : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            ItemID.Sets.KillsToBanner[BannerItem] = 10;
            NecromancyDatabase.NPCs.Add(Type, GhostInfo.One);
        }

        public override void SetDefaults()
        {
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
            NPC.SetLiquidSpeeds(water: 1f);
            NPC.npcSlots = 4f;

            Banner = Type;
            BannerItem = ModContent.ItemType<BreadOfCthulhuBanner>();
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry)
                .AddMainSpawn(BestiaryBuilder.CavernsBiome)
                .QuickUnlock();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            this.CreateLoot(npcLoot)
                .Add<BreadOfCthulhuMask>(chance: 7, stack: 1)
                .Add(new Conditions.IsCorruption(), ItemID.Vertebrae, chance: 1, stack: 1)
                .Add(new Conditions.IsCrimson(), ItemID.RottenChunk, chance: 1, stack: 1)
                .Add(ItemID.OldShoe, chance: 2, stack: (1, 2))
                .Add(ItemID.TinCan, chance: 2, stack: (1, 2))
                .Add(ItemID.FishingSeaweed, chance: 2, stack: (1, 2))
                .Add<PlasticBottle>(chance: 2, stack: (1, 2))
                .Add<Driftwood>(chance: 2, stack: (1, 2))
                .Add(ItemID.TrifoldMap, chance: 5, stack: 1)
                .Add<Baguette>(chance: 5, stack: 1);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            if (NPC.life <= 0)
            {
                for (int i = 0; i < 30; i++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood, hitDirection * 2);
                }
                for (int i = 0; i < 30; i++)
                {
                    var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.FoodPiece,
                        newColor: new Color(Main.rand.Next(20, 100), 200, 20, 200));
                    d.velocity = new Vector2(Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-3f, -6f));
                }
                NPC.DeathGore("BreadOfCthulhu_0", new Vector2(NPC.width / 2f * NPC.direction, 0f));
                NPC.DeathGore("BreadOfCthulhu_1", new Vector2(NPC.width / 2f * -NPC.direction, 0f));
                NPC.DeathGore("BreadOfCthulhu_2");
            }
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood, hitDirection * 2);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return NPC.alpha <= 0;
        }

        public override void AI()
        {
            if (NPC.justHit)
            {
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
            }

            if (NPC.alpha > 0)
            {
                if (NPC.ai[0] == 0)
                {
                    if (!NPC.HasValidTarget)
                    {
                        NPC.TargetClosest();
                    }
                    else
                    {
                        NPC.FaceTarget();
                    }
                    NPC.velocity.Y = -5f;
                    NPC.velocity.X = 3f * NPC.direction;
                    NPC.ai[0] = 1f;
                    int waterDust = Dust.dustWater();
                    for (int i = 0; i < 50; i++)
                    {
                        var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, waterDust);
                        d.velocity = new Vector2(Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-3f, -6f));
                    }
                }
                if (Main.rand.NextBool(10))
                {
                    var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, Dust.dustWater());
                    d.velocity *= 0.2f;
                    d.velocity -= NPC.velocity * 0.2f;
                }
                NPC.alpha -= 5;
                if (NPC.alpha < 0)
                {
                    NPC.alpha = 0;
                }
                else
                {
                    return;
                }
            }

            if (NPC.velocity.Y == 0)
            {
                int jumpTime = (int)(NPC.ai[0] % 1000f);
                int jumpType = (int)(NPC.ai[0] / 1000f);

                NPC.ai[0]++;
                NPC.velocity.X *= 0.9f;

                if (jumpType == 2)
                {
                    if (jumpTime > 75)
                    {
                        if (NPC.ai[1] < 2)
                        {
                            if ((NPC.ai[2] - NPC.position.X).Abs() < 10f)
                            {
                                NPC.ai[1]++;
                                if ((int)NPC.ai[1] >= 2)
                                {
                                    NPC.direction = -NPC.direction;
                                }
                                else
                                {
                                    NPC.TargetClosest();
                                }
                            }
                            else
                            {
                                NPC.TargetClosest();
                            }
                        }
                        else
                        {
                            NPC.ai[1]++;
                            if ((int)NPC.ai[1] >= 5)
                            {
                                NPC.ai[1] = 0f;
                            }
                        }

                        NPC.velocity.X = 10f * NPC.direction;
                        NPC.velocity.Y = -2f;
                        if (Main.player[NPC.target].position.Y + 160 < NPC.position.Y)
                        {
                            NPC.velocity.Y -= 2f;
                        }
                        if (Main.player[NPC.target].position.Y + 320 < NPC.position.Y)
                        {
                            if ((Main.player[NPC.target].position.X - NPC.position.X).Abs() < 320f)
                            {
                                NPC.velocity.Y -= 10f;
                                NPC.velocity.X *= 0.4f;
                            }
                            else
                            {
                                NPC.velocity.Y -= 4f;
                                NPC.velocity.X *= 0.7f;
                            }
                        }
                        NPC.ai[0] = 0f;
                        NPC.ai[2] = NPC.position.X;
                        NPC.ai[3] = NPC.velocity.X;
                    }
                }
                else if (jumpTime > 50)
                {
                    if (NPC.ai[1] < 2)
                    {
                        if ((NPC.ai[2] - NPC.position.X).Abs() < 10f)
                        {
                            NPC.ai[1]++;
                            if ((int)NPC.ai[1] >= 2)
                            {
                                NPC.direction = -NPC.direction;
                            }
                            else
                            {
                                NPC.TargetClosest();
                            }
                        }
                        else
                        {
                            NPC.TargetClosest();
                        }
                    }
                    else
                    {
                        NPC.ai[1]++;
                        if ((int)NPC.ai[1] >= 5)
                        {
                            NPC.ai[1] = 0f;
                        }
                    }

                    NPC.velocity.X = 7f * NPC.direction;
                    NPC.velocity.Y = -4f;
                    if (Main.player[NPC.target].position.Y + 160 < NPC.position.Y)
                    {
                        NPC.velocity.Y -= 2f;
                    }
                    if (Main.player[NPC.target].position.Y + 320 < NPC.position.Y)
                    {
                        NPC.velocity.Y -= 4f;
                        NPC.velocity.X *= 0.7f;
                    }
                    jumpType++;
                    NPC.ai[0] = jumpType * 1000f;
                    NPC.ai[2] = NPC.position.X;
                    NPC.ai[3] = NPC.velocity.X;
                }
            }
            else if (NPC.velocity.Y < 0f)
            {
                NPC.velocity.X += NPC.ai[3] / 20f;
                if (NPC.ai[3] > 0f)
                {
                    if (NPC.velocity.X > NPC.ai[3])
                    {
                        NPC.velocity.X = NPC.ai[3];
                    }
                }
                else
                {
                    if (NPC.velocity.X < NPC.ai[3])
                    {
                        NPC.velocity.X = NPC.ai[3];
                    }
                }
            }
            NPC.spriteDirection = NPC.direction;
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                NPC.alpha = 0;
            }
            if (NPC.velocity.Y != 0)
            {
                NPC.frame.Y = frameHeight * 4;
            }
            else
            {
                NPC.frameCounter++;
                if ((NPC.ai[0] % 1000f) > 40f)
                {
                    NPC.frameCounter++;
                }
                if (NPC.frameCounter > 4)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                }
                if (NPC.frame.Y >= frameHeight * 3)
                {
                    NPC.frame.Y = 0;
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.expertMode)
            {
                target.AddBuff(BuffID.Bleeding, 600);
            }
            if (Main.rand.NextBool(Main.expertMode ? 2 : 8))
            {
                target.AddBuff(BuffID.Confused, 120);
            }
        }
    }
}