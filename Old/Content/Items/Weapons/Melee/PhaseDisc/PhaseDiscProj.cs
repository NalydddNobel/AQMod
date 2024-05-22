using Aequus.DataSets;
using Aequus.Old.Content.Items.Weapons.Melee.Valari;
using Aequus.Old.Content.Particles;
using System;
using Terraria.Audio;
using Terraria.GameContent;

namespace Aequus.Old.Content.Items.Weapons.Melee.PhaseDisc;

public class PhaseDiscProj : ValariProj {
    public bool IsIce => (int)Projectile.ai[1] == 1;
    public Color EffectColor => Projectile.frame == 0 ? new(30, 120, 190, 0) : new(255, 70, 30, 0);

    public override string Texture => BaseTexture;

    public override void SetStaticDefaults() {
        Main.projFrames[Type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileDataSet.PushableByTypeId.Add(Type);
    }

    public override void SetDefaults() {
        Projectile.width = 142;
        Projectile.height = 142;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.manualDirectionChange = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 1200;
        Projectile.extraUpdates = 2;
        Projectile.alpha = 255;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 12;
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.White;
    }

    public override void AI() {
        if (Projectile.ai[1] > 0f && !IsIce) {
            //Projectile.GetGlobalProjectile<>().heatDamage = false;
            Projectile.coldDamage = false;
        }
        else if (Projectile.ai[1] > 0f) {
            //Projectile.GetGlobalProjectile<>().heatDamage = false;
            Projectile.coldDamage = true;
        }
        else if (!IsIce) {
            //Projectile.GetGlobalProjectile<>().heatDamage = true;
            Projectile.coldDamage = false;
        }
        var hitbox = Projectile.Hitbox;
        hitbox.Inflate(16, 16);
        for (int i = 0; i < Main.maxItems; i++) {
            if (Main.item[i] != null && Main.item[i].active && Main.item[i].noGrabDelay <= 0 && Main.player[Projectile.owner].ItemSpace(Main.item[i]).CanTakeItem && Main.item[i].Hitbox.Intersects(hitbox)) {
                if (Main.item[i].Distance(Projectile.Center) < 10f || Projectile.ai[0] > 100f) {
                    Main.item[i].Center = Projectile.Center;
                    Main.item[i].velocity = Projectile.velocity;
                }
                else {
                    Main.item[i].velocity = Vector2.Lerp(Main.item[i].velocity,
                        Main.item[i].DirectionTo(Projectile.Center) * Math.Max(16f, Projectile.velocity.Length()), 0.2f);
                }
                Main.timeItemSlotCannotBeReusedFor[i] = 2;
                if (!Main.item[i].instanced && Main.netMode == NetmodeID.Server) {
                    NetMessage.SendData(MessageID.SyncItem, number: i);
                }
            }
        }
        if (Projectile.localAI[1] > 0.6f && Main.rand.NextBool(2)) {
            var c = EffectColor;

            var p = LegacyBloomParticle.New();
            p.Location = Projectile.Center + (Projectile.rotation - MathHelper.PiOver2 + (MathHelper.PiOver4 + 0.5f + Main.rand.NextFloat(-1f, 0f)) * Projectile.direction).ToRotationVector2() *
                Projectile.Size * 0.5f * Main.rand.NextFloat(0.5f, 1.1f);
            p.Velocity = Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(2f, 6f) * Projectile.direction;
            p.Color = (c * Main.rand.NextFloat(1f, 3f)) with { A = 120 };
            p.BloomColor = c * 0.2f;
            p.Scale = Main.rand.NextFloat(0.5f, 2f) * Projectile.localAI[1];
            p.BloomScale = Main.rand.NextFloat(0.1f, 0.4f);
        }
        Lighting.AddLight(Projectile.Center, new Vector3(0.5f, 0.4f, 0.5f) * Projectile.localAI[1]);
        Projectile.rotation += 0.125f * Projectile.direction;
        if (Projectile.alpha > 0) {
            Projectile.alpha -= 20;
            if (Projectile.alpha <= 0)
                Projectile.alpha = 0;
        }
        if (Projectile.ai[1] > 1f) {
            Projectile.rotation += 0.02f * Projectile.direction;
            if (Projectile.ai[0] < 70f) {
                int target = Projectile.FindTargetWithLineOfSight(500f);
                var targetLocation = target == -1 ? Main.MouseWorld : Main.npc[target].Center;
                if (target == -1) {
                    Projectile.netUpdate = true;
                }
                if (target != -1 || Projectile.owner == Main.myPlayer) {
                    Projectile.velocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity, targetLocation - Projectile.Center, 0.01f)) * Projectile.velocity.Length();
                }
            }
            if (Projectile.ai[1] > 2f) {
                int identity = (int)(Projectile.ai[1] - 3);
                int proj = ExtendProjectile.FindProjectileIdentity(Projectile.owner, identity);
                if (proj != -1) {
                    Projectile.rotation = Main.projectile[proj].rotation + MathHelper.Pi;
                    Projectile.Center = Main.projectile[proj].Center;
                    Projectile.velocity *= 0.5f;
                }
            }
        }
        else {
            Projectile.frame = IsIce ? 0 : 1;
        }
        Projectile.ai[0]++;
        if (Projectile.ai[0] > 70f) {
            float speed = Math.Max((Main.player[Projectile.owner].Center - Projectile.Center).Length() / 60f, 10f) + Projectile.ai[0] * 0.0003f;
            var l = (Projectile.Center - Main.player[Projectile.owner].Center).Length();
            if (l < 500f) {
                Projectile.localAI[1] -= 0.01f;
                if (Projectile.localAI[1] < 0f) {
                    Projectile.localAI[1] = 0f;
                }
                speed *= 1.5f;
            }
            Projectile.tileCollide = false;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Main.player[Projectile.owner].Center - Projectile.Center) * speed, Math.Max(1f - (Main.player[Projectile.owner].Center - Projectile.Center).Length() / 40f, 0.04f));
            if (l < 20f) {
                Projectile.Kill();
            }
            return;
        }
        if (Projectile.ai[0] > 46f) {
            Projectile.velocity *= 0.95f;
        }
        Projectile.localAI[1] += 0.02f;
        if (Projectile.localAI[1] > 1f) {
            Projectile.localAI[1] = 1f;
        }
        hitbox = Projectile.Hitbox;
        for (int i = 0; i < Main.maxProjectiles; i++) {
            if (Main.projectile[i].active && i != Projectile.whoAmI && Main.projectile[i].type == Type && Projectile.owner == Main.projectile[i].owner
                && (int)Main.projectile[i].ai[1] < 2 && Projectile.Colliding(hitbox, Main.projectile[i].getRect())) {

                if ((int)Main.projectile[i].ai[1] != (int)Projectile.ai[1] && Projectile.Distance(Main.player[Projectile.owner].Center) > Main.projectile[i].Distance(Main.player[Projectile.owner].Center)) {
                    Projectile.ai[0] -= 5f;
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 10f;
                    Projectile.ai[1] = 2f;
                    Projectile.timeLeft = 2400;
                    Projectile.damage *= 2;
                    Projectile.netUpdate = true;

                    Main.projectile[i].damage *= 2;
                    Main.projectile[i].ai[0] = Projectile.ai[0];
                    Main.projectile[i].ai[1] = 3f + Projectile.identity;
                    Main.projectile[i].rotation = Projectile.rotation + MathHelper.Pi;
                    Main.projectile[i].direction = Projectile.direction;
                    Main.projectile[i].timeLeft = 2400;
                    Main.projectile[i].velocity = Vector2.Zero;
                    Main.projectile[i].netUpdate = true;

                    SoundEngine.PlaySound(AequusSounds.PowerReady with { Volume = 0.5f }, Projectile.Center);
                    return;
                }
            }
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
        Collision.HitTiles(Projectile.position + new Vector2(Projectile.width / 8f, Projectile.height / 8f), oldVelocity, Projectile.width / 5, Projectile.height / 4);
        if (Projectile.velocity.X != oldVelocity.X)
            Projectile.velocity.X = -oldVelocity.X;
        if (Projectile.velocity.Y != oldVelocity.Y)
            Projectile.velocity.Y = -oldVelocity.Y;
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        Projectile.damage = (int)(Projectile.damage * 0.5f);
        if (Projectile.ai[1] > 0f) {
            target.AddBuff(BuffID.Frostburn2, 120);
        }
        if (!IsIce) {
            target.AddBuff(BuffID.OnFire3, 120);
        }
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info) {
        if (Projectile.ai[1] > 0f) {
            target.AddBuff(BuffID.Frostburn2, 120);
        }
        if (!IsIce) {
            target.AddBuff(BuffID.OnFire3, 120);
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        var texture = TextureAssets.Projectile[Type].Value;
        Main.instance.LoadProjectile(ProjectileID.NightsEdge);
        Projectile.GetDrawInfo(out var _, out var off, out var _, out var origin, out int _);
        origin.Y *= 2f;
        var frame = texture.Frame(verticalFrames: 2, frameY: Projectile.frame);
        var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        var drawPosition = Projectile.position + off - Main.screenPosition;
        Color color = EffectColor;
        Main.spriteBatch.Draw(AequusTextures.Bloom, drawPosition,
            null, color * Projectile.Opacity * 0.6f, 0f, AequusTextures.Bloom.Size() / 2f, Projectile.scale * 0.7f, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(texture, drawPosition,
            frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

        var slash = TextureAssets.Projectile[ProjectileID.NightsEdge].Value;
        var slashFrame = slash.Frame(verticalFrames: 4);
        float slashScale = MathF.Pow(1f * Projectile.scale * Projectile.localAI[1], 2f);
        var slashOrigin = slashFrame.Size() / 2f;
        var slashColor = color * Projectile.Opacity * Projectile.localAI[1];
        for (int i = -2; i <= 2; i++) {
            Main.spriteBatch.Draw(
                slash,
                drawPosition,
                slashFrame.Frame(0, i == 0 ? 0 : 2),
                slashColor * (i == 0 ? 1f : 0.33f),
                Projectile.rotation + i * 0.3f,
                slashOrigin,
                slashScale,
                SpriteEffects.None, 0f
            );
            Main.spriteBatch.Draw(
                slash,
                drawPosition,
                slashFrame.Frame(0, 3),
                slashColor,
                Projectile.rotation + i * 0.3f,
                slashOrigin,
                slashScale,
                SpriteEffects.None, 0f
            );
        }
        return false;
    }
}