using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    [AutoloadEquip(EquipType.Back, EquipType.Front)]
    public class Stormcloak : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemDefaults.RarityDustDevil;
            Item.value = ItemDefaults.DustDevilValue;
            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accDustDevilExpert = Item;
        }

        public override void AddRecipes()
        {
            ModContent.GetInstance<TheReconstructionGlobalItem>().addEntry(Type);
        }
    }
}