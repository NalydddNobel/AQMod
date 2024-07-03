namespace Aequu2.Old.Content.Enemies.DemonSiege.Keeper;

public class ChainedSoulExplosion : ModProjectile {
    public override string Texture => Aequu2Textures.GenericExplosion.Path;

    public override void SetStaticDefaults() {
        Main.projFrames[Type] = 7;
    }

    public override void SetDefaults() {
        Projectile.width = 46;
        Projectile.height = 46;
        Projectile.timeLeft = 2;
        Projectile.hostile = true;
        Projectile.aiStyle = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 20;
    }

    public override void AI() {
        Projectile.frameCounter++;
        if (Projectile.frameCounter > 2) {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            if (Projectile.frame >= Main.projFrames[Type]) {
                Projectile.hide = true;
            }
        }
    }

    public override Color? GetAlpha(Color lightColor) {
        return new Color(100, 10, 255, 150);
    }

    public override bool PreDraw(ref Color lightColor) {
        Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int _);
        Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
        return false;
    }
}
