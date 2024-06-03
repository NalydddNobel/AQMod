using Aequus.Common;
using Aequus.Content.Dusts;
using Aequus.Old.Content.Necromancy.Sceptres.Evil;
using System;
using System.IO;
using Terraria.Audio;
using Terraria.GameContent;

namespace Aequus.Old.Content.Necromancy.Sceptres.Dungeon;

public class InsurgencyProj : CorruptionSceptreProj {
    public int homingDelay;

    public override void SetStaticDefaults() {
        Main.projFrames[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 15;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        base.SetStaticDefaults();
    }

    public override void SetDefaults() {
        base.SetDefaults();
        Projectile.width = 60;
        Projectile.height = 60;
        Projectile.penetrate = -1;
        Projectile.alpha = 250;
        Projectile.scale = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 6;
        Projectile.DamageType = Commons.NecromancyClass;

        primDistance = 8f;
        InitTrail();
    }

    public override Color? GetAlpha(Color lightColor) {
        return new Color(90, 255, 255 - (int)Helper.Oscillate(Main.GlobalTimeWrappedHourly * 10f, 0, 120), 200);
    }

    public override void AI() {
        if (Main.netMode != NetmodeID.Server) {
            UpdateTrail();
        }

        if ((int)Projectile.ai[0] == 2) {
            Projectile.velocity = Vector2.Zero;

            int npc = (int)Projectile.ai[1];
            if (npc > -1) {
                if (!Main.npc[npc].active) {
                    Projectile.ai[1] = -2f;
                }
                else {
                    Projectile.Center = Main.npc[npc].Center;
                }
            }
            if (Projectile.alpha < 40) {
                Projectile.scale -= 0.2f;
            }
            else {
                float add = Projectile.Opacity / 20f;
                if (Projectile.alpha < 60) {
                    add /= 2f;
                }
                Projectile.scale += 0.02f + add;
            }
            Projectile.alpha -= 5;
            if (Projectile.alpha < 128 && Main.netMode != NetmodeID.Server && Main.rand.NextBool()) {
                var normal = Main.rand.NextVector2Unit();
                float distance = Projectile.width * Projectile.scale;
                var d = Dust.NewDustPerfect(Projectile.Center + normal * distance, ModContent.DustType<MonoDust>(), -normal * distance / 16f,
                    newColor: new Color(255 - Projectile.alpha, 255, 255 - Main.rand.Next((int)(120 * (1f - Projectile.Opacity))), 100));
            }
            if (Projectile.alpha <= 0) {
                Projectile.alpha = 0;
                if (Main.myPlayer == Projectile.owner) {
                    SpawnExtraSouls();
                }
                Projectile.Kill();
            }
            Projectile.frame = 1;
            Projectile.rotation = 0f;
            Projectile.tileCollide = false;
            return;
        }

        if ((int)Projectile.ai[0] == 1) {
            Projectile.velocity *= 0.6f;
            Projectile.alpha += 15;
            if (Projectile.alpha > 255) {
                Projectile.scale -= 0.8f;
                Projectile.alpha = 255;
                if (Projectile.ai[1] != -1f) {
                    Projectile.ai[0] = 2f;
                }
                else {
                    Projectile.Kill();
                }
                Projectile.frame = 1;
                Projectile.rotation = 0f;
            }
            Projectile.tileCollide = false;
            return;
        }

        if (Projectile.alpha > 0) {
            Projectile.alpha -= 40;
            if (Projectile.alpha < 0) {
                Projectile.alpha = 0;
            }
        }
        Projectile.ai[1] = -1f;

        if (homingDelay < 20) {
            homingDelay++;
            if (homingDelay == 20) {
                Projectile.netUpdate = true;
            }
        }
        else {
            int target = Projectile.FindTargetWithLineOfSight(600f);
            if (target != -1) {
                float speed = Projectile.velocity.Length();
                Projectile.velocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[target].Center) * speed, 0.125f)) * speed;
            }
        }

        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
        if (Projectile.spriteDirection == -1) {
            Projectile.rotation += MathHelper.Pi;
        }
        Projectile.frame = 0;
    }
    public void SpawnExtraSouls() {
        var center = Projectile.Center;
        int count = 1;
        var source = Projectile.GetSource_Death("Aequus:InsurgentSpawn");
        int collisionChecks = 0;
        int distance = 20;
        if (Projectile.ai[1] > 0f) {
            distance = (int)(Main.npc[(int)Projectile.ai[1]].Size.Length() / 2f);
        }
    RecalcShootDir:
        var normal = Main.rand.NextVector2Unit();
        if (Collision.SolidCollision(Projectile.Center + normal * distance - new Vector2(5f), 10, 10)) {
            collisionChecks++;
            if (collisionChecks > 50) {
                goto ShootAtEnemies;
            }
            goto RecalcShootDir;
        }
        var p = Projectile.NewProjectileDirect(source, Projectile.Center + normal * distance, normal * 10f, ModContent.ProjectileType<InsurgentBolt>(), 0, Projectile.knockBack, Projectile.owner, 0f, Projectile.ai[1]);
    ShootAtEnemies:
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (Main.npc[i].active && Main.npc[i].whoAmI != (int)Projectile.ai[1] && Main.npc[i].CanBeChasedBy(Projectile) && Projectile.Distance(Main.npc[i].Center) < 600f) {
                count++;
                normal = Vector2.Normalize(Main.npc[i].Center - Projectile.Center).RotatedBy(Main.rand.NextFloat(-0.025f, 0.025f));
                p = Projectile.NewProjectileDirect(source, Projectile.Center + normal * distance, normal * 10f, ModContent.ProjectileType<InsurgentBolt>(), 0, Projectile.knockBack, Projectile.owner, 0f, Projectile.ai[1]);
                if (count >= 4) {
                    break;
                }
            }
        }
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
        width = 10;
        height = 10;
        fallThrough = true;
        return true;
    }

    public void OnHit(Entity target) {
        Projectile.damage = 0;
        Projectile.ai[0] = 1f;
        Projectile.ai[1] = target.whoAmI;
        Projectile.alpha = 0;
        Projectile.netUpdate = true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;

        NecromancyDebuff.Apply<InsurgentDebuff>(target, 600, Projectile.owner);
        OnHit(target);
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info) {
        OnHit(target);
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        Projectile.damage = 0;
        Projectile.ai[0] = 1f;
        Projectile.ai[1] = -1f;
        Projectile.velocity = oldVelocity;
        Projectile.netUpdate = true;
        return false;
    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(homingDelay);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        homingDelay = reader.ReadInt32();
    }

    public override bool PreDraw(ref Color lightColor) {
        var texture = TextureAssets.Projectile[Type].Value;
        var frame = Projectile.Frame();
        var origin = frame.Size() / 2f;
        var effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        var drawColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;

        primScale = 4f;
        primColor = drawColor with { A = 0 } * 0.7f;
        if ((int)Projectile.ai[0] != 2) {
            //DrawHelper.SpriteBatchCache.Inherit(Main.spriteBatch);
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            DrawHelper.DrawBasicVertexLineWithProceduralPadding(AequusTextures.TrailShootingStar, Projectile.oldPos, Projectile.oldRot,
                (p) => new Color(100, 200, 150, 0) * 0.5f * Projectile.Opacity * (1f - p),
                (p) => 60f * Projectile.scale * (1f - p),
                Projectile.Size / 2f - Main.screenPosition);
            //DrawHelper.DrawBasicVertexLine(AequusTextures.Trail3, Projectile.oldPos, Projectile.oldRot,
            //    (p) => new Color(100, 200, 150, 0) * Projectile.Opacity * (1f - p),
            //    (p) => 12f * Projectile.scale * (1f - p),
            //    Projectile.Size / 2f - Main.screenPosition);
            DrawTrail(maxLength: ProjectileID.Sets.TrailCacheLength[Type] / 3 * 2);

            //Main.spriteBatch.End();
            //DrawHelper.SpriteBatchCache.Begin(Main.spriteBatch);
        }

        Texture2D bloom = AequusTextures.Bloom;
        Vector2 drawCoordinates = Projectile.Center - Main.screenPosition;
        Main.EntitySpriteDraw(bloom, drawCoordinates, null, drawColor * 0.5f, Projectile.rotation, bloom.Size() / 2f, Projectile.scale * 0.75f, effects, 0);
        Main.EntitySpriteDraw(texture, drawCoordinates, frame, drawColor, Projectile.rotation, origin, Projectile.scale, effects, 0);
        return false;
    }

    public override void OnKill(int timeLeft) {
        if (Main.netMode != NetmodeID.Server && Projectile.alpha < 128) {
            var center = Projectile.Center;
            int amt = 2;
            if (Projectile.ai[1] != -1f) {
                amt += 8;
                SoundEngine.PlaySound(SoundID.NPCDeath52 with { Volume = 0.9f, Pitch = -0.2f }, Projectile.Center);
            }
            for (int i = 0; i < amt; i++) {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SilverFlame, newColor: new Color(90, 255, 255 - Main.rand.Next(120), 100));
                d.velocity *= 0.2f;
                d.velocity += (d.position - center) / 8f;
                d.scale += Main.rand.NextFloat(-0.5f, 0f);
                d.fadeIn = d.scale + Main.rand.NextFloat(0.6f, 1f);
                d.noGravity = true;
            }
            for (int i = 0; i < amt * 3; i++) {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SilverFlame, newColor: new Color(90, 255, 255 - Main.rand.Next(120), 100));
                d.velocity *= 0.2f;
                d.velocity += (d.position - center) / 8f;
                d.scale += Main.rand.NextFloat(-0.1f, 0.2f);
                d.noGravity = true;
            }
        }
    }
}
