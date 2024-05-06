using Terraria.GameContent;

namespace Aequus.Core.ContentGeneration;

[Autoload(false)]
internal abstract class InstancedModGore(string name, string texture, bool safe = false) : ModGore {
    public override string Name => name;

    public override string Texture => texture;

    public override void SetStaticDefaults() {
        ChildSafety.SafeGore[Type] = safe;
    }
}