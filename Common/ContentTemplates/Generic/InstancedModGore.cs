using Terraria.GameContent;

namespace Aequus.Common.ContentTemplates.Generic;

/// <param name="name"></param>
/// <param name="texture"></param>
/// <param name="safe">Whether this gore is safe. (<see cref="ChildSafety.SafeGore"/>)</param>
[Autoload(false)]
internal abstract class InstancedModGore(string name, string texture, bool safe = false) : ModGore {
    public override string Name => name;

    public override string Texture => texture;

    public override void SetStaticDefaults() {
        ChildSafety.SafeGore[Type] = safe;
    }
}