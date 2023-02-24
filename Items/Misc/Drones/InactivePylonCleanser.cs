using Aequus.Content.DronePylons;
using Terraria;

namespace Aequus.Items.Misc.Drones
{
    public class InactivePylonCleanser : InactiveDroneBase<CleanserDroneSlot>
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.buyPrice(platinum: 2);
        }
    }
}