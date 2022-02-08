using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Potions;
using AQMod.Projectiles.Monster;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.CrabSeason
{
    public class SoldierCrab : ModNPC, IDecideFallThroughPlatforms
    {
        public const int FramesX = 2;
        public const int Phase_ShieldBash = 0;
        public const int Phase_ClawSnip = 1;
        public const int Phase_Jump = 2;

        private bool _setupFrame;
        public int frameIndex;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 10;
        }

        public override void SetDefaults()
        {
            npc.width = 40;
            npc.height = 30;
            npc.lifeMax = 48;
            npc.damage = 50;
            npc.knockBackResist = 0.75f;
            npc.noGravity = true;
            npc.aiStyle = -1;
            npc.value = Item.buyPrice(silver: 3);
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath8;
            npc.SetLiquidSpeed(water: 0.9f);

            //banner = npc.type;
            //bannerItem = ModContent.ItemType<Items.Placeable.Banners.SoliderCrabsBanner>();
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
            if (npc.velocity.Y == 0f && (int)npc.ai[0] != -1)
            {
                npc.velocity.X *= 0.9f;
                if (!npc.HasValidTarget)
                {
                    npc.TargetClosest(faceTarget: true);
                    if (!npc.HasValidTarget)
                    {
                        npc.ai[0] = -1f;
                    }
                }
                npc.direction = (npc.position.X + npc.width / 2f) < (Main.player[npc.target].position.X + Main.player[npc.target].width / 2f) ? 1 : -1;
                if ((int)npc.ai[0] == Phase_ShieldBash)
                {
                    npc.ai[1]++;
                    if (Main.netMode != NetmodeID.Server && (int)npc.ai[1] == 20)
                    {
                        if (Collision.CanHitLine(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                        Main.PlaySound(SoundID.Tink, (int)npc.position.X, (int)npc.position.Y, Style: 1);
                        int amount = 2;
                        if (Main.expertMode)
                        {
                            amount *= 2;
                        }
                        float f = 0.5f / (amount / 2f);
                        var center = npc.Center;
                        int damage = 20;
                        for (int i = 0; i < amount / 2; i++)
                        {
                            int rotationMultiplier = i;
                            float rotationAdd = -MathHelper.PiOver4;
                            var normal = (rotationAdd + f * -rotationMultiplier).ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.01f, 0.01f)); ;
                            int p = Projectile.NewProjectile(center + normal * 20f, normal * (Main.expertMode ? Main.rand.NextFloat(4f, 8f) : Main.rand.NextFloat(2f, 5f)),
                                ModContent.ProjectileType<SoldierCrabSandBall>(), damage, 1f);
                        }
                        for (int i = 0; i < amount / 2; i++)
                        {
                            int rotationMultiplier = i;
                            float rotationAdd = -MathHelper.PiOver4;
                            var normal = (rotationAdd + f * -rotationMultiplier).ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.01f, 0.01f));
                            normal.X = -normal.X;
                            int p = Projectile.NewProjectile(center + normal * 20f, normal * (Main.expertMode ? Main.rand.NextFloat(4f, 8f) : Main.rand.NextFloat(2f, 5f)),
                                ModContent.ProjectileType<SoldierCrabSandBall>(), damage, 1f);
                        }
                    }
                    if (npc.ai[1] > 42f)
                    {
                        npc.ai[2] = Phase_ClawSnip;
                        npc.ai[1] = 0f;
                        npc.ai[0] = Phase_Jump;
                    }
                }
                else if ((int)npc.ai[0] == Phase_ClawSnip)
                {
                    npc.ai[1]++;
                    if (Main.netMode != NetmodeID.Server && (int)npc.ai[1] == 20)
                    {
                        int damage = 20;
                        Main.PlaySound(SoundID.Item71, npc.position);
                        int p = Projectile.NewProjectile(npc.Center, Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * (Main.expertMode ? Main.rand.NextFloat(10f, 14f) : Main.rand.NextFloat(8f, 10f)),
                            ModContent.ProjectileType<SoldierCrabSnip>(), damage, 1f);
                    }
                    if (npc.ai[1] > 30f)
                    {
                        npc.ai[2] = Phase_ShieldBash;
                        npc.ai[1] = 0f;
                        npc.ai[0] = Phase_Jump;
                    }
                }
                else
                {
                    if (npc.ai[1] == 0f)
                    {
                        npc.ai[1]++;
                        npc.TargetClosest(faceTarget: true);
                        if (npc.HasValidTarget)
                        {
                            if (npc.position.Y - Main.player[npc.target].position.Y > 132f && Main.rand.NextBool())
                            {
                                if (npc.position.Y - Main.player[npc.target].position.Y > 220f)
                                    npc.velocity.Y = -16.5f;
                                else
                                    npc.velocity.Y = -11.5f;
                                if (Main.rand.NextBool())
                                    npc.velocity.X = 2f * npc.direction;
                                else
                                    npc.velocity.X = 4f * npc.direction;
                            }
                            else if (Main.rand.NextBool())
                            {
                                npc.velocity.Y = -5.5f;
                                npc.velocity.X = 8.5f * npc.direction;
                            }
                            else
                            {
                                npc.velocity.Y = -8f;
                                npc.velocity.X = 5f * npc.direction;
                            }
                        }
                        else
                        {
                            npc.velocity.Y = -8f;
                            npc.velocity.X = 5f * npc.direction;
                        }
                    }
                    if (npc.velocity.Y == 0f)
                    {
                        npc.ai[1]++;
                        if (npc.ai[1] >= 46f)
                        {
                            npc.TargetClosest(faceTarget: true);
                            if (npc.HasValidTarget && 
                                Collision.CanHitLine(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                                npc.ai[0] = (int)npc.ai[2];
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                        }
                    }
                }
            }
            if ((int)npc.ai[0] == -1)
            {
                npc.noTileCollide = true;
                npc.velocity.X *= 0.99f;
            }
            if (npc.velocity.Y < 16f)
            {
                npc.TargetClosest(faceTarget: false);
                npc.velocity.Y += 0.55f;
                if (npc.velocity.Y > 16f)
                    npc.velocity.Y = 16f;
            }
            npc.spriteDirection = npc.direction;
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

            if (npc.velocity.Y != 0f)
            {
                frameIndex = npc.velocity.Y > 10f ? 12 : 11;
            }
            else if ((int)npc.ai[0] != -1)
            {
                if ((int)npc.ai[0] == Phase_ShieldBash)
                {
                    if ((int)npc.ai[1] == 20)
                    {
                        frameIndex = 5;
                    }
                    else if ((int)npc.ai[1] > 20)
                    {
                        frameIndex = 0;
                    }
                    else
                    {
                        frameIndex = (int)npc.ai[1] / 4;
                    }
                }
                else if ((int)npc.ai[0] == Phase_ClawSnip)
                {
                    if ((int)npc.ai[1] >= 20)
                    {
                        frameIndex = 9;
                    }
                    else
                    {
                        frameIndex = 6 + (int)npc.ai[1] / 8;
                    }
                }
                else
                {
                    frameIndex = 0;
                }
            }
            else
            {
                frameIndex = 0;
            }

            npc.frame.Y = frameIndex * frameHeight;

            if (npc.frame.Y >= frameHeight * Main.npcFrameCount[npc.type])
            {
                npc.frame.X = npc.frame.Width * (npc.frame.Y / (frameHeight * Main.npcFrameCount[npc.type]));
                npc.frame.Y = npc.frame.Y % (frameHeight * Main.npcFrameCount[npc.type]);
            }
            else
            {
                npc.frame.X = 0;
            }
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