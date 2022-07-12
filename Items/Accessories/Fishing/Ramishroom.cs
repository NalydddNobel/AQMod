using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Fishing
{
    public class Ramishroom : ModItem
    {
        public static HashSet<int> RodsBlacklist { get; private set; }

        public override void Load()
        {
            RodsBlacklist = new HashSet<int>();
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(20, 20);
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().doubleBobbersItem = Item;
        }
    }
}