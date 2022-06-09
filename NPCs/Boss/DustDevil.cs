using Aequus.Common.Utilities;
using Aequus.Graphics;
using Aequus.Projectiles.Monster.DustDevil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Boss
{
    [AutoloadBossHead]
    public class DustDevil : AequusBoss
    {
        public const int ACTION_FIREBALLS = 4;
        public const int ACTION_SUCTIONENEMIES = 3;
        public const int ACTION_SUCTIONTILES = 2;

        public float HPRatio => NPC.life / (float)NPC.lifeMax;
        public bool PhaseTwo => NPC.life * (Main.expertMode ? 2f : 4f) <= NPC.lifeMax;

        public int SecondaryAction { get => (int)NPC.ai[2]; set => NPC.ai[2] = value; }
        public float DirectActionTimer { get => NPC.ai[3]; set => NPC.ai[3] = value; }

        //public override bool IsLoadingEnabled(Mod mod)
        //{
        //    return false;
        //}

        public List<DustParticle> dust;
        public int effectsTimer;
        public int auraTimer;

        public static DrawIndexCache DrawBack { get; internal set; }
        public static DrawIndexCache DrawFront { get; internal set; }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                DrawBack = new DrawIndexCache();
                DrawFront = new DrawIndexCache();
            }
        }

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
            NPC.damage = 25;
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

            if (Main.getGoodWorld)
            {
                NPC.height *= 10;
            }
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

            if (Action != ACTION_GOODBYE && !NPC.HasValidTarget)
            {
                if (!CheckTargets())
                {
                    return;
                }
            }

            switch (Action)
            {
                case ACTION_FIREBALLS:
                    Fireballs();
                    break;

                case ACTION_SUCTIONENEMIES:
                    SuctionEnemies();
                    break;

                case ACTION_SUCTIONTILES:
                    SuctionTiles();
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

            if (Main.getGoodWorld)
            {
                for (int i = 0; i < 4; i++)
                {
                    UpdateEffects();
                }
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
            int amt = Main.rand.Next((int)Math.Max(20 * HPRatio * HPRatio, 2));
            for (int i = 0; i < amt; i++)
            {
                dust.Add(new DustParticle(0f, NPC.height * 1.5f, 0f) { waveTime = Main.rand.NextFloat(MathHelper.TwoPi) });
            }

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

        public void Fireballs()
        {
            ActionTimer++;
            if (ActionTimer == 60 || ActionTimer == 150)
            {
                NPC.velocity = (Main.player[NPC.target].Center + new Vector2(0f, -300f) - NPC.Center) / 40f;
                NPC.netUpdate = true;
            }
            if (ActionTimer == 90 || ActionTimer == 180)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    foreach (var v in AequusHelpers.CircularVector(8, ActionTimer == 180 ? (MathHelper.PiOver4 / 2f) : 0f))
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center,
                            v * 10f, ModContent.ProjectileType<DustDevilFireball>(), CalcDamage(1.25f), 1f, Main.myPlayer);
                    }
                }
                NPC.netUpdate = true;
            }
            if (ActionTimer >= 220)
            {
                int newPhase = SecondaryAction;
                ClearAI();
                Action = newPhase;
            }
            NPC.velocity *= 0.95f;
        }

        public void SuctionEnemies()
        {
            ActionTimer++;
            if (ActionTimer <= 120)
            {
                int delay = Main.getGoodWorld ? 10 : Main.expertMode ? 25 : 50;

                if (ActionTimer % delay == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (SuctionEnemies_EnsureYouWontBreakTheNPCCap())
                        {
                            var point = ScanTilePoint(Main.rand.NextVector2Unit()).ToWorldCoordinates() + new Vector2(8f);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), point, Vector2.Normalize(NPC.Center - point),
                                ModContent.ProjectileType<SuctionedEnemy>(), CalcDamage(), 1f, Main.myPlayer, ai1: NPC.whoAmI);
                        }
                    }
                }
                NPC.velocity = -Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center);
            }
            else
            {
                if (ActionTimer > 400)
                {
                    SetAction(ACTION_SUCTIONTILES);
                }
                NPC.velocity = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 2f;
            }
        }
        public bool SuctionEnemies_EnsureYouWontBreakTheNPCCap()
        {
            return Main.npc.Count((n) => n.active) <= Main.maxNPCs - 20;
        }

        public void SuctionTiles()
        {
            if ((int)ActionTimer == 0)
            {
                if (!CheckTargets())
                {
                    return;
                }
                NPC.velocity = (Main.player[NPC.target].Center + new Vector2(0f, -300f) - NPC.Center) / 40f;
            }
            else
            {
                NPC.velocity *= 0.95f;
            }
            
            ActionTimer++;
            if (ActionTimer <= 120)
            {
                if (ActionTimer == 120)
                {
                    if (!CheckTargets())
                    {
                        return;
                    }
                    if (Aequus.ShouldDoScreenEffect(NPC.Center))
                    {
                        Shake(10);
                        SoundEngine.PlaySound(SoundID.Item14);
                    }
                }
                else 
                {
                    int delay = Main.getGoodWorld ? 1 : Main.expertMode ? 5 : 10;

                    if (PhaseTwo)
                    {
                        delay *= 2;
                    }

                    if (ActionTimer % delay == 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            var point = ScanTilePoint(Main.rand.NextVector2Unit()).ToWorldCoordinates() + new Vector2(8f);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), point, Vector2.Normalize(NPC.Center - point),
                                ModContent.ProjectileType<RippedTile>(), CalcDamage(), 1f, Main.myPlayer, ai1: NPC.whoAmI);
                        }
                    }
                }
            }
            else 
            {
                if (ActionTimer > 400)
                {
                    SetAction(ACTION_SUCTIONENEMIES);
                }
                NPC.velocity = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center);
            }
        }
        public Point ScanTilePoint(Vector2 dir)
        {
            var center = NPC.Center;
            dir = Vector2.Normalize(dir) * 16f;
            int scanTiles = 75;
            for (int i = 0; i < scanTiles; i++)
            {
                var point = (center + dir * i).ToTileCoordinates();
                if (!WorldGen.InWorld(point.X, point.Y, 10) || Main.tile[point].IsSolid())
                {
                    return point;
                }
            }
            return (center + dir * scanTiles).ToTileCoordinates();
        }

        public void Intro()
        {
            if (ActionTimer < 120f)
            {
                if ((int)ActionTimer == 0)
                {
                    if (!CheckTargets())
                    {
                        return;
                    }
                    if (Main.netMode != NetmodeID.Server)
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            UpdateEffects();
                        }
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
                SetAction(ACTION_SUCTIONTILES);
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

        public int CalcDamage(float mult = 1f)
        {
            return (int)((Main.masterMode ? NPC.damage / 3 : Main.expertMode ? NPC.damage / 2 : NPC.damage) * mult);
        }

        public void SetAction(int phase)
        {
            ClearAI();
            if (PhaseTwo)
            {
                SecondaryAction = phase;
                Action = ACTION_FIREBALLS;
            }
            else
            {
                Action = phase;
            }
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

        public static void AddDraw(int proj, float z)
        {
            if (z > 0f)
            {
                DrawBack.Add(proj);
                return;
            }
            DrawFront.Add(proj);
        }

        public static bool CurrentlyDrawing(float z)
        {
            return z > 0f ? DrawBack.renderingNow : DrawFront.renderingNow;
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

        public static void GetTornadoInfo(float npcHeight, float y, out float start, out float end, out float progress)
        {
            start = npcHeight * 1.5f;
            end = -start;
            progress = (-y + start) / (start * 2f);
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
                GetTornadoInfo(npcHeight, position.Y, out float start, out float end, out float progress);
                if (position.Y < end)
                {
                    alpha += Main.rand.Next(6);
                    rotVelocity *= 0.95f;
                    if (alpha > 255)
                    {
                        return false;
                    }
                }
                position.X = (float)Math.Sin((waveTime + position.Y * 0.05f * (1f - progress)));
                position.X *= npcWidth * progress * Math.Min(progress * 3f, 1f);
                position.X += npcWidth * progress * 0.2f * (float)Math.Sin(effectsTimer * 0.025f + position.Y * 0.1225f);
                position.Z = (float)Math.Cos(waveTime + position.Y * 0.05f) * progress;
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