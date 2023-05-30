using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.DronePylons.Items {
    [LegacyName("InactivePylonCleanser")]
    public class PylonCleanserItem : DroneItemBase<CleanserDroneSlot>
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.buyPrice(platinum: 2);
        }
    }
}