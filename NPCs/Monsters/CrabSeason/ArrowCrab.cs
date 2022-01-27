using AQMod.Items.Foods;
using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.CrabSeason
{
    public class ArrowCrab : ModNPC
    {
        private const int OrientationTileDown = 0;
        private const int OrientationTileUp = 1;
        private const int OrientationTileRight = 2;
        private const int OrientationTileLeft = 3;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.width = 12;
            npc.height = 12;
            npc.lifeMax = 50;
            npc.damage = 30;
            npc.knockBackResist = 0.4f;
            npc.noGravity = true;
            npc.aiStyle = -1;
            npc.value = Item.buyPrice(silver: 2);
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath8;
            banner = npc.type;
            bannerItem = ModContent.ItemType<Items.Placeable.Banners.ArrowCrabBanner>();
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int i = 0; i < 20; i++)
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
            var collisionBox = new Rectangle((int)npc.position.X - 4, (int)npc.position.Y - 4, npc.width + 8, npc.height + 8);
            if (npc.collideX || npc.collideY)
            {
                npc.oldVelocity = Vector2.Zero;
                npc.velocity = Vector2.Zero;
                npc.ai[1] = Main.rand.Next(30, 45);
                InternalUpdateOrientation();
            }
        }

        private void InternalUpdateOrientation()
        {
            int x = (int)(npc.position.X + npc.width / 2);
            int y = (int)(npc.position.Y + npc.height / 2);
            if (Main.tile[x, y + 1].active() && (Main.tile[x, y + 1].Solid() || Main.tile[x, y + 1].SolidTop()))
            {
                npc.ai[0] = OrientationTileDown;
            }
            else if (Main.tile[x, y - 1].active() && (Main.tile[x, y - 1].Solid() || Main.tile[x, y - 1].SolidTop()))
            {
                npc.ai[0] = OrientationTileUp;
            }
            else if (Main.tile[x + 1, y].active() && (Main.tile[x + 1, y].Solid() || Main.tile[x + 1, y].SolidTop()))
            {
                npc.ai[0] = OrientationTileRight;
            }
            else if (Main.tile[x - 1, y].active() && (Main.tile[x - 1, y].Solid() || Main.tile[x - 1, y].SolidTop()))
            {
                npc.ai[0] = OrientationTileLeft;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(8))
            {
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.PickBreak>(), 480);
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.Biomes().zoneCrabCrevice)
                return 0.2f;
            return 0f;
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool())
                Item.NewItem(npc.getRect(), ModContent.ItemType<CrabShell>());
            if (Main.rand.NextBool())
                Item.NewItem(npc.getRect(), ModContent.ItemType<AquaticEnergy>());
            if (Main.rand.NextBool(10))
                Item.NewItem(npc.getRect(), ModContent.ItemType<CheesePuff>());
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            var center = npc.Center;
            var drawPosition = npc.Center - Main.screenPosition;
            Main.spriteBatch.Draw(Main.npcTexture[npc.type], drawPosition, npc.frame, drawColor, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            return false;
        }
    }
}