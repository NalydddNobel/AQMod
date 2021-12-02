using AQMod.Items.Placeable.Banners;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.AtmosphericEvent
{
    public class RedSprite : ModNPC
    {
        private bool _setupFrame;
        public int frameIndex;
        public const int FramesX = 3;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 8;
        }

        public override void SetDefaults()
        {
            npc.width = 46;
            npc.height = 36;
            npc.lifeMax = 2750;
            npc.damage = 45;
            npc.defense = 15;
            npc.knockBackResist = 0f;
            npc.HitSound = SoundID.NPCHit30;
            npc.DeathSound = SoundID.NPCDeath33;
            npc.aiStyle = -1;
            npc.noGravity = true;
            npc.value = Item.buyPrice(silver: 30);
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.CursedInferno] = true;
            npc.buffImmune[BuffID.Ichor] = true;
            npc.buffImmune[BuffID.ShadowFlame] = true;
            npc.buffImmune[BuffID.Bleeding] = true;
            banner = npc.type;
            bannerItem = ModContent.ItemType<RedSpriteBanner>();
            npc.noTileCollide = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
        }

        public override void AI()
        {
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
            npc.frameCounter += 1.0d;
            if (npc.frameCounter > 4.0d)
            {
                npc.frameCounter = 0.0d;
                npc.frame.Y += frameHeight;
            }

            npc.frame.Y = frameIndex * frameHeight;

            if (npc.frame.Y >= frameHeight * Main.npcFrameCount[npc.type])
            {
                npc.frame.X += npc.frame.Width;
                if (npc.frame.X >= npc.frame.Width * FramesX)
                {
                    npc.frame.X = 0;
                }
                npc.frame.Y = 0;
            }
        }

        public override void NPCLoot()
        {
            Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Materials.Energies.AtmosphericEnergy>(), Main.rand.Next(2) + 2);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            return true;
        }
    }
}