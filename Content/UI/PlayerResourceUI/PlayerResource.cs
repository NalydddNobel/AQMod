#if CUSTOM_RESOURCE_UI
namespace Aequus.Content.UI.PlayerResourceUI;

public abstract class PlayerResource : ModType {
    public float Animation { get; internal set; } = 1f;
    public float Priority { get; protected set; } = 0f;
    public int Height => 224;
    public Color ResourceColor { get; protected set; } = Color.White;

    public bool IsActive(Player player, ResourcesPlayer resources, Item heldItem) {
        return Active(player, resources, heldItem);
    }

    protected abstract bool Active(Player player, ResourcesPlayer resources, Item heldItem);

    public abstract void DrawFancy(SpriteBatch sb, ResourceDrawInfo drawInfo);

    protected sealed override void Register() {
        Instance<CustomResourceUI>().RegisterNew(this);
    }

    public sealed override void SetupContent() {
        SetStaticDefaults();
    }
}

public readonly record struct ResourceDrawInfo(Vector2 Position, float ElapsedTime, float Opacity);
#endif