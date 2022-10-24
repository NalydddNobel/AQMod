using Aequus.Biomes;
using Aequus.Common;
using Aequus.NPCs.AIs;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters.Jungle.Might
{
    public class Hardwood : ModNPC
    {
        public override void SetStaticDefaults()
        {
            CoreOfMight.SpawnData.Add(new BaseCore.EnemySpawn_Any4Sides(Type));
            Main.npcFrameCount[Type] = 1;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry)
                .QuickUnlock();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            this.CreateLoot(npcLoot)
                .Add(ItemID.SoulofMight, chance: 1, stack: (3, 6));
        }

        public override void SetDefaults()
        {
            NPC.width = 72;
            NPC.height = 72;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.damage = 50;
            NPC.aiStyle = -1;
            NPC.lifeMax = 3500;
            NPC.defense = 20;
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.behindTiles = true;
            this.SetBiome<OrganicEnergyBiome>();
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, new Color(10, 100, 255).ToVector3());
            NPC.realLife = NPC.whoAmI;
            SiftIntoGround();
            AequusTile.Circles.Add(new AequusTile.IndestructibleCircle() { CenterPoint = NPC.Center.ToTileCoordinates(), tileRadius = 8f, });
            NPC.TargetClosest();
            if (Main.netMode != NetmodeID.Server)
            {
                SpawnRoot();
            }
        }
        public void SpawnRoot()
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<HardwoodRoot>() && (int)Main.npc[i].ai[3] == NPC.whoAmI)
                {
                    return;
                }
            }
            var n = NPC.NewNPCDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0f, NPC.height - 20f), ModContent.NPCType<HardwoodRoot>(), ai3: NPC.whoAmI, target: NPC.target);
            n.realLife = NPC.whoAmI;
            NPC.netUpdate = true;
        }

        public void SiftIntoGround()
        {
            int x = (int)((NPC.position.X + NPC.width / 2) / 16f);
            int startY = (int)(NPC.position.Y / 16f);
            int lowestTile = startY;
            for (int i = -2; i <= 2; i++)
            {
                for (int j = startY + 4; j < startY + 6; j++)
                {
                    if (!WorldGen.InWorld(x + i, j))
                    {
                        continue;
                    }
                    if (!Main.tile[x + i, j].IsFullySolid())
                    {
                        if (lowestTile < j)
                            lowestTile = j;
                        break;
                    }
                }
            }
            if (lowestTile > startY)
            {
                NPC.velocity.Y = 8f;
            }
            else
            {
                NPC.velocity.Y = 0f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return true;
        }
    }
}
