#if CUSTOM_RESOURCE_UI
namespace Aequus.Content.UI.PlayerResourceUI;

public class ManaResource : PlayerResource {
    // Do nothing for drawing, vanilla draws this bar.
    public override void DrawFancy(SpriteBatch sb, ResourceDrawInfo drawInfo) {
    }

    public override void Load() {
        Instance<CustomResourceUI>().Mana = this;
        ResourceColor = new Color(120, 120, 255);
        Priority = -1f;
    }

    protected override bool Active(Player player, ResourcesPlayer resources, Item heldItem) {
        return true;
    }
}
#endif