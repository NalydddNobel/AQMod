using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Potions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.CrabSeason
{
    public class SoldierCrab : ModNPC
    {
        public const int FramesX = 2;
        public const int Phase_ShieldBash = 0;
        public const int Phase_ClawSnip = 1;
        public const int Phase_Jump = 2;

        private bool _setupFrame;
        public int frameIndex;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 10;
        }

        public override void SetDefaults()
        {
            npc.width = 30;
            npc.height = 20;
            npc.lifeMax = 28;
            npc.damage = 50;
            npc.knockBackResist = 0.75f;
            npc.noGravity = true;
            npc.aiStyle = -1;
            npc.value = Item.buyPrice(silver: 3);
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath8;
            npc.behindTiles = true;
            npc.gfxOffY = 4f;

            banner = npc.type;
            bannerItem = ModContent.ItemType<Items.Placeable.Banners.SoliderCrabsBanner>();
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, npc.velocity.X, npc.velocity.X, 0, default(Color), 1.25f);
                }
            }
            else
            {
                for (int i = 0; i < damage / 5; i++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, npc.velocity.X, npc.velocity.X, 0, default(Color), 0.9f);
                }
            }
        }

        public override void AI()
        {
            if (npc.velocity.Y == 0f && (int)npc.ai[0] != -1)
            {
                npc.velocity.X *= 0.9f;
                if (!npc.HasValidTarget)
                {
                    npc.TargetClosest(faceTarget: true);
                    if (!npc.HasValidTarget)
                    {
                        npc.ai[0] = -1f;
                    }
                }
                npc.direction = (npc.position.X + npc.width / 2f) < (Main.player[npc.target].position.X + Main.player[npc.target].width / 2f) ? 1 : -1;
                if ((int)npc.ai[0] == Phase_ShieldBash)
                {
                    npc.ai[1]++;
                    if (Main.netMode != NetmodeID.Server && (int)npc.ai[1] == 20f)
                    {
                        Main.PlaySound(SoundID.Tink, (int)npc.position.X, (int)npc.position.Y, Style: 1);
                    }
                    if (npc.ai[1] > 24f)
                    {
                        npc.ai[2] = (int)npc.ai[0];
                        npc.ai[1] = 0f;
                        npc.ai[0] = Phase_Jump;
                    }
                }
                else
                {
                    if (npc.ai[1] == 0f)
                    {
                        npc.ai[1]++;
                        npc.TargetClosest(faceTarget: true);
                        npc.velocity.Y = -8f;
                        npc.velocity.X = 5f * npc.direction;
                    }
                    if (npc.velocity.Y == 0f)
                    {
                        npc.ai[1]++;
                        if (npc.ai[1] >= 20f)
                        {
                            npc.ai[0] = (int)npc.ai[2];
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                        }
                    }
                }
            }
            if ((int)npc.ai[0] == -1)
            {
                npc.noTileCollide = true;
                npc.velocity.X *= 0.99f;
            }
            if (npc.velocity.Y < 16f)
            {
                npc.TargetClosest(faceTarget: false);
                npc.velocity.Y += 0.55f;
                if (npc.velocity.Y > 16f)
                    npc.velocity.Y = 16f;
            }
            npc.spriteDirection = npc.direction;
        }

        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            if (!_setupFrame)
            {
                _setupFrame = true;
                npc.frame.Width = npc.frame.Width / FramesX;
            }

            if (npc.velocity.Y != 0f)
            {
                frameIndex = npc.velocity.Y > 10f ? 12 : 11;
            }
            else if ((int)npc.ai[0] != -1)
            {
                if ((int)npc.ai[0] == Phase_ShieldBash)
                {
                    if ((int)npc.ai[1] == 20)
                    {
                        frameIndex = 5;
                    }
                    else if ((int)npc.ai[1] > 20)
                    {
                        frameIndex = 0;
                    }
                    else
                    {
                        frameIndex = (int)npc.ai[1] / 4;
                    }
                }
                else
                {
                    frameIndex = 0;
                }
            }
            else
            {
                frameIndex = 0;
            }

            npc.frame.Y = frameIndex * frameHeight;

            if (npc.frame.Y >= frameHeight * Main.npcFrameCount[npc.type])
            {
                npc.frame.X = npc.frame.Width * (npc.frame.Y / (frameHeight * Main.npcFrameCount[npc.type]));
                npc.frame.Y = npc.frame.Y % (frameHeight * Main.npcFrameCount[npc.type]);
            }
            else
            {
                npc.frame.X = 0;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(8))
            {
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.PickBreak>(), 480);
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.player.Biomes().zoneCrabCrevice ? 0.2f : 0f;

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(10))
                Item.NewItem(npc.getRect(), ModContent.ItemType<CheesePuff>());
            if (Main.rand.NextBool(3))
                Item.NewItem(npc.getRect(), ModContent.ItemType<CrabShell>());
            if (Main.rand.NextBool())
                Item.NewItem(npc.getRect(), ModContent.ItemType<AquaticEnergy>());
        }
    }
}