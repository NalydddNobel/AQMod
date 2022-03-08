using AQMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic
{
    public class StudiesoftheInkblot : ModItem, IDedicatedItem
    {
        Color IDedicatedItem.DedicatedColoring => new Color(110, 110, 128, 255);

        private void DefaultUse()
        {
            item.damage = 200;
            item.useTime = 1;
            item.useAnimation = 1;
            item.mana = 2;
        }
        public override void SetDefaults()
        {
            item.width = 80;
            item.height = 80;
            item.knockBack = 0f;
            item.magic = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/TouhouShoot");
            item.rare = AQItem.RarityDedicatedItem;
            item.shootSpeed = 10f;
            item.autoReuse = true;
            item.noMelee = true;
            item.value = Item.sellPrice(gold: 20);
            item.shoot = ModContent.ProjectileType<StudiesoftheInkblotOrbiter>();
            DefaultUse();
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.damage = 600;
                item.useTime = 62;
                item.useAnimation = 62;
                item.mana = 20;
            }
            else
            {
                DefaultUse();
            }
            return true;
        }

        public override void HoldItem(Player player)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            if (Main.myPlayer == player.whoAmI && player.ownedProjectileCounts[item.shoot] == 0 && aQPlayer.itemCombo <= 0)
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
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            float speed = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
            if (player.altFunctionUse == 2)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == item.shoot && Main.projectile[i].owner == player.whoAmI)
                    {
                        Main.projectile[i].Kill();
                    }
                }
                int rand = Main.rand.Next(62);
                for (int i = 0; i < 62; i++)
                {
                    int p = Projectile.NewProjectile(position, new Vector2(speed * 0.3f, 0f).RotatedBy(MathHelper.TwoPi / 62f * i),
                        ModContent.ProjectileType<StudiesoftheInkblotBullet>(), damage, knockBack, player.whoAmI, 100f + 40f * (1f / 62f * ((i + rand) % 62f)), speed * 0.5f);
                    Main.projectile[p].localAI[0] = 1.5f;
                    Main.projectile[p].frame = aQPlayer.itemCombo > 0 ? 1 : 5;
                }
                if (aQPlayer.itemCombo <= 0)
                {
                    aQPlayer.itemCombo = (ushort)(item.useTime * 2);
                }
            }
            else
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == item.shoot && Main.projectile[i].owner == player.whoAmI)
                    {
                        int p = Projectile.NewProjectile(Main.projectile[i].Center, Vector2.Normalize(position - Main.projectile[i].Center) * 0.01f, ModContent.ProjectileType<StudiesoftheInkblotBullet>(), damage, knockBack, player.whoAmI, 0f, speed);
                        Main.projectile[p].localAI[0] = 1.5f;
                        Main.projectile[p].frame = Main.projectile[i].frame;
                    }
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.LunarFlareBook);
            r.AddIngredient(ItemID.SpellTome);
            r.AddIngredient(ItemID.Shrimp);
            r.AddIngredient(ItemID.SuspiciousLookingEye);
            r.AddIngredient(ItemID.BlackInk, 5);
            r.SetResult(this);
            r.AddTile(TileID.LunarCraftingStation);
            r.AddRecipe();
        }
    }
}