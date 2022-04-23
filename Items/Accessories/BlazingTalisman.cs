using Aequus.Items.Misc;
using Aequus.Items.Misc.Energies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class BlazingTalisman : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.rare = ItemRarityConstants.GaleStreams;
            Item.value = ItemPriceProperties.GaleStreamsValue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.resistCold = true;
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[BuffID.Frozen] = true;
            player.buffImmune[BuffID.Frostburn] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.WarmthPotion, 3)
                .AddIngredient<AtmosphericEnergy>()
                .AddIngredient<Fluorescence>(20)
                .AddIngredient(ItemID.SoulofFlight, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}