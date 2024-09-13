#if CUSTOM_RESOURCE_UI
namespace Aequus.Content.UI.PlayerResourceUI;

public class ManaResourceOverride : ModResourceOverlay {
    public override bool PreDrawResource(ResourceOverlayDrawContext context) {
        CustomResourceUI resourceUI = Instance<CustomResourceUI>();
        resourceUI.Activate();
        if (resourceUI.Mana!.Animation < 1f) {
            if (context.texture.Name.Contains("Star")) {
                var drawInfo = resourceUI.GetDrawParams(resourceUI.Mana, Vector2.Zero);
                context.position += drawInfo.Position;
                context.color *= drawInfo.Opacity;
                context.Draw();
                return false;
            }
        }

        return true;
    }
}
#endif