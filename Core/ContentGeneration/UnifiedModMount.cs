namespace AequusRemake.Core.ContentGeneration;

/// <summary>A <see cref="ModMount"/> which automagically registers a related <see cref="ModItem"/> and <see cref="ModBuff"/>, suffixed with +"Item" and +"Buff" respectively.</summary>
public abstract class UnifiedModMount : ModMount, ILocalizedModType {
    public ModItem MountItem { get; private set; }
    public ModBuff MountBuff { get; private set; }

    public string LocalizationCategory => "Mounts";

    internal virtual ModBuff CreateMountBuff() {
        return new InstancedMountBuff(this);
    }
    internal virtual ModItem CreateMountItem() {
        return new InstancedMountItem(this);
    }

    protected virtual void OnLoad() { }
    protected virtual void OnSetStaticDefaults() { }

    public sealed override void Load() {
        MountItem = CreateMountItem();
        MountBuff = CreateMountBuff();
        OnLoad();
        Mod.AddContent(MountItem);
        Mod.AddContent(MountBuff);
    }

    public sealed override void SetStaticDefaults() {
        MountData.buff = MountBuff.Type;
        OnSetStaticDefaults();
    }
}
