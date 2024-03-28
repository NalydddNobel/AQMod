using Terraria.DataStructures;

namespace Aequus.Old.Content.Equipment.Accessories.WarHorn;

public class WarHornMinionGlobalProjectile : GlobalProjectile {
    private static bool _loop;

    public ushort warHornFrenzyTime;

    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) {
        return entity.minion || entity.sentry;
    }

    public override void AI(Projectile projectile) {
        if (_loop) {
            return;
        }

        if (warHornFrenzyTime > 0) {
            if (Main.rand.NextBool(2 * projectile.MaxUpdates)) {
                var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height,
                    DustID.RedTorch, projectile.velocity.X, projectile.velocity.Y, Scale: Main.rand.NextFloat(0.8f, 1.5f));
                d.noGravity = true;
                d.fadeIn = d.scale + 0.2f;
                d.noLightEmittence = true;
            }
            warHornFrenzyTime--;

            _loop = true;
            projectile.AI();
            _loop = false;
        }
    }

    public override bool PreDraw(Projectile projectile, ref Color lightColor) {
        if (warHornFrenzyTime == 0) {
            return true;
        }

        float frenzyOpacity = warHornFrenzyTime < 60 ? warHornFrenzyTime / 60f : 1f;

        var texture = AequusTextures.WarHornEffect.Value;

        var color = Color.Red;
        var drawCoords = projectile.Center - Main.screenPosition;
        var textureOrigin = texture.Size() / 2f;
        float scale = projectile.scale * 0.5f;
        int swishTimeMax = 20;
        int swishTime = warHornFrenzyTime % swishTimeMax;
        float swishOpacity = frenzyOpacity;
        if (swishTime < 8) {
            swishOpacity *= swishTime / 8f;
        }
        else if (swishTime > swishTimeMax - 8) {
            swishOpacity *= 1f - (swishTimeMax - swishTime) / 8f;
        }

        Main.EntitySpriteDraw(AequusTextures.BloomStrong, drawCoords, null, color, 0f, AequusTextures.BloomStrong.Size() / 2f, scale, SpriteEffects.None, 0);
        for (int i = -1; i <= 1; i += 2) {
            Main.EntitySpriteDraw(texture, drawCoords + new Vector2(i * projectile.Frame().Width * (1f - warHornFrenzyTime % swishTimeMax / (float)swishTimeMax), 0f), null, color with { A = 128 } * swishOpacity, MathHelper.PiOver2 * i, textureOrigin, scale * 0.5f, SpriteEffects.None, 0);
        }

        return true;
    }
}

public class WarHornActivatorGlobalProjectile : GlobalProjectile {
    private int _sourceMinion = -1;

    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) {
        return entity.minion || entity.sentry || ProjectileID.Sets.MinionShot[entity.type] || ProjectileID.Sets.SentryShot[entity.type];
    }

    public override void AI(Projectile projectile) {
        if (_sourceMinion != -1) {
            int proj = ExtendProjectile.FindProjectileIdentity(projectile.owner, _sourceMinion);
            if (proj == -1 || !Main.projectile[proj].active || Main.projectile[proj].owner != projectile.owner || !Main.projectile[proj].TryGetGlobalProjectile<WarHornMinionGlobalProjectile>(out _)) {
                _sourceMinion = -1;
            }
        }
    }

    public override void OnSpawn(Projectile projectile, IEntitySource source) {
        if (source is EntitySource_Parent parent && parent.Entity is Projectile parentProjectile) {
            if (parentProjectile.TryGetGlobalProjectile(out WarHornActivatorGlobalProjectile parentActivator) && parentActivator._sourceMinion != -1) {
                _sourceMinion = parentActivator._sourceMinion;
            }
            else {
                _sourceMinion = parentProjectile.identity;
            }
        }
    }

    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone) {
        if (target.SpawnedFromStatue || target.immortal 
            || !Main.player[projectile.owner].TryGetModPlayer(out AequusPlayer aequusPlayer)
            || aequusPlayer.accWarHorn <= 0 || Main.player[projectile.owner].HasBuff<WarHornCooldown>()) {
            return;
        }
        ActivateWarhorn(projectile);
    }

    public void ActivateWarhorn(Projectile projectile) {
        int proj;
        if (projectile.minion || projectile.sentry) {
            proj = projectile.whoAmI;
        }
        else {
            proj = ExtendProjectile.FindProjectileIdentity(projectile.owner, _sourceMinion);
        }

        if (proj != -1 && Main.projectile[proj].TryGetGlobalProjectile(out WarHornMinionGlobalProjectile minion)) {
            Main.player[Main.projectile[proj].owner].AddBuff(ModContent.BuffType<WarHornCooldown>(), 600);
            if (minion.warHornFrenzyTime <= 30) {
                // Play Sound
            }
            minion.warHornFrenzyTime = (ushort)(240 * Main.player[projectile.owner].GetModPlayer<AequusPlayer>().accWarHorn);
            for (int i = 0; i < 20; i++) {
                var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height,
                    DustID.RedTorch, Scale: Main.rand.NextFloat(0.8f, 1.5f));
                d.noGravity = true;
                d.velocity *= 5f;
                d.fadeIn = d.scale + 0.2f;
                d.noLightEmittence = true;
            }
            Main.projectile[proj].netUpdate = true;
        }
    }
}
