using Aequus.Content.DataSets;
using Aequus.Core.DataSets;
using Aequus.Old.Content.StatusEffects.DamageOverTime;
using System;
using Terraria.Audio;
using Terraria.GameContent;

namespace Aequus.Old.Content.Weapons.Demon.Magic;

public class BombarderRodProj : ModProjectile {
    public override string Texture => AequusTextures.Projectile(ProjectileID.Flamelash);

    public override void SetStaticDefaults() {
        Main.projFrames[Type] = Main.projFrames[ProjectileID.Flamelash];
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileSets.PushableByTypeId.Add((ProjectileEntry)Type);
        ProjectileSets.DealsHeatDamage.Add((ProjectileEntry)Type);
    }

    public override void SetDefaults() {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.extraUpdates = 2;
        Projectile.timeLeft = 120;
        Projectile.alpha = 200;
    }

    public override void AI() {
        Projectile.UpdateShimmerReflection();

        if ((int)Projectile.ai[0] == 0) {
            int dir = Main.rand.NextBool().ToDirectionInt();
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.PiOver2 * dir * 0.2f);
            Projectile.ai[1] = dir;
            Projectile.ai[0] += Main.rand.NextFloat(-5f, 5f);
            Projectile.netUpdate = true;
        }
        if (Projectile.alpha < 200 && Main.rand.NextBool(2)) {
            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Scale: Main.rand.NextFloat(2f, 2.66f) * Projectile.Opacity);
            d.velocity *= 0.5f;
            d.velocity -= Projectile.velocity * 0.2f;
            d.noGravity = true;
        }
        if (Projectile.alpha < 200 && Main.rand.NextBool(5)) {
            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Scale: Main.rand.NextFloat(0.5f, 1.66f) * Projectile.Opacity);
            d.velocity *= 0.5f;
            d.velocity -= Projectile.velocity * 0.2f;
            d.noGravity = true;
        }
        if (Projectile.timeLeft < 100) {
            Projectile.velocity *= 0.995f;
        }
        if (Projectile.timeLeft < 50) {
            Projectile.alpha += 6;
            Projectile.scale -= 0.0075f;
            Projectile.velocity *= 0.99f;
        }
        else {
            if (Projectile.alpha > 0) {
                Projectile.alpha -= 5;
                if (Projectile.alpha < 0) {
                    Projectile.alpha = 0;
                }
            }
        }

        var target = Projectile.FindTargetWithLineOfSight(240f);
        if (target != -1) {
            Projectile.velocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity,
                Vector2.Normalize(Main.npc[target].Center - Projectile.Center) * Projectile.velocity.Length(), 0.05f)) * Projectile.velocity.Length();
        }
        else if (Projectile.ai[0] < 25f) {
            Projectile.velocity = Projectile.velocity.RotatedBy(0.04f * -Projectile.ai[1] * (Math.Max(Projectile.ai[0], 0f) / 25f));
        }
        Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        if (Collision.SolidCollision(Projectile.position - new Vector2(32f, 32f), Projectile.width + 64, Projectile.height + 64)) {
            Projectile.ai[0]++;
        }
        if (Collision.SolidCollision(Projectile.position - new Vector2(16f, 16f), Projectile.width + 32, Projectile.height + 32)) {
            Projectile.ai[0]++;
        }
        Projectile.ai[0]++;
        if ((int)Projectile.ai[0] == 0) {
            Projectile.ai[0] = 1;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        if (hit.Crit) {
            target.AddBuff(ModContent.BuffType<CrimsonHellfire>(), 300);

            for (int i = 0; i < Main.maxNPCs; i++) {
                if (i == target.whoAmI) {
                    continue;
                }
                if (Main.npc[i].active && !Main.npc[i].friendly && Main.npc[i].Distance(target.Center) < 180f) {
                    Main.npc[i].AddBuff(ModContent.BuffType<CrimsonHellfire>(), 300);
                }
            }

            if (Main.player[Projectile.owner].hostile) {
                for (int i = 0; i < Main.maxPlayers; i++) {
                    if (Main.player[i].active && !Main.player[i].dead && !Main.player[i].ghost && Main.player[i].hostile && Main.player[i].team != Main.player[Projectile.owner].team
                        && Main.player[i].Distance(target.Center) < 180f) {
                        Main.player[i].AddBuff(ModContent.BuffType<CrimsonHellfire>(), 300);
                    }
                }
            }

            for (int i = 0; i < 75; i++) {
                var d = Dust.NewDustDirect(target.position, target.width, target.height, DustID.Torch, Scale: Main.rand.NextFloat(1f, 4f) * Projectile.Opacity);
                d.velocity *= 0.5f;
                d.velocity = Vector2.Normalize(target.Center - d.position) * Main.rand.NextFloat() * 16f;
                d.noGravity = true;
            }
        }
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
        width = 10;
        height = 10;
        fallThrough = true;
        return true;
    }

    public override bool PreDraw(ref Color lightColor) {
        var texture = TextureAssets.Projectile[Type].Value;
        var textureFrame = Projectile.Frame();
        var textureOrigin = textureFrame.Size() / 2f;

        var bloom = AequusTextures.Bloom;
        var bloomFrame = bloom.Value.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
        var bloomOrigin = bloomFrame.Size() / 2f;

        var center = Projectile.Center;
        var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
        int trailLength = ProjectileID.Sets.TrailCacheLength[Type];
        for (int i = 0; i < trailLength; i++) {
            float p = i / trailLength;
            Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.oldPos[i] + offset - Main.screenPosition, textureFrame,
                Color.Lerp(new Color(255, 255, 255, 128), new Color(255, 10, 10, 128), 1f - p) * Projectile.Opacity * p * p * 0.5f, Projectile.rotation, textureOrigin, Projectile.scale, SpriteEffects.None, 0f);
        }

        Main.spriteBatch.Draw(bloom, Projectile.position + offset - Main.screenPosition, null, CrimsonHellfire.FireColor * Projectile.Opacity * 2f,
            Projectile.rotation, bloomOrigin, Projectile.scale * 0.25f, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.position + offset - Main.screenPosition, textureFrame,
           Color.White * Projectile.Opacity, Projectile.rotation, textureOrigin, Projectile.scale, SpriteEffects.None, 0f);
        return false;
    }

    public override void OnKill(int timeLeft) {
        if (Main.netMode == NetmodeID.Server || (Projectile.alpha > 128 && Projectile.ai[0] > 25f)) {
            return;
        }

        SoundEngine.PlaySound(SoundID.Item89 with { Volume = 0.75f, Pitch = 0.5f }, Projectile.Center);
        var center = Projectile.Center;
        for (int i = 0; i < 20; i++) {
            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
            d.velocity = (d.position - center) / 8f;
            d.noGravity = true;
            if (Main.rand.NextBool(3)) {
                d.velocity *= 2f;
                d.scale *= 1.75f;
                d.fadeIn = d.scale + Main.rand.NextFloat(0.5f, 0.75f);
            }
        }
    }
}