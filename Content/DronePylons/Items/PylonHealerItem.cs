using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.DronePylons.Items
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