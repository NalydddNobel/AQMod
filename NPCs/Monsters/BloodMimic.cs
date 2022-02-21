using AQMod.Items.Accessories.Shields;
using AQMod.Items.Potions;
using AQMod.Items.Tools;
using AQMod.Items.Tools.GrapplingHooks;
using AQMod.NPCs.AIs;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters
{
    public class BloodMimic : AIMimic
    {
        internal static bool _usePlayerRectangle = true;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.width = 24;
            npc.height = 24;
            npc.aiStyle = -1;
            npc.damage = 25;
            npc.defense = 10;
            npc.lifeMax = 125;
            npc.HitSound = SoundID.NPCHit16;
            npc.DeathSound = SoundID.NPCDeath41;
            npc.value = Item.buyPrice(silver: 50);
            npc.knockBackResist = 0.4f;
            npc.rarity = 2;
            banner = Item.NPCtoBanner(NPCID.Mimic);
            bannerItem = ItemID.MimicBanner;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            if (AQMod.calamityMod.IsActive)
            {
                npc.lifeMax = (int)(npc.lifeMax * 1.5f);
                npc.damage = (int)(npc.damage * 1.25f);
                npc.defense *= 2;
            }
        }

        protected override int GetJumpTimer() => npc.ai[1] == 0f ? 5 : 10;

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection * 2);
                }
                Gore.NewGore(npc.Center, npc.velocity, mod.GetGoreSlot("Gores/BloodMimic_0"));
                Gore.NewGore(npc.Center, npc.velocity, mod.GetGoreSlot("Gores/BloodMimic_1"));
                Main.gore[Gore.NewGore(npc.Center, npc.velocity, mod.GetGoreSlot("Gores/BloodMimic_1"))].rotation = MathHelper.Pi;
            }
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection * 2);
            }
        }

        protected override void Jump()
        {
            if (npc.ai[1] == 2f)
            {
                npc.velocity.X = npc.direction * 5f;
                npc.velocity.Y = -4f;
                npc.ai[1] = 0f;
            }
            else
            {
                npc.velocity.X = npc.direction * 7f;
                npc.velocity.Y = -2f;
            }
        }

        private const int SPAWN_RECT_SIZE = 20;

        private const int SPAWN_RECT_SIZE_HALF = SPAWN_RECT_SIZE / 2;

        private const int NPC_SIZE_REFERENCE = 24;

        public override bool PreAI()
        {
            if (npc.ai[0] < 0f)
            {
                npc.ai[0]++;
                npc.velocity.X = 0f;
            }
            return true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return !Main.dayTime && Main.bloodMoon && !NPC.AnyNPCs(ModContent.NPCType<BloodMimic>()) && spawnInfo.spawnTileY < Main.worldSurface ? 0.02f : 0f;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.immuneTime /= 2;
            target.starCloak = false;
            if (target.thorns <= 0f)
            {
                npc.velocity *= -0.98f;
                npc.position += npc.velocity;
                npc.netUpdate = true;
            }
            target.AddBuff(BuffID.Bleeding, 60);
        }

        public override int SpawnNPC(int tileX, int tileY)
        {
            tileY -= 1;
            int spawnCount = Main.rand.Next(3, 8);
            int lastX = tileX;
            int lastY = tileY;
            Player nearest = Main.player[Player.FindClosest(new Vector2(tileX, tileY), 16, 16)];
            Rectangle nearestPlayerSights = _usePlayerRectangle
                ? new Rectangle((int)nearest.position.X / 16 - NPC.safeRangeX, (int)nearest.position.Y / 16 - NPC.safeRangeY, NPC.safeRangeX * 2, NPC.safeRangeY * 2)
                : new Rectangle(0, 0, 1, 1);
            int[] invalidX = new int[(spawnCount + 1) * 4];
            int[] invalidY = new int[(spawnCount + 1) * 4];
            invalidX[spawnCount] = tileX;
            invalidX[spawnCount + 1] = tileX + 1;
            invalidX[spawnCount + 2] = tileX;
            invalidX[spawnCount + 3] = tileX + 1;
            invalidY[spawnCount] = tileY;
            invalidY[spawnCount + 1] = tileY - 1;
            invalidY[spawnCount + 2] = tileY - 1;
            invalidY[spawnCount + 3] = tileY;
            for (int i = 0; i < spawnCount; i++)
            {
                var rectangle = new Rectangle(lastX - SPAWN_RECT_SIZE_HALF, lastY - SPAWN_RECT_SIZE_HALF, SPAWN_RECT_SIZE, SPAWN_RECT_SIZE);
                for (int j = 0; j < 50; j++)
                {
                    int x = Main.rand.Next(rectangle.X, rectangle.X + SPAWN_RECT_SIZE);
                    int y = Main.rand.Next(rectangle.Y, rectangle.Y + SPAWN_RECT_SIZE);
                    bool contactsMimic = false;
                    for (int k = 0; k < spawnCount * 4; k++)
                    {
                        if (x == invalidX[k] && y == invalidY[k])
                        {
                            contactsMimic = true;
                            break;
                        }
                    }
                    if (contactsMimic || new Rectangle(x, y, 1, 1).Intersects(nearestPlayerSights))
                        continue;
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (!tile.active() && !Main.wallHouse[tile.wall] &&
                        !Framing.GetTileSafely(x + 1, y).active() && !Main.wallHouse[Main.tile[x + 1, y].wall] &&
                        !Framing.GetTileSafely(x, y - 1).active() && !Main.wallHouse[Main.tile[x, y - 1].wall] &&
                        !Framing.GetTileSafely(x + 1, y - 1).active() && !Main.wallHouse[Main.tile[x + 1, y - 1].wall] &&
                        Framing.GetTileSafely(x, y + 1).active() && Main.tileSolid[Main.tile[x, y + 1].type] && !Main.tile[x, y + 1].halfBrick() && Main.tile[x, y + 1].slope() == 0 &&
                        Framing.GetTileSafely(x + 1, y + 1).active() && Main.tileSolid[Main.tile[x + 1, y + 1].type] && !Main.tile[x + 1, y + 1].halfBrick() && Main.tile[x + 1, y + 1].slope() == 0)
                    {
                        int n = NPC.NewNPC(x * 16, y * 16, npc.type, 0, -20);
                        Main.npc[n].Center = new Vector2(x * 16f + 8f, y * 16f - 8f);
                        Main.npc[n].velocity = new Vector2(0f, 0f);
                        lastX = x;
                        lastY = y;
                        int index = i * 4;
                        invalidX[index] = x;
                        invalidX[index + 1] = x + 1;
                        invalidX[index + 2] = x;
                        invalidX[index + 3] = x + 1;
                        invalidY[index] = y;
                        invalidY[index + 1] = y - 1;
                        invalidY[index + 2] = y - 1;
                        invalidY[index + 3] = y;
                        break;
                    }
                }
            }
            int n1 = NPC.NewNPC(tileX * 16, tileY * 16, npc.type, 0, -20);
            Main.npc[n1].Center = new Vector2(tileX * 16f + 8f, tileY * 16f - 8f);
            return n1;
        }

        public override void NPCLoot()
        {
            Rectangle rect = npc.getRect();
            if (NPC.CountNPCS(npc.type) <= 1)
            {
                var choices = new List<int> { ModContent.ItemType<TargeoftheBlodded>(), };
                for (int i = 0; i < 3; i++)
                {
                    randDrops();
                }
                if (NPC.downedBoss3)
                    choices.Add(ModContent.ItemType<ATM>());
                if (NPC.downedPlantBoss)
                    choices.Add(ModContent.ItemType<VampireHook>());
                if (Main.rand.NextBool(8))
                    Item.NewItem(npc.getRect(), ItemID.AdhesiveBandage);
                if (Main.rand.NextBool(3))
                    Item.NewItem(npc.getRect(), ItemID.MoneyTrough);
                if (Main.rand.NextBool(3))
                    Item.NewItem(npc.getRect(), ItemID.SharkToothNecklace);
                int choice = Main.rand.Next(choices.Count);
                Item.NewItem(rect, choices[choice]);
            }
            else
            {
                randDrops();
            }
        }

        private void randDrops()
        {
            if (Main.rand.NextBool(3))
                Item.NewItem(npc.getRect(), ModContent.ItemType<SuspiciousLookingSteak>());
            if (Main.rand.NextBool(5))
                Item.NewItem(npc.getRect(), ModContent.ItemType<BloodshedPotion>());
        }
    }
}