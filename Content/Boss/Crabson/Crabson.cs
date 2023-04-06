using Aequus.Common.Effects;
using Aequus.Content.Biomes.CrabCrevice;
using Aequus.Content.Boss.Crabson;
using Aequus.Content.Boss.Crabson.Projectiles;
using Aequus.Content.Boss.Crabson.Rewards;
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
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Boss.Crabson {
    public abstract class CrabsonSegment : AequusBoss {
        public const int ACTION_CLAWSHOTS = 2;
        public const int PHASE_GROUNDBUBBLES = 3;
        public const int ACTION_CLAWSLAMS = 4;
        public const int PHASE2_GROUNDBUBBLES_SPAMMY = 5;
        public const int ACTION_P2_CLAWSHOTS_SHRAPNEL = 6;
        public const int ACTION_CLAWRAIN = 9;
        public const int ACTION_NOATTACK = 10;

        public int npcHandLeft = -1;
        public int npcHandRight = -1;
        public int npcBody = -1;

        public bool contactDamage;

        public NPC HandLeft => Main.npc[npcHandLeft];
        public NPC HandRight => Main.npc[npcHandRight];
        public NPC Body => Main.npc[npcBody];

        public int SharedAction => (int)Main.npc[npcBody].ai[0];
        public bool PhaseTwo => Main.npc[NPC.realLife].life * (Main.expertMode ? 2f : 4f) <= NPC.lifeMax;

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
        }

        protected bool CheckClaws() {
            return !(npcHandLeft == -1 || !HandLeft.active || HandLeft.type != NPC.type ||
                npcHandRight == -1 || !HandRight.active || HandRight.type != NPC.type);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) {
            return contactDamage;
        }

        protected void DrawClaw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, float mouthAnimation) {
            var claw = AequusTextures.CrabsonClaw.Value;
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
                    origin.X = AequusTextures.CrabsonClaw.Width - origin.X;
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

        public const float BossProgression = 2.66f;

        public int hitPlayer;

        public static ConfiguredMusicData BossMusic = new(MusicID.Boss3, MusicID.OtherworldlyBoss1);

        public EyeManager eyeManager;
        public WalkManager walkManager;

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

        public override void Unload() {
            BossMusic.Unload();
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

            if (!Main.dedServ && BossMusic != null) {
                Music = BossMusic.GetID();
                SceneEffectPriority = SceneEffectPriority.BossLow;
            }

            this.SetBiome<CrabCreviceBiome>();

            npcBody = -1;
            npcHandLeft = -1;
            npcHandRight = -1;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale) {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
        }

        public override void HitEffect(int hitDirection, double damage) {
            if (NPC.life <= 0) {
                if (Main.netMode != NetmodeID.Server) {
                    for (int i = 0; i < 50; i++) {
                        Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
                    }
                }
            }
            else {
                for (int i = 0; i < Math.Min(damage / 20 + 1, 1); i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
            }
        }

        #region AI
        private List<(int, Point)> GetGroundSmashingData(int tilesAmt) {
            var centerTile = NPC.Center.ToTileCoordinates();
            int floor = Helper.FindFloor(centerTile.X, centerTile.Y, 30);
            if (floor == -1) {
                floor = centerTile.Y + 10;
            }
            int leftY = floor;
            int rightY = floor;
            var result = new List<(int, Point)>();
            for (int i = 0; i < tilesAmt; i++) {

                if (WorldGen.InWorld(centerTile.X + i, rightY, fluff: 30) && Main.tile[centerTile.X + i, rightY].IsFullySolid()) {
                    result.Add((i, new(centerTile.X + i, rightY)));
                    rightY = Helper.FindFloor(centerTile.X + i, rightY - 1, 10);
                }
                if (WorldGen.InWorld(centerTile.X - i - 1, leftY, fluff: 30)) {
                    leftY = Helper.FindFloor(centerTile.X - i - 1, leftY - 1, 10);
                    result.Add((i, new(centerTile.X - i - 1, leftY)));
                }
            }
            return result;
        }

        private void WalkTowards(Vector2 location) {
            var center = NPC.Center;
            float distance = NPC.Distance(location);
            float distanceX = Math.Abs(center.X - location.X);
            float distanceY = Math.Abs(center.Y - location.Y);
            
            if (NPC.collideX || distanceX < 80f) {
                if (center.Y > location.Y - 48f) {

                    if (NPC.velocity.Y == 0f) {
                        NPC.velocity.Y = Math.Min(-distanceY / 28f, -12f);
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
            else if (location.Y > center.Y - 128f && NPC.velocity.Y == 0f) {
                NPC.noGravity = false;
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

            int direction = Math.Sign(location.X - center.X);
            NPC.velocity.X += direction * 0.1f;
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

            AequusSystem.CrabsonNPC = NPC.whoAmI;
            AequusNPC.ForceZen(NPC);
            if (NPC.alpha > 0) {
                NPC.alpha -= 5;
                if (NPC.alpha < 0) {
                    NPC.alpha = 0;
                }
            }

            if (!NPC.HasPlayerTarget || !NPC.HasValidTarget) {
                NPC.TargetClosest();
            }
            float distance = NPC.Distance(Main.player[NPC.target].Center);
            if (distance > 600f) {
                NPC.TargetClosest();
            }
            if (!NPC.HasPlayerTarget || !NPC.HasValidTarget) {
                Action = ACTION_GOODBYE;
                return;
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
            if (Action != ACTION_INIT && CheckClaws()) {
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
                        if (distance > 500f || !Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height)) {
                            WalkTowards(Main.player[NPC.target].Center);
                        }
                        else {
                            HaltMovement();
                        }
                        HandRight.ai[1]++;
                        HandLeft.ai[1]++;
                        ActionTimer += 1f + battleProgress * 2f;
                        if (ActionTimer > 600) {
                            Action = ACTION_INTRO;
                            ActionTimer = 0;
                        }
                    }
                    break;


                case 8: {
                        if (ActionTimer == 0) {
                            if (Math.Abs(NPC.velocity.X) > 1f) {
                                HaltMovement();
                                break;
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
                            break;
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

                        ActionTimer++;
                        if (ActionTimer == 50) {
                            SoundEngine.PlaySound(AequusSounds.chargeUp0, NPC.Center);
                        }
                        if (ActionTimer == 100) {
                            SoundEngine.PlaySound(AequusSounds.largeSlam with { Volume = 0.5f, }, NPC.Center);
                            SoundEngine.PlaySound(AequusSounds.superAttack, NPC.Center);
                        }
                        if (ActionTimer == 106) {
                            int floor = Helper.FindFloor(centerTile.X, centerTile.Y, 30);
                            if (floor == -1) {
                                floor = 12;
                            }
                            var where = new Vector2(NPC.Center.X, centerTile.Y + floor * 16f);
                            ScreenShake.SetShake(80f, where: where);
                            ScreenFlash.Flash.Set(where, 1f, multiplier: 0.75f);
                        }
                        if (ActionTimer > 107 && ActionTimer < 127) {
                            int tilesAmt = 90;
                            int end = (int)(tilesAmt / 20 * (ActionTimer - 107));
                            foreach (var data in GetGroundSmashingData(end)) {
                                float opacity = 1f - MathF.Pow(data.Item1 / (float)tilesAmt, 4f);
                                var d = Dust.NewDustDirect(new Vector2(data.Item2.X * 16f, data.Item2.Y * 16f - 4f), 16, 8, DustID.GemSapphire, 0f, Main.rand.NextFloat(-4f, 0f), 
                                    Scale: Main.rand.NextFloat(opacity * 2f));
                                d.noGravity = true;
                                d.velocity *= Main.rand.NextFloat() * opacity * 2f;
                            }
                        }
                        if (ActionTimer > 180) {
                            Action = 10;
                            ResetActionTimers();
                        }
                        break;
                    }

                case 9: {
                        ActionTimer++;
                        HaltMovement();
                        if (ActionTimer > 120) {
                            ActionTimer += battleProgress * 10f;
                            Action = ACTION_NOATTACK;
                            ActionTimer = 0;
                        }
                    }
                    break;

                case 10: {
                        bool canSeePlayer = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height);
                        if (distance > 500f || !canSeePlayer) {
                            WalkTowards(Main.player[NPC.target].Center);
                        }
                        else {
                            HaltMovement();
                        }
                        ActionTimer++;
                        if (distance < 600f && canSeePlayer) {
                            ActionTimer += battleProgress * 10f;
                        }
                        if (ActionTimer > 600) {
                            Action = Main.rand.NextFromList(ACTION_CLAWSHOTS, ACTION_CLAWRAIN);
                            Action = 8;
                            ResetActionTimers();
                        }
                        break;
                    }
            }
        }
        #endregion

        public override void FindFrame(int frameHeight) {

            eyeManager.Update(NPC);
            walkManager.Update(NPC);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit) {
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

        private void DrawChain(SpriteBatch spriteBatch, Vector2 currentPosition, Vector2 endPosition, Vector2 screenPos) {
            var chain = AequusTextures.Crabson_Chain.Value;

            // Adapted from Star Construct's arm rendering code
            Vector2[] bezierPoints = { endPosition, new Vector2(currentPosition.X + Math.Sign(currentPosition.X - NPC.Center.X) * 100f, (endPosition.Y + currentPosition.Y) / 2f + 100f), currentPosition };

            if (Action == 8) {

                float lerpAmount = 1f;
                if (ActionTimer < 80f) {
                    lerpAmount = ActionTimer / 80f;
                }
                if (ActionTimer > 100f) {
                    lerpAmount = Math.Max(1f - MathF.Pow(ActionTimer - 100, 2f) / 30f, 0f);
                }
                bezierPoints[1].Y = MathHelper.Lerp(bezierPoints[1].Y, NPC.position.Y - 200f, lerpAmount);
            }
            float bezierProgress = 0;
            float bezierIncrement = 18;

            int reps = 0;
            while (bezierProgress < 1 && reps < 150) {
                //draw stuff
                Vector2 oldPos = BezierCurve(bezierPoints, bezierProgress);
                //increment progress
                reps++;
                while ((oldPos - BezierCurve(bezierPoints, bezierProgress)).Length() < bezierIncrement) {
                    bezierProgress += 0.1f / BezierCurveDerivative(bezierPoints, bezierProgress).Length();
                }

                Vector2 newPos = BezierCurve(bezierPoints, bezierProgress);
                float rotation = (newPos - oldPos).ToRotation();

                Vector2 drawingPos = (oldPos + newPos) / 2;

                var chainFrame = chain.Frame(verticalFrames: 3, frameY: bezierProgress < 0.3f ? 2 : (bezierProgress < 0.6f ? 1 : 0));
                spriteBatch.Draw(
                    chain,
                    drawingPos - screenPos,
                    chainFrame,
                    Helper.GetColor(drawingPos),
                    rotation,
                    chainFrame.Size() / 2f,
                    NPC.scale, SpriteEffects.None, 0f);
            }
        }

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

            switch (Action) {

                case 8: {
                        if (ActionTimer < 66 || ActionTimer > 110) {
                            break;
                        }

                        float globalOpacity = MathF.Pow(Math.Clamp((ActionTimer - 70) / 20f, 0f, 1f), 3f);
                        Color bloomColor = Color.Lerp(Color.Cyan, Color.Blue, Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.4f, 0.6f)) with { A = 0 } * 0.5f * globalOpacity;
                        Color bloomLargeColor = Color.Blue with { A = 0 } * 0.5f * globalOpacity;
                        Rectangle bloomFrame = new(AequusTextures.Bloom0.Width / 2, 0, 1, AequusTextures.Bloom0.Height / 2);
                        Vector3 tileClr = Color.Blue.ToVector3() * 0.5f * globalOpacity;
                        int tilesAmt = 90;
                        var texture = AequusTextures.Bloom0.Value;
                        foreach (var data in GetGroundSmashingData(90)) {
                            float opacity = (1f - MathF.Pow(data.Item1 / (float)tilesAmt, 4f));
                            Vector2 bloomSize = new(16f, opacity * 0.2f);
                            Vector2 largeBloomSize = bloomSize with { Y = bloomSize.Y * 1.5f };
                            spriteBatch.Draw(
                                texture, 
                                new Vector2(data.Item2.X * 16f, data.Item2.Y * 16f - bloomFrame.Height * largeBloomSize.Y + 4f) - screenPos, 
                                bloomFrame, 
                                bloomLargeColor * opacity,
                                0f, 
                                Vector2.Zero, 
                                largeBloomSize, 
                                SpriteEffects.None, 0f);
                            spriteBatch.Draw(
                                texture, 
                                new Vector2(data.Item2.X * 16f, data.Item2.Y * 16f - bloomFrame.Height * bloomSize.Y + 4f) - screenPos, 
                                bloomFrame,
                                bloomColor * opacity,
                                0f, 
                                Vector2.Zero,
                                bloomSize, 
                                SpriteEffects.None, 0f);
                            Lighting.AddLight(data.Item2.X, data.Item2.Y - 1, tileClr.X * opacity, tileClr.Y * opacity, tileClr.Z * opacity);
                        }

                        break;
                    }
            }

            Vector2 chainOffset = new(44f, -14f);
            Vector2 chainEndOffset = new(20f, 0f);
            if (npcHandLeft > -1)
                DrawChain(spriteBatch, NPC.Center + chainOffset with { X = -chainOffset.X }, HandLeft.Center + chainEndOffset.RotatedBy(HandLeft.rotation == 0f ? MathHelper.Pi : HandLeft.rotation), screenPos);
            if (npcHandRight > -1)
                DrawChain(spriteBatch, NPC.Center + chainOffset, HandRight.Center + chainEndOffset.RotatedBy(HandRight.rotation), screenPos);
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
                        float wantedTime = 30f + 30f * lifeRatio;
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
                                    p.extraUpdates /= 2;
                                    p.timeLeft *= 4;
                                }
                            }
                        }
                        break;
                    }

                case 8: {
                        if (body.NPC.ai[1] > 90f) {
                            if (NPC.velocity.Y < 31f)
                                NPC.velocity.Y += 2f;
                            NPC.stepSpeed = 2f;
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, startingRotation, 0.1f);
                            NPC.noTileCollide = false;
                            break;
                        }
                        NPC.noTileCollide = true;
                        if (body.NPC.ai[1] > 85f) {
                            NPC.velocity *= 0.8f;
                            break;
                        }

                        var gotoPosition = GetRestingPosition(body.NPC, Vector2.Zero);
                        if (body.NPC.ai[1] > 50f) {

                            NPC.rotation = startingRotation + ((body.NPC.ai[1] - 50f) * -0.04f + 1f) * -NPC.direction;
                            gotoPosition.X -= body.NPC.ai[1] * NPC.direction;
                            gotoPosition.Y -= body.NPC.ai[1] * 2f;
                            NPC.Center = Vector2.Lerp(NPC.Center, gotoPosition, 0.05f);
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
                            NPC.rotation = Utils.AngleLerp(NPC.rotation, startingRotation, 0.1f);
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

            switch (SharedAction) {
                case 8: {
                        if (Body.ai[1] > 60f && NPC.velocity.Y != 0f) {
                            float intensity = Body.ai[1] / 20f;
                            screenPos += new Vector2(Main.rand.NextFloat(-intensity, intensity), Main.rand.NextFloat(-intensity, intensity) * 2f);

                            foreach (var v in Helper.CircularVector(4, NPC.rotation)) {
                                DrawClaw(NPC, spriteBatch, screenPos + v * 2f, new Color(10, 40, 250, 0) * intensity, mouthAnimation);
                            }
                        }
                        break;
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
                    .UseColor(Color.Aqua)
                    .UseImage(AequusTextures.EffectNoise.Value, 1)
                    .UseIntensity(0.15f);
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