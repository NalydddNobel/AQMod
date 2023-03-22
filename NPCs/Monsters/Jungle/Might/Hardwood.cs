using Aequus.Content.Events;
using Aequus.Projectiles.Monster;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters.Jungle.Might
{
    public class Hardwood : ModNPC
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }

        public override void SetStaticDefaults()
        {
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
            NPC.HitSound = SoundID.NPCHit7;
            NPC.DeathSound = SoundID.NPCDeath5;
            NPC.behindTiles = true;
            this.SetBiome<OrganicEnergyBiomeManager>();
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, new Color(10, 100, 255).ToVector3());
            SiftIntoGround();
            AequusTile.Circles.Add(new AequusTile.IndestructibleCircle() { CenterPoint = NPC.Center.ToTileCoordinates(), tileRadius = 8f, });
            NPC.TargetClosest();
            if (NPC.HasValidTarget && NPC.Distance(Main.player[NPC.target].Center) < 600f)
            {
                NPC.ai[0]--;
                if (NPC.ai[0] <= 0f)
                {
                    NPC.ai[0] = 600f;
                }
                if (NPC.ai[0] > 550f && (int)NPC.ai[0] % 10 == 0)
                {
                    SpawnTendrils(Main.player[NPC.target]);
                }
            }
        }

        public void SpawnTendrils(Player target)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int size = 60;
            int projectileCount = 2;
            var tilePos = target.Center.ToTileCoordinates();
            tilePos.fluffize(fluff: 10);
            var sizeCorner = new Point(tilePos.X - size / 2, tilePos.Y - size / 2);
            sizeCorner.fluffize(fluff: 10);
            var validSpots = new List<Point>();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int tileX = sizeCorner.X + i;
                    int tileY = sizeCorner.Y + j;
                    if (Main.tile[tileX, tileY].IsSolid())
                    {
                        if (Main.tile[tileX + 1, tileY].IsSolid() &&
                            Main.tile[tileX - 1, tileY].IsSolid() &&
                            Main.tile[tileX, tileY + 1].IsSolid() &&
                            Main.tile[tileX, tileY - 1].IsSolid())
                        {
                            continue;
                        }
                        var pos = new Vector2(tileX * 16f + 8f, tileY * 16f + 8f);
                        pos += Vector2.Normalize(target.Center - pos) * 18f;
                        if (Collision.CanHitLine(target.Center, 2, 2, pos, 0, 0))
                        {
                            validSpots.Add(new Point(tileX, tileY));
                        }
                    }
                }
            }
            if (validSpots.Count <= 0)
            {
                return;
            }
            if (validSpots.Count < projectileCount)
            {
                projectileCount = validSpots.Count;
            }
            var source = NPC.GetSource_FromAI();
            for (int i = 0; i < projectileCount; i++)
            {
                int random = Main.rand.Next(validSpots.Count);
                var spawnPosition = new Vector2(validSpots[random].X * 16 + 8f, validSpots[random].Y * 16f + 8f);
                Projectile.NewProjectile(source, spawnPosition, Vector2.Normalize(target.Center - spawnPosition).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)),
                    ModContent.ProjectileType<HardwoodProj>(), NPC.FixedDamage(), 1f, Main.myPlayer);
            }
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
