using Aequu2.Core;
using Aequu2.Content.Enemies.PollutedOcean.Eel;
using Aequu2.Content.Graphics.Particles;
using Aequu2.Core.ContentGeneration;
using Aequu2.Core.Particles;
using System;
using System.Collections.Generic;
using Terraria.Localization;

namespace Aequu2.Content.Items.Weapons.Summon.Whips.EekWhip;

[WorkInProgress]
public class ElectricEelWhip : UnifiedWhipItem, IMinionTagController {
    public static readonly int TagDamage = 10;

    ModBuff IMinionTagController.TagBuff { get; set; }
    int IMinionTagController.TagDuration => 240;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TagDamage);

    public override void SetDefaults() {
        Item.DefaultToWhip(WhipProjectile.Type, 8, 5f, 5f, animationTotalTime: 26);
        Item.rare = ItemRarityID.Green;
        Item.value = Commons.Cost.BiomeOcean;
    }

    private static void SpawnWhipParticle(Vector2 where, Vector2 velocity, float scale) {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        int dustType = DustID.BreatheBubble;
        if (Collision.WetCollision(where, 2, 2)) {
            if (Main.rand.NextBool(4)) {
                Particle<UnderwaterBubbles.Bubble>.New().Setup(where, velocity * 0.05f, 0.003f, (byte)(scale * 1.5f));
            }
        }
        else {
            dustType = Dust.dustWater();
            velocity *= 0.5f;
        }

        Dust d = Dust.NewDustPerfect(where, dustType, velocity, Scale: scale);
        d.noGravity = true;
    }

    public override void WhipAI(Projectile projectile, List<Vector2> WhipPoints, float Progress) {
        Vector2 endPoint = WhipPoints[^1];
        if (Vector2.Distance(endPoint, Main.player[projectile.owner].Center) > 32f && projectile.localAI[0] > 0f) {
            Vector2 velocity = (endPoint - new Vector2(projectile.localAI[0], projectile.localAI[1])) * 0.3f;

            SpawnWhipParticle(endPoint, velocity * 0.33f, 2f);

            if (Main.rand.NextBool()) {
                Vector2 randomPoint = Main.rand.Next(WhipPoints) + Main.rand.NextVector2Square(-16f, 16f);
                SpawnWhipParticle(randomPoint, Vector2.Zero, Main.rand.NextFloat(0.9f, 1.2f));
            }
        }

        projectile.localAI[0] = endPoint.X;
        projectile.localAI[1] = endPoint.Y;
    }

    public override void DrawWhip(IWhipController.WhipDrawParams drawInfo) {
        Texture2D texture = drawInfo.Texture;
        int i = drawInfo.SegmentIndex;
        int count = drawInfo.SegmentCount;

        Vector2 originOffset = Vector2.Zero;
        int frameIndex;
        bool head = i == count - 2;
        if (head) { frameIndex = 0; }
        else if (i > 0) { frameIndex = i % 3 + 1; }
        else {
            frameIndex = 4;
            originOffset = new Vector2(0f, 8f);
        }

        Rectangle frame = texture.Frame(2, 5, 0, frameIndex);

        Color lightColor = ExtendLight.Get(drawInfo.Position);
        float rotation = (drawInfo.Position - drawInfo.Next).ToRotation() - MathHelper.PiOver2;
        Vector2 origin = frame.Size() / 2f + originOffset;
        float scale = drawInfo.Projectile.scale;
        SpriteEffects effects = drawInfo.SpriteEffects;

        Vector2 drawCoordinates = drawInfo.Position - Main.screenPosition;
        Main.EntitySpriteDraw(texture, drawCoordinates, frame, lightColor, rotation, origin, scale, effects, 0);
        Main.EntitySpriteDraw(texture, drawCoordinates, frame with { X = frame.Width }, Color.White, rotation, origin, scale, effects, 0);

        if (head) {
            Projectile.GetWhipSettings(drawInfo.Projectile, out float timeToFlyOut, out int _, out float _);
            Texture2D flareTexture = AequusTextures.FlareSoft;
            float progress = drawInfo.Projectile.ai[0] / timeToFlyOut;
            float opacity = Math.Clamp(Vector2.Distance(drawInfo.Position, DrawHelper.ScreenCenter) / 180f - 0.5f, 0f, 1f);
            Color glowColor = Eel.EyeFlareColor * opacity;
            Vector2 flareOrigin = flareTexture.Size() / 2f;
            Vector2 flareScale = new Vector2(1f, 0.5f) * scale;
            Main.EntitySpriteDraw(AequusTextures.FlareSoft, drawCoordinates, null, glowColor, 0f, flareOrigin, flareScale, SpriteEffects.None, 0);
        }
    }

    public override void SetWhipSettings(Projectile projectile, ref WhipSettings settings) {
        settings.Segments = 18;
    }

    void IMinionTagNPCController.ModifyMinionHit(NPC npc, Projectile minionProj, ref NPC.HitModifiers modifiers, float tagMultiplier) {
        modifiers.FlatBonusDamage += TagDamage * tagMultiplier;
    }

    public override Color GetWhipStringColor(Vector2 position) {
        return Color.White;
    }
}
