using Aequus.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Weapons.Melee.Yoyo.Pistachiyo;

public class PistachiyoProj : YoyoProjectileBase {
    private float flareEffect;
    private float shellRotation;
    private float[] _shellRotations;
    private Vector2[] _shellPositions;
    private Rectangle[] _shellHitboxes;

    public override void SetStaticDefaults() {
        ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = 20f;
        ProjectileID.Sets.YoyosMaximumRange[Type] = 300f;
        ProjectileID.Sets.YoyosTopSpeed[Type] = 18f;
    }

    public override void SetDefaults() {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        _shellRotations = new float[Pistachiyo.MaximumShells];
        _shellPositions = new Vector2[Pistachiyo.MaximumShells];
        _shellHitboxes = new Rectangle[Pistachiyo.MaximumShells];
    }

    public float GetShellRotation(int i) {
        return shellRotation + MathHelper.TwoPi / Pistachiyo.MaximumShells * i;
    }

    public Vector2 GetShellCenter(int i, float rotation) {
        return rotation.ToRotationVector2() * (40f + i + flareEffect * 0.25f);
    }

    public override void AI() {
        Lighting.AddLight(Projectile.Center, TorchID.Jungle);
        shellRotation += 0.45f;

        if (flareEffect > 0f) {
            flareEffect--;
        }

        for (int i = 0; i < Pistachiyo.MaximumShells; i++) {
            float shellRotation = GetShellRotation(i);
            _shellRotations[i] = shellRotation;
            _shellPositions[i] = Projectile.Center + GetShellCenter(0, shellRotation);
            _shellHitboxes[i] = Utils.CenteredRectangle(_shellPositions[i], new Vector2(20f) * Projectile.scale);
        }
    }

    public override void CutTiles() {
        var cutTilesIgnore = Main.player[Projectile.owner].GetTileCutIgnorance(allowRegrowth: false, Projectile.trap);
        for (int i = 0; i < _shellHitboxes.Length; i++) {
            TileHelper.CutTilesRectangle(_shellHitboxes[i], TileCuttingContext.AttackProjectile, cutTilesIgnore);
        }
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
        for (int i = 0; i < _shellHitboxes.Length; i++) {
            if (targetHitbox.Intersects(_shellHitboxes[i])) {
                return true;
            }
        }
        return null;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        flareEffect = 10f;
        if (Projectile.ai[0] == -1f || target.immortal || target.SpawnedFromStatue || IsYoyoGloveYoyo()) {
            return;
        }

        int vineProjectileID = ModContent.ProjectileType<PistachiyoVineProj>();
        if (target.getRect().Intersects(Projectile.getRect()) && Main.myPlayer == Projectile.owner) {
            if (Main.player[Projectile.owner].ownedProjectileCounts[vineProjectileID] >= Pistachiyo.MaximumVines) {
                //Projectile.ai[0] = -1f;
                //Projectile.netUpdate = true;
                //int amountHealed = 0;
                //for (int i = 0; i < Main.maxProjectiles; i++) {
                //    if (Main.projectile[i].active && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].type == vineProjectileID) {
                //        amountHealed += Pistachiyo.HealAmount;
                //        Main.projectile[i].Kill();
                //    }
                //}
                //Main.player[Projectile.owner].Heal(Pistachiyo.HealAmount);
                return;
            }
            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, Projectile.DirectionTo(target.Center) * 10f,
                ModContent.ProjectileType<PistachiyoVineProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }

    public override bool PreDrawExtras() {
        return true;
    }

    public override bool PreDraw(ref Color lightColor) {
        var drawCoordinates = Projectile.Center - Main.screenPosition;
        var shellTexture = AequusTextures.SlashVanillaSmall;
        var shellFrame = shellTexture.Frame(verticalFrames: 4, frameY: 0);
        var shellOrigin = new Vector2(shellFrame.Width, shellFrame.Height / 2);
        var shellColor = new Color(170, 150, 74, 200);
        var shellTrailColor = new Color(100, 35, 27, 200);
        float shellScale = Projectile.scale;
        for (int i = 0; i < _shellPositions.Length; i++) {
            float rotation = GetShellRotation(i);
            float shellOpacity = 1f;
            for (int j = 1; j < 3; j++) {
                float rotationOffset = j * -0.5f;
                Main.EntitySpriteDraw(shellTexture, drawCoordinates + GetShellCenter(0, rotation + rotationOffset), shellTexture.Frame(verticalFrames: 4, frameY: j / 2),
                    (shellTrailColor * shellOpacity) with { A = 200 } * 0.1f, rotation + rotationOffset, shellOrigin, shellScale, SpriteEffects.None);
                shellOpacity *= 0.5f;
            }
            var finalSlashPosition = drawCoordinates + GetShellCenter(0, rotation);
            Main.EntitySpriteDraw(shellTexture, drawCoordinates + GetShellCenter(2, rotation + 0.33f), shellTexture.Frame(verticalFrames: 4, frameY: 2), Color.Chartreuse * 0.05f, rotation + 0.33f, shellOrigin, shellScale, SpriteEffects.None);
            Main.EntitySpriteDraw(shellTexture, finalSlashPosition, shellTexture.Frame(verticalFrames: 4, frameY: 0), shellColor * 0.1f, rotation, shellOrigin, shellScale, SpriteEffects.None);
            Main.EntitySpriteDraw(shellTexture, finalSlashPosition, shellTexture.Frame(verticalFrames: 4, frameY: 3), Color.Chartreuse * 0.3f, rotation, shellOrigin, shellScale, SpriteEffects.None);
        }

        var texture = TextureAssets.Projectile[Type].Value;
        var bloomColor = new Color(100, 255, 70, 0);

        Main.EntitySpriteDraw(AequusTextures.Bloom, drawCoordinates, null, bloomColor * 0.15f, 0f, AequusTextures.Bloom.Size() / 2f, Projectile.scale * 0.66f, SpriteEffects.None);
        Main.EntitySpriteDraw(texture, drawCoordinates, null, Color.White, 0f, texture.Size() / 2f, Projectile.scale * 0.9f + MathF.Sin(Main.GlobalTimeWrappedHourly * 5f) * 0.1f, SpriteEffects.None);
        if (flareEffect > 0f) {
            var flareTexture = AequusTextures.Flare;
            float intensity = MathF.Pow(flareEffect / 10f, 2f);
            Main.EntitySpriteDraw(flareTexture, drawCoordinates, null, bloomColor * intensity, 0f, flareTexture.Size() / 2f, new Vector2(0.6f, 2f) * Projectile.scale * intensity, SpriteEffects.None);
            Main.EntitySpriteDraw(flareTexture, drawCoordinates, null, bloomColor * intensity, MathHelper.PiOver2, flareTexture.Size() / 2f, new Vector2(0.6f, 2f) * Projectile.scale * intensity, SpriteEffects.None);
        }
        return false;
    }
}