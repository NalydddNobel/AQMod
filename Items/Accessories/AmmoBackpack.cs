using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class AmmoBackpack : ModItem
    {
        public static HashSet<int> AmmoBlacklist { get; private set; }

        public override void Load()
        {
            AmmoBlacklist = new HashSet<int>()
            {
                AmmoID.FallenStar,
                AmmoID.Gel,
                AmmoID.Solution,
            };
        }

        public override void Unload()
        {
            AmmoBlacklist?.Clear();
            AmmoBlacklist = null;
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(20, 20);
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 3, silver: 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().ammoBackpackItem = Item;
        }
    }
}