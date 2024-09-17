#if CUSTOM_RESOURCE_UI
using Aequus.Common.Preferences;

namespace Aequus.Content.UI.PlayerResourceUI;

[Autoload(Side = ModSide.Client)]
public class ResourcesPlayer : ModPlayer {
    public override bool IsLoadingEnabled(Mod mod) {
        return ClientConfig.Instance.CustomResourceBars;
    }
}
#endif