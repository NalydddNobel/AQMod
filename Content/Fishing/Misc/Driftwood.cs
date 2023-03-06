using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Fishing.Misc
{
    public class Driftwood : ModItem
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