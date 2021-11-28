using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic
{
    public class UnityMirror : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 100;
            item.magic = true;
            item.useTime = 30;
            item.useAnimation = 30;
            item.width = 50;
            item.height = 50;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<Projectiles.Magic.MoonMoonMirror>();
            item.shootSpeed = 24.11f;
            item.mana = 11;
            item.autoReuse = true;
            item.UseSound = SoundID.Item101;
            item.value = AQItem.Prices.AtmosphericCurrentsValue;
            item.knockBack = 6f;
            item.channel = true;
            item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player player)
        {
            return (!Main.dayTime || (Main.dayTime && Main.eclipse)) && AQItem.Similarities.Mirror_CanUseItem(player);
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.MeteoriteBar, 20);
            r.AddIngredient(ModContent.ItemType<Materials.Energies.AtmosphericEnergy>(), 5);
            r.AddIngredient(ModContent.ItemType<Materials.Energies.CosmicEnergy>());
            r.AddIngredient(ItemID.SoulofFlight, 10);
            r.AddIngredient(ItemID.SoulofNight, 2);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}