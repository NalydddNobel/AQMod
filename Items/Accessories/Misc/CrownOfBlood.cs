using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Misc
{
    public class CrownOfBlood : ModItem, ItemHooks.IUpdateItemDye
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(14, 20);
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.value = Item.buyPrice(gold: 7, silver: 50);
            Item.canBePlacedInVanityRegardlessOfConditions = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accBloodCrownSlot = 3;
        }

        public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            if (!isSetToHidden || !isNotInVanitySlot)
            {
                player.Aequus().crown = Type;
                player.Aequus().cCrown = dyeItem.dye;
            }
        }
    }
}

namespace Aequus
{
    public partial class AequusPlayer : ModPlayer
    {
        public void PostUpdateEquips_CrownOfBlood()
        {
            if (Player.boneGloveItem != null && Player.boneGloveTimer > 1)
            {
                Player.boneGloveTimer -= Player.boneGloveItem.Aequus().accStacks - 1;
                if (Player.boneGloveTimer < 1)
                {
                    Player.boneGloveTimer = 1;
                }
            }
        }
    }
}