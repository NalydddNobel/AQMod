﻿using Aequus.Common.Players;
using Aequus.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic
{
    public class StudiesOfTheInkblot : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            this.SetResearch(1);
        }

        private void DefaultUse()
        {
            Item.damage = 200;
            Item.useTime = 1;
            Item.useAnimation = 1;
            Item.mana = 2;
        }
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.knockBack = 0f;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundLoader.GetLegacySoundSlot(Mod, "Sounds/touhoushoot");
            Item.rare = ItemRarityID.Red;
            Item.shootSpeed = 10f;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.value = Item.sellPrice(gold: 20);
            Item.shoot = ModContent.ProjectileType<TouhouOrbiter>();
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
                Item.damage = 600;
                Item.useTime = 62;
                Item.useAnimation = 62;
                Item.mana = 80;
            }
            else
            {
                DefaultUse();
            }
            return true;
        }

        public override void HoldItem(Player player)
        {
            var cooldowns = player.GetModPlayer<CooldownsPlayer>();
            if (Main.myPlayer == player.whoAmI)
            {
                if (player.ownedProjectileCounts[Item.shoot] == 0 && cooldowns.itemCombo <= 0)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].type == Item.shoot && Main.projectile[i].owner == player.whoAmI)
                        {
                            Main.projectile[i].Kill();
                        }
                    }
                    TouhouOrbiter.Spawn4(new EntitySource_ItemUse_WithAmmo(player, Item, 0), player.whoAmI);
                }
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var cooldowns = player.GetModPlayer<CooldownsPlayer>();
            float speed = velocity.Length();
            if (player.altFunctionUse == 2)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == Item.shoot && Main.projectile[i].owner == player.whoAmI)
                    {
                        Main.projectile[i].Kill();
                    }
                }
                int rand = Main.rand.Next(62);
                for (int i = 0; i < 62; i++)
                {
                    int p = Projectile.NewProjectile(source, position, new Vector2(speed * 0.3f, 0f).RotatedBy(MathHelper.TwoPi / 62f * i),
                        ModContent.ProjectileType<TouhouBullet>(), damage, knockback, player.whoAmI, 100f + 40f * (1f / 62f * ((i + rand) % 62f)), speed * 0.5f);
                    Main.projectile[p].localAI[0] = 1.5f;
                    Main.projectile[p].frame = cooldowns.itemCombo > 0 ? 1 : 5;
                }
                if (cooldowns.itemCombo <= 0)
                {
                    cooldowns.itemCombo = (ushort)(Item.useTime * 2);
                }
                DefaultUse();
            }
            else
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == Item.shoot && Main.projectile[i].owner == player.whoAmI)
                    {
                        int p = Projectile.NewProjectile(source, Main.projectile[i].Center, Vector2.Normalize(position - Main.projectile[i].Center) * 0.01f, ModContent.ProjectileType<TouhouBullet>(), damage, knockback, player.whoAmI, 0f, speed);
                        Main.projectile[p].localAI[0] = 1.5f;
                        Main.projectile[p].frame = Main.projectile[i].frame;
                    }
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarFlareBook)
                .AddIngredient(ItemID.SpellTome)
                .AddIngredient(ItemID.Shrimp, 5)
                .AddIngredient(ItemID.SuspiciousLookingEye)
                .AddIngredient(ItemID.BlackInk)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}