using Aequus.Content.DataSets;
using Aequus.Old.Core.Utilities;
using System;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Localization;

namespace Aequus.Content.Enemies.PollutedOcean.Conductor;

public class ConductorProj : ModNPC {
    public override string Texture => AequusTextures.ScrapBlockItem.Path;

    public int State => (int)NPC.ai[0];
    public int Parent => (int)NPC.ai[2];
    public bool WaterSphere => NPC.localAI[1] == 1f;

    public const int A_IDLE = 0;
    public const int PRE_FIRE_BACKPEDAL_TIME = 36;

    public const int A_MOVING = 1;

    public const int A_PARENT_DEAD = 2;

    public override LocalizedText DisplayName => ModContent.GetInstance<Conductor>().GetLocalization("ProjectileName");

    public override void SetStaticDefaults() {
        NPCSets.SpecificDebuffImmunity[Type][BuffID.Shimmer] = true;
        NPCSets.NPCBestiaryDrawOffset[Type] = new() { Hide = true, };
        NPCMetadata.PushableByTypeId.Add(Type);
    }

    public override void SetDefaults() {
        NPC.width = 24;
        NPC.height = 24;
        NPC.lifeMax = 25;
        NPC.defense = 4;
        NPC.noTileCollide = true;
        NPC.timeLeft = 120;
        NPC.alpha = 255;
        NPC.knockBackResist = 1f;
        NPC.HitSound = SoundID.Item143;
        NPC.DeathSound = SoundID.Item10;
        NPC.noGravity = true;
        NPC.aiStyle = -1;
        NPC.damage = 25;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot) {
        return (int)NPC.ai[0] == A_MOVING;
    }

    public override void AI() {
        int myTimer = (int)NPC.ai[1];
        NPC.ai[1]++;

        if (NPC.alpha > 0) {
            NPC.alpha -= 20;
            if (NPC.alpha < 0) {
                NPC.alpha = 0;
            }
        }

        NPC.TargetClosest(faceTarget: true);
        NPC.rotation += NPC.direction * 0.25f;
        if (!Main.npc[Parent].active) {
            NPC.ai[0] = A_PARENT_DEAD;
        }

        switch ((int)NPC.ai[0]) {
            case A_IDLE:
                Action_Idle(myTimer);
                break;

            case A_MOVING:
                Action_Moving();
                break;

            case A_PARENT_DEAD:
                Action_Dead();
                return;
        }

        if (State != A_IDLE) {
            HandleCollisionAndTimeLeft();
        }
        if (State != A_PARENT_DEAD) {
            NPC.localAI[0] += 1f;
        }

        EmitParticles();
    }

    private void Action_Idle(int myTimer) {
        Conductor.GetAttackTimings(out _, out _, out int attackTime);

        int closestPlayer = Player.FindClosest(NPC.position, NPC.width, NPC.height);
        bool collision = Collision.SolidCollision(NPC.position, NPC.width, NPC.height);
        NPC.noTileCollide = true;
        bool wobble = true;

        int parentTimer = (int)Main.npc[Parent].ai[1];

        if (collision) {
            NPC.velocity += NPC.DirectionTo(Main.player[closestPlayer].Center) * 0.4f;

            float speedCap = 2f;
            if (Main.expertMode) {
                speedCap = 4f;
            }
            if (Main.getGoodWorld) {
                speedCap = 24f;
            }
            if (NPC.velocity.Length() > speedCap) {
                NPC.velocity.Normalize();
                NPC.velocity *= speedCap;
            }
        }
        else {
            NPC.velocity.X *= 0.9f;
        }
        if (myTimer == 0) {
            NPC.velocity.Y = -5f;
            if (Collision.WetCollision(NPC.position, NPC.width, NPC.height)) {
                NPC.localAI[1] = 1f;
                NPC.HitSound = SoundID.SplashWeak;
            }
        }
        else if (myTimer < 40 && NPC.velocity.Y < 0f) {
            NPC.velocity.Y += 0.22f;
        }
        else if (parentTimer > attackTime - PRE_FIRE_BACKPEDAL_TIME && !collision) {
            float speed = Conductor.ATTACK_SHOOT_VELOCITY_CLASSIC;
            if (Main.expertMode) {
                speed = Conductor.ATTACK_SHOOT_VELOCITY_EXPERT;
            }

            Vector2 wantedVector = NPC.DirectionTo(Main.player[closestPlayer].Center);
            Vector2 wantedVelocity = wantedVector * speed * 1.6f;

            if (Main.npc[Parent].ai[1] > attackTime) {
                NPC.ai[0] = 1f;
                NPC.velocity = wantedVelocity;
                NPC.netUpdate = true;
            }
            else if (Main.npc[Parent].ai[1] > attackTime - 5) {
                NPC.velocity = Vector2.Lerp(NPC.velocity, wantedVelocity, 0.33f);
            }
            else {
                float progress = 1f - MathF.Pow(1f - (attackTime - parentTimer) / (float)PRE_FIRE_BACKPEDAL_TIME, 5f);
                NPC.velocity -= wantedVector * (0.1f * progress);
            }
            wobble = false;
        }
        else {
            NPC.velocity *= 0.85f;
        }

        if (myTimer % 12 == 0) {
            Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.MagicMirror);
            d.noGravity = true;
            d.velocity *= 0.8f;
            d.fadeIn = d.scale + 0.2f;
        }
        if (wobble && myTimer % 11 == 0 && Main.netMode != NetmodeID.MultiplayerClient) {
            NPC.velocity += Main.rand.NextVector2Unit() * 1f;
            NPC.netUpdate = true;
        }
    }

    private void Action_Moving() {
        NPC.knockBackResist = 0f;
        NPC.noTileCollide = false;
        NPC.UpdateShimmerReflection();
    }

    private void Action_Dead() {
        if (WaterSphere) {
            NPC.KillEffects();
            SoundEngine.PlaySound(NPC.DeathSound, NPC.Center);
        }
        NPC.noTileCollide = false;
        NPC.UpdateShimmerReflection();
        NPC.velocity.X *= 0.94f;
        NPC.velocity.Y += 0.2f;
    }

    private void HandleCollisionAndTimeLeft() {
        NPC.ai[3]++;
        if (NPC.collideX || NPC.collideY || NPC.ai[3] > 60) {
            NPC.KillEffects();
            SoundEngine.PlaySound(NPC.DeathSound, NPC.Center);
        }
    }

    private void EmitParticles() {
        if (Main.rand.NextBool(3)) {
            Vector2 randomVector2 = Main.rand.NextVector2Unit();
            Dust d = Dust.NewDustPerfect(NPC.Center + randomVector2 * NPC.Size / 2f, DustID.MagicMirror, Velocity: randomVector2 - NPC.velocity * 0.25f);
            d.noGravity = true;
        }

        if (WaterSphere && Main.rand.NextBool()) {
            int dustType = Dust.dustWater();

            Dust d = Dust.NewDustPerfect(NPC.Center, dustType, Scale: 1f);
            d.noGravity = true;
            d.fadeIn = d.scale + 0.1f;
            d.customData = this;
        }
    }

    public override void HitEffect(NPC.HitInfo hit) {
        if (NPC.life <= 0) {
            for (int i = 0; i < 20; i++) {
                int dustType = WaterSphere ? Dust.dustWater() : (i % 2 == 0 ? DustID.Copper : DustID.Tin);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, dustType, NPC.velocity.X * 0.2f, NPC.velocity.Y * 0.2f);
            }
        }
        else {
            int dustType = WaterSphere ? Dust.dustWater() : (Main.rand.NextBool() ? DustID.Copper : DustID.Tin);
            Dust.NewDust(NPC.position, NPC.width, NPC.height, dustType, NPC.velocity.X * 0.2f, NPC.velocity.Y * 0.2f);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        NPC.GetDrawInfo(out Texture2D texture, out Vector2 offset, out Rectangle frame, out Vector2 origin, out _);
        Vector2 drawCoordinates = NPC.position + offset - screenPos;
        SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        drawColor = NPC.GetAlpha(NPC.GetNPCColorTintedByBuffs(drawColor)) * NPC.Opacity;

        if ((int)NPC.ai[0] != 2) {
            spriteBatch.Draw(AequusTextures.Bloom, drawCoordinates, null, drawColor * 0.2f, NPC.rotation, AequusTextures.Bloom.Size() / 2f, NPC.scale * 0.66f, effects, 0f);
        }

        //Main.EntitySpriteDraw(texture, drawCoordinates, frame, lightColor, 0f, origin, NPC.scale, effects, 0f);

        int dustType;
        int maxDust = Math.Max((int)(70 * Main.gfxQuality), 20);
        Texture2D dustTexture;
        Rectangle dustFrame;
        Color dustColor;
        float dustBaseScale;
        if (!WaterSphere) {
            dustType = DustID.Copper;
            maxDust /= 2;
            dustColor = drawColor;
            dustBaseScale = 1f;
        }
        else {
            dustType = Dust.dustWater();
            dustColor = Color.Lerp(drawColor, Color.White with { A = 200 }, 0.5f) * 0.8f;
            dustBaseScale = 2f;
        }

        if (dustType < DustID.Count) {
            dustTexture = TextureAssets.Dust.Value;
            dustFrame = new Rectangle(10 * (dustType % 100), 30 * (dustType / 100), 8, 8);
        }
        else {
            dustTexture = DustLoader.GetDust(dustType).Texture2D.Value;
            dustFrame = new Rectangle(0, 0, 8, 8);
        }

        float animationTimer = NPC.localAI[0] / 30f;
        for (int i = 0; i < maxDust; i++) {
            float animation = (i / (float)maxDust * 4f + animationTimer) % 4f;
            if (animation > 1f) {
                continue;
            }
            Vector2 vector = (i * i * 10f + NPC.whoAmI).ToRotationVector2();
            float particleDistance = 12f + MathF.Sin(i) * 4f;
            float scale = dustBaseScale + MathF.Sin(i * 1.33f) * 0.4f;
            float animationWave = MathF.Sin(animation * MathHelper.Pi);
            spriteBatch.Draw(dustTexture, drawCoordinates + vector * particleDistance * MathF.Pow(1f - animation, 2f), dustFrame, dustColor * animationWave, NPC.rotation + i, dustFrame.Size() / 2f, NPC.scale * scale * animationWave, effects, 0f);
        }

        if (!WaterSphere) {
            spriteBatch.Draw(texture, drawCoordinates, frame, drawColor, NPC.rotation, origin, NPC.scale, effects, 0f);
        }

        return false;
    }

    public override bool? CanFallThroughPlatforms() {
        return true;
    }
}
