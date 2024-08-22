﻿using Aequus.Common.Projectiles.SentryChip;
using System;
using System.Collections.Generic;
using System.IO;

namespace Aequus.Content.Items.Accessories.CelesteTorus;

public class CelesteTorusProj : ModProjectile {
    public Vector3 rotation;
    public Vector3 rotation2;
    public bool show2ndRing;
    public float currentRadius;
    public Rectangle[] _hitboxesCache;
    public int hitboxesCount;

    public override void SetDefaults() {
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.hide = true;
        Projectile.netImportant = true;
        Projectile.ignoreWater = true;
        Projectile.manualDirectionChange = true;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 20;
        _hitboxesCache = new Rectangle[13];
    }

    public override bool? CanCutTiles() {
        return false;
    }

    public override bool? CanHitNPC(NPC target) {
        return NPCID.Sets.CountsAsCritter[target.type] ? false : null;
    }

    private void UpdateHitboxes() {
        hitboxesCount = 5;
        for (int i = 0; i < 5; i++) {
            var pos = GetRot(i, rotation, currentRadius);
            var collisionCenter = Projectile.Center + new Vector2(pos.X, pos.Y);
            _hitboxesCache[i] = Utils.CenteredRectangle(collisionCenter, new Vector2(Projectile.width, Projectile.width));
        }
        if (show2ndRing) {
            hitboxesCount += 8;
            for (int i = 0; i < 8; i++) {
                var pos = GetRot(i, rotation2, currentRadius * 2f, 8);
                var collisionCenter = Projectile.Center + new Vector2(pos.X, pos.Y);
                _hitboxesCache[i + 5] = Utils.CenteredRectangle(collisionCenter, new Vector2(Projectile.width, Projectile.width) * 1.2f);
            }
        }
    }

    public override void AI() {
        int projIdentity = (int)Projectile.ai[0] - 1;
        AequusPlayer aequus;
        if (projIdentity > -1) {
            projIdentity = Helper.FindProjectileIdentity(Projectile.owner, projIdentity);
            if (projIdentity == -1 || !Main.projectile[projIdentity].active || !Main.projectile[projIdentity].TryGetGlobalProjectile<SentryAccessoriesGlobalProj>(out var value)) {
                Projectile.Kill();
                return;
            }

            aequus = value.dummyPlayer?.GetModPlayer<AequusPlayer>();
            Projectile.Center = Main.projectile[projIdentity].Center;
        }
        else {
            aequus = Main.player[Projectile.owner].GetModPlayer<AequusPlayer>();
            Projectile.Center = Main.player[Projectile.owner].Center;
        }
        Projectile.scale = 1f;

        var player = Main.player[Projectile.owner];
        if (!player.active || player.dead || (aequus?.accStariteExpert) == null) {
            return;
        }
        Projectile.timeLeft = 2;

        if (Projectile.active) {
            int damage = player.GetWeaponDamage(aequus.accStariteExpert);
            if (Projectile.damage != damage) {
                if (Projectile.damage < damage) {
                    Projectile.damage = Math.Min(Projectile.damage + 2, damage);
                }
                else {
                    Projectile.damage = Math.Max(Projectile.damage - 10, damage);
                }
            }

            float playerPercent = player.statLife / (float)player.statLifeMax2;
            float gotoRadius = Math.Min((int)((float)Math.Sqrt(player.width * player.height) + 20f + player.wingTimeMax * 0.15f + player.wingTime * 0.15f + (1f - playerPercent) * 90f + player.statDefense), 600);
            currentRadius = MathHelper.Lerp(currentRadius, gotoRadius, 0.1f);
            Projectile.scale *= 0.8f + 0.2f * currentRadius / 100f;

            var center = Projectile.Center;
            bool danger = false;
            for (int i = 0; i < Main.maxNPCs; i++) {
                if (Main.npc[i].CanBeChasedBy(Projectile) && Vector2.Distance(Main.npc[i].Center, center) < 2000f) {
                    danger = true;
                    break;
                }
            }

            if (Main.myPlayer == Projectile.owner && Main.netMode != NetmodeID.SinglePlayer) {
                if (rotation.X.Abs() > MathHelper.TwoPi || rotation.Y.Abs() > MathHelper.TwoPi || rotation.Z.Abs() > MathHelper.TwoPi || Main.GameUpdateCount % 60 == 0) {
                    Projectile.netUpdate = true;
                }
            }

            rotation.X %= MathHelper.TwoPi;
            rotation.Y %= MathHelper.TwoPi;
            rotation.Z %= MathHelper.TwoPi;

            show2ndRing = aequus.accStariteExpert?.GetEquipEmpowerment()?.HasAbilityBoost == true;
            if (danger) {
                rotation.X = rotation.X.AngleLerp(0f, 0.01f);
                rotation.Y = rotation.Y.AngleLerp(0f, 0.0075f);
                rotation.Z += 0.04f + (1f - playerPercent) * 0.0314f;

                rotation2.X = rotation.X.AngleLerp(0f, 0.01f);
                rotation2.Y = rotation.Y.AngleLerp(0f, 0.0075f);
                rotation2.Z += 0.04f + (1f - playerPercent) * 0.0314f;
            }
            else {
                rotation.X += 0.0157f;
                rotation.Y += 0.01f;
                rotation.Z += 0.0314f;

                if (show2ndRing) {
                    rotation2.X += 0.0157f;
                    rotation2.Y += 0.0314f;
                    rotation2.Z += 0.011f;
                }
            }

            Lighting.AddLight(Projectile.Center, new Vector3(0.3f, 0.3f, 0.8f));
            UpdateHitboxes();

            // Create orb draw data.
            // Also really bad, but whatever, its 2 am and im tired.
            if (aequus != null) {
                List<Vector3> orbs = [];

                for (int i = 0; i < 5; i++) {
                    orbs.Add(GetRot(i, rotation, currentRadius, 5));
                }

                if (show2ndRing) {
                    for (int i = 0; i < 8; i++) {
                        orbs.Add(GetRot(i, rotation2, currentRadius * 2f, 8));
                    }
                }

                orbs.Sort((v, v2) => -v.Z.CompareTo(v2.Z));

                aequus.stariteExpertDrawData = new CelesteTorusDrawData(orbs);
            }
        }
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
        for (int i = 0; i < hitboxesCount; i++) {
            if (_hitboxesCache[i].Intersects(targetHitbox)) {
                return true;
            }
        }
        return false;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
        modifiers.HitDirectionOverride = target.position.X < Main.player[Projectile.owner].position.X ? -1 : 1;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        //Projectile.NewProjectile(
        //    Projectile.GetSource_OnHit(target),
        //    Main.rand.NextFromRect(target.Hitbox),
        //    Vector2.Zero,
        //    ModContent.ProjectileType<CelesteTorusOnHitProj>(),
        //    0,
        //    0f,
        //    Projectile.owner
        //);
    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(rotation.X);
        writer.Write(rotation.Y);
        writer.Write(rotation.Z);
        writer.Write(rotation2.X);
        writer.Write(rotation2.Y);
        writer.Write(rotation2.Z);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        rotation.X = reader.ReadSingle();
        rotation.Y = reader.ReadSingle();
        rotation.Z = reader.ReadSingle();
        rotation2.X = reader.ReadSingle();
        rotation2.Y = reader.ReadSingle();
        rotation2.Z = reader.ReadSingle();
    }

    public static Vector3 GetRot(int i, Vector3 rotation, float currentRadius, int max = 5) {
        return Vector3.Transform(new Vector3(currentRadius, 0f, 0f), Matrix.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z + MathHelper.TwoPi / max * i));
    }
}