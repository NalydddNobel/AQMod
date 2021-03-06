using Aequus.Items.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Summon.Necro
{
    public sealed class RitualisticSkull : ModItem
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
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(gold: 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<RitualisticSkullPlayer>().minionsToGhosts = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PygmyNecklace)
                .AddIngredient<Hexoplasm>(12)
                .AddIngredient(ItemID.SoulofFright, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register((r) => r.SortAfterFirstRecipesOf(ItemID.PapyrusScarab));
        }
    }

    public class RitualisticSkullPlayer : ModPlayer
    {
        public bool minionsToGhosts;

        public override void ResetEffects()
        {
            minionsToGhosts = false;
        }

        public override void PostUpdateEquips()
        {
            if (minionsToGhosts)
            {
                Player.Aequus().ghostSlotsMax += Player.maxMinions - 1;
                Player.maxMinions = 1;
            }
        }
    }
}