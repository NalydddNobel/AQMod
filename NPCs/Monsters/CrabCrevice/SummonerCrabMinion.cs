using Aequus.Biomes;
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
    public class SummonerCrabMinion : ModNPC
    {
        public int NPCOwner { get => (int)NPC.ai[0]; set => NPC.ai[0] = value; }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 3;
        }

        public override void SetDefaults()
        {
            NPC.width = 28;
            NPC.height = 28;
            NPC.damage = 50;
            NPC.lifeMax = 135; 
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = Item.buyPrice(copper: 50);
            NPC.knockBackResist = 0.33f;

            this.SetBiome<CrabCreviceBiome>();
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry);
        }

        public override void AI()
        {
            if (NPCOwner >= 0)
            {
                if (!Main.npc[NPCOwner].active)
                {
                    NPCOwner = -1;
                }
            }
            if (NPC.justHit)
            {
                NPC.ai[1] = Math.Max(NPC.ai[1] - 30f, -80f);
            }
            NPC.ai[1]++;
            NPC.velocity *= 0.93f;
            if (NPC.ai[1] > 0f)
            {
                if (NPC.ai[2] >= 3f)
                {
                    NPC.ai[2] = 0f;
                    NPC.ai[1] = -60f + Main.rand.Next(-15, 15);
                }
                else
                {
                    NPC.ai[1] = -20 + Main.rand.Next(-5, 0);
                }
                NPC.ai[2]++;
                NPC.velocity = NPC.DirectionTo(Main.player[NPC.target].Center) * 12f;
                NPC.direction = Math.Sign(NPC.velocity.X);
                NPC.TargetClosest(faceTarget: true);
            }
            NPC.rotation = NPC.velocity.X * 0.05f;
            NPC.spriteDirection = NPC.direction;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 1.0)
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y = (NPC.frame.Y + frameHeight) % (frameHeight * (Main.npcFrameCount[Type]));
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
            return true;
        }
    }
}