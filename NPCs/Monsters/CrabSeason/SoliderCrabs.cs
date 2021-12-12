using AQMod.Assets;
using AQMod.Common;
using AQMod.Common.WorldGeneration;
using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.CrabSeason
{
    public class SoliderCrabs : ModNPC
    {
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
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath8;
            npc.behindTiles = true;
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
            if (npc.ai[1] == 0f)
            {
                npc.width = Main.rand.Next(npc.width, npc.width * 2);
                npc.ai[1] = 1f;
                npc.netUpdate = true;
            }
            npc.TargetClosest();
            float distanceX = Main.player[npc.target].Center.X - npc.Center.X;
            if (distanceX.Abs() > 12f)
            {
                int dir = distanceX > 0f ? 1 : -1;
                npc.velocity.X = MathHelper.Lerp(npc.velocity.X, (Main.expertMode ? 5f : 2f) * dir, 0.01f);
            }
            if (Main.netMode != NetmodeID.Server && _visualCrabs != null)
            {
                foreach (var vc in _visualCrabs)
                {
                    vc.Update(npc);
                }
            }
            HandleGravity();
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
            if (Content.WorldEvents.CrabSeason.CrabSeason.Active && Collision.WetCollision(new Vector2(spawnInfo.spawnTileX * 16f - 16f, spawnInfo.spawnTileY * 16f - 16f), 48, 48) && spawnInfo.spawnTileY < Main.worldSurface && SpawnCondition.OceanMonster.Active)
                return SpawnCondition.OceanMonster.Chance * 0.4f;
            return 0f;
        }

        public override void NPCLoot()
        {
            WorldDefeats.DownedCrabSeason = true;
            if (Main.rand.NextBool(10))
                Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Foods.CrabSeason.CheesePuff>());
            if (Main.rand.NextBool(3))
                Item.NewItem(npc.getRect(), ModContent.ItemType<CrabShell>());
            if (Main.rand.NextBool())
                Item.NewItem(npc.getRect(), ModContent.ItemType<AquaticEnergy>());
        }

        private bool HandleGravity()
        {
            var center = npc.Center;
            var solidPosition = center + new Vector2(0, -4f);
            bool fall = !Collision.SolidCollision(solidPosition, 4, 4) &&
                !Collision.WetCollision(solidPosition, 4, 4);
            bool aboveTargets = center.Y >= Main.player[npc.target].Center.Y;
            if (fall || !aboveTargets)
            {
                if (aboveTargets && npc.velocity.Y > 0f)
                {
                    int X = (int)center.X;
                    int Y = (int)center.Y + 2;
                    int X2 = X / 16;
                    int Y2 = Y / 16;
                    if (AQWorldGen.ActiveAndSolid(X2, Y2))
                    {
                        if (center.Y + npc.velocity.Y > Y2 * 16f)
                        {
                            npc.position.Y = (int)(Y2 * 16f - npc.height / 2f);
                            if (npc.velocity.Y > 3f)
                            {
                                if (Main.tile[X2, Y2].type == TileID.Sand)
                                    Dust.NewDust(npc.position, npc.width, npc.width, 32);
                            }
                            npc.velocity.Y = 0f;
                            return false;
                        }
                    }
                }
                npc.velocity.Y += 0.2f;
                if (npc.velocity.Y > 8)
                    npc.velocity.Y = 8f;
                return true;
            }
            else
            {
                if (npc.velocity.Y > 0)
                {
                    npc.velocity.Y = 0f;
                }
                else
                {
                    npc.velocity.Y -= 0.1f;
                    if (npc.velocity.Y < -8)
                        npc.velocity.Y = -8f;
                }
                return false;
            }
        }

        private class SoliderCrabVisual
        {
            public SoliderCrabVisual(Vector2 position, Vector2 velocity, int frame)
            {
                this.position = position;
                this.velocity = velocity;
                this.frame = frame;
            }

            public Vector2 position;
            public Vector2 velocity;
            private int frame;

            public void Update(NPC npc)
            {
                switch (frame)
                {
                    case 0:
                    case 1:
                    frame = (int)Main.GameUpdateCount % 12 / 6;
                    break;
                }

                float distanceX = npc.Center.X - position.X;
                if (distanceX.Abs() > 12f)
                {
                    int dir = distanceX > 0f ? 1 : -1;
                    velocity.X = MathHelper.Lerp(velocity.X, 20 * dir, 0.01f);
                }

                if (!HandleGravity(npc))
                {
                    int dustChance = 8 - (int)velocity.X * 2;
                    if (Main.tile[(int)position.X / 16, (int)position.Y / 16].type == TileID.Sand && (dustChance <= 1 || Main.rand.NextBool(dustChance)))
                        Dust.NewDust(position, 4, 4, 32);
                }

                position += velocity;
            }

            private bool HandleGravity(NPC npc)
            {
                var solidPosition = position + new Vector2(0, -4f);
                bool fall = !Collision.SolidCollision(solidPosition, 4, 4) &&
                    !Collision.WetCollision(solidPosition, 4, 4);
                bool aboveTargets = position.Y >= npc.Center.Y;
                if (fall || !aboveTargets)
                {
                    if (aboveTargets && velocity.Y > 0f)
                    {
                        int X = (int)position.X;
                        int Y = (int)position.Y + 2;
                        int X2 = X / 16;
                        int Y2 = Y / 16;
                        if (AQWorldGen.ActiveAndSolid(X2, Y2))
                        {
                            if (position.Y + velocity.Y > Y2 * 16f)
                            {
                                position.Y = (int)(Y2 * 16f);
                                if (velocity.Y > 3f)
                                {
                                    if (Main.tile[X2, Y2].type == TileID.Sand)
                                    {
                                        Dust.NewDust(position, 4, 4, 32);
                                        Dust.NewDust(position, 4, 4, 32);
                                    }
                                }
                                velocity.Y = 0f;
                                return false;
                            }
                        }
                    }
                    velocity.Y += 0.2f;
                    if (velocity.Y > 8)
                        velocity.Y = 8f;
                    return true;
                }
                else
                {
                    if (velocity.Y > 0)
                    {
                        velocity.Y = 0f;
                    }
                    else
                    {
                        velocity.Y -= 0.1f;
                        if (velocity.Y < -8)
                            velocity.Y = -8f;
                    }
                    return false;
                }
            }

            public void Draw()
            {
                Rectangle rectangle;
                float yOff = -2f;
                switch (frame)
                {
                    default:
                    {
                        rectangle = new Rectangle(0, 0, 24, 16);
                    }
                    break;

                    case 1:
                    {
                        rectangle = new Rectangle(26, 0, 24, 16);
                    }
                    break;

                    case 2:
                    {
                        rectangle = new Rectangle(0, 18, 12, 12);
                    }
                    break;
                }
                var texture = TextureGrabber.GetNPC(ModContent.NPCType<SoliderCrabs>());
                Main.spriteBatch.Draw(texture, position + new Vector2(0f, yOff) - Main.screenPosition, rectangle, Lighting.GetColor((int)position.X / 16, (int)position.Y / 16), 0f, rectangle.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            position += new Vector2(0f, -npc.height / 2f);
            return null;
        }

        private List<SoliderCrabVisual> _visualCrabs;

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            var center = npc.Center;
            if (_visualCrabs == null)
            {
                int amount = Main.rand.Next(npc.width / 3) + 3;
                var rand = new Terraria.Utilities.UnifiedRandom(Main.mouseX + Main.mouseY);
                var offset = npc.width / 2;
                _visualCrabs = new List<SoliderCrabVisual>();
                for (int i = 0; i < amount; i++)
                {
                    _visualCrabs.Add(new SoliderCrabVisual(center + Utils.RandomVector2(rand, -offset, offset), npc.velocity, Main.rand.NextBool() ? 2 : 0));
                }
            }
            var frame = new Rectangle(0, 32, 52, 32);
            Main.spriteBatch.Draw(TextureGrabber.GetNPC(npc.type), center + new Vector2(0f, 2f) - Main.screenPosition, frame, drawColor, npc.rotation, frame.Size() / 2f, npc.scale, SpriteEffects.None, 0f);
            foreach (var vc in _visualCrabs)
            {
                vc.Draw();
            }
            return false;
        }
    }
}