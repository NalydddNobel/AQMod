using Aequus;
using Aequus.Common;
using Aequus.Common.NPCs;
using Aequus.Items.Potions.Healing.Restoration;
using Aequus.NPCs.BossMonsters.Crabson.Common;
using Aequus.NPCs.BossMonsters.Crabson.Projectiles;
using Aequus.NPCs.BossMonsters.Crabson.Segments;
using Aequus.NPCs.Town.ExporterNPC;
using Aequus.Particles;
using System;
using System.IO;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;

namespace Aequus.NPCs.BossMonsters.Crabson;

[AutoloadBossHead]
[WorkInProgress]
public class Crabson : CrabsonBossNPC, ICrabson {
    public const float BossProgression = 2.66f;

    public int hitPlayer;

    public CrabsonDrawManager DrawManager { get; set; }

    public override bool IsLoadingEnabled(Mod mod) {
        return Aequus.DevelopmentFeatures;
    }

    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 4;

        NPCID.Sets.TrailingMode[Type] = 7;
        NPCID.Sets.TrailCacheLength[Type] = 8;
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Wet] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Suffocation] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Lovestruck] = true;
        NPCID.Sets.NPCBestiaryDrawOffset[Type] = new() {
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

#if !CRAB_CREVICE_DISABLE
        this.SetBiome<global::Aequus.Content.Biomes.CrabCrevice.CrabCreviceBiome>();
#endif
        npcBody = -1;
        npcHandLeft = -1;
        npcHandRight = -1;

        DrawManager = new();
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment) {
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
            for (int i = 0; i < Math.Min(hit.Damage / 20 + 1, 1); i++) {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
            }
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
        DrawManager?.OnAIUpdate();
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
        DrawManager?.FindFrame(NPC, HandLeft, HandRight, frameHeight);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo) {
        target.AddBuff(ModContent.BuffType<Buffs.Debuffs.PickBreak>(), 1200);
        DrawManager?.OnHitPlayer();
    }

    public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone) {
        DrawManager?.OnDamageRecieved(hit);
    }

    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone) {
        DrawManager?.OnDamageRecieved(hit);
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
        //LegacyEffects.NPCsBehindAllNPCs.Add(NPC);
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

        DrawManager?.legs.Draw(NPC, spriteBatch, drawPosition, bodyDrawColor);
        DrawManager?.eyes.Draw(NPC, spriteBatch, drawPosition, screenPos, offset, new Vector2(0f, -40f));
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

        DrawManager?.arms.DrawArms(NPC, spriteBatch, screenPos);
        DrawBody(spriteBatch, screenPos, new(), NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)));
        return false;
    }

    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) {
        scale = 1.5f;
        return null;
    }
    #endregion

    public override void BossLoot(ref string name, ref int potionType) {
        potionType = ModContent.ItemType<LesserRestorationPotion>();
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
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

    public static void Laugh() {
        if (AequusSystem.CrabsonNPC != -1 && Main.npc[AequusSystem.CrabsonNPC].ModNPC is ICrabson crabson) {
            crabson.DrawManager?.mood.Laugh();
        }
    }
}