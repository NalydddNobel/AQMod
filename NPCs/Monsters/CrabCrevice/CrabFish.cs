using Aequus;
using Aequus.Buffs.Debuffs;
using Aequus.Common.Utilities;
using Aequus.Content.Biomes.CrabCrevice;
using Aequus.Items.Accessories.Misc.Money;
using Aequus.Items.Materials.PearlShards;
using Aequus.NPCs.AIs;
using Aequus.Tiles.Banners.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters.CrabCrevice {
    public class CrabFish : AIFish
    {
        public bool inDarkness;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 14;
        }

        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 16;
            NPC.damage = 15;
            NPC.lifeMax = 40;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = Item.buyPrice(silver: 2);
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<CrabFishBanner>();

            this.SetBiome<CrabCreviceBiome>();
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            this.CreateLoot(npcLoot)
                .Add(ItemID.GoldCoin, chance: 1, stack: 1)
                .AddOptions(chance: 15, ModContent.ItemType<BusinessCard>(), ModContent.ItemType<FaultyCoin>(), ModContent.ItemType<FoolsGoldRing>())
                .Add<PearlShardWhite>(chance: 5, stack: 1)
                .Add<PearlShardBlack>(chance: 10, stack: 1)
                .Add<PearlShardPink>(chance: 15, stack: 1)
                .Add(ItemID.Blindfold, chance: 25, stack: 1);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood, NPC.velocity.X, NPC.velocity.X, 0, default(Color), 1.25f);
                }
            }
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood, NPC.velocity.X, NPC.velocity.X, 0, default(Color), 0.9f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (Main.expertMode || Main.rand.NextBool())
            {
                target.AddBuff(ModContent.BuffType<PickBreak>(), 120);
            }
            if (Main.rand.NextBool(Main.expertMode ? 2 : 8))
            {
                target.AddBuff(BuffID.Darkness, 300);
            }
        }

        public override void GetChaseSpeeds(out float speedX, out float speedY, out Vector2 capX, out Vector2 capY)
        {
            NPC.ai[1]++;
            base.GetChaseSpeeds(out speedX, out speedY, out capX, out capY);
            if (NPC.ai[1] > 480f)
            {
                NPC.ai[1] = 1f;
            }
            else if (NPC.ai[1] > 180f)
            {
                speedX *= 2f;
                speedY *= 2f;
                capX *= 2f - (NPC.ai[1] - 180f) / 300f;
                capY *= 2f - (NPC.ai[1] - 180f) / 300f;
            }
            else if (NPC.ai[1] > 120f)
            {
                if (NPC.Distance(Main.player[NPC.target].Center) < 200f)
                {
                    NPC.ai[1] += 0.5f;
                }
                else
                {
                    NPC.ai[1] -= 0.5f;
                }
                speedX *= -1.5f;
                speedY *= 0.6f;
                capX *= 2f;
            }
        }

        public override void GetIdleSpeeds(out float speedX, out float speedY, out Vector2 capX, out float capY)
        {
            NPC.ai[1] = 0f;
            base.GetIdleSpeeds(out speedX, out speedY, out capX, out capY);
            speedX *= 0.01f;
            capX.X *= 0.02f;
            capX.Y = 0.5f;
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server && NPC.wet)
            {
                var tile = NPC.Center.ToTileCoordinates();
                inDarkness = NPC.GetAlpha(Lighting.GetColor(tile.X, tile.Y)).ToVector3().Length() < 0.33f;
                if (NPC.localAI[0] > 0f)
                {
                    NPC.localAI[0]--;
                    if (NPC.ai[1] != 0f)
                    {
                        NPC.localAI[0] -= 5f;
                    }
                    if (NPC.localAI[0] < 0f)
                    {
                        NPC.localAI[0] = 0f;
                    }
                }
                else if (NPC.ai[1] == 0f && NPC.wet && inDarkness && Main.rand.NextBool(120))
                {
                    NPC.localAI[0] = 240f;
                    NPC.Opacity = 0.1f;
                    SoundEngine.PlaySound(SoundID.CoinPickup, NPC.Center);
                    int size = 16;
                    var loc = NPC.Center + new Vector2(14f * NPC.direction - size / 2f, NPC.gfxOffY - 8f - size / 2f);
                    for (int i = 0; i < 5; i++)
                    {
                        var d = Dust.NewDustDirect(loc, size, size, DustID.GoldCoin);
                        d.scale *= 0.75f;
                        d.fadeIn = d.scale + 0.2f;
                        d.velocity *= 0.01f;
                    }
                }
                else if (NPC.Opacity < 1f)
                {
                    NPC.Opacity += 0.02f;
                    if (NPC.ai[1] != 0f)
                    {
                        NPC.Opacity += 0.05f;
                    }
                    if (NPC.Opacity > 1f)
                    {
                        NPC.Opacity = 1f;
                    }
                }
            }

            NPC.ShowNameOnHover = !inDarkness || NPC.Opacity < 0.5f;
            NPC.gfxOffY = 6f;
            base.AI();
            NPC.spriteDirection = NPC.direction;
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                NPC.wet = true;
            }
            NPC.frameCounter++;
            int flopFrameStart = frameHeight * 7;
            if (NPC.wet)
            {
                if (NPC.frame.Y >= flopFrameStart)
                {
                    NPC.frame.Y = 0;
                }
                if (NPC.frameCounter > 6.0 / Math.Clamp(NPC.velocity.Length() / 2f, 1f, 2f))
                {
                    NPC.frameCounter = 0.0;
                    if (NPC.frame.Y > 0)
                    {
                        NPC.frame.Y = 0;
                    }
                    else
                    {
                        NPC.frame.Y += frameHeight;
                    }
                }
                return;
            }
            if (NPC.frame.Y < flopFrameStart)
            {
                NPC.frame.Y = flopFrameStart;
            }
            if (NPC.frameCounter > 3.0)
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > frameHeight * 13)
                {
                    NPC.frame.Y = flopFrameStart;
                }
                if (NPC.velocity.Y < 0f)
                {
                    if (NPC.frame.Y > frameHeight * 10)
                    {
                        NPC.frame.Y = frameHeight * 10;
                    }
                }
                else
                {
                    if (NPC.frame.Y == frameHeight * 10)
                    {
                        NPC.frame.Y = frameHeight * 9;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int _);
            spriteBatch.Draw(texture, NPC.position + offset + new Vector2(0f, NPC.gfxOffY) - screenPos, frame, drawColor * NPC.Opacity, NPC.rotation, origin, NPC.scale, NPC.spriteDirection.ToSpriteEffect(), 0f);
            spriteBatch.Draw(ModContent.Request<Texture2D>($"{Texture}_Glow", AssetRequestMode.ImmediateLoad).Value, NPC.position + offset + new Vector2(0f, NPC.gfxOffY) - screenPos, frame,
                drawColor * 3f * NPC.Opacity, NPC.rotation, origin, NPC.scale, NPC.spriteDirection.ToSpriteEffect(), 0f);
            if (NPC.Opacity < 1f)
            {
                var coin = TextureAssets.Coin[2].Value;
                var coinFrame = coin.Frame(verticalFrames: 8, frameY: (int)(Main.GameUpdateCount / 6) % 8);
                spriteBatch.Draw(coin, NPC.position + offset + new Vector2(14f * NPC.spriteDirection, NPC.gfxOffY - 8f) - screenPos, coinFrame, drawColor * (1f - NPC.Opacity), NPC.rotation, coinFrame.Size() / 2f, NPC.scale, (-NPC.spriteDirection).ToSpriteEffect(), 0f);
            }
            return false;
        }

        public override bool? CanFallThroughPlatforms()
        {
            return !NPC.HasValidTarget || Main.player[NPC.target].position.Y
                > NPC.position.Y + NPC.height;
        }
    }
}