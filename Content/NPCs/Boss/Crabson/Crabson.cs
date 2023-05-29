using Aequus;
using Aequus.Common;
using Aequus.Common.Effects;
using Aequus.Content.Biomes.CrabCrevice;
using Aequus.Content.Boss.Crabson.Projectiles;
using Aequus.Content.NPCs.Boss.Crabson;
using Aequus.Content.NPCs.Boss.Crabson.Projectiles;
using Aequus.Content.NPCs.Boss.Crabson.Rewards;
using Aequus.Content.Town.ExporterNPC;
using Aequus.Items.Consumables.Permanent;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Weapons.Ranged.Misc;
using Aequus.NPCs;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.NPCs.Boss.Crabson {
    public abstract class CrabsonSegment : AequusBoss {
        #region Constants
        public const int ACTION_CLAWSHOTS = 2;
        public const int PHASE_GROUNDBUBBLES = 3;
        public const int ACTION_COMEONANDSLAM = 4;
        public const int PHASE2_GROUNDBUBBLES_SPAMMY = 5;
        public const int ACTION_P2_CLAWSHOTS_SHRAPNEL = 6;
        public const int ACTION_WELCOMETOTHESLAMJAM = 8;
        public const int ACTION_CLAWRAIN = 9;
        public const int ACTION_NOATTACK = 10;
        #endregion

        public int npcHandLeft = -1;
        public int npcHandRight = -1;
        public int npcBody = -1;

        public bool contactDamage;

        public NPC HandLeft => Main.npc[npcHandLeft];
        public NPC HandRight => Main.npc[npcHandRight];
        public NPC Body => Main.npc[npcBody];

        public int SharedAction => (int)Main.npc[npcBody].ai[0];
        public bool PhaseTwo => Body.life * (Main.expertMode ? 2f : 4f) <= Body.lifeMax;
        public float LifeRatio => Math.Clamp(Body.life / (float)Body.lifeMax, 0f, 1f);
        public float BattleProgress => 1f - LifeRatio;
        public static readonly ConfiguredMusicData ConfiguredMusic = new(MusicID.Boss3, MusicID.OtherworldlyBoss2);

        public override void SetDefaults() {
            npcHandLeft = -1;
            npcHandRight = -1;
            npcBody = -1;

            NPC.lifeMax = 2500;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(gold: 2);
            NPC.aiStyle = -1;
            NPC.lavaImmune = true;
            NPC.trapImmune = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            if (Main.netMode != NetmodeID.Server) {
                Music = ConfiguredMusic.GetID();
                SceneEffectPriority = SceneEffectPriority.BossLow;
            }
        }

        protected bool CheckClaws() {
            if (npcHandLeft != -1 && (!HandLeft.active || HandLeft.ModNPC is not CrabsonSegment)) {
                return false;
            }
            if (npcHandRight != -1 && (!HandRight.active || HandRight.ModNPC is not CrabsonSegment)) {
                return false;
            }
            return true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) {
            return contactDamage;
        }

        protected void SharedAI() {
            if (!NPC.dontTakeDamage) {
                AequusPlayer.DashImmunityHack.Add(NPC);
            }
        }

        protected void DrawClaw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, float mouthAnimation) {
            var claw = AequusTextures.CrabsonClaw_Crabson.Value;
            var origin = new Vector2(claw.Width / 2f + 20f, claw.Height / 8f);
            var drawCoords = npc.Center + new Vector2(npc.direction * 10f, -20f) - screenPos;
            if (NPC.ModNPC != null) {
                drawCoords.Y += NPC.ModNPC.DrawOffsetY;
            }
            SpriteEffects spriteEffects;
            bool flip;
            if (npc.rotation == 0f) {
                spriteEffects = npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                flip = npc.direction == 1;
                if (!flip) {
                    origin.X = AequusTextures.CrabsonClaw_Crabson.Width - origin.X;
                }
            }
            else {
                spriteEffects = Math.Abs(npc.rotation) > MathHelper.PiOver2 ? SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally : SpriteEffects.FlipHorizontally;
                flip = spriteEffects.HasFlag(SpriteEffects.FlipVertically);
            }

            DrawClawManual(spriteBatch, claw, drawCoords, drawColor, origin, npc.rotation, flip ? -mouthAnimation : mouthAnimation, npc.scale, spriteEffects);
        }
        protected void DrawClawManual(SpriteBatch spriteBatch, Texture2D claw, Vector2 drawCoords, Color drawColor, Vector2 origin, float rotation, float mouthAnimation, float scale, SpriteEffects spriteEffects) {
            int frameHeight = claw.Height / 4;
            var clawFrame = new Rectangle(0, frameHeight, claw.Width, frameHeight - 2);
            spriteBatch.Draw(claw, drawCoords, clawFrame, drawColor, -mouthAnimation + 0.1f * NPC.direction + rotation, origin, scale, spriteEffects, 0f);
            spriteBatch.Draw(claw, drawCoords, clawFrame with { Y = 0, }, drawColor, mouthAnimation + 0.1f * NPC.direction + rotation, origin, scale, spriteEffects, 0f);
            spriteBatch.Draw(claw, drawCoords, clawFrame with { Y = frameHeight * (Math.Abs(mouthAnimation) > 0.05f ? 3 : 2), }, drawColor, 0.1f * NPC.direction + rotation, origin, scale, spriteEffects, 0f);
        }

        protected void ResetActionTimers() {
            ActionTimer = 0;
            NPC.ai[2] = 0f;
            NPC.ai[3] = 0f;
            NPC.localAI[0] = 0f;
            HandLeft.ai[1] = 0f;
            HandRight.ai[1] = 0f;
        }
    }

    [AutoloadBossHead]
    public class Crabson : CrabsonSegment {
        public struct WalkManager {

            public const int MaxFrames = 8;

            public int frame;
            public float frameCounter;
            public int soundDelay;
            public SlotId walkingSoundSlot;

            public void Update(NPC npc) {

                float magnitudeX = Math.Abs(npc.velocity.X);
                if (soundDelay > 0) {
                    soundDelay--;
                }

                bool aboveAir = Math.Abs(npc.velocity.Y) > 0.001f;
                if (aboveAir) {
                    frame = 7;
                    frameCounter = 0f;
                }
                else {
                    frameCounter += magnitudeX;
                    if (frameCounter > 10f) {
                        frame = (frame + 1) % 7;
                        frameCounter = 0f;
                    }

                    if (magnitudeX < 0.1f) {
                        frame = 0;
                    }
                }

                if (npc.IsABestiaryIconDummy || Main.netMode == NetmodeID.Server) {
                    return;
                }

                if (magnitudeX > 0.5f && !aboveAir) {

                    if (!walkingSoundSlot.IsValid || soundDelay <= 0) {
                        walkingSoundSlot = SoundEngine.PlaySound(AequusSounds.walk with { Volume = 0.5f }, npc.Bottom + new Vector2(0f, -12f));
                        soundDelay = 70;
                    }
                }
                else if ((aboveAir || magnitudeX < 0.2f) && walkingSoundSlot.IsValid && SoundEngine.TryGetActiveSound(walkingSoundSlot, out var sound)) {

                    sound.Stop();
                }
            }
        }
        public struct EyeManager {
            public int eyeFrame;
            public int eyeJitter;

            public Vector2 pupil;

            public void Update(NPC npc) {
                eyeJitter++;

                if (!npc.HasPlayerTarget) {
                    pupil *= 0.9f;
                    return;
                }

                pupil = Vector2.Lerp(pupil, npc.DirectionTo(Main.player[npc.target].Center) * 2f, 0.05f);
            }

            public int GetFrame() {
                return eyeFrame * 2 + GetPupilFrame();
            }
            public int GetPupilFrame() {
                return eyeJitter / 3 % 2;
            }
        }
        public struct ArmsManager {
            public record struct ArmPoint(Vector2 Position, Vector2 OldPosition, float Progress);

            public List<ArmPoint> generatedPoints;

            public Vector2 beizerPoint;
            public float beizerPointTransition;

            private static Vector2 BezierCurve(Vector2[] bezierPoints, float bezierProgress) {
                if (bezierPoints.Length == 1) {
                    return bezierPoints[0];
                }
                else {
                    Vector2[] newBezierPoints = new Vector2[bezierPoints.Length - 1];
                    for (int i = 0; i < bezierPoints.Length - 1; i++) {
                        newBezierPoints[i] = bezierPoints[i] * bezierProgress + bezierPoints[i + 1] * (1 - bezierProgress);
                    }
                    return BezierCurve(newBezierPoints, bezierProgress);
                }
            }
            private static Vector2 BezierCurveDerivative(Vector2[] bezierPoints, float bezierProgress) {
                if (bezierPoints.Length == 2) {
                    return bezierPoints[0] - bezierPoints[1];
                }
                else {
                    Vector2[] newBezierPoints = new Vector2[bezierPoints.Length - 1];
                    for (int i = 0; i < bezierPoints.Length - 1; i++) {
                        newBezierPoints[i] = bezierPoints[i] * bezierProgress + bezierPoints[i + 1] * (1 - bezierProgress);
                    }
                    return BezierCurveDerivative(newBezierPoints, bezierProgress);
                }
            }

            public ArmsManager() {
                generatedPoints = new();
                beizerPoint = Vector2.Zero;
                beizerPointTransition = 0f;
            }

            public void Clear() {
                generatedPoints.Clear();
            }

            public void Update(NPC npc, Vector2 start, Vector2 end) {

                if (Main.netMode == NetmodeID.Server) {
                    return;
                }

                Vector2[] bezierPoints = { end, new Vector2(start.X + Math.Sign(start.X - npc.Center.X) * 100f, (end.Y + start.Y) / 2f + 100f), start };

                // Code is taken from Star Construct, which is from the Polarities mod
                float bezierProgress = 0;
                float bezierIncrement = 18 * npc.scale;

                int loops = 0;
                while (bezierProgress < 1 && loops < 150) {

                    Vector2 oldPos = BezierCurve(bezierPoints, bezierProgress);

                    while ((oldPos - BezierCurve(bezierPoints, bezierProgress)).Length() < bezierIncrement) {
                        bezierProgress += 0.1f / BezierCurveDerivative(bezierPoints, bezierProgress).Length();
                    }

                    Vector2 newPos = BezierCurve(bezierPoints, bezierProgress);
                    generatedPoints.Add(new ArmPoint(newPos, oldPos, bezierProgress));
                }
            }

            public void DrawArms(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos) {

                var chain = AequusTextures.Crabson_Chain.Value;
                foreach (var data in generatedPoints) {

                    var drawPos = (data.OldPosition + data.Position) / 2f;
                    var chainFrame = chain.Frame(verticalFrames: 3, frameY: data.Progress < 0.3f ? 2 : data.Progress < 0.6f ? 1 : 0);
                    spriteBatch.Draw(
                        chain,
                        drawPos - screenPos,
                        chainFrame,
                        Helper.GetColor(drawPos),
                        (data.OldPosition - data.Position).ToRotation(),
                        chainFrame.Size() / 2f,
                        npc.scale, SpriteEffects.None, 0f);
                }
            }
        }

        public const float BossProgression = 2.66f;

        public int hitPlayer;

        public EyeManager eyeManager;
        public WalkManager walkManager;
        public ArmsManager arms;

        public override void SetStaticDefaults() {
            Main.npcFrameCount[Type] = 4;

            NPCID.Sets.TrailingMode[Type] = 7;
            NPCID.Sets.TrailCacheLength[Type] = 8;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.DebuffImmunitySets[Type] = new() {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Wet,
                    BuffID.Confused,
                    BuffID.Suffocation,
                    BuffID.Lovestruck,
                }
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = new(0) {
                PortraitPositionYOverride = 48f,
                Position = new(0f, 60f),
                Scale = 0.8f,
                PortraitScale = 1f,
                Velocity = 2f,
                Direction = -1,
            };

            SnowgraveCorpse.NPCBlacklist.Add(Type);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
            this.CreateEntry(database, bestiaryEntry);
        }

        public override void SetDefaults() {
            base.SetDefaults();

            NPC.width = 90;
            NPC.height = 60;
            NPC.damage = 10;
            NPC.defense = 6;
            NPC.boss = true;
            NPC.behindTiles = true;

            this.SetBiome<CrabCreviceBiome>();

            npcBody = -1;
            npcHandLeft = -1;
            npcHandRight = -1;
            eyeManager = new();
            walkManager = new();
            arms = new();
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: balance -> balance (bossAdjustment is different, see the docs for details) */ {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance);
        }

        public override void HitEffect(NPC.HitInfo hit) {
            if (NPC.life <= 0) {
                if (Main.netMode != NetmodeID.Server) {
                    for (int i = 0; i < 50; i++) {
                        Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
                    }
                }
            }
            else {
                for (int i = 0; i < Math.Min(hit.Damage / 20 + 1, 1); i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
            }
        }

        #region AI
        private void WalkTowards(Vector2 location, float horizontalSpeed = 0.1f, float jumpingDistanceThreshold = 80f) {
            var center = NPC.Center;
            var tileCenter = center.ToTileCoordinates();
            float distance = NPC.Distance(location);
            float distanceX = Math.Abs(center.X - location.X);
            float distanceY = Math.Abs(center.Y - location.Y);
            int direction = Math.Sign(location.X - center.X);
            if (PhaseTwo) {
                horizontalSpeed *= 2f;
            }
            if (Main.getGoodWorld) {
                horizontalSpeed *= 2f;
            }

            if (NPC.velocity.Y == 0f || NPC.velocity.Length() < 1f) {
                int horizontalCheck = 4 + (int)Math.Abs(NPC.velocity.X / 8f);
                int verticalCheck = (int)Math.Ceiling(NPC.height / 32f);
                for (int j = 0; j < verticalCheck; j++) {
                    int y = tileCenter.Y + j;
                    for (int i = 0; i <= horizontalCheck; i++) {
                        int x = tileCenter.X + (i + NPC.width / 16) * direction;
                        if (!WorldGen.InWorld(x, y)) {
                            continue;
                        }
                        //if (Main.GameUpdateCount % 10 == 0)
                        //    Helper.DebugDust(x, y);
                        var tile = Main.tile[x, y];
                        if (!tile.IsFullySolid()) {
                            continue;
                        }

                        NPC.velocity.Y = -7f;
                        NPC.velocity.X += 1f * -direction;
                        break;
                    }
                }
            }

            if (NPC.collideX || distanceX < jumpingDistanceThreshold) {
                if (center.Y > location.Y - 48f) {

                    if (NPC.velocity.Y == 0f) {
                        NPC.velocity.Y = Math.Min(-distanceY / 16f, -12f);
                    }
                    else if (NPC.collideY) {
                        NPC.velocity.Y = NPC.oldVelocity.Y;
                    }
                }
                else {
                    NPC.noGravity = false;
                    NPC.noTileCollide = false;
                }
            }
            else if (location.Y > center.Y - 64f) {
                NPC.noGravity = false;
            }

            if (NPC.velocity.Y > -12f && center.Y < location.Y) {
                NPC.velocity.Y *= 0.925f;
            }

            if (!Collision.CanHitLine(NPC.position + NPC.velocity, NPC.width, NPC.height, location, 0, 0)) {
                NPC.noTileCollide = true;
                if (NPC.position.Y > location.Y) {
                    NPC.noGravity = true;
                    if (NPC.velocity.Y > 0f) {
                        NPC.velocity.Y *= 0.9f;
                        NPC.velocity.Y -= 0.3f;
                        if (NPC.velocity.Y < 0f) {
                            NPC.velocity.Y = 0f;
                        }
                    }
                }
            }

            NPC.velocity.X += direction * horizontalSpeed;
            if (Math.Sign(NPC.velocity.X) != direction) {
                NPC.velocity.X *= 0.8f;
            }
        }

        private void HaltMovement() {
            NPC.velocity.X *= 0.95f;
            NPC.noTileCollide = false;
            NPC.noGravity = false;
        }

        public override void AI() {

            SharedAI();
            npcBody = NPC.whoAmI;
            NPC.realLife = NPC.whoAmI;
            AequusSystem.CrabsonNPC = NPC.whoAmI;
            AequusNPC.ForceZen(NPC);
            if (NPC.alpha > 0) {
                NPC.alpha -= 5;
                if (NPC.alpha < 0) {
                    NPC.alpha = 0;
                }
            }

            if (Action == ACTION_GOODBYE) {
                State_Goodbye();
                return;
            }

            if (!NPC.HasPlayerTarget || !NPC.HasValidTarget) {
                NPC.TargetClosest();
            }
            float distance = NPC.Distance(Main.player[NPC.target].Center);
            if (distance > 600f) {
                NPC.TargetClosest();
            }
            float lifeRatio = Math.Clamp(NPC.life / (float)NPC.lifeMax, 0f, 1f);
            float battleProgress = 1f - lifeRatio;
            Player target = Main.player[NPC.target];

            if (NPC.noGravity && NPC.velocity.Y < 0f) {
                NPC.velocity.Y += 0.3f;
                if (NPC.velocity.Y > 0f) {
                    NPC.velocity.Y = 0f;
                }
            }

            var topLeftTile = NPC.position.ToTileCoordinates();
            var centerTile = NPC.Center.ToTileCoordinates();
            if (Main.netMode != NetmodeID.Server && Main.GameUpdateCount % 12 == 0) {

                for (int i = 0; i < NPC.width / 16 + 1; i++) {
                    for (int j = 0; j < NPC.height / 16 + 1; j++) {
                        if (Main.tile[topLeftTile.X + i, topLeftTile.Y + j].IsFullySolid()) {
                            goto DigEffect;
                        }
                    }
                }
                goto CrabsonActions;

            DigEffect:
                Collision.HitTiles(NPC.position, NPC.velocity, NPC.width, NPC.height);
                SoundEngine.PlaySound(SoundID.WormDig, NPC.Center);
            }

        CrabsonActions:
            if (Action != ACTION_INIT && !CheckClaws()) {
                NPC.KillEffects(quiet: true);
                return;
            }

            switch (Action) {
                case ACTION_INIT: {

                        NPC.TargetClosest();
                        npcHandLeft = NPC.NewNPC(NPC.GetSource_FromThis(),
                            (int)NPC.Center.X + -100, (int)NPC.Center.Y,
                            ModContent.NPCType<CrabsonClaw>(),
                            NPC.whoAmI,
                            ai3: -1f
                        );
                        npcHandRight = NPC.NewNPC(NPC.GetSource_FromThis(),
                            (int)NPC.Center.X + 100, (int)NPC.Center.Y,
                            ModContent.NPCType<CrabsonClaw>(),
                            NPC.whoAmI,
                            ai3: 1f
                        );
                        Action = ACTION_INTRO;
                        break;
                    }

                case ACTION_INTRO: {
                        Action = 10;
                        break;
                    }

                case ACTION_CLAWSHOTS: {
                        bool canSeeTarget = CanSeeTarget();
                        if (distance > 500f || !canSeeTarget) {
                            WalkTowards(Main.player[NPC.target].Center);
                        }
                        else {
                            HaltMovement();
                        }
                        HandRight.ai[1]++;
                        HandLeft.ai[1]++;
                        ActionTimer += 1f;
                        if (canSeeTarget) {
                            ActionTimer += battleProgress * 2f;
                        }
                        if (ActionTimer > 600) {
                            Action = ACTION_INTRO;
                            ActionTimer = 0;
                        }
                    }
                    break;

                case ACTION_COMEONANDSLAM: {
                        if (distance > 200f || !CanSeeTarget()) {
                            WalkTowards(Main.player[NPC.target].Center);
                        }
                        else {
                            HaltMovement();
                        }
                        break;
                    }

                case ACTION_WELCOMETOTHESLAMJAM: {
                        State_WelcomeToTheSlamJam(distance, centerTile, target);
                        break;
                    }

                case ACTION_CLAWRAIN: {
                        ActionTimer += 1f + BattleProgress * 3f;
                        bool canSeeTarget = CanSeeTarget();
                        if (distance > 100f || !canSeeTarget) {
                            WalkTowards(Main.player[NPC.target].Center);
                        }
                        else {
                            HaltMovement();
                        }
                        if (ActionTimer > 120) {
                            Action = ACTION_NOATTACK;
                            ActionTimer = 0;
                        }
                    }
                    break;

                case ACTION_NOATTACK: {
                        NPC.localAI[0] = 0f;
                        if (!NPC.HasPlayerTarget || !NPC.HasValidTarget) {
                            Action = ACTION_GOODBYE;
                            ActionTimer = 0;
                            NPC.netUpdate = true;
                            return;
                        }
                        if (ActionTimer == 0) {
                            NPC.noGravity = false;
                            NPC.noTileCollide = false;
                        }
                        bool canSeePlayer = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height);
                        if (distance > 500f || !canSeePlayer) {
                            WalkTowards(Main.player[NPC.target].Center);
                        }
                        else {
                            HaltMovement();
                        }
                        ActionTimer++;
                        if (distance < 600f && canSeePlayer) {
                            ActionTimer += battleProgress * 30f;
                        }
                        if (ActionTimer > 600) {
                            Action = Main.rand.NextFromList(ACTION_CLAWRAIN, ACTION_WELCOMETOTHESLAMJAM);
                            ResetActionTimers();
                        }
                        break;
                    }
            }
        }

        private void State_WelcomeToTheSlamJam(float distance, Point centerTile, Player target) {
            if (ActionTimer == 0) {
                if ((distance > 120f || !CanSeeTarget() || !target.pulley && Helper.FindFloor(centerTile.X, centerTile.Y, 3) == -1) && distance > 50f) {
                    WalkTowards(target.Center);
                    return;
                }

                ActionTimer++;
            }

            if (ActionTimer == 1) {
                if (Math.Abs(NPC.velocity.Y) > 0.1f && !target.pulley) {
                    NPC.velocity *= 0.8f;
                    NPC.noGravity = true;
                    NPC.noTileCollide = false;
                    return;
                }
                SoundEngine.PlaySound(AequusSounds.superJump with { Pitch = 0.33f, Volume = 0.5f, }, NPC.Center);
                NPC.velocity.X = 0f;
                NPC.velocity.Y = -17f;
                NPC.netUpdate = true;
                NPC.noGravity = true;
                NPC.noTileCollide = true;
                ActionTimer++;

                for (int i = 0; i < 30; i++) {
                    var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GemSapphire, NPC.velocity.X, NPC.velocity.Y);
                    d.velocity += NPC.velocity;
                    d.velocity *= Main.rand.NextFloat();
                    d.noGravity = true;
                    d.fadeIn = d.scale + Main.rand.NextFloat(2f);
                }
                return;
            }

            if (NPC.velocity.Length() > 1f && ActionTimer > 10) {
                var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GemSapphire, NPC.velocity.X, -NPC.velocity.Y);
                d.velocity *= 0.1f;
                d.velocity.X += NPC.velocity.X;
                d.velocity.Y -= NPC.velocity.Y;
                d.noGravity = true;
                d.fadeIn = d.scale + Main.rand.NextFloat(0.5f);
            }
            NPC.velocity.Y -= 0.2f;
            NPC.velocity.Y *= 0.93f;
            NPC.velocity.X *= 0.9f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;

            ActionTimer += 1f + BattleProgress * 2f;
            if (ActionTimer >= 50 && NPC.localAI[0] <= 0f) {
                SoundEngine.PlaySound(AequusSounds.chargeUp0, NPC.Center);
                NPC.localAI[0]++;
            }
            if (ActionTimer >= 70 && NPC.ai[2] <= 0f) {
                NewProjectile<CrabsonSlamProj>(default, Vector2.Zero, Mode(70, 40, 20));
                NPC.ai[2]++;
            }
            if (ActionTimer > 180) {
                Action = ACTION_NOATTACK;
                ResetActionTimers();
                NPC.noGravity = false;
                NPC.noTileCollide = false;
            }
        }

        private void State_Goodbye() {
            if (ActionTimer == 0) {
                NPC.noGravity = false;
                NPC.noTileCollide = false;
                if (!Main.player[NPC.target].active) {
                    NPC.direction = Main.rand.NextBool() ? -1 : 1;
                    NPC.netUpdate = true;
                }
                else {
                    NPC.direction = Math.Sign(NPC.Center.X - Main.player[NPC.target].Center.X);
                }
            }
            NPC.timeLeft = Math.Min(NPC.timeLeft, 60);
            WalkTowards(NPC.Center + new Vector2(NPC.direction * 100f, 0f),
                horizontalSpeed: 0.02f);
            if (Math.Abs(NPC.velocity.X) > 4f) {
                NPC.velocity.X *= 0.9f;
            }
        }
        #endregion

        public override void FindFrame(int frameHeight) {

            eyeManager.Update(NPC);
            walkManager.Update(NPC);
            if (Main.netMode == NetmodeID.Server || NPC.IsABestiaryIconDummy) {
                return;
            }

            Vector2 chainOffset = new(44f, -14f);
            Vector2 chainEndOffset = new(20f, 0f);
            arms.Clear();
            arms.Update(NPC, NPC.Center + chainOffset with { X = -chainOffset.X }, HandLeft.Center + chainEndOffset.RotatedBy(HandLeft.rotation == 0f ? MathHelper.Pi : HandLeft.rotation));
            arms.Update(NPC, NPC.Center + chainOffset, HandRight.Center + chainEndOffset.RotatedBy(HandRight.rotation));
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo) {
            if (Main.rand.NextBool(8)) {
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.PickBreak>(), 480);
            }
        }

        public override void SendExtraAI(BinaryWriter writer) {
            writer.WriteNPCIndex(npcHandLeft);
            writer.WriteNPCIndex(npcHandRight);
            writer.WriteNPCIndex(npcBody);
        }

        public override void ReceiveExtraAI(BinaryReader reader) {
            npcHandLeft = reader.ReadNPCIndex();
            npcHandRight = reader.ReadNPCIndex();
            npcBody = reader.ReadNPCIndex();
        }

        public override void DrawBehind(int index) {
            Main.instance.DrawCacheProjsBehindNPCsAndTiles.Add(index);
        }

        #region Drawing
        private void DrawBody(SpriteBatch spriteBatch, Vector2 screenPos, Vector2 offset, Color bodyDrawColor) {
            offset.Y -= 24;
            var drawPosition = NPC.Center - screenPos + offset;
            spriteBatch.Draw(
                TextureAssets.Npc[NPC.type].Value,
                drawPosition,
                NPC.frame,
                bodyDrawColor,
                NPC.rotation,
                NPC.frame.Size() / 2f,
                NPC.scale, SpriteEffects.None, 0f);

            var legFrame = AequusTextures.Crabson_Legs.Frame(verticalFrames: WalkManager.MaxFrames, frameY: walkManager.frame);
            spriteBatch.Draw(
                AequusTextures.Crabson_Legs,
                drawPosition,
                legFrame,
                bodyDrawColor,
                NPC.rotation,
                legFrame.Size() / 2f,
                NPC.scale, NPC.velocity.X < 0f ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            var eyeOffset = new Vector2(0f, -40f);
            var eyePosition = drawPosition + eyeOffset;
            var eyeFrame = AequusTextures.Crabson_Eyes.Frame(verticalFrames: 8, frameY: eyeManager.GetFrame());
            var eyeOrigin = eyeFrame.Size() / 2f;
            spriteBatch.Draw(
                AequusTextures.Crabson_Eyes,
                eyePosition,
                eyeFrame,
                Color.White,
                NPC.rotation,
                eyeOrigin,
                NPC.scale, SpriteEffects.None, 0f);

            var pupilFrame = AequusTextures.Crabson_Pupil.Frame(verticalFrames: 2, frameY: eyeManager.GetPupilFrame());
            var pupilOrigin = pupilFrame.Size() / 2f;
            spriteBatch.Draw(
                AequusTextures.Crabson_Pupil,
                eyePosition + eyeManager.pupil,
                pupilFrame,
                Color.White,
                NPC.rotation,
                pupilOrigin,
                NPC.scale, SpriteEffects.None, 0f);

            var trailOffset = NPC.Size / 2f;
            int trailLength = NPCID.Sets.TrailCacheLength[Type];
            var trailColor = Color.White with { A = 0 } * Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.25f, 0.4f);
            for (int i = 0; i < trailLength; i++) {

                float progress = Helper.CalcProgress(trailLength, i);
                var trailClr = trailColor * MathF.Pow(progress, 2f);
                var eyeTrailPosition = NPC.oldPos[i] + trailOffset + eyeOffset - screenPos + offset;
                spriteBatch.Draw(
                    AequusTextures.Crabson_Eyes,
                    eyeTrailPosition,
                    eyeFrame,
                    trailClr,
                    NPC.rotation,
                    eyeOrigin,
                    NPC.scale, SpriteEffects.None, 0f);

                spriteBatch.Draw(
                    AequusTextures.Crabson_Pupil,
                    eyeTrailPosition + eyeManager.pupil,
                    pupilFrame,
                    trailClr,
                    NPC.rotation,
                    pupilOrigin,
                    NPC.scale, SpriteEffects.None, 0f);
            }
        }

        private void DrawBestiary(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            Vector2 offset = new Vector2(120f, 58f) * NPC.scale;
            NPC.direction = 1;
            NPC.spriteDirection = 1;
            DrawClaw(NPC, spriteBatch, screenPos + (offset with { X = -offset.X }), Color.White, 0f);
            NPC.direction = -1;
            NPC.spriteDirection = -1;
            DrawClaw(NPC, spriteBatch, screenPos + offset, Color.White, 0f);

            DrawBody(
                spriteBatch,
                screenPos,
                new(0f, 0f),
                Color.White
            );
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            if (NPC.IsABestiaryIconDummy) {
                DrawBestiary(spriteBatch, screenPos, drawColor);
                return false;
            }

            arms.DrawArms(NPC, spriteBatch, screenPos);
            DrawBody(spriteBatch, screenPos, new(), NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)));
            return false;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) {
            scale = 1.5f;
            return null;
        }
        #endregion

        public override void ModifyNPCLoot(NPCLoot npcLoot) {
            this.CreateLoot(npcLoot)
                .AddBossLoot<CrabsonTrophy, CrabsonRelic, CrabsonBag>()
                .ExpertDropForCrossModReasons<MoneyTrashcan>()
                .AddPerPlayer<AquaticEnergy>(stack: 3)

                .SetCondition(new Conditions.NotExpert())
                .Add<CrabsonMask>(chance: 7, stack: 1)
                .AddOptions(chance: 1, ModContent.ItemType<JunkJet>())
                .RegisterCondition();
        }

        private void CheckClosestSegmentForLoot(byte player, ref float distance, NPC npc) {
            float d = npc.Distance(Main.player[player].Center);
            if (Collision.SolidCollision(npc.position, npc.width, npc.height)) {
                d *= 2f;
            }
            if (Collision.CanHitLine(Main.player[player].position, Main.player[player].width, Main.player[player].height, npc.position, npc.width, npc.height)) {
                d /= 20f;
            }
            if (d < distance) {
                distance = d;
                NPC.Center = npc.Center;
            }
        }
        public override bool PreKill() {
            byte player = Player.FindClosest(NPC.position, NPC.width, NPC.height);
            float distance = NPC.Distance(Main.player[player].Center);
            CheckClosestSegmentForLoot(player, ref distance, HandLeft);
            CheckClosestSegmentForLoot(player, ref distance, HandRight);
            return true;
        }

        public override void OnKill() {
            Rectangle rect = NPC.getRect();
            if (!AequusWorld.downedCrabson) {
                if (!NPC.AnyNPCs(ModContent.NPCType<Exporter>())) {
                    NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.position.X + NPC.width / 2, (int)NPC.position.Y + NPC.height / 2, ModContent.NPCType<Exporter>());
                }
                AequusWorld.MarkAsDefeated(ref AequusWorld.downedCrabson, NPC.type);
            }
        }
    }

    [AutoloadBossHead]
    public class CrabsonClaw : CrabsonSegment {

        public float mouthAnimation;

        public override void SetStaticDefaults() {
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = new(0) {
                Hide = true,
            };
        }

        public override void SetDefaults() {
            base.SetDefaults();
            NPC.width = 90;
            NPC.height = 90;
            NPC.damage = 40;
            NPC.defense = 20;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            DrawOffsetY = 14f;
        }

        public override void OnSpawn(IEntitySource source) {
            if (!Helper.HereditarySource(source, out var entity) || entity is not NPC parentNPC) {
                return;
            }

            npcBody = parentNPC.whoAmI;
        }

        private void SetHand(CrabsonSegment segment) {
            if (segment.NPC.direction == -1) {
                npcHandLeft = segment.NPC.whoAmI;
            }
            else {
                npcHandRight = segment.NPC.whoAmI;
            }
        }

        private Vector2 GetRestingPosition(NPC body, Vector2 offset) {
            return body.Center + new Vector2(NPC.direction * 170f, body.height * -1.5f) + offset;
        }

        private void GoTo(Vector2 where, float scalingSpeed, float flatSpeed, float maxSpeed = 6f, float restingDistance = 50f, float restingSpeed = 0.9f) {
            var diff = where - NPC.Center;
            NPC.velocity += diff * scalingSpeed;
            NPC.velocity += Vector2.Normalize(diff) * flatSpeed;
            if (NPC.velocity.Length() > maxSpeed || diff.Length() < restingDistance) {
                NPC.velocity *= restingSpeed;
            }
        }

        private void GoToDefaultPosition(NPC body, float scalingSpeed, float flatSpeed, float maxSpeed = 6f, float restingDistance = 50f, float restingSpeed = 0.9f) {
            GoTo(GetRestingPosition(body, Vector2.Zero), scalingSpeed, flatSpeed, maxSpeed, restingDistance, restingSpeed);
        }

        private void GoToDefaultPosition(NPC body) {
            GoToDefaultPosition(body, 0.01f, 1f);
        }

        public override void AI() {

            SharedAI();
            NPC.direction = (int)NPC.ai[3];
            NPC.spriteDirection = (int)NPC.ai[3];
            if (npcBody == -1 || !Main.npc[npcBody].active || Main.npc[npcBody].ModNPC is not CrabsonSegment body) {
                NPC.KillEffects(quiet: true);
                return;
            }

            SetHand(this);
            NPC.realLife = npcBody;

            NPC.CollideWithOthers(0.3f);

            NPC.target = body.NPC.target;
            Player target = Main.player[NPC.target];
            float lifeRatio = Math.Clamp(body.NPC.life / (float)body.NPC.lifeMax, 0f, 1f);
            float battleProgress = 1f - lifeRatio;
            float startingRotation = (int)NPC.ai[3] == -1 ? MathHelper.Pi : 0f;
            var topLeftTile = NPC.position.ToTileCoordinates();
            var centerTile = NPC.Center.ToTileCoordinates();
            NPC.noTileCollide = true;
            switch (SharedAction) {
                case ACTION_CLAWSHOTS: {
                        if (NPC.ai[1] <= 0f) {
                            break;
                        }

                        if (NPC.Distance(target.Center) > 450f) {
                            NPC.velocity += NPC.DirectionTo(target.Center);
                        }
                        else {
                            NPC.velocity *= 0.92f;
                        }

                        ActionTimer += Main.rand.NextFloat(0.5f);
                        if (ActionTimer > 90f) {
                            mouthAnimation = MathHelper.Lerp(mouthAnimation, 0.4f, 0.3f);

                            if (ActionTimer > 100f && (int)ActionTimer % 80 == 0) {
                                NPC.velocity = -NPC.DirectionTo(target.Center) * 10f;
                                NPC.netUpdate = true;
                                SoundEngine.PlaySound(AequusSounds.shoot_Umystick, NPC.Center);
                                if (Main.netMode != NetmodeID.MultiplayerClient) {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -NPC.velocity * 1.5f, ModContent.ProjectileType<CrabsonPearl>(), 20, 0f, Main.myPlayer, ai1: 1f);
                                }
                            }
                        }
                        NPC.rotation = (NPC.Center - target.Center).ToRotation();
                        break;
                    }

                case ACTION_CLAWRAIN: {
                        GoToDefaultPosition(body.NPC);
                        if (NPC.rotation == 0f) {
                            NPC.rotation = startingRotation;
                        }
                        mouthAnimation = MathHelper.Lerp(mouthAnimation, 0.4f, 0.3f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, startingRotation + MathHelper.PiOver2 * NPC.ai[3], 0.1f);
                        ActionTimer++;
                        float wantedTime = 20f + 40f * lifeRatio;
                        if (ActionTimer == (int)wantedTime) {
                            ActionTimer = wantedTime;
                            NPC.velocity = NPC.rotation.ToRotationVector2() * 20f;
                            NPC.netUpdate = true;
                            SoundEngine.PlaySound(AequusSounds.shoot_Umystick, NPC.Center);

                            int projAmount = 13;
                            if (Main.netMode != NetmodeID.MultiplayerClient) {
                                for (int i = 0; i < projAmount; i++) {
                                    var p = Projectile.NewProjectileDirect(
                                        NPC.GetSource_FromThis(),
                                        NPC.Center,
                                        -Vector2.Normalize(NPC.velocity).RotatedBy((i - projAmount / 2f) * 0.1f) * 9f,
                                        ProjectileID.MoonlordArrowTrail,
                                        20, 0f,
                                        Main.myPlayer);
                                    p.friendly = false;
                                    p.hostile = true;
                                    p.tileCollide = false;
                                    if (!PhaseTwo) {
                                        p.extraUpdates /= 2;
                                    }
                                    p.timeLeft *= 4;
                                }
                            }
                        }
                        break;
                    }

                case ACTION_WELCOMETOTHESLAMJAM: {
                        if (body.NPC.ai[1] <= 0f) {
                            goto default;
                        }

                        if (body.NPC.ai[1] > 90f) {
                            if (ActionTimer == 0) {
                                int floor = Helper.FindFloor(centerTile.X, centerTile.Y, 16);
                                if (floor != -1) {
                                    if (Main.netMode != NetmodeID.Server) {
                                        Vector2 where = new((body.HandLeft.Center.X + body.HandRight.Center.X) / 2f, floor * 16f);
                                        SoundEngine.PlaySound(AequusSounds.largeSlam with { Volume = 0.5f, }, where);
                                        SoundEngine.PlaySound(AequusSounds.superAttack, where);
                                        ScreenShake.SetShake(80f, where: where);
                                        ScreenFlash.Flash.Set(where, 1f, multiplier: 0.75f);
                                    }
                                    ActionTimer++;
                                }
                            }
                            if (NPC.velocity.Y == 0f) {
                                if (Main.netMode != NetmodeID.MultiplayerClient) {
                                    var p = Helper.FindProjectile(ModContent.ProjectileType<CrabsonSlamProj>(), Main.myPlayer);
                                    if (p != null) {
                                        p.ai[0]++;
                                        p.netUpdate = true;
                                    }
                                }
                            }
                            if (NPC.velocity.Y < 31f)
                                NPC.velocity.Y += 2f + 2f * BattleProgress;
                            NPC.stepSpeed = 2f;
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, startingRotation, 0.1f);
                            NPC.noTileCollide = false;
                            break;
                        }
                        if (body.NPC.ai[1] > 85f) {
                            NPC.velocity *= 0.8f;
                            break;
                        }
                        NPC.velocity *= 0.9f;

                        var gotoPosition = GetRestingPosition(body.NPC, Vector2.Zero);
                        if (body.NPC.ai[1] > 50f) {

                            NPC.rotation = startingRotation + ((body.NPC.ai[1] - 50f) * -0.04f + 1f) * -NPC.direction;
                            gotoPosition.X -= body.NPC.ai[1] * NPC.direction;
                            gotoPosition.Y -= body.NPC.ai[1] * 2f;
                            NPC.Center = Vector2.Lerp(NPC.Center, gotoPosition, 0.1f);
                            break;
                        }
                        NPC.rotation = startingRotation + body.NPC.ai[1] * 0.02f * -NPC.direction;
                        GoTo(gotoPosition, 0.0001f, 0.005f);
                        NPC.velocity.Y = Math.Min(NPC.velocity.Y, -1f);
                    }
                    break;

                default: {

                        GoToDefaultPosition(body.NPC);
                        mouthAnimation *= 0.95f;
                        if (NPC.rotation != 0f) {
                            NPC.rotation = NPC.rotation.AngleLerp(startingRotation, 0.1f);
                            if (Math.Abs(NPC.rotation) <= 0.1f) {
                                NPC.rotation = 0f;
                            }
                        }
                        break;
                    }
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) {
            return false;
        }

        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects) {
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {

            if (!NPC.IsABestiaryIconDummy) {
                switch (SharedAction) {
                    case ACTION_WELCOMETOTHESLAMJAM: {
                            if (Body.ai[1] > 60f && NPC.velocity.Y != 0f) {
                                float intensity = Math.Min(Body.ai[1] / 60f, 1f);
                                screenPos += new Vector2(Main.rand.NextFloat(-intensity, intensity), Main.rand.NextFloat(-intensity, intensity) * 2f);

                                var glowColor = new Color(0, 10, 250, 0);
                                foreach (var v in Helper.CircularVector(4, NPC.rotation)) {
                                    DrawClaw(NPC, spriteBatch, screenPos + v * 8f * intensity, glowColor * intensity, mouthAnimation);
                                }

                                if (NPC.velocity.Y > 0f) {
                                    for (int i = 0; i < 10; i++) {
                                        DrawClaw(NPC, spriteBatch, screenPos + NPC.velocity * i * 0.1f, glowColor * intensity * 0.33f, mouthAnimation);
                                    }
                                }
                            }
                            break;
                        }
                }
            }
            DrawClaw(NPC, spriteBatch, screenPos, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), mouthAnimation);
            return false;
        }
    }

    public class CrabsonSceneEffect : ModSceneEffect {

        public static ScreenFliterEffect ScreenFilter { get; private set; }

        public override void Load() {
            if (!Main.dedServ) {
                ScreenFilter = new(this.NamespacePath() + "/Shader/CrabsonScreenShader", "Crabson", "CrabsonScreenShaderPass", EffectPriority.High);
                ScreenFilter.Load()
                    .UseColor(new Color(0, 10, 200))
                    .UseImage(AequusTextures.EffectNoise.Value, 1)
                    .UseIntensity(0.25f);
            }
        }

        public override void Unload() {
            ScreenFilter = null;
        }

        public override bool IsSceneEffectActive(Player player) {
            return AequusSystem.CrabsonNPC != -1;
        }

        public override void SpecialVisuals(Player player, bool isActive) {
            ScreenFilter.Manage(isActive);
        }
    }
}

namespace Aequus.Content.Boss.Crabson.Projectiles {
    public class CrabsonSlamProj : ModProjectile {

        public override string Texture => AequusTextures.None.Path;

        public float[,] effectLookup;

        public override void SetDefaults() {
            Projectile.width = 4000;
            Projectile.height = 1760;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
        }

        public override void AI() {

            int slamEffectTime = 20;
            Projectile.velocity = Vector2.Zero;
            if (Projectile.alpha > 0 && Projectile.timeLeft > slamEffectTime) {
                Projectile.alpha -= 20;
                if (Projectile.alpha < 0) {
                    Projectile.alpha = 0;
                }
            }

            var tileCoordinates = Projectile.position.ToTileCoordinates();
            int x = tileCoordinates.X;
            int y = tileCoordinates.Y;
            if (effectLookup == null) {
                int width = Projectile.width / 16;
                int height = Projectile.height / 16;
                effectLookup = new float[width, height];
                for (int i = 0; i < width; i++) {
                    for (int j = 0; j < height; j++) {
                        int k = x + i;
                        int l = y + j;
                        if (!WorldGen.InWorld(k, l, 40)) {
                            continue;
                        }

                        if ((Main.tile[k, l].IsFullySolid() || Main.tile[k, l].SolidTopType()) && !Main.tile[k, l - 1].IsFullySolid()) {
                            effectLookup[i, j] = 1f - (Math.Abs(i - width / 2f) / width + Math.Abs(j - height / 2f) / height);
                        }
                    }
                }
                return;
            }

            if (Projectile.ai[0] > 0f) {
                Projectile.timeLeft = Math.Min(Projectile.timeLeft, slamEffectTime - 1);
                Projectile.alpha += byte.MaxValue / slamEffectTime;
                int width = Projectile.width / 16;
                int chunk = (int)(width / (float)slamEffectTime * (slamEffectTime - Projectile.timeLeft)) / 2;
                int length = effectLookup.GetLength(0);
                int halfLength = length / 2;
                int start = Math.Max(halfLength - chunk, 0);
                int end = Math.Min(halfLength + chunk, length);
                int height = effectLookup.GetLength(1);
                for (int i = start; i < end; i++) {
                    for (int j = 0; j < height; j++) {

                        if (effectLookup[i, j] <= 0f) {
                            continue;
                        }

                        float opacity = Math.Abs(((i - start) / (float)(end - start) - 0.5f) * 2f);
                        var d = Dust.NewDustDirect(new Vector2((i + tileCoordinates.X) * 16f, (j + tileCoordinates.Y) * 16f - 4f), 16, 8, DustID.GemSapphire, 0f, Main.rand.NextFloat(-4f, 0f),
                            Scale: Main.rand.NextFloat(opacity * 3f));
                        d.noGravity = true;
                        d.velocity *= Main.rand.NextFloat() * opacity * 1.5f;
                    }
                }
                if (end < length && start > 0) {
                    for (int j = 0; j < height; j++) {

                        if (effectLookup[start, j] > 0f) {
                            CrabsonSlamParticle.New(start + tileCoordinates.X, j + tileCoordinates.Y, effectLookup[start, j]);
                            //Helper.DebugDustLine(Main.LocalPlayer.Center, new Vector2(start + tileCoordinates.X, j + tileCoordinates.Y) * 16f, 100);
                        }
                        if (effectLookup[end, j] > 0f) {
                            CrabsonSlamParticle.New(end + tileCoordinates.X, j + tileCoordinates.Y, effectLookup[start, j]);
                        }
                    }
                }
            }

            Vector3 tileClr = Color.Blue.ToVector3() * 0.5f * Projectile.Opacity;
            int effectLookupWidth = effectLookup.GetLength(0);
            int effectLookupHeight = effectLookup.GetLength(1);
            for (int i = 0; i < effectLookupWidth; i++) {
                for (int j = 0; j < effectLookupHeight; j++) {
                    int k = x + i;
                    int l = y + j;
                    if (!WorldGen.InWorld(k, l, 40) || ScreenCulling.OnScreenWorld(k, l)) {
                        continue;
                    }

                    float opacity = effectLookup[i, j];
                    Lighting.AddLight(k, l - 1, tileClr.X * opacity, tileClr.Y * opacity, tileClr.Z * opacity);
                }
            }
        }

        public override bool ShouldUpdatePosition() {
            return false;
        }

        public override bool? CanDamage() {
            return Projectile.ai[0] > 0f;
        }
        // Cannot hit NPCs, since it has extremely high range
        public override bool? CanHitNPC(NPC target) {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            if (effectLookup == null || !projHitbox.Intersects(targetHitbox)) {
                return false;
            }

            int myX = (int)Projectile.position.X / 16;
            int myY = (int)Projectile.position.Y / 16;
            int x = Math.Max(targetHitbox.X / 16, myX);
            int y = Math.Max(targetHitbox.Y / 16, myY);
            int width = Math.Max(targetHitbox.Width / 16, 1);
            int height = Math.Max(targetHitbox.Height / 16, 1) + 2;
            int startX = x - myX;
            int startY = y - myY;
            for (int i = x - myX; i < width + startX; i++) {
                if (i > effectLookup.GetLength(0)) {
                    break;
                }
                for (int j = y - myY; j < height + startY; j++) {
                    if (j > effectLookup.GetLength(1)) {
                        break;
                    }

                    if (effectLookup[i, j] > 0f) {
                        return true;
                    }
                }
            }

            return false;
        }

        private void GetAuraParameters(out Texture2D texture, out Color bloomColor, out Color secondaryBloomColor, out Rectangle frame) {
            bloomColor = Color.Lerp(Color.Cyan, Color.Blue, Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.4f, 0.6f)) with { A = 0 } * Projectile.Opacity;
            secondaryBloomColor = Color.Blue with { A = 0 } * 0.5f * Projectile.Opacity;
            frame = new(AequusTextures.Bloom0.Width / 2, 0, 1, AequusTextures.Bloom0.Height / 2);
            texture = AequusTextures.Bloom0.Value;
        }

        private void DrawTileAura_Solid(SpriteBatch spriteBatch, Texture2D texture, int i, int j, float opacity, Color bloomColor, Color secondaryBloomColor, Rectangle frame, Vector2 bloomSize, Vector2 largeBloomSize, Vector2 drawCoordinates) {

            if (Main.tile[i, j].IsHalfBlock) {
                drawCoordinates.Y += 8f;
            }

            spriteBatch.Draw(
                texture,
                drawCoordinates + new Vector2(0f, -frame.Height * largeBloomSize.Y + 4f),
                frame,
                secondaryBloomColor * opacity,
                0f,
                Vector2.Zero,
                largeBloomSize,
                SpriteEffects.None, 0f);
            spriteBatch.Draw(
                texture,
                drawCoordinates + new Vector2(0f, -frame.Height * bloomSize.Y + 4f),
                frame,
                bloomColor * opacity,
                0f,
                Vector2.Zero,
                bloomSize,
                SpriteEffects.None, 0f);
        }
        private void DrawTileAura_Sloped(SpriteBatch spriteBatch, Texture2D texture, int i, int j, float opacity, Color bloomColor, Color secondaryBloomColor, Rectangle frame, Vector2 bloomSize, Vector2 largeBloomSize, Vector2 drawCoordinates, float dirX, float dirY) {
            for (int k = 0; k < 8; k++) {
                spriteBatch.Draw(
                    texture,
                    drawCoordinates + new Vector2(k * 2f * dirX, -frame.Height * largeBloomSize.Y + 4f + k * 2f * dirY),
                    frame,
                    secondaryBloomColor * opacity,
                    0f,
                    Vector2.Zero,
                    largeBloomSize with { X = 2f, },
                    SpriteEffects.None, 0f);
                spriteBatch.Draw(
                    texture,
                    drawCoordinates + new Vector2(k * 2f * dirX, -frame.Height * bloomSize.Y + 4f + k * 2f * dirY),
                    frame,
                    bloomColor * opacity,
                    0f,
                    Vector2.Zero,
                    bloomSize with { X = 2f, },
                    SpriteEffects.None, 0f);
            }
        }

        private void DrawTileAura(SpriteBatch spriteBatch, Texture2D texture, int i, int j, float opacity, Color bloomColor, Color secondaryBloomColor, Rectangle frame) {

            int dir = 1;
            if (i > Projectile.Center.X / 16) {
                dir = -1;
            }
            opacity *= Helper.Wave((i + j) * 0.3f + Main.GlobalTimeWrappedHourly * 10f * dir, 1f, 1.5f);
            Vector2 bloomSize = new(16f, opacity * 0.2f);
            Vector2 largeBloomSize = bloomSize with { Y = bloomSize.Y * 1.5f };
            Vector2 drawCoordinates = new Vector2(i * 16f, j * 16f) - Main.screenPosition;
            switch (Main.tile[i, j].Slope) {
                case SlopeType.SlopeUpLeft:
                case SlopeType.SlopeUpRight:
                case SlopeType.Solid: {
                        DrawTileAura_Solid(spriteBatch, texture, i, j, opacity, bloomColor, secondaryBloomColor, frame, bloomSize, largeBloomSize, drawCoordinates);
                        break;
                    }

                case SlopeType.SlopeDownLeft: {
                        DrawTileAura_Sloped(spriteBatch, texture, i, j, opacity, bloomColor, secondaryBloomColor, frame, bloomSize, largeBloomSize, drawCoordinates,
                            1f, 1f);
                        break;
                    }
                case SlopeType.SlopeDownRight: {
                        DrawTileAura_Sloped(spriteBatch, texture, i, j, opacity, bloomColor, secondaryBloomColor, frame, bloomSize, largeBloomSize, drawCoordinates + new Vector2(14f, 0f),
                            -1f, 1f);
                        break;
                    }
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            if (effectLookup == null) {
                return false;
            }

            var tileCoordinates = Projectile.position.ToTileCoordinates();
            int x = tileCoordinates.X;
            int y = tileCoordinates.Y;
            int width = effectLookup.GetLength(0);
            int height = effectLookup.GetLength(1);

            GetAuraParameters(out var texture, out var bloomColor, out var secondaryBloomColor, out var frame);
            ScreenCulling.Prepare(16);
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    int k = x + i;
                    int l = y + j;
                    if (!WorldGen.InWorld(k, l, 40) || !ScreenCulling.OnScreenWorld(k, l) || effectLookup[i, j] <= 0f) {
                        continue;
                    }

                    DrawTileAura(Main.spriteBatch, texture, k, l, effectLookup[i, j], bloomColor, secondaryBloomColor, frame);
                }
            }
            return false;
        }
    }

    public class CrabsonSlamParticle : BaseParticle<CrabsonSlamParticle> {

        public int frameY;
        public int frameCounter;

        public static CrabsonSlamParticle New(int i, int j, float opacity) {
            return ParticleSystem.New<CrabsonSlamParticle>(ParticleLayer.AboveDust)
                .Setup(
                new(i * 16f + 8f, j * 16f - 6f),
                -Vector2.UnitY,
                Color.White, 1f, 0f
            );
        }

        public override CrabsonSlamParticle CreateInstance() {
            return new();
        }

        protected override void SetDefaults() {
            SetTexture(AequusTextures.SlamEffect0, 4, 0);
            frameY = 0;
            dontEmitLight = true;
        }

        public override void Update(ref ParticleRendererSettings settings) {

            if (frameCounter++ > 5) {

                frameY++;
                if (frameY > 4) {
                    ShouldBeRemovedFromRenderer = true;
                    return;
                }
                frameCounter = 0;
                frame.Y = frame.Height * frameY;
            }

            Velocity *= 0.9f;
            Position += Velocity;
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch) {
            spritebatch.Draw(texture, Position - Main.screenPosition, frame, Color.White, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }
    }
}

namespace Aequus {
    partial class AequusSystem {
        public static int CrabsonNPC = -1;

        public void PreUpdateEntities_CheckCrabson() {
            if (CrabsonNPC == -1 || Main.npc[CrabsonNPC].active && Main.npc[CrabsonNPC].ModNPC is CrabsonSegment) {
                return;
            }

            CrabsonNPC = -1;
        }
    }
}