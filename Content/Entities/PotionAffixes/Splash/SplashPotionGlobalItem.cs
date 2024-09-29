using Aequus.Common.DataSets;
using Aequus.Common.Entities.Players;

namespace Aequus.Content.Entities.PotionAffixes.Splash;

public class SplashPotionGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return ItemSets.IsPotion.Contains(entity.type);
    }

    public override bool CanUseItem(Item item, Player player) {
        if (PlayerHooks.QuickBuff && item.buffType > 0 && item.buffTime > 0) {
            return PrefixLoader.GetPrefix(item.prefix) is not SplashPrefix;
        }

        return true;
    }
}
