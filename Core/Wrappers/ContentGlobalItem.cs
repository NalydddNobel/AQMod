using Aequus.Core;
using Terraria.ModLoader;

namespace Aequus;

public abstract class ContentGlobalItem : GlobalItem {
    public override bool IsLoadingEnabled(Mod mod) {
        return LiteAttribute.LiteCheck(this);
    }
}