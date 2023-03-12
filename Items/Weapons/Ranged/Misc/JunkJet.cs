using Aequus.Items.Materials;
using Aequus.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged.Misc
{
    public class JunkJet : ModItem
    {
        public static Dictionary<int, int> AmmoItemToProj { get; private set; }

        public override void Load()
        {
            AmmoItemToProj = new Dictionary<int, int>()
            {
                [ItemID.MiniNukeI] = ProjectileID.MiniNukeRocketI,
                [ItemID.MiniNukeII] = ProjectileID.MiniNukeRocketII,
            };
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void Unload()
        {
            AmmoItemToProj?.Clear();
        }

        public override void SetDefaults()
        {
            Item.SetWeaponValues(4, 4f, 4);
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.width = 30;
            Item.height = 30;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 6f;
            Item.UseSound = AequusSounds.shoot_JunkJet.Sound with { Volume = 0.66f, PitchVariance = 0.15f, MaxInstances = 3, };
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.rare = ItemDefaults.RarityCrabCrevice;
            Item.value = ItemDefaults.ValueCrabCrevice;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override bool? CanChooseAmmo(Item ammo, Player player)
        {
            return ammo.damage > 0 && !ammo.IsACoin && ammo.DamageType.CountsAsClass(DamageClass.Ranged) && ammo.ammo > 0;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextBool();
        }
        
        public static void GetConversionType(int bulletItem, ref int projType)
        {
            if (AmmoItemToProj.TryGetValue(bulletItem, out int convertType))
            {
                projType = convertType;
                return;
            }
            if (projType == ProjectileID.PurificationPowder || projType == ProjectileID.EnchantedBoomerang)
            {
                string itemName = ItemID.Search.GetName(bulletItem);
                if (ProjectileID.Search.TryGetId(itemName, out int projID))
                {
                    projType = projID;
                }
                else
                {
                    projType = ProjectileID.Bullet;
                }
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            GetConversionType(source.AmmoItemIdUsed, ref type);
            Projectile p;
            for (int i = 0; i < 2; i++)
            {
                p = Projectile.NewProjectileDirect(source, position, velocity.RotatedBy(Main.rand.NextFloat(-0.15f, 0.15f)), type, damage, knockback, player.whoAmI);
                p.timeLeft = Math.Min(p.timeLeft, 600);
            }
            p = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            p.timeLeft = Math.Min(p.timeLeft, 600);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<StarPhish>()
                .AddIngredient<PearlShardWhite>(3)
                .AddIngredient<AquaticEnergy>()
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}