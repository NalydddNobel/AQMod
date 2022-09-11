using Aequus.Biomes;
using Aequus.NPCs.AIs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters.CrabCrevice
{
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
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = Item.buyPrice(silver: 2);

            this.SetBiome<CrabCreviceBiome>();
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry)
                .AddMainSpawn(BestiaryBuilder.OceanBiome);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            this.CreateLoot(npcLoot)
                .Add(ItemID.GoldCoin, chance: 1, stack: 1);
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
                capX *= 4f - (NPC.ai[1] - 180f) / 300f * 3f;
                capY *= 4f - (NPC.ai[1] - 180f) / 300f * 3f;
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
                speedX = -speedX * 1.5f;
                speedY *= 0.6f;
                capX *= 2f;
            }
        }

        public override void GetIdleSpeeds(out float speedX, out float speedY, out Vector2 capX, out float capY)
        {
            NPC.ai[1] = 0f;
            base.GetIdleSpeeds(out speedX, out speedY, out capX, out capY);
            speedX *= 0.1f;
            capX.X *= 2f;
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server && NPC.wet)
            {
                var tile = NPC.Center.ToTileCoordinates();
                inDarkness = Lighting.Brightness(tile.X, tile.Y) < 0.1f;
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
                else if (NPC.ai[1] == 0f && inDarkness && Main.rand.NextBool(120))
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
            FindFrameKinda(NPC.frame.Height);
            NPC.spriteDirection = NPC.direction;
        }

        private void FindFrameKinda(int height)
        {
            NPC.frameCounter++;
            int flopFrameStart = height * 7;
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
                        NPC.frame.Y += height;
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
                NPC.frame.Y += height;
                if (NPC.frame.Y > height * 13)
                {
                    NPC.frame.Y = flopFrameStart;
                }
                if (NPC.velocity.Y < 0f)
                {
                    if (NPC.frame.Y > height * 10)
                    {
                        NPC.frame.Y = height * 10;
                    }
                }
                else
                {
                    if (NPC.frame.Y == height * 10)
                    {
                        NPC.frame.Y = height * 9;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int _);
            spriteBatch.Draw(texture, NPC.position + offset + new Vector2(0f, NPC.gfxOffY) - screenPos, frame, drawColor * NPC.Opacity, NPC.rotation, origin, NPC.scale, (-NPC.spriteDirection).ToSpriteEffect(), 0f);
            if (NPC.Opacity < 1f)
            {
                var coin = TextureAssets.Coin[2].Value;
                var coinFrame = coin.Frame(verticalFrames: 8, frameY: (int)(Main.GameUpdateCount / 6) % 8);
                spriteBatch.Draw(coin, NPC.position + offset + new Vector2(14f * NPC.spriteDirection, NPC.gfxOffY - 8f) - screenPos, coinFrame, drawColor * (1f - NPC.Opacity), NPC.rotation, coinFrame.Size() / 2f, NPC.scale, (-NPC.spriteDirection).ToSpriteEffect(), 0f);
            }
            return false;
        }
    }
}