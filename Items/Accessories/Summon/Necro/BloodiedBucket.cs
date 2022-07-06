using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Aequus.Items.Accessories.Summon.Necro
{
    public sealed class BloodiedBucket : ModItem
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
            Item.rare = ItemRarityID.Green;
            Item.value = ItemDefaults.BloodMimicItemValue * 3;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().ghostLifespan *= 3;
        }
    }
}