#if CUSTOM_RESOURCE_UI
using Aequus.Common.Preferences;
using Terraria.GameContent;

namespace Aequus.Content.UI.PlayerResourceUI;

public class ManaResourceOverride : ModResourceOverlay {
    public override bool IsLoadingEnabled(Mod mod) {
        return ClientConfig.Instance.CustomResourceBars;
    }

    public override bool PreDrawResource(ResourceOverlayDrawContext context) {
        CustomResourceUI resourceUI = Instance<CustomResourceUI>();

        if (!resourceUI.TryGetResourceDrawProvider(out var resourceDrawProvider)) {
            return true;
        }

        resourceUI.Activate();
        if (resourceUI.Mana!.Animation < 1f) {
            var drawInfo = resourceDrawProvider.GetInfo(resourceUI.Mana);

            if (context.texture.Name.Contains("Star") || context.texture == TextureAssets.Mana) {
                context.position += drawInfo.Offset;
                context.color *= drawInfo.Opacity;
                context.Draw();
                return false;
            }

            if (context.texture.Name.Contains("MP_") && drawInfo.Opacity <= 0f) {
                return false;
            }

            if (context.texture.Name.EndsWith("Panel_Left") && context.position.Y > 40f && drawInfo.Opacity <= 0f) {
                return false;
            }
        }

        return true;
    }
}
#endif