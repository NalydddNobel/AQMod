using Aequus.Content.DronePylons;
using Terraria;

namespace Aequus.Items.Misc.Drones
{
    public class InactivePylonHealer : InactiveDroneBase<HealerDroneSlot>
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.buyPrice(gold: 10);
        }
    }
}