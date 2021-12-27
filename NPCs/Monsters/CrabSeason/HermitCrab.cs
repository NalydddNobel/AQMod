using AQMod.Common;
using AQMod.Items.Armor;
using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Vanities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.CrabSeason
{
    public class HermitCrab : AIFighter, IDecideFallThroughPlatforms
    {
        public const int FramesX = 2;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 24;
            npc.height = 24;
            npc.lifeMax = 50;
            npc.damage = 20;
            npc.knockBackResist = 0.02f;
            npc.aiStyle = -1;
            npc.value = Item.buyPrice(silver: 5);
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath8;
            npc.gfxOffY = -4;
            npc.behindTiles = true;
            banner = npc.type;
            bannerItem = ModContent.ItemType<Items.Placeable.Banners.HermitCrabBanner>();
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, npc.velocity.X, npc.velocity.X, 0, default(Color), 1.25f);
                }
                var center = npc.Center;
                var offset = new Vector2(6f * npc.direction, 0f);
                int type = mod.GetGoreSlot("Gores/HermitCrab_2");
                Gore.NewGore(center + offset, npc.velocity, type);
                Gore.NewGore(center + offset + new Vector2(2f * npc.direction, 0f), npc.velocity, type);
                Gore.NewGore(center + new Vector2(-8f * npc.direction, 0f), npc.velocity, mod.GetGoreSlot("Gores/HermitCrab_3"));
                Gore.NewGore(center, npc.velocity, mod.GetGoreSlot("Gores/HermitCrab_0"));
                Gore.NewGore(center, npc.velocity, mod.GetGoreSlot("Gores/HermitCrab_4"));
                switch ((int)npc.localAI[0])
                {
                    default:
                    Gore.NewGore(center, npc.velocity, mod.GetGoreSlot("Gores/HermitCrab_1"));
                    break;
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

        public bool Aggro()
        {
            if (npc.life < npc.lifeMax)
                return true;
            npc.TargetClosest();
            if (npc.HasValidTarget)
            {
                var target = Main.player[npc.target];
                float detectionDistance = 300f;
                if (!Collision.CanHitLine(npc.position, npc.width, npc.height, target.position, target.width, target.height))
                    detectionDistance /= 2f;
                if ((npc.Center - Main.player[npc.target].Center).Length() < detectionDistance)
                    return true;
            }
            return false;
        }

        public override bool KnocksOnDoors => false;
        public override float SpeedCap => 1.5f;

        public override bool PreAI()
        {
            if ((int)npc.localAI[0] == 0)
            {
                npc.localAI[0] = 1f;
                npc.frame.Width /= FramesX;
            }
            if (Aggro())
            {
                return true;
            }
            else
            {
                npc.velocity.X *= 0.95f;
                return false;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(8))
            {
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.PickBreak>(), 600);
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (Content.LegacyWorldEvents.CrabSeason.CrabSeason.Active && spawnInfo.spawnTileY < Main.worldSurface && SpawnCondition.OceanMonster.Active)
                return SpawnCondition.OceanMonster.Chance * 0.6f;
            return 0f;
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(20))
                Item.NewItem(npc.getRect(), ModContent.ItemType<FishyFins>());
            if (Main.rand.NextBool(10))
                Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Foods.CrabSeason.CheesePuff>());
            if (Main.rand.NextBool(8))
                Item.NewItem(npc.getRect(), ModContent.ItemType<HermitShell>());
            if (Main.rand.NextBool())
                Item.NewItem(npc.getRect(), ModContent.ItemType<AquaticEnergy>());
            if (Main.rand.NextBool())
                Item.NewItem(npc.getRect(), ModContent.ItemType<CrabShell>());
        }

        public override void FindFrame(int frameHeight)
        {
            if (npc.velocity.Y > 1f)
            {
                npc.frameCounter = 0;
                npc.frame.Y = frameHeight * 2;
            }
            else if (npc.velocity.X.Abs() > 1f)
            {
                npc.frameCounter += npc.velocity.X.Abs() * 0.5f;
                if (npc.frameCounter > 6)
                {
                    npc.frame.Y += frameHeight;
                    npc.frameCounter = 0;
                }
            }
            else
            {
                npc.frameCounter = 0;
            }
            if (npc.frame.Y >= frameHeight * 4)
                npc.frame.Y = 0;
        }

        public Rectangle getShellFrame()
        {
            return new Rectangle(npc.frame.X + npc.frame.Width * (int)npc.localAI[0], npc.frame.Y, npc.frame.Width, npc.frame.Height);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            var center = npc.Center;
            var drawPosition = npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY);
            var shellFrame = getShellFrame();
            var origin = npc.frame.Size() / 2f;
            var effects = npc.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(Main.npcTexture[npc.type], drawPosition, npc.frame, drawColor, npc.rotation, origin, npc.scale, effects, 0f);
            Main.spriteBatch.Draw(Main.npcTexture[npc.type], drawPosition, shellFrame, drawColor, npc.rotation, origin, npc.scale, effects, 0f);
            return false;
        }

        bool IDecideFallThroughPlatforms.Decide()
        {
            if (!npc.HasValidTarget)
            {
                return false;
            }
            return Main.player[npc.target].position.Y > npc.position.Y;
        }
    }
}