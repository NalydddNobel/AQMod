using Aequus.Biomes.CrabCrevice;
using Aequus.Items.Placeable.Banners;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters.CrabCrevice
{
    public class SummonerCrab : ModNPC
    {
        public int state = 0;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 15;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Velocity = 1f,
                Direction = -1,
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 28;
            NPC.damage = 10;
            NPC.lifeMax = 380;
            NPC.defense = 20;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = Item.buyPrice(silver: 30);
            NPC.knockBackResist = 0f;
            NPC.aiStyle = NPCAIStyleID.Fighter;
            NPC.behindTiles = true;
            AIType = NPCID.CorruptBunny;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<HijivarchCrabBanner>();
            NPC.npcSlots = 4f;

            this.SetBiome<CrabCreviceBiome>();
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, NPC.velocity.X, NPC.velocity.X, 0, default(Color), 1.25f);
                }
            }
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, NPC.velocity.X, NPC.velocity.X, 0, default(Color), 0.9f);
        }

        public override bool PreAI()
        {
            if (state < 2)
            {
                if (!NPC.HasValidTarget)
                {
                    NPC.TargetClosest(faceTarget: true);
                }
                if (NPC.velocity.Y < 0.5f && (!NPC.HasValidTarget ||
                    NPC.Distance(Main.player[NPC.target].Center) < 500f && (CheckLine(NPC.position) || CheckLine(NPC.position - new Vector2(0f, NPC.height / 2f)))))
                {
                    if (state != 1)
                    {
                        ClearAI();
                    }
                    state = 1;
                    NPC.aiStyle = -1;
                    AIType = 0;
                    return true;
                }

                if (state != 0)
                {
                    NPC.ai[1] -= 2f;
                    if (NPC.ai[1] < 15f)
                    {
                        ClearAI();
                        if (NPC.ai[1] > 30f)
                        {
                            NPC.velocity.Y = -6f;
                        }
                        NPC.TargetClosest(faceTarget: true);
                        NPC.aiStyle = NPCAIStyleID.Fighter;
                        AIType = NPCID.CorruptBunny;
                        state = 0;
                    }
                }
            }
            return true;
        }

        public bool CheckLine(Vector2 where)
        {
            return Collision.CanHitLine(
                        where, NPC.width, NPC.height,
                        Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height);
        }

        public void ClearAI()
        {
            NPC.ai[0] = 0f;
            NPC.ai[1] = 0f;
        }

        public override void AI()
        {
            if (state == 1)
            {
                NPC.velocity.X *= 1f - Math.Min(NPC.ai[1] * 0.01f, 0.2f);
                if (NPC.velocity.Y.Abs() < 0.5f)
                {
                    NPC.ai[1]++;
                    NPC.TargetClosest(faceTarget: true);

                    int timer = (int)NPC.ai[1] % 300;
                    if (timer == 60 || timer == 90 || timer == 120)
                    {
                        int count = 0;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<SummonerCrabMinion>() && (int)Main.npc[i].ai[0] == NPC.whoAmI)
                            {
                                count++;
                            }
                        }
                        if (count < 3)
                        {
                            SoundEngine.PlaySound(SoundID.NPCHit1, NPC.Center);
                            NPC.NewNPCDirect(NPC.GetSource_FromAI(), NPC.Center, ModContent.NPCType<SummonerCrabMinion>(), ai0: NPC.whoAmI, target: NPC.target);
                        }
                    }
                }
            }
            NPC.spriteDirection = NPC.direction;
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.Y != 0f)
            {
                NPC.frame.Y = frameHeight * 7;
                return;
            }

            switch (state)
            {
                case 0:
                    {
                        NPC.frameCounter += NPC.velocity.X.Abs() * 0.6f;
                        if (NPC.frameCounter >= 6.0)
                        {
                            NPC.frameCounter = 0.0;
                            NPC.frame.Y = NPC.frame.Y + frameHeight;
                        }
                        NPC.frame.Y %= frameHeight * 4;
                    }
                    break;

                case 1:
                    {
                        if (NPC.ai[1] > 10f)
                        {
                            NPC.frameCounter++;
                            if (NPC.frameCounter >= 3.0)
                            {
                                NPC.frameCounter = 0.0;
                                NPC.frame.Y += frameHeight;
                            }
                        }
                        NPC.frame.Y = Math.Clamp(NPC.frame.Y, frameHeight * 4, frameHeight * (Main.npcFrameCount[Type] - 1));
                    }
                    break;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC.GetDrawInfo(out var t, out var _, out var frame, out var origin, out int _);
            var drawLoc = NPC.position + new Vector2(NPC.width / 2f, NPC.height - frame.Height / 2f + 4f);
            spriteBatch.Draw(t, drawLoc - screenPos, frame, NPC.IsABestiaryIconDummy ? Color.White : NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, origin, NPC.scale, (-NPC.spriteDirection).ToSpriteEffect(), 0f);
            spriteBatch.Draw(ModContent.Request<Texture2D>($"{Texture}_Glow", AssetRequestMode.ImmediateLoad).Value, drawLoc - screenPos, frame, Color.White, NPC.rotation, origin, NPC.scale, (-NPC.spriteDirection).ToSpriteEffect(), 0f);
            return false;
        }

        public override bool? CanFallThroughPlatforms()
        {
            return !NPC.HasValidTarget || Main.player[NPC.target].position.Y
                > NPC.position.Y + NPC.height;
        }
    }
}