using Aequus.Items.Accessories.CrownOfBlood.Projectiles;
using Aequus.Particles;
using log4net.Util;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.CrownOfBlood {
    public partial class CrownOfBloodItem {
        private void LoadExpertEffects() {
            On_Player.SporeSac += Hook_NaniteBombs;
        }
        private static void Hook_NaniteBombs(On_Player.orig_SporeSac orig, Player self, Item sourceItem) {
            var empowerment = sourceItem.Aequus().equipEmpowerment;
            if ((empowerment?.addedStacks) > 0) {
                SpawnNaniteBombs(self, sourceItem);
                return;
            }

            orig(self, sourceItem);
        }
        public static void SpawnNaniteBombs(Player player, Item sourceItem) {
            int damage = 70;
            float knockBack = 1.5f;
            if (!Main.rand.NextBool(15)) {
                return;
            }
            int num = 0;
            for (int i = 0; i < 1000; i++) {
                if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<NaniteSpore>()) {
                    num++;
                }
            }
            if (Main.rand.Next(15) < num || num >= 10) {
                return;
            }
            int num2 = 50;
            int num3 = 24;
            int num4 = 90;
            int num5 = 0;
            Vector2 center;
            while (true) {
                if (num5 >= num2) {
                    return;
                }
                int num6 = Main.rand.Next(200 - num5 * 2, 400 + num5 * 2);
                center = player.Center;
                center.X += Main.rand.Next(-num6, num6 + 1);
                center.Y += Main.rand.Next(-num6, num6 + 1);
                if (!Collision.SolidCollision(center, num3, num3) && !Collision.WetCollision(center, num3, num3)) {
                    center.X += num3 / 2;
                    center.Y += num3 / 2;
                    if (Collision.CanHit(new Vector2(player.Center.X, player.position.Y), 1, 1, center, 1, 1) || Collision.CanHit(new Vector2(player.Center.X, player.position.Y - 50f), 1, 1, center, 1, 1)) {
                        int num7 = (int)center.X / 16;
                        int num8 = (int)center.Y / 16;
                        bool flag = false;
                        if (Main.rand.NextBool(3) && Main.tile[num7, num8] != null && Main.tile[num7, num8].WallType > 0) {
                            flag = true;
                        }
                        else {
                            center.X -= num4 / 2;
                            center.Y -= num4 / 2;
                            if (Collision.SolidCollision(center, num4, num4)) {
                                center.X += num4 / 2;
                                center.Y += num4 / 2;
                                flag = true;
                            }
                            else if (Main.tile[num7, num8] != null && Main.tile[num7, num8].HasTile && Main.tile[num7, num8].TileType == 19) {
                                flag = true;
                            }
                        }
                        if (flag) {
                            for (int j = 0; j < 1000; j++) {
                                if (Main.projectile[j].active && Main.projectile[j].owner == player.whoAmI && Main.projectile[j].aiStyle == 105) {
                                    var val = center - Main.projectile[j].Center;
                                    if (val.Length() < 48f) {
                                        flag = false;
                                        break;
                                    }
                                }
                            }
                            if (flag && Main.myPlayer == player.whoAmI) {
                                break;
                            }
                        }
                    }
                }
                num5++;
            }
            Projectile.NewProjectile(player.GetSource_Accessory(sourceItem), center.X, center.Y, 0f, 0f, ModContent.ProjectileType<NaniteSpore>(), damage, knockBack, player.whoAmI);
        }

        public static void SpecialUpdate_ShieldOfCthulhu(Item item, Player player, bool hideVisual) {
            if (player.dashType != 2) {
                return;
            }
            if (player.timeSinceLastDashStarted == 1) {
                //SoundEngine.PlaySound(SoundID.ForceRoar with { Pitch = 0.5f, }, player.Center);
                player.velocity.X = Math.Max(Math.Abs(player.velocity.X), 19f) * player.direction;
            }
            if (player.dashDelay < 0) {
                int particleCount = Math.Clamp((int)(Math.Abs(player.velocity.X) / 6f), 1, 3);
                for (int i = 0; i < particleCount; i++) {
                    ParticleSystem.New<BloomParticle>(ParticleLayer.BehindPlayers)
                        .Setup(
                        Main.rand.NextFromRect(player.Hitbox),
                        -player.velocity * Main.rand.NextFloat(0.3f),
                        Color.Red with { A = 20 } * 0.6f,
                        Color.Red with { A = 0 } * 0.16f,
                        Main.rand.NextFloat(0.7f, 1.4f),
                        0.3f
                    );
                }
            }
            if (player.dashDelay > 1) {
                player.dashDelay--;
            }
            //if (player.eocDash > 0 && Main.myPlayer == player.whoAmI && player.ownedProjectileCounts[ModContent.ProjectileType<ShieldOfCthulhuBoost>()] <= 0) {
            //    Projectile.NewProjectile(player.GetSource_Accessory(item), player.Center, new Vector2(player.direction, 0f), ModContent.ProjectileType<ShieldOfCthulhuBoost>(), player.GetWeaponDamage(item) * 2, 1f, player.whoAmI);
            //}
        }

        public static void SpecialUpdate_WormScarf(Item item, Player player, bool hideVisual) {
        }

        public static void SpecialUpdate_BrainOfConfusion(Item item, Player player, bool hideVisual) {
        }

        public static void OnSpawn_BoneGlove(IEntitySource source, Item item, Projectile projectile) {
            projectile.Aequus().transform = ModContent.ProjectileType<Bonesaw>();
            projectile.velocity *= 1.25f;
            projectile.damage = (int)(projectile.damage * 1.5f);
        }
        public static void OnSpawn_VolatileGelatin(IEntitySource source, Item item, Projectile projectile) {
            projectile.Aequus().transform = ModContent.ProjectileType<ThermiteGel>();
            projectile.velocity *= 1.25f;
            projectile.damage = (int)(projectile.damage * 1.5f);
        }
    }
}

namespace Aequus {
    public partial class AequusPlayer {
        public int accBoneGloveStacks;
    }
}