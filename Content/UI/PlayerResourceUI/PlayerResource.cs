#if CUSTOM_RESOURCE_UI
using Aequus.Common.Preferences;

namespace Aequus.Content.UI.PlayerResourceUI;

public abstract class PlayerResource : ModType, ILocalizedModType {
    public float Animation { get; internal set; } = 1f;
    public float Priority { get; protected set; } = 0f;
    public Color ResourceColor { get; protected set; } = Color.White;

    string ILocalizedModType.LocalizationCategory => "UI.ResourceBars";

    public bool IsActive(Player player, ResourcesPlayer resources, Item heldItem) {
        return Active(player, resources, heldItem);
    }

    protected abstract bool Active(Player player, ResourcesPlayer resources, Item heldItem);

    public abstract void DrawFancy(SpriteBatch sb, ResourceDrawInfo drawInfo);
    public abstract void DrawBar(SpriteBatch sb, ResourceDrawInfo drawInfo);
    public abstract void DrawClassic(SpriteBatch sb, ResourceDrawInfo drawInfo);
    public virtual void Update(ResourceDrawInfo drawInfo) { }

    public sealed override bool IsLoadingEnabled(Mod mod) {
        return ClientConfig.Instance.CustomResourceBars;
    }

    protected sealed override void Register() {
        Instance<CustomResourceUI>().RegisterNew(this);
    }

    public sealed override void SetupContent() {
        SetStaticDefaults();
    }
}

public readonly record struct ResourceDrawInfo(Vector2 Position, Vector2 Origin, Vector2 Offset, float Opacity);
#endif