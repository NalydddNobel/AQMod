using Aequus.Core;
using Terraria.ModLoader;

namespace Aequus;

public abstract class ContentTile : ModTile {
    public override bool IsLoadingEnabled(Mod mod) {
        return LiteAttribute.LiteCheck(this);
    }
}