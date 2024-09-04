using Aequus.Content.Items.Weapons.RangedBows.SkyHunterCrossbow;
using Aequus.Projectiles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.Audio;

namespace Aequus.Content.Villagers.SkyMerchant;

public class SkyMerchantProjectile : ModProjectile {
    public override string Texture => AequusTextures.Projectile(ProjectileID.WoodenArrowFriendly);

    private bool _playedSound;
    public bool retreat;
    public int animationTimer;
    public bool _playedRetreatSound;

    public override void SetDefaults() {
        Projectile.CloneDefaults(ProjectileID.WoodenArrowFriendly);
        AIType = ProjectileID.WoodenArrowFriendly;
        Projectile.hide = true;
        Projectile.npcProj = true;
        Projectile.penetrate = -1;
    }

    public override bool PreAI() {
        if (animationTimer != -1 && !retreat) {
            animationTimer++;
        }
        if (!_playedSound) {
            SoundEngine.PlaySound(AequusSounds.CrossbowShoot with { Volume = 0.4f, PitchVariance = 0.2f });
            _playedSound = true;
        }

        if (!Projectile.TryGetGlobalProjectile(out AequusProjectile projectileSource) || !projectileSource.HasNPCOwner) {
            Projectile.Kill();
            return true;
        }

        var difference = Main.npc[projectileSource.sourceNPC].Center - Projectile.Center;
        float distance = difference.Length();
        if (distance > SkyHunterCrossbow.MaximumDistance) {
            retreat = true;
            Projectile.netUpdate = true;
        }
        if (animationTimer == -1) {
            retreat = true;
        }

        if (!retreat) {
            return true;
        }
        if (animationTimer > 0) {
            animationTimer = -1;
        }

        if (!_playedRetreatSound) {
            SoundEngine.PlaySound(AequusSounds.RopeRetract with { Volume = 1.2f, PitchVariance = 0.2f }, Projectile.Center);
            _playedRetreatSound = true;
        }

        float speed = Math.Max(Main.npc[Projectile.owner].velocity.Length() * 2f, 60f) / Projectile.MaxUpdates;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.timeLeft = Math.Clamp(Projectile.timeLeft, 22, 44);

        if (Projectile.numUpdates == -1) {
            if (animationTimer == -1) {
                Projectile.velocity /= Projectile.MaxUpdates;
                Projectile.netUpdate = true;
            }
            animationTimer--;
            Projectile.rotation = Projectile.rotation.AngleTowards(difference.ToRotation() - MathHelper.PiOver2, 0.02f);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(difference) * speed, MathF.Min(-animationTimer * 0.01f, 1f));
        }
        else {
            Projectile.timeLeft++;
        }
        if (distance < 32f) {
            if (Main.netMode == NetmodeID.Server) {
                if (Main.netMode != NetmodeID.SinglePlayer) {
                    NetMessage.SendData(MessageID.KillProjectile, -1, -1, null, Projectile.identity, Projectile.owner);
                }
            }
            Projectile.active = false;
            SoundEngine.PlaySound(AequusSounds.CrossbowReload with { Volume = 0.8f, PitchVariance = 0.1f }, Projectile.Center);
        }
        return false;
    }

    public override void AI() {
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        Projectile.tileCollide = false;
        animationTimer = -4;
        Projectile.netUpdate = true;
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        Projectile.tileCollide = false;
        animationTimer = -4;
        Projectile.netUpdate = true;
        return false;
    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(retreat);
        writer.Write(animationTimer);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        retreat = reader.ReadBoolean();
        animationTimer = reader.ReadInt32();
    }

    public override bool PreDraw(ref Color lightColor) {
        if (!Projectile.TryGetGlobalProjectile(out AequusProjectile projectileSource) || !projectileSource.HasNPCOwner) {
            return false;
        }

        SkyHunterCrossbow.DrawChain(Projectile.Center, Main.npc[projectileSource.sourceNPC].Center, Projectile.Opacity, animationTimer, Main.npc[projectileSource.sourceNPC].type, Projectile.timeLeft);
        return true;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
        behindNPCs.Add(index);
    }
}
