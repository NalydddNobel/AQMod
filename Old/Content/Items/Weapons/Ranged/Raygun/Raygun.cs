using Aequu2.Core;
using Aequu2.Content.Dusts;
using Aequu2.Core.Entities.Items.Components;
using Aequu2.Core.Entities.Projectiles;
using Aequu2.Core.Particles;
using Aequu2.DataSets;
using System;
using Terraria.DataStructures;

namespace Aequu2.Old.Content.Items.Weapons.Ranged.Raygun;

public class Raygun : ModItem, IManageProjectile {
    public override void SetDefaults() {
        Item.width = 32;
        Item.height = 24;
        Item.DamageType = DamageClass.Ranged;
        Item.SetWeaponValues(32, 4f, 0);
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.noMelee = true;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useAmmo = AmmoID.Bullet;
        Item.shoot = ProjectileID.Bullet;
        Item.shootSpeed = 1.5f;
        Item.autoReuse = true;
        Item.UseSound = Aequu2Sounds.Raygun with { Volume = 0.5f };
        Item.rare = Commons.Rare.BossOmegaStarite;
        Item.value = Commons.Cost.BossOmegaStarite;
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.White;
    }

    public override Vector2? HoldoutOffset() {
        return new Vector2(-4f, -4f);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
    }

    bool IManageProjectile.PreAIProjectile(Projectile projectile) {
        if (!projectile.TryGetGlobalProjectile(out ProjectileItemData item)) { return true; }

        if (item.ItemData == 0) {
            projectile.MaxUpdates *= 12;
        }

        Color color = ProjectileDataSet.GetColor(projectile) with { A = 120 };
        if (item.ItemData % 10 == 5 && Main.netMode != NetmodeID.Server) {
            float scale = 1f;

            Particle<RaygunTrail.Particle>.New().Setup(projectile.Center, projectile.velocity, color, scale);
        }
        if (projectile.Distance(Main.player[projectile.owner].Center) > 20f) {
            Dust d = Dust.NewDustPerfect(projectile.Center, ModContent.DustType<MonoDust>(), newColor: color with { A = 0 } * 0.5f, Scale: 1.25f);
            d.velocity *= 0.5f;
            d.velocity -= projectile.velocity * 0.5f;
        }

        item.ItemData++;

        return true;
    }

    public static void SpawnExplosion(IEntitySource source, Projectile projectile) {
        if (!projectile.AllowSpecialAbilities()) {
            return;
        }

        if (source is EntitySource_OnHit onHit && onHit.Victim is NPC npc) {
            npc.SetIDStaticHitCooldown<RaygunExplosionProj>(10);
        }

        var center = projectile.Center;
        if (Main.netMode != NetmodeID.Server) {
            int amt = (int)(75 * Math.Max(Main.gfxQuality, 0.5f));
            var color = ProjectileDataSet.GetColor(projectile) with { A = 0 };
            for (int i = 0; i < amt; i++) {
                float scale = Main.rand.NextFloat(1f, 3f);
                var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, color, scale);
                var r = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                d.position = center + r * Main.rand.NextFloat(24f);
                float speed = Main.rand.NextFloat(9f, 14f);
                d.velocity = r * (speed - Math.Min(scale * 4f, speed - 0.1f));
            }
        }
        switch (projectile.type) {
            case ProjectileID.CrystalBullet: {
                    for (int i = 0; i < 6; i++) {
                        var r = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                        var explosionPos = projectile.Center + r * Main.rand.NextFloat(16f, 60f);
                        if (Main.netMode != NetmodeID.Server) {
                            int amt = (int)(35 * Math.Max(Main.gfxQuality, 0.5f));
                            var color = ProjectileDataSet.GetColor(projectile).UseA(0) * 0.8f;
                            for (int j = 0; j < amt; j++) {
                                float scale = Main.rand.NextFloat(1f, 3f);
                                var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, color, scale * 0.75f);
                                var r2 = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                                d.position = explosionPos + r2 * Main.rand.NextFloat(6f);
                                float speed = Main.rand.NextFloat(9f, 14f);
                                d.velocity = r2 * (speed - Math.Min(scale * 4f, speed - 0.1f)) * 1.2f;
                            }
                        }
                        if (Main.myPlayer == projectile.owner) {
                            Projectile.NewProjectile(source, explosionPos, Vector2.Normalize(projectile.velocity), ModContent.ProjectileType<RaygunExplosionProj>(), projectile.damage, projectile.knockBack, projectile.owner);
                            Projectile.NewProjectile(source, explosionPos, r * 8f, ProjectileID.CrystalShard, projectile.damage / 10, projectile.knockBack, projectile.owner);
                        }
                    }

                    break;
                }

            case ProjectileID.PartyBullet: {
                    if (Main.myPlayer == projectile.owner) {
                        Projectile.NewProjectile(source, center, Vector2.Normalize(projectile.velocity), ProjectileID.ConfettiGun, 0, 0f, projectile.owner);
                    }

                    break;
                }

            case ProjectileID.ExplosiveBullet: {
                    if (Main.netMode != NetmodeID.Server) {
                        int amt = (int)(175 * Math.Max(Main.gfxQuality, 0.5f));
                        var color = ProjectileDataSet.GetColor(projectile).UseA(0) * 1.2f;
                        for (int j = 0; j < amt; j++) {
                            float scale = Main.rand.NextFloat(0.6f, 2.5f);
                            if (Main.rand.NextBool(4)) {
                                scale *= 1.5f;
                            }
                            var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, color, scale);
                            var r2 = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                            d.position = center + r2 * Main.rand.NextFloat(50f);
                            float speed = Main.rand.NextFloat(9f, 14f);
                            d.velocity = r2 * (speed - Math.Min(scale * 4f, speed - 0.01f)) * 2.15f;
                        }
                    }
                    if (Main.myPlayer == projectile.owner) {
                        for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.PiOver4 + 0.01f) {
                            var r = f.ToRotationVector2();
                            var explosionPos = projectile.Center + r * Main.rand.NextFloat(42f, 68f);
                            Projectile.NewProjectile(source, explosionPos, Vector2.Normalize(projectile.velocity), ModContent.ProjectileType<RaygunExplosionProj>(), projectile.damage / 2, projectile.knockBack, projectile.owner);
                        }
                    }

                    break;
                }
        }
        if (Main.myPlayer == projectile.owner) {
            // A small bit of velocity is given to this explosion projectile to make it knockback enemies in the correct direction
            // I could just override the modify hit methods and manually apply direction there but blah
            Projectile.NewProjectile(source, projectile.Center, Vector2.Normalize(projectile.velocity), ModContent.ProjectileType<RaygunExplosionProj>(), projectile.damage / 2, projectile.knockBack, projectile.owner);
        }
    }

    void IManageProjectile.OnHitNPCProjectile(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone) {
        SpawnExplosion(projectile.GetSource_OnHit(target), projectile);
        projectile.GetGlobalProjectile<ProjectileItemData>().NoSpecialEffects = true;
    }

    void IManageProjectile.OnKillProjectile(Projectile projectile, int timeLeft) {
        if (timeLeft <= 0) {
            return;
        }
        SpawnExplosion(projectile.GetSource_Death(), projectile);
    }
}