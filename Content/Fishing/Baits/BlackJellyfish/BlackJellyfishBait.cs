using AequusRemake.Content.Graphics.Particles;
using AequusRemake.Core.ContentGeneration;
using AequusRemake.Core.Entities.Items.Components;
using AequusRemake.Core.Entities.Projectiles;
using AequusRemake.Core.Graphics;
using System;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Shaders;

namespace AequusRemake.Content.Fishing.Baits.BlackJellyfish;

public class BlackJellyfishBait : UnifiedModBait, IOnPullBobber {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 3;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.GreenJellyfish);
        Item.makeNPC = 0;
    }

    // Consumes less often
    public override bool? CanConsumeBait(Player player) {
        return Main.rand.NextBool() ? null : false;
    }

    public void PostPullBobber(Player player, Projectile bobber, int bait) {
        int npcType = (int)-bobber.localAI[1];
        if (npcType <= 0 || ContentSamples.NpcsByNetId[npcType].friendly) {
            return;
        }

        if (Main.myPlayer == player.whoAmI) {
            int damage = player.HeldItem.fishingPole * 6;
            if (Main.expertMode) {
                damage *= 2;
            }
            if (Main.hardMode) {
                damage *= 4;
            }
            Projectile.NewProjectile(new EntitySource_FishedOut(player), bobber.position, Vector2.Zero, ModContent.ProjectileType<BlackJellyfishBaitExplosion>(), damage + bobber.damage, bobber.knockBack, player.whoAmI);
        }
    }
}

public class BlackJellyfishBaitExplosion : ModProjectile, DrawLayers.IDrawLayer, RenderTargetRequests.IRenderTargetRequest {
    public override string Texture => AequusTextures.None.FullPath;

    public Point Dimensions => new Point(270, 270);
    public RenderTarget2D RenderTarget { get; set; }
    public bool IsDisposed { get; set; }
    public bool Active { get; set; }

    private const int LightningSegments = 70;
    private Vector2[] lightningDrawCoordinates;
    private float[] lightningDrawRotations;

    public override void SetDefaults() {
        Projectile.SetDefaultNoInteractions();
        Projectile.width = 250;
        Projectile.height = 250;
        Projectile.timeLeft = 20;
        Projectile.friendly = true;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = Projectile.timeLeft + 2;
    }

    public override bool? CanHitNPC(NPC target) {
        return target.realLife < 0 || target.realLife == target.whoAmI ? null : false;
    }

    public override void AI() {
        Rectangle hitbox = Projectile.Hitbox;
        if (Main.netMode == NetmodeID.Server || !Cull2D.Rectangle(hitbox)) {
            return;
        }

        if (Projectile.localAI[0] == 0f) {
            Projectile.localAI[0] = 1f;
            SoundEngine.PlaySound(AequusSounds.BlackJellyfishExplosion with { PitchVariance = 0.1f }, Projectile.Center);

            // Dusts
            for (int i = 0; i < 30; i++) {
                Vector2 randomVector = Main.rand.NextVector2Unit();
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.MartianSaucerSpark, Alpha: 0, Scale: Main.rand.NextFloat(0.8f, 1.8f));
                d.rotation = 0f;
                d.velocity = randomVector * Main.rand.NextFloat(4f);
                d.fadeIn = d.scale + Main.rand.NextFloat(0.8f);
                d.noGravity = true;
            }
            for (int i = 0; i < 60; i++) {
                Vector2 randomVector = Main.rand.NextVector2Unit();
                Dust d = Dust.NewDustPerfect(Projectile.Center + randomVector * Main.rand.NextFloat(0.8f, 1f) * Projectile.width / 2f, DustID.MartianSaucerSpark, Alpha: 0, Scale: Main.rand.NextFloat(0.8f, 2.5f));
                d.rotation = 0f;
                d.velocity += randomVector * 4f * Main.rand.NextFloat();
                d.noGravity = true;
            }

            int particleCount = Math.Max((int)(Projectile.width / 2 * Main.gfxQuality), Projectile.width / 4);
            for (int i = 0; i < particleCount; i++) {
                Vector2 velocity = Main.rand.NextVector2Unit();
                Vector2 location = Projectile.Center + velocity * Main.rand.NextFloat(0.1f, 0.6f) * Projectile.width / 2f;
                if (!Collision.WetCollision(location, 2, 2)) {
                    continue;
                }

                var particle = UnderwaterBubbles.New();
                particle.Location = location;
                particle.Frame = (byte)Main.rand.Next(5);
                particle.Velocity = velocity * Main.rand.NextFloat(1f);
                particle.UpLift = (1f - particle.Velocity.X) * 0.003f;
                particle.Opacity = Main.rand.NextFloat(0.8f, 1f);
            }

            // Water Ripples
            DrawHelper.AddWaterRipple(Projectile.Center, 0.2f, 0.66f, 0f, new Vector2(30f, 30f), RippleShape.Circle, 0f);
            DrawHelper.AddWaterRipple(Projectile.Center, 0.5f, 0.8f, 0f, new Vector2(60f, 60f), RippleShape.Circle, 0f);

            // Screen Shake
            ViewHelper.PunchCameraTo(Projectile.Center, strength: 8f, frames: Projectile.timeLeft + 12);
        }

        if (Main.rand.NextBool()) {
            Vector2 location = Projectile.Center + Main.rand.NextVector2Unit() * Projectile.width / 2f;
            DrawHelper.AddWaterRipple(location, 0.2f, 0.66f, 0f, new Vector2(10f, 10f), RippleShape.Circle, 0f);
            DrawHelper.AddWaterRipple(location, 0.5f, 0.8f, 0f, new Vector2(30f, 30f), RippleShape.Circle, 0f);
        }
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
        DrawLayers.Instance.PostDrawLiquids += Projectile;
        RenderTargetRequests.Instance.Request(this);
    }

    public override bool PreDraw(ref Color lightColor) {
        return false;
    }

    public void DrawOntoLayer(SpriteBatch spriteBatch, DrawLayers.DrawLayer layer) {
        var drawCoordinates = Projectile.Center;

        if (lightningDrawCoordinates == null) {
            lightningDrawCoordinates = new Vector2[LightningSegments];
            lightningDrawRotations = new float[LightningSegments];
        }

        Color lightningColor = new Color(255, 200, 100, 10);
        float attackProgress = 1f - Projectile.timeLeft / 20f;
        float attackRange;
        float attackStart = 0.3f;
        if (attackProgress < attackStart) {
            attackRange = attackProgress / attackStart;
        }
        else {
            attackRange = 1f - (attackProgress - attackStart) / (1f - attackStart);
        }
        Main.EntitySpriteDraw(AequusTextures.BloomStrong, drawCoordinates - Main.screenPosition, null, lightningColor, 0f, AequusTextures.BloomStrong.Size() / 2f, attackRange * 1.5f, SpriteEffects.None, 0f);
        Main.EntitySpriteDraw(AequusTextures.Bloom, drawCoordinates - Main.screenPosition, null, lightningColor, 0f, AequusTextures.Bloom.Size() / 2f, attackRange * 1.25f, SpriteEffects.None, 0f);

        if (RenderTarget != null && !RenderTarget.IsDisposed && !RenderTarget.IsContentLost) {
            Color color = Color.White;
            drawCoordinates = Projectile.Center - Main.screenPosition;
            Vector2 origin = RenderTarget.Size() / 2f;
            spriteBatch.Draw(RenderTarget, drawCoordinates, null, color, 0f, origin, 1f, SpriteEffects.None, 0f);
        }
    }

    public void DrawOntoRenderTarget(GraphicsDevice graphics, SpriteBatch sb) {
        if (Projectile == null || !Projectile.active) {
            Active = false;
            return;
        }

        graphics.Clear(Color.Transparent);

        var drawCoordinates = RenderTarget.Size() / 2f;

        if (lightningDrawCoordinates == null) {
            lightningDrawCoordinates = new Vector2[LightningSegments];
            lightningDrawRotations = new float[LightningSegments];
        }

        Color lightningColor = new Color(255, 200, 100, 10);
        float attackProgress = 1f - Projectile.timeLeft / 20f;
        float attackRange;
        float attackStart = 0.3f;
        if (attackProgress < attackStart) {
            attackRange = attackProgress / attackStart;
        }
        else {
            attackRange = 1f - (attackProgress - attackStart) / (1f - attackStart);
        }

        attackRange = MathF.Sin(attackRange * MathHelper.Pi) * Projectile.width / 2f;

        for (int i = 0; i < lightningDrawCoordinates.Length; i++) {
            float rotation = i * MathHelper.TwoPi / (lightningDrawCoordinates.Length - 1) * 2f + Main.GlobalTimeWrappedHourly * 38f;
            lightningDrawCoordinates[i] = drawCoordinates + rotation.ToRotationVector2() * attackRange + Main.rand.NextVector2Square(-attackRange, attackRange) / 12f;
            lightningDrawRotations[i] = rotation - MathHelper.PiOver2;
        }

        DrawHelper.ApplyBasicEffect(DrawHelper.View, DrawHelper.Projection, AequusTextures.BlackJellyfishVertexStrip);
        DrawHelper.VertexStrip.PrepareStrip(lightningDrawCoordinates, lightningDrawRotations,
            p => new Color(255, 200, 100, 10),
            p => 9f,
            Vector2.Zero, includeBacksides: true);
        DrawHelper.VertexStrip.DrawTrail();
    }

    public void Dispose() {
        if (RenderTarget != null && !RenderTarget.IsDisposed) {
            RenderTarget.Dispose();
            RenderTarget = null;
        }

        IsDisposed = true;
        Active = false;

        GC.SuppressFinalize(this);
    }

    ~BlackJellyfishBaitExplosion() {
        if (!IsDisposed) {
            Dispose();
        }
    }
}