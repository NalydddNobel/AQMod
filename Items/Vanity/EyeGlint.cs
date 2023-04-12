using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Vanity
{
    public class EyeGlint : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(20, 20);
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(silver: 20);
            Item.vanity = true;
            Item.hasVanityEffects = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.Aequus().eyeGlint = true;
        }

        public override void UpdateVanity(Player player)
        {
            player.Aequus().eyeGlint = true;
        }
    }
}