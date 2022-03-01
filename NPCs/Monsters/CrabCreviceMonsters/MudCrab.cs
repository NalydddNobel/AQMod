using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Potions.Foods;
using AQMod.NPCs.AIs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.CrabCreviceMonsters
{
    public class MudCrab : AIFighter, IDecideFallThroughPlatforms
    {
        public override bool KnocksOnDoors => false;
        public override float Speed => base.Speed * npc.ai[3];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 32;
            npc.height = 20;
            npc.lifeMax = 60;
            npc.damage = 50;
            npc.knockBackResist = 0.15f;
            //npc.noGravity = true;
            npc.aiStyle = -1;
            npc.value = Item.buyPrice(silver: 3);
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath8;
            npc.SetLiquidSpeed(water: 0.9f);
            npc.behindTiles = true;

            //banner = npc.type;
            //bannerItem = ModContent.ItemType<Items.Placeable.Banners.SoliderCrabsBanner>();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            if (AQMod.calamityMod.IsActive)
            {
                npc.lifeMax = (int)(npc.lifeMax * 2.5f);
                npc.damage = (int)(npc.damage * 1.5f);
                npc.defense *= 2;
            }
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
            if (npc.ai[3] == 0f)
            {
                npc.TargetClosest(faceTarget: true);
                npc.ai[3] = -1f;
            }
            else if (!Collision.CanHitLine(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) || npc.Distance(Main.player[npc.target].Center) > 1000f)
            {
                if (npc.gfxOffY < 20f)
                {
                    npc.gfxOffY += 1f;
                    if (npc.gfxOffY > 20f)
                    {
                        npc.gfxOffY = 20f;
                    }
                }
                npc.ai[3] = MathHelper.Lerp(npc.ai[3], -1f, 0.025f);
                npc.velocity.X *= 0.7f;
                return;
            }
            if (npc.gfxOffY > 2f)
            {
                npc.gfxOffY -= 1f;
                if (npc.gfxOffY < 2f)
                {
                    npc.gfxOffY = 2f;
                }
            }
            base.AI();
            npc.spriteDirection = npc.direction;
            npc.ai[3] += Main.rand.NextFloat(-0.005f, 0.015f);
            if (npc.ai[3] > 1.2f)
            {
                npc.ai[3] = 1.2f;
            }
            else if (npc.ai[3] < -1.6f)
            {
                npc.ai[3] = -1.6f;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(8))
            {
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.PickBreak>(), 480);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += npc.velocity.X.Abs() * 0.3f;
            if (npc.frameCounter > 3.0)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y >= frameHeight * Main.npcFrameCount[npc.type])
                {
                    npc.frame.Y = 0;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.player.Biomes().zoneCrabCrevice ? 0.2f : 0f;

        public override void NPCLoot()
        {
            if (Main.rand.NextBool())
                Item.NewItem(npc.getRect(), ModContent.ItemType<CrabShell>());
            if (Main.rand.NextBool(10))
                Item.NewItem(npc.getRect(), ModContent.ItemType<AquaticEnergy>());
            if (Main.rand.NextBool(25))
                Item.NewItem(npc.getRect(), ModContent.ItemType<CheesePuff>());
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            spriteBatch.Draw(Main.npcTexture[npc.type], npc.Center + new Vector2(0f, npc.gfxOffY) - Main.screenPosition,
                npc.frame, drawColor, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }

        bool IDecideFallThroughPlatforms.Decide()
        {
            return npc.HasValidTarget && Main.player[npc.target].position.Y > npc.position.Y + npc.height;
        }
    }
}