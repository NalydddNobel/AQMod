using Aequus.Core;
using Terraria.ModLoader;

namespace Aequus;

public abstract class ContentPrefix : ModPrefix {
    public override bool IsLoadingEnabled(Mod mod) {
        return LiteAttribute.LiteCheck(this);
    }
}