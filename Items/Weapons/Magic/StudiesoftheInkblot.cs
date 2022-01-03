using AQMod.Content.DedicatedItemTags;
using AQMod.Items.Dedicated;
using AQMod.Projectiles.Magic;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic
{
    public class StudiesoftheInkblot : ModItem, IDedicatedItem
    {
        public override void SetDefaults()
        {
            item.width = 80;
            item.height = 80;
            item.damage = 100;
            item.knockBack = 0f;
            item.magic = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 1;
            item.useAnimation = 1;
            item.rare = AQItem.Rarities.DedicatedItem;
            item.shootSpeed = 10f;
            item.autoReuse = true;
            item.noMelee = true;
            item.value = Item.sellPrice(gold: 20);
            item.mana = 2;
            item.shoot = ModContent.ProjectileType<StudiesoftheInkblotOrbiter>();
        }

        Color IDedicatedItem.DedicatedItemColor => new Color(110, 110, 128, 255);
        IDedicationType IDedicatedItem.DedicationType => new BasicDedication();

        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI && player.ownedProjectileCounts[item.shoot] == 0)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == item.shoot && Main.projectile[i].owner == player.whoAmI)
                    {
                        Main.projectile[i].Kill();
                    }
                }
                StudiesoftheInkblotOrbiter.Spawn4(player.whoAmI);
            }
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float speed = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == item.shoot && Main.projectile[i].owner == player.whoAmI)
                {
                    int p = Projectile.NewProjectile(Main.projectile[i].Center, Vector2.Normalize(position - Main.projectile[i].Center) * 0.01f, ModContent.ProjectileType<StudiesoftheInkblotBullet>(), damage, knockBack, player.whoAmI, 0f, speed);
                    Main.projectile[p].localAI[0] = 1.5f;
                    Main.projectile[p].frame = Main.projectile[i].frame;
                }
            }

            int p2 = Projectile.NewProjectile(position, Vector2.Normalize(Main.MouseWorld - position) * 0.01f, ModContent.ProjectileType<StudiesoftheInkblotBullet>(), damage, knockBack, player.whoAmI, 0f, speed);
            Main.projectile[p2].localAI[0] = 1.5f;

            if (((int)Main.GameUpdateCount % 3) == 0)
                AQSound.LegacyPlay(SoundType.Item, "Sounds/Item/TouhouShoot", 0.5f, -0.01f);
            return false;
        }
    }
}