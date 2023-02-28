using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.DronePylons.Items
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