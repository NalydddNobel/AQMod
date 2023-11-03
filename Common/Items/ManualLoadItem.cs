using Terraria.ModLoader;

namespace Aequus.Common.Items;

/// <summary>
/// Base class for manually loaded items. Has a cache for the name and texture path.
/// <para>You still need to manually add the <see cref="AutoloadAttribute"/> to classes which inherit this base class.</para>
/// </summary>
public abstract class ManualLoadItem : ModItem {
    protected string _texturePath;
    protected string _internalName;

    public override string Name => _internalName;
    public override string Texture => _texturePath;

    protected override bool CloneNewInstances => true;
}