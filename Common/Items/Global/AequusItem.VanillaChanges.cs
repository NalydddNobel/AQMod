using Aequus.Common.Preferences;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items {
    public partial class AequusItem
    {
        internal void SetDefaults_VanillaChanges(Item item)
        {
            if (item.type == ItemID.ShadowKey)
            {
                item.rare = ItemRarityID.Blue;
                item.value = Item.buyPrice(gold: 15);
                item.StatsModifiedBy.Add(Mod);
            }
            else if (item.type == ItemID.PortalGun)
            {
                if (GameplayConfig.Instance.EarlyPortalGun)
                {
                    item.expert = false;
                    item.value = Item.buyPrice(gold: 10);
                    item.StatsModifiedBy.Add(Mod);
                }
            }
            else if (item.type == ItemID.GravityGlobe)
            {
                if (GameplayConfig.Instance.EarlyGravityGlobe)
                {
                    item.expert = false;
                    item.value = Item.buyPrice(gold: 5);
                    item.StatsModifiedBy.Add(Mod);
                }
            }
        }
    }
}