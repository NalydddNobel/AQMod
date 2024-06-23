using Aequus.Core.Entities.Projectiles;
using System;
using System.Collections.Generic;
using Terraria.Audio;

namespace Aequus.Content.Items.Weapons.Melee.DynaKnife;

public class DynaknifeStabProj : ModProjectile {
    public const int ExplodeDelay = 120;

    public Vector2 normalOffset;

    public int NPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }

    public override string Texture => AequusTextures.DynaknifeProj.Path;

    public override void SetDefaults() {
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = ExplodeDelay;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
        normalOffset = default;
    }

    public override void AI() {
        var npc = Main.npc[NPC];

        if (npc.active) {
            Projectile.localAI[0] *= 0.8f;
            Projectile.localAI[0] -= 0.1f;
            if (normalOffset == default) {
                Projectile.localAI[0] = 10f;
                normalOffset = Projectile.DirectionFrom(npc.Center).SafeNormalize(Vector2.UnitY).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f));
            }
            Projectile.Center = npc.Center;
            var dustPosition = Projectile.Center + normalOffset * Main.npc[NPC].Size / 2f;
            float dustVelocity = 16f * MathF.Pow(Projectile.timeLeft / (float)ExplodeDelay, 2f);
            var d = Terraria.Dust.NewDustPerfect(dustPosition, DustID.Torch, normalOffset.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * Main.rand.NextFloat(dustVelocity));
            d.noGravity = true;
            d.fadeIn = d.scale + 0.2f;
            if (Projectile.timeLeft <= 40) {
                float distance = Main.rand.NextFloat(Projectile.timeLeft * 2f);
                var v = Main.rand.NextVector2Unit();
                d = Terraria.Dust.NewDustPerfect(dustPosition + v * distance, DustID.Torch, v * -distance * Main.rand.NextFloat(0.1f));
                d.noGravity = true;
                d.fadeIn = d.scale + 0.2f;
            }
        }
        else {
            Projectile.timeLeft = Math.Min(Projectile.timeLeft, 5);
        }

        if (Projectile.timeLeft == 5) {
            Projectile.friendly = true;
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            var dustPosition = Projectile.Center;
            for (int i = 0; i < Projectile.width; i++) {
                float distance = Main.rand.NextFloat(Projectile.width / 2f);
                var v = Main.rand.NextVector2Unit();
                var d = Terraria.Dust.NewDustPerfect(dustPosition + v * distance, DustID.Smoke, v * distance * Main.rand.NextFloat(0.1f), Scale: Main.rand.NextFloat(2f));
                d.noGravity = true;
                d.fadeIn = d.scale + 0.2f;
            }
            for (int i = 0; i < Projectile.width; i++) {
                float distance = Main.rand.NextFloat(Projectile.width / 2f);
                var v = Main.rand.NextVector2Unit();
                var d = Terraria.Dust.NewDustPerfect(dustPosition + v * distance, DustID.Torch, v * distance * Main.rand.NextFloat(0.2f), Scale: Main.rand.NextFloat(2f));
                d.noGravity = true;
                d.fadeIn = d.scale + 0.2f;
            }
            ViewHelper.PunchCameraTo(Projectile.Center);
        }
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
        if (Projectile.localAI[0] > 0f) {
            overPlayers.Add(index);
            Projectile.hide = true;
            return;
        }

        Projectile.hide = false;
    }

    public override bool PreDraw(ref Color lightColor) {
        Projectile.GetDrawInfo(out var texture, out _, out var frame, out var origin, out _);

        var npc = Main.npc[NPC];
        var drawPosition = npc.Center + new Vector2(0f, npc.gfxOffY) + normalOffset * Main.npc[NPC].Size / 2f;
        var effects = SpriteEffects.None;
        float rotation = normalOffset.ToRotation() + MathHelper.PiOver4 * 5f;
        float opacity = 1f;
        if (Projectile.localAI[0] > 0f) {
            Main.EntitySpriteDraw(AequusTextures.Flare, drawPosition - Main.screenPosition, null, Color.Red with { A = 100 } * Projectile.scale, MathHelper.PiOver2, AequusTextures.Flare.Size() / 2f, new Vector2(0.5f, Projectile.localAI[0] * 0.5f) * Projectile.scale, SpriteEffects.None, 0);
            opacity = 1f - Projectile.localAI[0] / 10f;
            drawPosition += new Vector2(Main.rand.NextFloat(-Projectile.localAI[0], Projectile.localAI[0]), Main.rand.NextFloat(-Projectile.localAI[0], Projectile.localAI[0]));
            drawPosition += normalOffset * Projectile.localAI[0] * 3f;
        }
        if (Projectile.timeLeft < 15) {
            float bloomOpacity = opacity * (1f - (Projectile.timeLeft - 5f) / 10f);
            Main.EntitySpriteDraw(AequusTextures.BloomStrong, drawPosition - Main.screenPosition, null, Color.Red with { A = 0 } * bloomOpacity, rotation, AequusTextures.BloomStrong.Size() / 2f, Projectile.scale * bloomOpacity * 0.4f, effects, 0);
        }

        Main.EntitySpriteDraw(texture, drawPosition - Main.screenPosition, frame, lightColor * opacity, rotation, origin, Projectile.scale * opacity, effects, 0);
        Main.EntitySpriteDraw(AequusTextures.Dynaknife_Glow, drawPosition - Main.screenPosition, frame, Color.White with { A = 0 } * opacity, rotation, origin, Projectile.scale * opacity, effects, 0);
        return false;
    }
}