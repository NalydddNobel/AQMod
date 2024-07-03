using Aequu2.Core.Entities.Projectiles;
using System;

namespace Aequu2.Content.Dedicated.IronLotus;

public class IronLotusFlare : ModProjectile {
    public override string Texture => AequusTextures.Flare.Path;

    public override void SetDefaults() {
        Projectile.width = 200;
        Projectile.height = 200;
        Projectile.aiStyle = -1;
        Projectile.penetrate = -1;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 11;
        Projectile.timeLeft = 14;
    }

    public override void AI() {
        base.AI();
        for (int i = 0; i < 3; i++) {
            var d = Terraria.Dust.NewDustDirect(Projectile.Center - new Vector2(10f), 20, 20, DustID.Flare, Scale: Main.rand.NextFloat(0.3f, 2f));
            d.noGravity = true;
            d.velocity += Projectile.DirectionTo(d.position) * 2f;
            d.velocity *= Main.rand.NextFloat(3f);
            d.fadeIn = d.scale + 0.2f;
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        Projectile.GetDrawInfo(out var texture, out var off, out var frame, out var origin, out int _);
        float scale = Projectile.scale * (float)Math.Sin(Projectile.timeLeft / 14f * MathHelper.Pi);
        var color = Color.Lerp(Color.Red, Color.Yellow, scale * 0.5f) with { A = 30 };
        var flareColor = Color.Lerp(Color.Yellow, Color.White, scale * 0.66f) with { A = 30 };
        var drawCoords = Projectile.position + off - Main.screenPosition;
        var flareScale = new Vector2(1.2f, 2.3f) * scale;
        Main.spriteBatch.Draw(texture, drawCoords, null, flareColor, Projectile.rotation - MathHelper.PiOver4, origin, flareScale * 0.66f, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(texture, drawCoords, null, flareColor, Projectile.rotation + MathHelper.PiOver4, origin, flareScale * 0.66f, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(texture, drawCoords, null, color, Projectile.rotation, origin, flareScale, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(texture, drawCoords, null, color, Projectile.rotation + MathHelper.PiOver2, origin, flareScale, SpriteEffects.None, 0f);
        return false;
    }
}