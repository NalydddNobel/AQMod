using Aequus.Content.Town.PhysicistNPC.Analysis;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Offense
{
    public class PrecisionGloves : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 5);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accPreciseCrits += 1f;
        }
    }
}