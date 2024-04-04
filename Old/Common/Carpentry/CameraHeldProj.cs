using Terraria.GameContent;

namespace Aequus.Old.Common.Carpentry;

public abstract class CameraHeldProj : ModProjectile {
    public abstract int ShootProj { get; }

    public override void SetDefaults() {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.aiStyle = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }

    public override void AI() {
        var player = Main.player[Projectile.owner];
        int camProj = ShootProj;

        if (Projectile.ai[0] == 0) {
            Projectile.ai[0]++;
            if (Main.myPlayer == Projectile.owner) {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Projectile.velocity,
                    camProj,
                    0,
                    0f,
                    Projectile.owner
                );
            }
            player.ownedProjectileCounts[camProj]++;
        }

        if (player.ownedProjectileCounts[camProj] <= 0) {
            return;
        }

        Projectile.timeLeft = 2;
        player.itemTime = 2;
        player.itemAnimation = 2;
        player.heldProj = Projectile.whoAmI;
    }

    public override bool PreDraw(ref Color lightColor) {
        var texture = TextureAssets.Projectile[Type];

        var position = Main.GetPlayerArmPosition(Projectile);
        position.Y += Main.player[Projectile.owner].gfxOffY;
        var dir = Main.player[Projectile.owner].itemRotation.ToRotationVector2();
        var drawCoords = position + dir * -8f;
        drawCoords.Y += Main.player[Projectile.owner].gfxOffY;
        float rotation = Main.player[Projectile.owner].itemRotation + (Main.player[Projectile.owner].direction == -1 ? 0f : MathHelper.Pi);
        var origin = texture.Value.Size() / 2f;
        var spriteEffects = Main.player[Projectile.owner].direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        Main.EntitySpriteDraw(texture.Value, drawCoords - Main.screenPosition, null, ExtendLight.Get(drawCoords),
             rotation, origin, Projectile.scale, spriteEffects, 0);
        return false;
    }
}
