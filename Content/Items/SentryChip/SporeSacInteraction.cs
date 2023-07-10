using Aequus.Common.Items.SentryChip;
using Aequus.Projectiles;
using Aequus.Projectiles.Misc.SporeSac;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.SentryChip {
    public class SporeSacInteraction : SentryInteraction {
        public override void Load(Mod mod) {
            AddTo(ItemID.SporeSac);
        }

        private void UseSporeSac(SentryAccessoryInfo info) {
            if (!Main.rand.NextBool(15)) {
                return;
            }

            int damage = 70;
            float knockBack = 1.5f;
            int sporeSacCount = 0;
            for (int i = 0; i < 1000; i++) {
                if (Main.projectile[i].active && Main.projectile[i].owner == info.Projectile.owner && Main.projectile[i].ModProjectile is SporeSacSentry) {
                    sporeSacCount++;
                }
            }
            if (Main.rand.Next(15) < sporeSacCount || sporeSacCount >= 10) {
                return;
            }

            int attempts = 50;
            int size = 24;
            int collisionSize = 90;
            for (int j = 0; j < attempts; j++) {
                int num5 = Main.rand.Next(200 - j * 2, 400 + j * 2);
                var center = info.Projectile.Center;
                center.X += Main.rand.Next(-num5, num5 + 1);
                center.Y += Main.rand.Next(-num5, num5 + 1);
                if (Collision.SolidCollision(center, size, size) || Collision.WetCollision(center, size, size)) {
                    continue;
                }
                center.X += size / 2;
                center.Y += size / 2;
                if (!Collision.CanHit(new Vector2(info.Projectile.Center.X, info.Projectile.position.Y), 1, 1, center, 1, 1) && !Collision.CanHit(new Vector2(info.Projectile.Center.X, info.Projectile.position.Y - 50f), 1, 1, center, 1, 1)) {
                    continue;
                }
                int tileX = (int)center.X / 16;
                int tileY = (int)center.Y / 16;
                bool canspawn = false;
                if (Main.rand.NextBool(3) && Main.tile[tileX, tileY].WallType > WallID.None) {
                    canspawn = true;
                }
                else {
                    center.X -= collisionSize / 2;
                    center.Y -= collisionSize / 2;
                    if (Collision.SolidCollision(center, collisionSize, collisionSize)) {
                        center.X += collisionSize / 2;
                        center.Y += collisionSize / 2;
                        canspawn = true;
                    }
                    else if (Main.tile[tileX, tileY].HasTile && TileID.Sets.Platforms[Main.tile[tileX, tileY].TileType]) {
                        canspawn = true;
                    }
                }
                if (!canspawn) {
                    continue;
                }

                for (int k = 0; k < 1000; k++) {
                    if (Main.projectile[k].active && Main.projectile[k].owner == info.Projectile.owner && Main.projectile[k].aiStyle == 105) {
                        Vector2 difference = center - Main.projectile[k].Center;
                        if (difference.Length() < 48f) {
                            canspawn = false;
                            break;
                        }
                    }
                }
                if (canspawn && Main.myPlayer == info.Projectile.owner) {
                    Projectile.NewProjectile(info.Projectile.GetSource_ItemUse(info.Accessory), center.X, center.Y, 0f, 0f, Main.rand.Next(ProjectileID.SporeTrap, ProjectileID.SporeTrap2 + 1), damage, knockBack, info.Projectile.owner);
                    break;
                }
            }
        }

        public override void OnSentryAI(SentryAccessoryInfo info) {
            UseSporeSac(info);
            info.DummyPlayer.sporeSac = true;
        }

        public override void OnSentryCreateProjectile(IEntitySource source, Projectile newProjectile, AequusProjectile newAequusProjectile, SentryAccessoryInfo info) {
            if (newAequusProjectile.transform > 0) {
                return;
            }
            SporeSacSentry.Convert(newProjectile, newAequusProjectile);
        }
    }
}