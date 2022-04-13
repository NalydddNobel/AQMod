using Aequus.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Boss
{
    public sealed class DustDevil : ModNPC
    {
        public class DustDevilCloudData
        {
            public static List<int> ChooseableClouds { get; internal set; }

            public int type;
            public Vector2 offset;
            public Vector2 velocity;
            public Vector2 velocityVelocity;
            public float opacity;
            public float scale;
            public SpriteEffects effects;

            public DustDevilCloudData(int type, Vector2 velocity, Vector2 velocityVelocity)
            {
                this.type = type;
                this.velocity = velocity;
                this.velocityVelocity = velocityVelocity;
                opacity = 1f;
                offset = Vector2.Zero;
                scale = 0f;
                effects = EffectsSystem.EffectRand.RandChance(2) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }

            public void Update()
            {
                offset += velocity;
                velocity += velocityVelocity;
                if (offset.Length() > 10f)
                {
                    opacity *= 0.95f;
                    velocityVelocity *= 0.95f;
                    velocity *= 0.975f;
                    scale *= 0.99f;
                }
                else
                {
                    scale = MathHelper.Lerp(scale, 1f, 0.35f);
                }
            }

            public static int ChooseCloud()
            {
                return ChooseableClouds[Main.rand.Next(ChooseableClouds.Count)];
            }
        }

        public const int PupilMaxLength = 20;
        public const int BlinkTime = 45;
        public const int BlinkFrames = 14;
        public const int BlinkDelay = 90;

        public static Asset<Texture2D> EyeTexture { get; private set; }
        public static Asset<Texture2D> EyeBGTexture { get; private set; }

        public int blinkState;
        public int nextBlink;

        public Vector2 pupilOffset;

        public int spawnCloud;
        public List<DustDevilCloudData> cloudData;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                string path = Texture;
                EyeTexture = ModContent.Request<Texture2D>(path + "_Eye");
                EyeBGTexture = ModContent.Request<Texture2D>(path + "_EyeBG");
                DustDevilCloudData.ChooseableClouds = new List<int>() 
                {
                    CloudID.Regular1, CloudID.Regular2, CloudID.Regular3, CloudID.Regular4,
                    CloudID.Cummulus1, CloudID.Cummulus2, CloudID.Cummulus3, CloudID.Cummulus4,
                    CloudID.Cumulonimbus1, CloudID.Cumulonimbus2, CloudID.Cumulonimbus3, CloudID.Cumulonimbus4,
                }; 
            }
        }

        public override void Unload()
        {
            EyeTexture = null;
            EyeBGTexture = null;
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 14;
        }

        public override void SetDefaults()
        {
            NPC.width = 64;
            NPC.height = 64;
            NPC.lifeMax = 18000;
            NPC.damage = 50;
            NPC.defense = 16;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(gold: 20);
            NPC.npcSlots = 10f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.trapImmune = true;
            NPC.lavaImmune = true;
            NPC.boss = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f) + 4000 * numPlayers;
        }

        public override void AI()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }
            Pupil_LookAtPlayer();
        }
        private void Pupil_LookAtPlayer()
        {
            FacePupilTowards(Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)].Center);
        }

        public override void FindFrame(int frameHeight)
        {
            UpdateClouds();
            NaturalBlinking();
        }
        private void UpdateClouds()
        {
            if (cloudData == null)
            {
                cloudData = new List<DustDevilCloudData>();
            }
            for (int i = 0; i < cloudData.Count; i++)
            {
                cloudData[i].Update();
                if (cloudData[i].opacity <= 0.05f)
                {
                    cloudData.RemoveAt(i);
                    i--;
                }
            }
            if ((spawnCloud < 0 || cloudData.Count < 7) && cloudData.Count < 64)
            {
                spawnCloud = 2;
                cloudData.Add(new DustDevilCloudData(DustDevilCloudData.ChooseCloud(), 
                    Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(0.4f, 1.1f), Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(0.01f, 0.125f)));
            }
            else
            {
                spawnCloud--;
            }
        }
        private void FacePupilTowards(Vector2 position)
        {
            var difference = position - NPC.Center;
            pupilOffset = Vector2.Lerp(pupilOffset, Vector2.Normalize(difference) * Math.Min(difference.Length() * 0.25f, PupilMaxLength), 0.25f);
        }
        private void NaturalBlinking()
        {
            if (blinkState > 0)
            {
                if (Main.rand.NextBool(4))
                {
                    blinkState++;
                }
                else
                {
                    blinkState += 3;
                }
                if (blinkState > 30)
                {
                    blinkState = 0;
                }
            }
            else if (nextBlink <= 0)
            {
                blinkState = 1;
                nextBlink = BlinkDelay;
            }
            else
            {
                nextBlink--;
                if (Main.rand.NextBool(90))
                {
                    nextBlink -= 60;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var center = NPC.Center;
            RenderClouds(spriteBatch, center - screenPos);
            if (blinkState < BlinkFrames)
            {
                DrawEye(spriteBatch, blinkState, center - screenPos);
            }
            else if (blinkState > BlinkTime - BlinkFrames)
            {
                DrawEye(spriteBatch, BlinkTime - blinkState, center - screenPos);
            }
            return false;
        }
        private void RenderClouds(SpriteBatch spriteBatch, Vector2 drawCoordinates)
        {
            if (cloudData == null)
            {
                return;
            }
            foreach (var c in cloudData)
            {
                var v = drawCoordinates + c.offset;
                InternalRenderSingleCloud(spriteBatch, c.type, v, c.opacity, c.scale, c.effects);
            }
        }
        private void InternalRenderSingleCloud(SpriteBatch spriteBatch, int type, Vector2 drawCoordinates, float opacity, float scale, SpriteEffects effects)
        {
            var texture = TextureAssets.Cloud[type].Value;
            int largestSize = texture.Width > texture.Height ? texture.Width : texture.Height;
            if (largestSize > 350)
            {
                scale = Math.Min(scale, 350 / largestSize);
            }
            spriteBatch.Draw(texture, drawCoordinates, TextureAssets.Cloud[type].Frame(), Color.White.UseA(75) * opacity, 0f, TextureAssets.Cloud[type].Size() / 2f, NPC.scale * scale, effects, 0f);
        }
        private void DrawEye(SpriteBatch spriteBatch, int frameY, Vector2 drawCoordinates)
        {
            var texture = TextureAssets.Npc[Type].Value;
            var frame = texture.Frame(verticalFrames: BlinkFrames, frameY: frameY);
            var origin = frame.Size() / 2f;
            foreach (var v in AequusHelpers.CircularVector(4))
            {
                spriteBatch.Draw(texture, drawCoordinates + v * 2f, frame, Color.Black, 0f, origin, NPC.scale, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(texture, drawCoordinates, frame, NPC.GetNPCColorTintedByBuffs(Color.White), 0f, origin, NPC.scale, SpriteEffects.None, 0f);

            var eyeTexture = EyeTexture.Value;
            var eyeFrame = eyeTexture.Frame();
            var eyeOrigin = eyeFrame.Size() / 2f;

            spriteBatch.Draw(eyeTexture, drawCoordinates + pupilOffset, eyeFrame, NPC.GetNPCColorTintedByBuffs(new Color(20, 20, 75, 255)), 0f, eyeOrigin, NPC.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(EyeBGTexture.Value, drawCoordinates, frame, NPC.GetNPCColorTintedByBuffs(Color.White), 0f, origin, NPC.scale, SpriteEffects.None, 0f);
        }
    }
}