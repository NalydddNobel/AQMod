using Terraria.ModLoader;

namespace Aequus.Common.Items;

[Autoload(false)]
public abstract class ManualLoadItem : ModItem {
    protected string _texturePath;
    protected string _internalName;

    public override string Name => _internalName;
    public override string Texture => _texturePath;

    public override bool IsLoadingEnabled(Mod mod) {
        return _internalName != null;
    }
}