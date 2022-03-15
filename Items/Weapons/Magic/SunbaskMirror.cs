using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic
{
    public class SunbaskMirror : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 125;
            item.magic = true;
            item.useTime = 38;
            item.useAnimation = 38;
            item.width = 50;
            item.height = 50;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<Projectiles.Magic.SunbaskMirror>();
            item.shootSpeed = 24.11f;
            item.mana = 30;
            item.autoReuse = true;
            item.UseSound = SoundID.Item101;
            item.value = AQItem.Prices.GaleStreamsWeaponValue;
            item.knockBack = 6f;
            item.channel = true;
            item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player player)
        {
            return Main.dayTime && AQItem.MirrorCheck(player);
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.MeteoriteBar, 20);
            r.AddIngredient(ModContent.ItemType<Materials.Energies.AtmosphericEnergy>());
            r.AddIngredient(ModContent.ItemType<Materials.Fluorescence>(), 12);
            r.AddIngredient(ItemID.SoulofFlight, 12);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}