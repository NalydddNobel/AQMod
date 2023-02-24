using Aequus.Items.Materials;
using Aequus.Items.Materials.Energies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Summon.Necro
{
    public class NaturesCruelty : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 4);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.Aequus().ghostHealthDR > 0f)
            {
                player.Aequus().ghostHealthDR += (1f - player.Aequus().ghostHealthDR) * 0.25f;
            }
            else
            {
                player.Aequus().ghostHealthDR = 0.25f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.NaturesGift)
                .AddIngredient<DemonicEnergy>()
                .AddIngredient<SoulGemFilled>(5)
                .AddTile(TileID.DemonAltar)
                .TryRegisterAfter(ItemID.ManaFlower);
        }
    }
}