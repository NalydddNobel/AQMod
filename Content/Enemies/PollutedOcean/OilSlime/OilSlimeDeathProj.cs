using Terraria.DataStructures;

namespace Aequus.Content.Enemies.PollutedOcean.OilSlime;

public class OilSlimeDeathProj : ModProjectile, IOilSlimeInheritedBurning {
    public bool OnFire { get; set; }

    public override void SetStaticDefaults() {
        Main.projFrames[Type] = 6;
    }

    public override void SetDefaults() {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.aiStyle = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }

    public override void OnSpawn(IEntitySource source) {
        if (source is EntitySource_Parent parent && parent.Entity is IOilSlimeInheritedBurning burningInst) {
            OnFire = burningInst.OnFire;
        }
    }

    public override void AI() {
        if ((int)Projectile.ai[0] == 1) {
            if (Projectile.localAI[0] == 0f) {
                Projectile.localAI[0] = 1f;
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }

            if (Projectile.frame == 0 || Projectile.frameCounter++ >= 2) {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame < Main.projFrames[Type] - 1) {
                    for (int i = 0; i < Projectile.frame * 3; i++) {
                        Vector2 wantedPosition = Projectile.Center;
                        Vector2 velocity = Main.rand.NextVector2Unit();
                        Dust d = Dust.NewDustPerfect(wantedPosition + velocity * Main.rand.NextFloat(0.5f, 1f) * Projectile.frame * 8f, DustID.TintableDust, Alpha: Projectile.alpha, newColor: OilSlime.SlimeColor);
                        d.velocity += velocity * 4f;
                        d.noGravity = true;
                    }
                }
            }

            return;
        }

        Projectile.velocity.Y += 0.3f;
        Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

        if (Projectile.velocity.Y > 5f) {
            Projectile.tileCollide = true;
        }

        if (Main.GameUpdateCount % 2 == 0) {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.TintableDust, Alpha: Projectile.alpha, newColor: OilSlime.SlimeColor);
            d.velocity *= 0.25f;
            d.velocity -= Projectile.velocity * 0.2f;
            d.noGravity = true;
            d.scale *= 1.8f;
        }

        if (Projectile.tileCollide && Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height)) {
            OnCollision();
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        OnCollision();
        return false;
    }

    private void OnCollision() {
        if (Projectile.ai[0] == 1f) {
            return;
        }

        if (Main.myPlayer == Projectile.owner) {
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<OilSlimeHazard>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }

        Projectile.ai[0] = 1f;
        Projectile.netUpdate = true;
        Projectile.timeLeft = 60;
        Projectile.velocity = Vector2.Zero;
    }

    public override bool PreDraw(ref Color lightColor) {
        if (Projectile.frame >= Main.projFrames[Type]) {
            return false;
        }

        Texture2D texture = ProjectileTexture[Type].Value;
        Rectangle frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
        Vector2 drawCoordinates = Projectile.Center - Main.screenPosition;

        Main.EntitySpriteDraw(texture, drawCoordinates, frame, lightColor, Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None);
        return false;
    }
}
