using Aequus.Content.DataSets;
using Aequus.Core.DataSets;
using Aequus.Old.Content.Particles;
using Aequus.Old.Content.StatusEffects.DamageOverTime;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Aequus.Old.Content.Weapons.Demon.Ranged;

public class HamaYumiArrow : ModProjectile {
    public override void SetStaticDefaults() {
        Main.projFrames[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 10;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileSets.PushableByTypeId.Add((ProjectileEntry)Type);
        ProjectileSets.DealsHeatDamage.Add((ProjectileEntry)Type);
    }

    public override void SetDefaults() {
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.friendly = true;
        Projectile.aiStyle = -1;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 12;
        Projectile.timeLeft = 28;
        Projectile.extraUpdates = 1;
        Projectile.alpha = 200;
    }

    public override void OnSpawn(IEntitySource source) {
        if (source is EntitySource_ItemUse_WithAmmo ammo) {
            Projectile.ai[0] = ContentSamples.ItemsByType[ammo.AmmoItemIdUsed].shoot;
        }
        else {
            Projectile.ai[0] = ProjectileID.WoodenArrowFriendly;
        }
        Projectile.ai[1] = Projectile.damage;
    }

    public override void AI() {
        if (Projectile.alpha > 0) {
            Projectile.alpha -= 20;
            if (Projectile.alpha < 0) {
                Projectile.alpha = 0;
            }
        }
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, System.Int32 damageDone) {
        target.AddBuff(ModContent.BuffType<CorruptionHellfire>(), 120);
        if (Main.myPlayer == Projectile.owner) {
            Projectile.NewProjectile(Projectile.GetSource_Death(), target.Center, Vector2.Normalize(Projectile.velocity) * 0.01f, ModContent.ProjectileType<HamaYumiExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner, target.whoAmI + 1);
        }
        Projectile.damage = (System.Int32)(Projectile.damage * 0.75f);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info) {
        target.AddBuff(ModContent.BuffType<CorruptionHellfire>(), 120);
    }

    public override System.Boolean TileCollideStyle(ref System.Int32 width, ref System.Int32 height, ref System.Boolean fallThrough, ref Vector2 hitboxCenterFrac) {
        width = 2;
        height = 2;
        return true;
    }

    public override System.Boolean OnTileCollide(Vector2 oldVelocity) {
        if (Main.netMode != NetmodeID.Server) {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        }
        if (Main.myPlayer == Projectile.owner) {
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Normalize(Projectile.velocity) * 0.01f, ModContent.ProjectileType<HamaYumiExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
        return true;
    }

    public override void OnKill(System.Int32 timeLeft) {
        if (Main.myPlayer == Projectile.owner) {
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity * 2f, (System.Int32)Projectile.ai[0], (System.Int32)Projectile.ai[1], Projectile.knockBack, Projectile.owner);
        }
    }

    public override System.Boolean PreDraw(ref Color lightColor) {
        var texture = TextureAssets.Projectile[Type].Value;
        var origin = new Vector2(texture.Width / 2f, 8f);

        DrawHelper.VertexStrip.PrepareStrip(Projectile.oldPos, Projectile.oldRot,
            (p) => CorruptionHellfire.BloomColor * 3 * (1f - p),
            (p) => 8f,
            Projectile.Size / 2f);
        DrawHelper.VertexStrip.DrawTrail();

        var frame = Projectile.Frame();

        Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, Color.White * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

        System.Single opacity = 1f;
        if (Projectile.timeLeft < 12) {
            opacity = Projectile.timeLeft / 12f;
        }
        frame.Y += frame.Height;
        Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, CorruptionHellfire.FireColor * Projectile.Opacity * opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
        return false;
    }
}

public class HamaYumiExplosion : ModProjectile {
    public override System.String Texture => AequusTextures.GenericExplosion.Path;

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
        return CorruptionHellfire.BloomColor with { A = 30 } * 5;
    }

    public override System.Boolean? CanHitNPC(NPC target) {
        return target.whoAmI + 1 == (System.Int32)Projectile.ai[0] ? false : null;
    }

    public override void AI() {
        if (Projectile.frame == 0 && Main.netMode != NetmodeID.Server) {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            foreach (var p in ModContent.GetInstance<LegacyBloomParticle>().NewMultiple(5)) {
                Vector2 randomVector2Unit = Main.rand.NextVector2Unit();

                p.Location = Projectile.Center + randomVector2Unit * Main.rand.NextFloat(16f);
                p.Velocity = randomVector2Unit * Main.rand.NextFloat(3f, 12f);
                p.Color = CorruptionHellfire.FireColor;
                p.BloomColor = CorruptionHellfire.BloomColor * 0.2f;
                p.Scale = 1.25f;
                p.BloomScale = 0.3f;
                p.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                p.dontEmitLight = false;
                p.Frame = (System.Byte)Main.rand.Next(3);
            }
            for (System.Int32 i = 0; i < 15; i++) {
                var v = Main.rand.NextVector2Unit();
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<VoidDust>(), v * Main.rand.NextFloat(1f, 12f), 0,
                    new Color(175, 50, 255), Main.rand.NextFloat(0.4f, 1.5f));
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

    public override System.Boolean PreDraw(ref Color lightColor) {
        Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out System.Int32 _);
        Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
        return false;
    }
}