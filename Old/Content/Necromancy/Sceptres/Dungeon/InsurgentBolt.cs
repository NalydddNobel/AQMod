using Aequus.Content.DataSets;
using Aequus.Core.DataSets;
using Terraria.GameContent;

namespace Aequus.Old.Content.Necromancy.Sceptres.Dungeon;

public class InsurgentBolt : InsurgencyProj {
    public override string Texture => AequusTextures.Projectile(ProjectileID.RainbowCrystalExplosion);

    public override void SetStaticDefaults() {
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileMetadata.PushableByTypeId.Add(Type);
    }

    public override void SetDefaults() {
        base.SetDefaults();
        Projectile.extraUpdates = 1;
        Projectile.scale = 0.1f;
        Projectile.alpha = 250;
    }

    public override void AI() {
        if ((int)Projectile.ai[0] == 1) {
            Projectile.velocity *= 0.6f;
            Projectile.alpha += 15;
            if (Projectile.alpha > 255) {
                Projectile.scale -= 0.8f;
                Projectile.alpha = 255;
                Projectile.Kill();
                Projectile.frame = 1;
                Projectile.rotation = 0f;
            }
            Projectile.tileCollide = false;
            return;
        }

        Projectile.tileCollide = true;
        int target = Projectile.FindTargetWithLineOfSight(400f);
        bool hasValidTarget = false;
        if (target != -1 && target != (int)Projectile.ai[1]) {
            hasValidTarget = true;
            Projectile.tileCollide = false;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Main.npc[target].Center - Projectile.Center) * 10f, 0.05f);
        }
        else {
            if (Projectile.velocity.Length() < 10f) {
                Projectile.velocity *= 1.05f;
            }
        }

        Projectile.rotation = Projectile.velocity.ToRotation();
        if (Projectile.alpha > 0) {
            Projectile.alpha -= 6;
            if (Projectile.alpha < 0) {
                Projectile.alpha = 0;
            }
        }
        if (Projectile.scale < 0.5f) {
            Projectile.scale += 0.025f;
            if (Projectile.scale > 0.5f) {
                Projectile.scale = 0.5f;
            }
        }
        if (Projectile.alpha < 150 || hasValidTarget) {
            Projectile.damage = 50;
        }

        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        //NecromancyDebuff.ApplyDebuff<InsurgentDebuff>(target, 3600, Projectile.owner);
        Projectile.damage = 0;
        Projectile.ai[0] = 1f;
        Projectile.ai[1] = target.whoAmI;
        Projectile.alpha = 0;
        Projectile.netUpdate = true;
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
        width = 10;
        height = 10;
        fallThrough = true;
        return true;
    }

    public override bool PreDraw(ref Color lightColor) {
        var texture = TextureAssets.Projectile[Type].Value;
        var frame = Projectile.Frame();
        var origin = frame.Size() / 2f;
        var effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        var drawColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;

        if ((int)Projectile.ai[0] != 2) {
            DrawHelper.DrawBasicVertexLine(AequusTextures.Trail, Projectile.oldPos, Projectile.oldRot,
                (p) => drawColor * 0.8f * Projectile.Opacity * (1f - p),
                (p) => 4f * Projectile.scale * (1f - p),
                Projectile.Size / 2f - Main.screenPosition);
        }

        Texture2D bloom = AequusTextures.Bloom;
        Vector2 drawCoordinates = Projectile.Center - Main.screenPosition;
        Main.EntitySpriteDraw(texture, drawCoordinates, frame, drawColor, Projectile.rotation + MathHelper.PiOver2, origin, Projectile.scale, effects, 0);
        return false;
    }
}