using Aequus.Items.Accessories.Summon.Sentry;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Utility
{
    public class HaltingMachine : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;

            SantankInteractions.OnAI.Add(Type, SantankInteractions.ApplyEquipFunctional_AI);
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.accessory = true;
            Item.rare = ItemDefaults.RarityOmegaStarite;
            Item.value = Item.buyPrice(gold: 5);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().antiGravityItemRadius = 300f;
        }
    }
}