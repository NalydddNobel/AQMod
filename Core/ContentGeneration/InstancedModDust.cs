using Terraria.GameContent;

namespace Aequus.Core.ContentGeneration;

/// <param name="name"></param>
/// <param name="texture"></param>
/// <param name="safe">Whether this dust is safe. (<see cref="ChildSafety.SafeDust"/>)</param>
[Autoload(value: false)]
public class InstancedModDust(string name, string texture, bool safe = true) : ModDust {
    public override string Name => name;

    public override string Texture => texture;

    public override void SetStaticDefaults() {
        ChildSafety.SafeDust[Type] = safe;
    }
}

public class InstancedCloneDust(string name, string texture, bool safe = true, int updateType = -1)
    : InstancedModDust(name, texture, safe) {
    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        UpdateType = updateType;
    }
}
