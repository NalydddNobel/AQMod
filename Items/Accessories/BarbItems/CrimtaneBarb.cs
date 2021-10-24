using AQMod.Content.HookBarbs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.BarbItems
{
    public class CrimtaneBarb : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.damage = 30;
            item.knockBack = 1.1f;
            item.crit = 4;
            item.accessory = true;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(gold: 1, silver: 15);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<HookBarbPlayer>().AddBarb(new DamageBarbAttachmentType(item));
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.CrimtaneBar, 8);
            r.AddIngredient(ItemID.Obsidian, 20);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}