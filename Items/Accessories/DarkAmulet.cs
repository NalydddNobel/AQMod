using AQMod.Content.HookBarbs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    public class DarkAmulet : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.damage = 65;
            item.knockBack = 1.5f;
            item.crit = 4;
            item.accessory = true;
            item.defense = 2;
            item.rare = ItemRarityID.LightRed;
            item.value = Item.sellPrice(gold: 3, silver: 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AQPlayer>().darkAmulet = true;
            player.GetModPlayer<HookBarbPlayer>().AddBarb(new DamageBarbAttachmentType(item));
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddRecipeGroup(AQRecipes.RecipeGroups.EvilBarb);
            r.AddIngredient(ItemID.Shackle);
            r.AddIngredient(ItemID.CursedFlame, 10);
            r.AddIngredient(ItemID.DarkShard);
            r.AddIngredient(ItemID.SoulofNight, 15);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}