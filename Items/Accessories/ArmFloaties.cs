using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class ArmFloaties : ModItem
    {
        public static List<int> Equipped { get; private set; }

        public override void SetStaticDefaults()
        {
            Equipped = new List<int>();
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(20, 14);
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1, silver: 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            Equipped.Add(player.whoAmI);
            player.Aequus().accArmFloaties = true;
        }
    }
}