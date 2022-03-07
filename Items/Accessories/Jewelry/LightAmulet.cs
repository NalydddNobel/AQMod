using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.Jewelry
{
    public class LightAmulet : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.accessory = true;
            item.defense = 2;
            item.rare = ItemRarityID.LightRed;
            item.value = Item.sellPrice(gold: 3, silver: 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AQPlayer>().unholyDamage += 0.1f;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<Amulet>());
            r.AddIngredient(ItemID.LightShard);
            r.AddIngredient(ItemID.SoulofLight, 15);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}