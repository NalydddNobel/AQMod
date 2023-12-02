using Aequus.Core;
using Terraria.ModLoader;

namespace Aequus;

public abstract class ContentSystem : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return LiteAttribute.LiteCheck(this);
    }
}