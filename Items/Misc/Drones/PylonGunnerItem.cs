using Aequus.Content.DronePylons;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Drones
{
    [LegacyName("InactivePylonGunner")]
    public class PylonGunnerItem : DroneItemBase<GunnerDroneSlot>
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.buyPrice(gold: 10);
        }
    }
}