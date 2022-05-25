using Aequus.Common;
using Aequus.Items.Armor.Vanity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters
{
    public class BreadOfCthulhu : ModNPC
    {
        private const int SPAWNRECTANGLE_SIZE = 20;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
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
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry)
                .AddMainSpawn(BestiaryBuilder.CavernsBiome);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            this.CreateLoot(npcLoot)
                .Add<BreadOfCthulhuMask>(chance: 7, stack: 1);
        }

        public override void AI()
        {
            if (NPC.justHit)
            {
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
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
                        NPC.velocity.Y = -1f;
                        if (Main.player[NPC.target].position.Y + 320 > NPC.position.Y)
                        {
                            NPC.velocity.Y -= 2f;
                        }
                        if (Main.player[NPC.target].position.Y + 480 > NPC.position.Y)
                        {
                            NPC.velocity.Y -= 2f;
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
                    if (Main.player[NPC.target].position.Y + 160 > NPC.position.Y)
                    {
                        NPC.velocity.Y -= 2f;
                    }
                    if (Main.player[NPC.target].position.Y + 320 > NPC.position.Y)
                    {
                        NPC.velocity.Y -= 2f;
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
                NPC.DeathGore("BloodMimic_0");
                NPC.DeathGore("BloodMimic_1");
                NPC.DeathGore("BloodMimic_1").rotation += MathHelper.Pi;
            }
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood, hitDirection * 2);
        }

        public override void FindFrame(int frameHeight)
        {
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
                target.AddBuff(BuffID.Bleeding, 120);
            }
        }
    }
}