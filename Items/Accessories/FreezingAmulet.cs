using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    public class FreezingAmulet : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.accessory = true;
            item.defense = 2;
            item.rare = AQItem.Rarities.GaleStreamsRare;
            item.value = AQItem.Prices.GaleStreamsValue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AQPlayer>().coldAmulet = true;
            player.resistCold = true;
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[BuffID.Frozen] = true;
            player.buffImmune[BuffID.Frostburn] = true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.FrostCore);
            r.AddIngredient(ItemID.Shackle);
            r.AddIngredient(ModContent.ItemType<Materials.Energies.AtmosphericEnergy>(), 5);
            r.AddIngredient(ModContent.ItemType<Materials.SiphonTentacle>(), 20);
            r.AddIngredient(ItemID.SoulofFlight, 20);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}