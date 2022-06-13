using Aequus.Biomes;
using Aequus.Common.Utilities;
using Aequus.Graphics;
using Aequus.Items.Misc.Energies;
using Aequus.Particles.Dusts;
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
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Boss
{
    [AutoloadBossHead]
    public class DustDevil : AequusBoss
    {
        public const int ACTION_ICE = 5;
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

        public static DrawList DrawBack { get; internal set; }
        public static DrawList DrawFront { get; internal set; }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                DrawBack = new DrawList();
                DrawFront = new DrawList();
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

            this.SetBiome<GaleStreamsInvasion>();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * bossLifeScale);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry)
                .AddMainSpawn(BestiaryBuilder.SkyBiome);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            this.CreateLoot(npcLoot)
                .Add<AtmosphericEnergy>(stack: 3);
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return PhaseTwo ? Color.Red : Color.White;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            int count = 1;
            if (NPC.life <= 0)
            {
                count = 50;
                foreach (var d in dust)
                {
                    var d2 = Dust.NewDustDirect(new Vector2(d.position.X, d.position.Y) + NPC.Center, 2, 2, ModContent.DustType<MonoDust>(), newColor: d.color * d.Opacity, Scale: d.Scale);
                    d2.velocity *= 1.5f;
                    d2.rotation = d.rotation;
                }
            }
            for (int i = 0; i < count; i++)
            {
                var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, Main.rand.NextBool() ? DustID.Torch : DustID.Frost);
                d.velocity = (d.position - NPC.Center) / 8f;
                if (Main.rand.NextBool(3))
                {
                    d.velocity *= 2f;
                    d.scale *= 1.75f;
                    d.fadeIn = d.scale + Main.rand.NextFloat(0.5f, 0.75f);
                    d.noGravity = true;
                }
            }
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
                case ACTION_ICE:
                    Ice();
                    break;

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

        public int CalcDamage(float mult = 1f)
        {
            return (int)((Main.masterMode ? NPC.damage / 3 : Main.expertMode ? NPC.damage / 2 : NPC.damage) * mult);
        }

        public void SetAction(int phase)
        {
            int secondary = SecondaryAction;
            ClearAI();
            if (secondary == 0 && PhaseTwo)
            {
                SecondaryAction = phase;
                Action = Main.rand.NextBool() ? ACTION_FIREBALLS : ACTION_ICE;
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

        public void Ice()
        {
            SetFrame(1);
            ActionTimer++;
            if (ActionTimer == 60 || ActionTimer == 120)
            {
                NPC.velocity = (Main.player[NPC.target].Center + new Vector2(0f, -300f) - NPC.Center) / 20f;
                NPC.netUpdate = true;
            }
            if (ActionTimer == 90 || ActionTimer == 180)
            {
                if (!CheckTargets())
                {
                    return;
                }
                int delay = Mode(15, 10, 5);
                if (ActionTimer % delay == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item34, NPC.position);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float speed = Mode(12f, 20f, 48f);
                        var toPlayer = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center);

                        var source = NPC.GetSource_FromAI();
                        int damage = CalcDamage(1.25f);
                        foreach (var r in AequusHelpers.Circular(Mode(6, 15, 50), Main.rand.NextFloat(MathHelper.TwoPi)))
                        {
                            Projectile.NewProjectileDirect(source, NPC.Center, toPlayer.RotatedBy(r) * speed, ModContent.ProjectileType<DustDevilFrostball>(), damage, 1f, Main.myPlayer, ai0: 1f);
                            Projectile.NewProjectileDirect(source, NPC.Center, toPlayer.RotatedBy(r) * speed, ModContent.ProjectileType<DustDevilFrostball>(), damage, 1f, Main.myPlayer, ai0: -1f);
                        }
                    }
                }
                NPC.netUpdate = true;
            }
            if (ActionTimer >= 200)
            {
                SetAction(SecondaryAction);
            }
            if (NPC.Distance(Main.player[NPC.target].Center) < 2000f)
            {
                NPC.velocity *= 0.92f;
            }
        }

        public void Fireballs()
        {
            SetFrame(0);
            ActionTimer++;
            if (ActionTimer == 60 || ActionTimer == 120)
            {
                NPC.velocity = (Main.player[NPC.target].Center + new Vector2(0f, -300f) - NPC.Center) / 40f;
                NPC.netUpdate = true;
            }
            if ((ActionTimer >= 90 && ActionTimer < 120) || (ActionTimer >= 150 && ActionTimer < 180))
            {
                if (!CheckTargets())
                {
                    return;
                }
                int delay = Mode(15, 10, 5);
                if (ActionTimer % delay == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item34, NPC.position);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float speed = Mode(12f, 20f, 48f);
                        var toPlayer = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center);

                        var source = NPC.GetSource_FromAI();
                        int amt = Mode(1, 3, 7);
                        int damage = CalcDamage(1.25f);
                        float r = Main.rand.NextFloat(-0.5f, 0.5f);
                        for (int i = 0; i < amt; i++)
                        {
                            Projectile.NewProjectileDirect(source, NPC.Center, toPlayer.RotatedBy(MathHelper.PiOver4 / 2f * i + r) * speed, ModContent.ProjectileType<DustDevilFireball>(), damage, 1f, Main.myPlayer, ai0: 1f);
                            if (i != 0)
                                Projectile.NewProjectileDirect(source, NPC.Center, toPlayer.RotatedBy(MathHelper.PiOver4 / 2f * -i + r) * speed, ModContent.ProjectileType<DustDevilFireball>(), damage, 1f, Main.myPlayer, ai0: -1f);
                        }
                    }
                }
                NPC.netUpdate = true;
            }
            if (ActionTimer >= 200)
            {
                SetAction(SecondaryAction);
            }
            if (NPC.Distance(Main.player[NPC.target].Center) < 2000f)
            {
                NPC.velocity *= 0.92f;
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
                NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * Mode(3f, 6f, 6f), 0.1f);
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
            else if (NPC.Distance(Main.player[NPC.target].Center) < 2000f)
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
                        Flash(NPC.Center, 0.25f, 0.8f);
                        SoundEngine.PlaySound(SoundID.Item14);
                    }
                }
                else
                {
                    int delay = Main.getGoodWorld ? 1 : Main.expertMode ? 5 : 10;

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
                NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * Mode(3f, 6f, 6f), 0.1f);
            }
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

        public override void OnKill()
        {
            AequusSystem.MarkAsDefeated(ref AequusSystem.downedEventGaleStreams, NPC.type);
            AequusSystem.MarkAsDefeated(ref AequusSystem.downedDustDevil, NPC.type);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (dust == null)
            {
                dust = new List<DustParticle>();
                if (NPC.IsABestiaryIconDummy)
                {
                    for (int i = 0; i < 500; i++)
                    {
                        UpdateEffects();
                    }
                }
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