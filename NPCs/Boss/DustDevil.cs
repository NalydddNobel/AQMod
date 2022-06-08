using Aequus.Common.Utilities;
using Aequus.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Boss
{
    [AutoloadBossHead]
    public class DustDevil : AequusBoss
    {
        public const int ACTION_ = 2;

        public float HPRatio => NPC.life / (float)NPC.lifeMax;
        public bool PhaseTwo => NPC.life * (Main.expertMode ? 2f : 4f) <= NPC.lifeMax;

        //public override bool IsLoadingEnabled(Mod mod)
        //{
        //    return false;
        //}

        public List<DustParticle> dust;
        public int effectsTimer;
        public int auraTimer;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData()
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Wet,
                    BuffID.Confused,
                    BuffID.Suffocation,
                    BuffID.Lovestruck,
                }
            });

            SnowgraveCorpse.NPCBlacklist.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 100;
            NPC.lifeMax = 14500;
            NPC.damage = 50;
            NPC.defense = 12;
            NPC.aiStyle = -1;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.boss = true;
            NPC.value = Item.buyPrice(gold: 10);
            NPC.lavaImmune = true;
            NPC.trapImmune = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * bossLifeScale);
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return PhaseTwo ? Color.Red : Color.White;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override void AI()
        {
            MonsterSpawns.ForceZen(NPC);

            switch (Action)
            {
                case ACTION_:
                    _();
                    break;

                case ACTION_INTRO:
                    Intro();
                    break;

                case ACTION_INIT:
                    Action = ACTION_INTRO;
                    break;

                case ACTION_GOODBYE:
                    Goodbye();
                    break;
            }

            UpdateEffects();
        }

        public void UpdateEffects()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            if (dust == null)
            {
                dust = new List<DustParticle>();
            }
            dust.Add(new DustParticle(0f, NPC.height * 1.5f, 0f) { waveTime = Main.rand.NextFloat(MathHelper.TwoPi) });

            for (int i = 0; i < dust.Count; i++)
            {
                if (!dust[i].Update(NPC.width, NPC.height, effectsTimer))
                {
                    dust.RemoveAt(i);
                    i--;
                }
            }

            effectsTimer++;
            auraTimer++;
        }

        public void _()
        {

        }
        public void Intro()
        {
            if (ActionTimer < 60f)
            {
                if ((int)ActionTimer == 0)
                {
                    if (!CheckTargets())
                    {
                        return;
                    }
                    NPC.velocity.X = 0f;
                    NPC.velocity.Y = 6f;
                }
                else
                {
                    NPC.velocity.Y *= 0.92f;
                    if (NPC.velocity.Y < 0.1f)
                    {
                        NPC.velocity.Y = 0f;
                    }
                }
                NPC.ai[1]++;
            }
            else
            {
                if ((int)ActionTimer == 0)
                {
                    ActionTimer++;
                    if (!CheckTargets())
                    {
                        return;
                    }
                }
                SetAction(ACTION_);
            }

        }
        public void Goodbye()
        {
            if (NPC.ai[1] < 60f)
            {
                NPC.alpha += 5;
                NPC.velocity.X *= 0.8f;
                if ((int)NPC.ai[1] == 1)
                {
                    NPC.velocity.Y = -6f;
                }
                else
                {
                    NPC.velocity.Y *= 0.985f;
                }
                NPC.ai[1]++;
            }
            else
            {
                NPC.alpha = 255;
                NPC.timeLeft = 0;
                NPC.active = false;
                NPC.life = -1;
            }
        }

        public void SetAction(int phase)
        {
            ClearAI();
            Action = phase;
        }
        public bool CheckTargets(float checkDistance = 4000f)
        {
            NPC.TargetClosest(faceTarget: false);
            NPC.netUpdate = true;
            if (!NPC.HasValidTarget || NPC.Distance(Main.player[NPC.target].Center) > checkDistance)
            {
                ClearAI();
                ClearLocalAI();
                NPC.ai[0] = ACTION_GOODBYE;
                return false;
            }
            return true;
        }

        public Vector2 GetTo(Vector2 spot, float addSpeedX = 0.8f, float addSpeedY = 0.4f, float maxSpeed = 20f, float wrongWayMultiplier = 0.96f, float minDistance = 50f)
        {
            if (NPC.Distance(spot) < minDistance)
            {
                return NPC.velocity;
            }

            var velocity = NPC.velocity;
            if (NPC.position.X < spot.X)
            {
                if (velocity.X < maxSpeed)
                {
                    velocity.X += addSpeedX;
                    if (velocity.X < 0f)
                    {
                        velocity.X *= wrongWayMultiplier;
                    }
                }
            }
            else
            {
                if (velocity.X > -maxSpeed)
                {
                    velocity.X -= addSpeedX;
                    if (velocity.X > 0f)
                    {
                        velocity.X *= wrongWayMultiplier;
                    }
                }
            }

            if (NPC.position.Y < spot.Y)
            {
                if (velocity.Y < maxSpeed)
                {
                    velocity.Y += addSpeedX;
                    if (velocity.Y < 0f)
                    {
                        velocity.Y *= wrongWayMultiplier;
                    }
                }
            }
            else
            {
                if (velocity.Y > -maxSpeed)
                {
                    velocity.Y -= addSpeedX;
                    if (velocity.Y > 0f)
                    {
                        velocity.Y *= wrongWayMultiplier;
                    }
                }
            }

            return velocity;
        }
        public Vector2 GetTo(Vector2 spot, float addSpeed = 0.8f, float maxSpeed = 20f, float wrongWayMultiplier = 0.96f, float minDistance = 50f)
        {
            return GetTo(spot, addSpeed, addSpeed, maxSpeed, wrongWayMultiplier, minDistance);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (dust == null)
            {
                dust = new List<DustParticle>();
            }
            if (NPC.IsABestiaryIconDummy)
            {
                UpdateEffects();
            }

            NPC.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int _);

            DrawDustnado(spriteBatch, NPC.position + offset, screenPos, dust.Where((v) => v.Z > 0f));

            DrawHead(spriteBatch, texture, offset, screenPos, frame, origin);

            DrawDustnado(spriteBatch, NPC.position + offset, screenPos, dust.Where((v) => v.Z <= 0f));

            return false;
        }

        public void DrawHead(SpriteBatch spriteBatch, Texture2D texture, Vector2 offset, Vector2 screenPos, Rectangle frame, Vector2 origin)
        {
            var circular = AequusHelpers.CircularVector(8, Main.GlobalTimeWrappedHourly);
            for (int i = 0; i < 2; i++)
            {
                float time = (auraTimer / 20f + 10f * i) % 20f;
                foreach (var v in circular)
                {
                    spriteBatch.Draw(texture, NPC.position + offset + v * time - screenPos, frame, Color.White.UseA(0) * (1f - time / 20f) * 0.33f,
                        NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
                }
            }
            spriteBatch.Draw(texture, NPC.position + offset - screenPos, frame, Color.White, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
        }
        public void DrawDustnado(SpriteBatch spriteBatch, Vector2 center, Vector2 screenPos, IEnumerable<DustParticle> dusts)
        {
            var viewPos = NPC.IsABestiaryIconDummy ? NPC.Center : new Vector2(screenPos.X + Main.screenWidth / 2f, screenPos.Y + Main.screenHeight / 2f);
            var texture = ModContent.Request<Texture2D>("Aequus/Particles/Dusts/MonoDust");
            if (texture.State != AssetState.Loaded)
            {
                return;
            }

            var frame = new Rectangle(0, 0, 10, 10);
            var origin = frame.Size() / 2f;
            foreach (var d in dusts)
            {
                var drawPosition = OrthographicView.GetViewPoint(new Vector2(d.X, d.Y) + center, d.Z, viewPos) - screenPos;
                var drawScale = OrthographicView.GetViewScale(d.Scale, d.Z * 8f);

                spriteBatch.Draw(texture.Value, drawPosition, frame, d.color * drawScale * d.Opacity, d.rotation, origin, drawScale, SpriteEffects.None, 0f);
            }
        }

        public class DustParticle
        {
            public Vector3 position;
            public float waveTime;
            public float rotation;
            public float scale;
            public float scale2;
            public Color color;
            public int alpha;

            public float yVelocity;
            public float rotVelocity;

            public float X => position.X;
            public float Y => position.Y;
            public float Z => position.Z;
            public float Opacity => 1f - alpha / 255f;
            public float Scale => scale * scale2;

            public DustParticle(Vector3 position)
            {
                this.position = position;
                scale = Main.rand.NextFloat(0.5f, 2.5f);
                color = Color.White;
                if (Main.rand.NextBool())
                {
                    color = Color.Lerp(color, Color.Orange, Main.rand.NextFloat(0.8f));
                }
                if (Main.rand.NextBool())
                {
                    color = Color.Lerp(color, Color.Blue, Main.rand.NextFloat(0.8f));
                }
                if (Main.rand.NextBool())
                {
                    color *= Main.rand.NextFloat(0.5f);
                }
                color.A = 0;
                yVelocity = -Main.rand.NextFloat(0.6f, 1.5f);
                rotVelocity = Main.rand.NextFloat(scale * 0.5f, scale * 1.25f) * yVelocity;
                rotVelocity *= 0.05f;
                scale2 = 1f;
            }
            public DustParticle(float x, float y, float z) : this(new Vector3(x, y, z))
            {

            }

            public bool Update(float npcWidth, float npcHeight, int effectsTimer)
            {
                position.Y += yVelocity;
                rotation += rotVelocity;
                float start = npcHeight * 1.5f;
                float end = -start;
                if (position.Y < end)
                {
                    alpha += Main.rand.Next(6);
                    rotVelocity *= 0.95f;
                    if (alpha > 255)
                    {
                        return false;
                    }
                }
                position.X = (float)Math.Sin(waveTime + position.Y * 0.05f);
                float progress = (-position.Y + start) / (start * 2f);
                position.X *= npcWidth * progress * Math.Min(progress * 3f, 1f);
                position.X += npcWidth * progress * 0.2f * (float)Math.Sin(effectsTimer * 0.025f + position.Y * 0.1225f);
                position.Z = (float)Math.Cos(waveTime + position.Y * 0.05f);
                if (progress < 0.4f)
                {
                    scale2 = progress / 0.4f;
                }
                else
                {
                    scale2 = 1f;
                }
                return true;
            }
        }
    }
}