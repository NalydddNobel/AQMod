using Aequus.Core.ContentGeneration;
using Aequus.Old.Content.Events.Glimmer;
using Aequus.Old.Content.Particles;
using Aequus.Old.Content.StatusEffects;
using Aequus.Old.Core.Utilities;
using System;
using Terraria.GameContent;

namespace Aequus.Old.Content.Weapons.Glimmer.StariteMinion;

public class StariteMinion : UnifiedModMinion {
    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 18;
    }

    public override void SetDefaults() {
        base.SetDefaults();
        Projectile.width = 28;
        Projectile.height = 28;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 12;
    }

    public override bool? CanDamage() {
        return Projectile.ai[2] >= 30f;
    }

    public override bool MinionContactDamage() {
        return true;
    }

    public override bool? CanCutTiles() {
        return false;
    }

    private void EmitParticles() {
        if (Main.GameUpdateCount % 10 == 0) {
            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, Main.tenthAnniversaryWorld ? DustID.Enchanted_Pink : DustID.MagicMirror, Scale: Main.rand.NextFloat(1f, 2f));
            d.velocity = Projectile.velocity * 0.2f;
            d.noGravity = true;
        }
        if (Main.GameUpdateCount % 35 == 0) {
            Gore.NewGoreDirect(Projectile.GetSource_FromThis(), Projectile.position + new Vector2(Main.rand.Next(Projectile.width - 4), Main.rand.Next(Projectile.height - 4)), new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)), 16 + Main.rand.Next(2));
        }
    }

    public override void AI() {
        base.AI();
        Player player = Main.player[Projectile.owner];
        AequusPlayer aequus = player.GetModPlayer<AequusPlayer>();
        Vector2 center = Projectile.Center;
        Projectile.CollideWithOthers();

        if ((Projectile.Center - Main.player[Projectile.owner].Center).Length() > 2000f) {
            Projectile.Center = Main.player[Projectile.owner].Center + Main.rand.NextVector2Square(-2f, 2f);
            Projectile.netUpdate = true;
        }
        if (Projectile.velocity.HasNaNs() || Projectile.Center.HasNaNs()) {
            Projectile.velocity = Vector2.One;
            Projectile.Center = Main.player[Projectile.owner].position + Main.rand.NextVector2Square(-2f, 2f);
            Projectile.netUpdate = true;
        }
        EmitParticles();
        Lighting.AddLight(Projectile.Center, new Vector3(0.2f, 0.2f, 0.1f));
        Projectile.rotation = Projectile.velocity.ToRotation();

        if (Projectile.ai[2] < 30f) {
            Projectile.ai[2]++;
        }

        if (!Projectile.tileCollide) {
            var difference = Main.player[Projectile.owner].Center - Projectile.Center;
            if (difference.Length() < 10f || Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, player.position, player.width, player.height)) {
                Projectile.tileCollide = true;
                Projectile.netUpdate = true;
            }
            Projectile.velocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity, difference, 0.1f)) * Math.Max(6f, Projectile.velocity.Length());
            return;
        }

        int target = -1;
        float dist = 800f;
        if (player.HasMinionAttackTargetNPC) {
            var difference = Projectile.Center - center;
            if ((float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y) < dist && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, Projectile.position, Projectile.width, Projectile.height)) {
                target = player.MinionAttackTargetNPC;
            }
        }
        if (target == -1) {
            target = Projectile.FindTargetWithLineOfSight(650f);
        }

        var gotoPosition = target == -1 ? player.Center : Main.npc[target].Center;
        float speed = 13f;
        if ((int)Projectile.ai[0] == 0f) {
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(gotoPosition) * speed, 0.08f);
            if (Projectile.Distance(gotoPosition) < 64f) {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }
        }
        else if ((int)Projectile.ai[0] == 1f) {
            Projectile.velocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(gotoPosition) * speed, 0.15f)) * MathHelper.Lerp(Projectile.velocity.Length(), speed, 0.1f);
            Projectile.ai[1]++;
            if (Projectile.ai[1] > 30f) {
                Projectile.ai[1] = 0f;
                Projectile.ai[0] = 0f;
                Projectile.netUpdate = true;
            }
        }

        if (target == -1) {
            if (Projectile.tileCollide && !Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, player.position, player.width, player.height)) {
                Projectile.tileCollide = false;
                Projectile.netUpdate = true;
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        Projectile.ai[0] = 1f;
        int dustAmount = hit.SourceDamage / 10;
        if (dustAmount < 1) {
            dustAmount = 1;
        }
        if (hit.Crit) {
            dustAmount *= 2;
        }

        target.AddBuff(ModContent.BuffType<BlueFire>(), 240);
        for (int i = 0; i < dustAmount; i++) {
            Dust.NewDustPerfect(target.Center, ModContent.DustType<MonoSparkleDust>(),
                Vector2.UnitX.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi)) * (4f + Main.rand.NextFloat() * 4f), 150, new Color(150, 170, 200, 100)).noGravity = true;
        }
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
        fallThrough = true;
        return true;
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        if (Projectile.velocity.X != oldVelocity.X) {
            if (Math.Abs(oldVelocity.X) > 2f) {
                Projectile.velocity.X = -oldVelocity.X * 0.8f;
            }

            Projectile.localAI[0] *= -0.8f;
        }
        if (Projectile.velocity.Y != oldVelocity.Y) {
            if (Math.Abs(oldVelocity.Y) > 2f) {
                Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
            }

            Projectile.localAI[1] *= -0.8f;
        }
        return false;
    }

    public override Color? GetAlpha(Color lightColor) {
        return new Color(255, 255, 255, 250);
    }

    private void DrawTrail() {
        if (Projectile.isAPreviewDummy) {
            return;
        }

        Color trailStartColor = Main.tenthAnniversaryWorld ? Color.LightPink with { A = 0 } : Color.LightCyan with { A = 0 };
        Color trailEndColor = Main.tenthAnniversaryWorld ? Color.Pink with { A = 0 } : Color.Blue with { A = 0 };

        DrawHelper.DrawBasicVertexLineWithProceduralPadding(AequusTextures.Trail, Projectile.oldPos, Projectile.oldRot,
            p => Color.Lerp(trailStartColor, trailEndColor, 1f - MathF.Pow(1f - p, 2f)) * (1f - p),
            p => 12f * (1f - p),
            -Main.screenPosition + Projectile.Size / 2f);
    }

    public override bool PreDraw(ref Color lightColor) {
        DrawTrail();

        var texture = TextureAssets.Projectile[Type].Value;
        Rectangle frame = new Rectangle(0, 0, texture.Width, texture.Height);
        Vector2 offset = new Vector2(Projectile.width / 2, Projectile.height / 2);
        var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        var color = Main.tenthAnniversaryWorld ? Color.DeepPink with { A = 40 } * 0.4f : new Color(35, 80, 150, 40);
        var origin = frame.Size() / 2f;

        float time = Main.GameUpdateCount / 240f + Main.GlobalTimeWrappedHourly * 0.04f;
        float timer = Main.GlobalTimeWrappedHourly % 5f / 2.5f;
        if (timer >= 1f) {
            timer = 2f - timer;
        }
        timer = timer * 0.5f + 0.5f;

        float rotation = Main.GlobalTimeWrappedHourly * 5f;
        for (float f = 0f; f < 1f; f += 0.25f) {
            Main.EntitySpriteDraw(texture, Projectile.position + offset - Main.screenPosition + new Vector2(0f, 8f).RotatedBy((f + time) * (MathHelper.Pi * 2f)) * timer, null, new Color(30, 30, 80, 50), rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        }
        for (float f = 0f; f < 1f; f += 0.34f) {
            Main.EntitySpriteDraw(texture, Projectile.position + offset - Main.screenPosition + new Vector2(0f, 4f).RotatedBy((f + time) * (MathHelper.Pi * 2f)) * timer, null, new Color(80, 80, 180, 127), rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        }

        Main.EntitySpriteDraw(texture, Projectile.position + offset - Main.screenPosition, null, new Color(255, 255, 255, 255), rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        Main.EntitySpriteDraw(texture, Projectile.position + offset - Main.screenPosition, null, new Color(255, 255, 255, 0) * Helper.Oscillate(Main.GlobalTimeWrappedHourly * 6f, 0f, 0.5f), rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        return false;
    }

    internal override InstancedMinionBuff CreateMinionBuff() {
        return new InstancedMinionBuff(this, (p) => ref p.GetModPlayer<AequusPlayer>().minionStarite);
    }
}