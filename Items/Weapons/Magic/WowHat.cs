using Aequus.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Items.Weapons.Magic
{
    public class WowHat : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.SetWeaponValues(28, 4f);
            Item.DefaultToMagicWeapon(ProjectileID.BallofFire, 30, 12f, true);
            Item.mana = 12;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemDefaults.RarityGlimmer;
            Item.value = ItemDefaults.GlimmerValue;
            Item.UseSound = SoundID.Item8;
            Item.noUseGraphic = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            var r = new UnifiedRandom((int)(Main.GameUpdateCount / 180));
            switch (r.Next(20))
            {
                case 1:
                    type = ProjectileID.BallofFrost;
                    break;
                case 2:
                    type = ProjectileID.FrostBoltStaff;
                    break;
                case 3:
                    type = ModContent.ProjectileType<BallisticScreecherProj>();
                    velocity *= 0.33f;
                    break;
                case 4:
                    type = ModContent.ProjectileType<TouhouBullet>();
                    break;
                case 5:
                    //type = ProjectileID.VilethornTip;
                    break;
                case 6:
                    //type = ProjectileID.ZapinatorLaser;
                    break;
                case 7:
                    type = Utils.SelectRandom(Main.rand, ProjectileID.AmethystBolt, ProjectileID.TopazBolt, ProjectileID.SapphireBolt, ProjectileID.EmeraldBolt, ProjectileID.RubyBolt, ProjectileID.DiamondBolt, ProjectileID.AmberBolt);
                    break;
                case 8:
                    type = ProjectileID.WandOfSparkingSpark;
                    break;
                case 9:
                    type = ProjectileID.ThunderStaffShot;
                    break;
                case 10:
                    type = ProjectileID.WaterStream;
                    break;
                case 11:
                    type = player.beeType();
                    damage = player.beeDamage(damage);
                    knockback = player.beeKB(knockback);
                    break;
                case 12:
                    type = ProjectileID.GreenLaser;
                    break;
                case 13:
                    type = ProjectileID.DemonScythe;
                    break;
                case 14:
                    type = ProjectileID.BookOfSkullsSkull;
                    break;
                case 15:
                    type = ProjectileID.WaterBolt;
                    break;
                case 16:
                    type = ProjectileID.UnholyTridentFriendly;
                    break;
                case 17:
                    //type = ProjectileID.Bat;
                    break;
                case 18:
                    type = ProjectileID.SkyFracture;
                    break;
                case 19:
                    type = ProjectileID.CrystalStorm;
                    break;
            }
            if (Main.rand.NextBool(8))
            {
                velocity *= Main.rand.NextFloat(1f, 2f);
            }
            if (Main.rand.NextBool(4))
            {
                switch (Main.rand.Next(3))
                {
                    case 0:
                        if (!AequusHelpers.PointCollision(new Vector2(position.X, position.Y - 800f), 48, 48))
                        {
                            position.Y -= 800f;
                            velocity = Vector2.Normalize(Main.MouseWorld - position).UnNaN() * velocity.Length() * 2.5f;
                        }
                        break;

                    case 1:
                        if (!AequusHelpers.PointCollision(Main.MouseWorld, 16, 16))
                        {
                            position = Main.MouseWorld - Vector2.Normalize(velocity);
                        }
                        break;

                    case 2:
                        //velocity = -velocity;
                        break;
                }
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (type == Item.shoot)
            {

            }
            if (Main.rand.NextBool(8))
            {
                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.PiOver4 / 2f), type, damage, knockback, player.whoAmI);
                Projectile.NewProjectile(source, position, velocity.RotatedBy(-MathHelper.PiOver4 / 2f), type, damage, knockback, player.whoAmI);
            }
            return true;
        }
    }
}