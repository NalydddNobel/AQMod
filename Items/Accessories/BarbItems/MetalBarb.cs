using AQMod.Content.HookBarbs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.BarbItems
{
    public class MetalBarb : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.damage = 10;
            item.knockBack = 0f;
            item.crit = 4;
            item.value = Item.sellPrice(silver: 20);
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<HookBarbPlayer>().AddBarb(new DamageBarbAttachmentType(item));
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddRecipeGroup("IronBar", 3);
            r.AddIngredient(ItemID.Obsidian, 8);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}