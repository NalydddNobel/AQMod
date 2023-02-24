using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Fishing.Misc
{
    public class PlasticBottle : ModItem
    {
        public override void SetStaticDefaults()
        {
            AequusPlayer.TrashItemIDs.Add(Type);

            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.FishingSeaweed);
            Item.width = 10;
            Item.height = 10;
        }
    }
}