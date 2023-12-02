using Aequus.Core;
using Terraria.ModLoader;

namespace Aequus;

public abstract class ContentBuilderToggle : BuilderToggle {
    public override bool IsLoadingEnabled(Mod mod) {
        return LiteAttribute.LiteCheck(this);
    }
}