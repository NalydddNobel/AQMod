using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Potions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.CrabSeason
{
    public class CoconutCrab : AIFighter, IDecideFallThroughPlatforms
    {
        public override bool KnocksOnDoors => false;

        public override void SetDefaults()
        {
            npc.width = 26;
            npc.height = 26;
            npc.lifeMax = 115;
            npc.damage = 50;
            npc.knockBackResist = 0.15f;
            //npc.noGravity = true;
            npc.aiStyle = -1;
            npc.value = Item.buyPrice(silver: 3);
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath8;
            npc.SetLiquidSpeed(water: 0.9f);

            banner = npc.type;
            bannerItem = ModContent.ItemType<Items.Placeable.Banners.SoliderCrabsBanner>();
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
            base.AI();
            npc.rotation += npc.velocity.X * 0.02f;
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