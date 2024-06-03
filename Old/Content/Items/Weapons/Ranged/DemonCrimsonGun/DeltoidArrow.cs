using Aequus.DataSets;
using Aequus.Old.Content.Particles;
using Terraria.Audio;
using Terraria.GameContent;

namespace Aequus.Old.Content.Items.Weapons.Ranged.DemonCrimsonGun;

public class DeltoidArrow : ModProjectile {
    public static readonly Color FireColor = new Color(205, 15, 30, 60);
    public static readonly Color BloomColor = new Color(175, 10, 2, 10);

    public override void SetStaticDefaults() {
        ProjectileID.Sets.TrailCacheLength[Type] = 10;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileDataSet.PushableByTypeId.Add(Type);
        ProjectileDataSet.DealsHeatDamage.Add(Type);
    }

    public override void SetDefaults() {
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.friendly = true;
        Projectile.aiStyle = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 12;
        Projectile.timeLeft = 120;
        Projectile.extraUpdates = 1;
        Projectile.alpha = 200;
    }

    public override void AI() {
        if (Projectile.alpha > 0) {
            Projectile.alpha -= 20;
            if (Projectile.alpha < 0) {
                Projectile.alpha = 0;
            }
        }
        Projectile.ai[0]++;
        if (Projectile.ai[0] > 20f) {
            Projectile.velocity.Y += 0.45f;
        }
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        if (Main.rand.NextBool(6)) {
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SilverFlame,
                Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 0, new Color(255, 120, Main.rand.Next(70), 0), 1f).noGravity = true;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        if (Main.rand.NextBool(3)) {
            target.AddBuff(BuffID.OnFire3, 120);
        }
        if (Main.myPlayer == Projectile.owner) {
            Projectile.NewProjectile(Projectile.GetSource_Death(), target.Center, Vector2.Normalize(Projectile.velocity) * 0.01f, ModContent.ProjectileType<DeltoidExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner, target.whoAmI + 1);
        }
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info) {
        if (Main.rand.NextBool(3)) {
            target.AddBuff(BuffID.OnFire3, 120);
        }
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
        width = 2;
        height = 2;
        return true;
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        if (Main.netMode != NetmodeID.Server) {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        }
        if (Main.myPlayer == Projectile.owner) {
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Normalize(Projectile.velocity) * 0.01f, ModContent.ProjectileType<DeltoidExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
        return true;
    }

    public override bool PreDraw(ref Color lightColor) {
        var texture = TextureAssets.Projectile[Type].Value;
        var origin = new Vector2(texture.Width / 2f, 8f);

        DrawHelper.VertexStrip.PrepareStrip(Projectile.oldPos, Projectile.oldRot,
            (p) => BloomColor * 3 * (1f - p),
            (p) => 8f,
            Projectile.Size / 2f);
        DrawHelper.VertexStrip.DrawTrail();

        var frame = Projectile.Frame();
        Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, Color.White * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
        return false;
    }
}

public class DeltoidExplosion : ModProjectile {
    public override string Texture => AequusTextures.GenericExplosion.Path;

    public override void SetStaticDefaults() {
        Main.projFrames[Type] = 7;
    }

    public override void SetDefaults() {
        Projectile.SetDefaultNoInteractions();
        Projectile.timeLeft = 20;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.width = 90;
        Projectile.height = 90;
    }

    public override Color? GetAlpha(Color lightColor) {
        return DeltoidArrow.BloomColor with { A = 30 } * 5;
    }

    public override bool? CanHitNPC(NPC target) {
        return target.whoAmI + 1 == (int)Projectile.ai[0] ? false : null;
    }

    public override void AI() {
        if (Projectile.frame == 0 && Main.netMode != NetmodeID.Server) {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            foreach (var p in LegacyBloomParticle.NewMultiple(5)) {
                Vector2 randomVector2Unit = Main.rand.NextVector2Unit();

                p.Location = Projectile.Center + randomVector2Unit * Main.rand.NextFloat(16f);
                p.Velocity = randomVector2Unit * Main.rand.NextFloat(3f, 12f);
                p.Color = DeltoidArrow.FireColor;
                p.BloomColor = DeltoidArrow.BloomColor * 0.2f;
                p.Scale = 1.25f;
                p.BloomScale = 0.3f;
                p.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                p.dontEmitLight = false;
                p.Frame = (byte)Main.rand.Next(3);
            }
            for (int i = 0; i < 15; i++) {
                var v = Main.rand.NextVector2Unit();
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<VoidDust>(), v * Main.rand.NextFloat(1f, 12f), 0,
                    new Color(255, 85, 25), Main.rand.NextFloat(0.4f, 1.5f));
            }
        }
        Projectile.frameCounter++;
        if (Projectile.frameCounter > 2) {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            if (Projectile.frame >= Main.projFrames[Type]) {
                Projectile.hide = true;
            }
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int _);
        Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
        return false;
    }
}