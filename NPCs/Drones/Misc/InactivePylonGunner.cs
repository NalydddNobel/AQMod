using Aequus.Content.DronePylons;
using Terraria;

namespace Aequus.NPCs.Drones.Misc
{
    public class InactivePylonGunner : InactiveDroneBase<GunnerDroneSlot>
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.buyPrice(gold: 10);
        }
    }
}