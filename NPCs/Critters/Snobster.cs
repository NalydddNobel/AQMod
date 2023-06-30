using Aequus.Common.NPCs;
using Aequus.Tiles.CrabCrevice;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Aequus.NPCs.Critters {
    public class Snobster : ModNPC {
        public override void SetStaticDefaults() {
            Main.npcFrameCount[NPC.type] = 4;
            Main.npcCatchable[NPC.type] = true;

            NPCID.Sets.CountsAsCritter[Type] = true;
        }

        public override void SetDefaults() {
            NPC.width = 12;
            NPC.height = 12;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 5;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.npcSlots = 0.5f;
            NPC.catchItem = (short)ModContent.ItemType<SnobsterItem>();
            NPC.friendly = true;
        }

        public override void HitEffect(NPC.HitInfo hit) {
            for (int i = 0; i < 5; i++) {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.t_Slime, NPC.velocity.X, NPC.velocity.Y, 0, Helper.ColorGreenSlime);
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
            this.CreateEntry(database, bestiaryEntry)
                .AddSpawn(BestiaryBuilder.OceanBiome);
        }

        public override bool? CanBeHitByItem(Player player, Item item) {
            return true;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile) {
            return true;
        }

        public override void AI() {
            if (NPC.collideY && NPC.velocity.Y >= 0f)
                NPC.localAI[0] = 0f;
            if (NPC.collideX && NPC.velocity.X.Abs() > 1f)
                NPC.velocity.X = -NPC.oldVelocity.X * 0.9f;
            if (NPC.velocity.Y == 0) {
                NPC.ai[0]++;
                NPC.velocity.X *= 0.9f;
                if (NPC.ai[0] > 50f) {
                    NPC.ai[0] = Main.rand.Next(-10, 10);
                    NPC.direction = Main.rand.NextBool() ? -1 : 1;
                    NPC.spriteDirection = NPC.direction;
                    NPC.velocity.Y = -7f;
                    NPC.velocity.X = 4f * NPC.direction;
                    NPC.localAI[0] = 1f;
                }
            }
        }

        public override void FindFrame(int frameHeight) {
            if ((int)NPC.localAI[0] == 1) {
                if (NPC.velocity.Y < -0.1f) {
                    NPC.frame.Y = frameHeight * 2;
                }
                else if (NPC.velocity.Y > 0.1f) {
                    NPC.frame.Y = frameHeight * 3;
                }
            }
            else {
                NPC.frameCounter++;
                if (NPC.frameCounter < 6.0) {
                    NPC.frame.Y = 0;
                }
                else {
                    if (NPC.frameCounter >= 12.0)
                        NPC.frameCounter = 0.0;
                    NPC.frame.Y = frameHeight;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) {
            float oceanChance = SpawnCondition.Ocean.Chance;
            if (spawnInfo.SpawnTileType == ModContent.TileType<SedimentaryRockTile>()) {
                oceanChance += 1.5f;
            }
            float chance = oceanChance * 0.1f * (SpawnCondition.TownCritter.Chance * 5f + 1f);
            if (AequusWorld.downedCrabson)
                chance *= 5f;
            return chance;
        }
    }

    public class SnobsterItem : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults() {
            Item.DefaultToCapturedCritter((short)ModContent.NPCType<Snobster>());
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 5);
        }
    }
}