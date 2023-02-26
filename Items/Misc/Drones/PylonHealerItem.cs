using Aequus.Content.DronePylons;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Drones
{
    [LegacyName("InactivePylonHealer")]
    public class PylonHealerItem : DroneItemBase<HealerDroneSlot>
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.buyPrice(gold: 10);
        }
    }
}