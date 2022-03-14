using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee.Yoyo
{
    public class Venus : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[item.type] = true;
            ItemID.Sets.GamepadExtraRange[item.type] = 16;
            ItemID.Sets.GamepadSmartQuickReach[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.useAnimation = 25;
            item.useTime = 25;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.melee = true;
            item.damage = 13;
            item.knockBack = 5.11f;
            item.value = Item.sellPrice(silver: 12);
            item.UseSound = SoundID.Item1;
            item.rare = ItemRarityID.Blue;
            item.channel = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.shootSpeed = 10f;
            item.shoot = ModContent.ProjectileType<Projectiles.Melee.YoyoVenusProj>();
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.TungstenBar, 8);
            r.AddIngredient(ItemID.HealingPotion);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}