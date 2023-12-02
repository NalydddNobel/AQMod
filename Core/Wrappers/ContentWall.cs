using Aequus.Core;
using Terraria.ModLoader;

namespace Aequus;

public abstract class ContentWall : ModWall {
    public override bool IsLoadingEnabled(Mod mod) {
        return LiteAttribute.LiteCheck(this);
    }
}