using Aequus.Common;
using ReLogic.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Unused {
    public class UnusedItemGlobalItem : GlobalItem {
        public override bool AppliesToEntity(Item item, bool lateInstantiation) {
            return item.ModItem != null && item.ModItem.GetType().GetAttribute<UnusedContentAttribute>() != null;
        }

        public override void SetDefaults(Item item) {
            item.rare = -1;
        }
    }
}