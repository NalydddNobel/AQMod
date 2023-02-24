using Aequus.Biomes.CrabCrevice;
using Aequus.Buffs.Debuffs;
using Aequus.Items.Accessories;
using Aequus.Items.Materials;
using Aequus.Items.Placeable.Banners;
using Aequus.NPCs;
using Aequus.NPCs.AIs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters.CrabCrevice
{
    public class CoconutCrab : LegacyAIFighter
    {
        public const int FramesX = 2;
        public const int Phase_ShieldBash = 0;
        public const int Phase_ClawSnip = 1;
        public const int Phase_Jump = 2;

        private bool _setupFrame;
        public int frameIndex;
        public int customState;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 8;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Velocity = 1f,
                Direction = -1,
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 28;
            NPC.height = 28;
            NPC.lifeMax = 75;
            NPC.damage = 50;
            NPC.knockBackResist = 0.1f;
            NPC.aiStyle = -1;
            NPC.defense = 12;
            NPC.value = Item.buyPrice(silver: 3);
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.SetLiquidSpeeds(water: 0.9f);

            Banner = NPC.type;
            BannerItem = ModContent.ItemType<CoconutCrabBanner>();

            customState = 0;
            this.SetBiome<CrabCreviceBiome>();
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            this.CreateLoot(npcLoot)
                .Add(ItemID.Coconut, chance: 1, stack: 1)
                .Add(ItemID.WaterGun, chance: 30, stack: 1)
                .Add<PearlShardWhite>(chance: 5, stack: 1)
                .Add<PearlShardBlack>(chance: 10, stack: 1)
                .Add<PearlShardPink>(chance: 15, stack: 1)
                .Add<FoolsGoldRing>(chance: 10, stack: 1);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, NPC.velocity.X, NPC.velocity.X, 0, default(Color), 1.25f);
                }
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Ambient_DarkBrown, NPC.velocity.X, NPC.velocity.X, 0, default(Color), 1.25f);
                }
            }
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, NPC.velocity.X, NPC.velocity.X, 0, default(Color), 0.9f);
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Ambient_DarkBrown, NPC.velocity.X, NPC.velocity.X, 0, default(Color), 0.9f);
        }

        public override void AI()
        {
            if (NPC.justHit && customState != 1 && Main.rand.NextBool(3))
            {
                NPC.defense += 50;
                NPC.TargetClosest(faceTarget: true);
                NPC.velocity = Vector2.Normalize(NPC.targetRect.Center.ToVector2() - NPC.Center) * 7f;
                NPC.velocity.Y *= 0.33f;
                NPC.velocity.Y -= 4f;
                customState = 1;
                NPC.netUpdate = true;
            }

            switch (customState)
            {
                case 0:
                    {
                        NPC.rotation *= 0.9f;
                        if (NPC.velocity.Y >= 0f)
                        {
                            if (frameIndex < 1)
                            {
                                frameIndex = 1;
                            }
                        }
                        NPC.aiStyle = NPCAIStyleID.Fighter;
                    }
                    break;

                case 1:
                    {
                        NPC.aiStyle = -1;
                        frameIndex = 0;
                        NPC.rotation += NPC.velocity.X * 0.1f;
                        NPC.gfxOffY = 4f;
                        if (NPC.collideY)
                        {
                            NPC.velocity.X *= 0.975f;
                        }
                        NPC.ai[1] += 2f - NPC.velocity.Length();
                        if (NPC.ai[1] > 120f)
                        {
                            customState = 0;
                            NPC.ai[1] = 0f;
                            NPC.dontTakeDamage = false;
                            NPC.velocity.Y -= 5f;
                            NPC.rotation = MathHelper.WrapAngle(NPC.rotation);
                            NPC.gfxOffY = 0f;
                            NPC.defense = NPC.defDefense;
                            NPC.netUpdate = true;
                        }
                    }
                    NPC.spriteDirection = -NPC.direction;
                    return;
            }
            base.AI();
            NPC.spriteDirection = NPC.direction;
        }

        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            if (!_setupFrame)
            {
                _setupFrame = true;
                NPC.frame.Width = NPC.frame.Width / FramesX;
            }

            if (NPC.IsABestiaryIconDummy)
            {
                frameIndex = Math.Max(frameIndex, 10);
            }

            if (frameIndex >= 1 && frameIndex < 7)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > 3.0d)
                {
                    NPC.frameCounter = 0.0d;
                    frameIndex++;
                }
            }
            if (customState == 0 && frameIndex >= 7)
            {
                if (NPC.velocity.Y != 0f)
                {
                    frameIndex = 8;
                    NPC.frameCounter = 0.0;
                }
                else
                {
                    NPC.frameCounter += NPC.velocity.X.Abs();
                    if (NPC.frameCounter > 5.0d)
                    {
                        NPC.frameCounter = 0.0d;
                        frameIndex = (frameIndex + 1) % 16;
                    }
                    frameIndex = Math.Max(frameIndex, 10);
                }
            }
            NPC.frame.Y = frameIndex * frameHeight;

            if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
            {
                NPC.frame.X = NPC.frame.Width * (NPC.frame.Y / (frameHeight * Main.npcFrameCount[NPC.type]));
                NPC.frame.Y = NPC.frame.Y % (frameHeight * Main.npcFrameCount[NPC.type]);
            }
            else
            {
                NPC.frame.X = 0;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.expertMode || Main.rand.NextBool())
            {
                target.AddBuff(ModContent.BuffType<PickBreak>(), 1080);
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.Aequus().ZoneCrabCrevice ? 0.2f : 0f;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var origin = new Vector2(28f, 33f);
            spriteBatch.Draw(TextureAssets.Npc[Type].Value, NPC.Center + new Vector2(0f, NPC.gfxOffY + 2f) - screenPos,
                NPC.frame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, origin, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(ModContent.Request<Texture2D>($"{Texture}_Glow", AssetRequestMode.ImmediateLoad).Value, NPC.Center + new Vector2(0f, NPC.gfxOffY + 2f) - screenPos,
                NPC.frame, Color.White, NPC.rotation, origin, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }

        public override bool? CanFallThroughPlatforms()
        {
            return NPC.HasValidTarget && Main.player[NPC.target].position.Y > NPC.position.Y + NPC.height;
        }
    }
}