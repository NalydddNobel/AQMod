using Aequus.Content.WorldGeneration;
using Aequus.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic
{
    public class SavingGrace : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            Item.staff[Type] = true;
            HardmodeChestBoost.HardmodeJungleChestLoot.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.damage = 94;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.width = 20;
            Item.height = 20;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemDefaults.RarityEarlyHardmode;
            Item.shoot = ModContent.ProjectileType<SavingGraceProj>();
            Item.shootSpeed = 30f;
            Item.mana = 80;
            Item.autoReuse = true;
            Item.UseSound = Aequus.GetSound("Item/savingGraceCast");
            Item.value = ItemDefaults.ValueEarlyHardmode;
        }

        public override bool CanUseItem(Player player)
        {
            return !player.manaSick;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(2f, 2f);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.StaffofRegrowth)
                .AddIngredient(ItemID.JungleSpores, 12)
                .AddIngredient(ItemID.SoulofLight, 16)
                .AddTile(TileID.Anvils)
                .TryRegisterAfter(ItemID.OnyxBlaster);
        }
    }
}