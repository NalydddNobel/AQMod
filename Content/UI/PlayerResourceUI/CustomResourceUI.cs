#if CUSTOM_RESOURCE_UI
using Aequus.Common.Preferences;
using Aequus.Common.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.UI;

namespace Aequus.Content.UI.PlayerResourceUI;

public class CustomResourceUI : NewUILayer {
    private readonly List<PlayerResource> _registeredResources = [];
    private readonly List<PlayerResource> _resources = [];

    public PlayerResource? Mana { get; internal set; }
    public PlayerResource? Minion { get; internal set; }
    public PlayerResource? Sentry { get; internal set; }

    public PlayerResource? ForcedVisible;
    public PlayerResource? WantedResource { get; private set; }

    public Dictionary<Type, IResourceDrawProvider> SupportedHBs = new() {
        [typeof(FancyClassicPlayerResourcesDisplaySet)] = new ClassicFancyResourceDrawProvider(),
        [typeof(ClassicPlayerResourcesDisplaySet)] = new ClassicResourceDrawProvider(),
        [typeof(HorizontalBarsPlayerResourcesDisplaySet)] = new BarResourceDrawProvider(),
    };

    public sealed override bool IsLoadingEnabled(Mod mod) {
        return ClientConfig.Instance.CustomResourceBars;
    }

    internal void RegisterNew(PlayerResource resource) {
        _registeredResources.Add(resource);
    }

    public override void PostSetupContent() {
        _resources.AddRange(_registeredResources);
        _resources.Sort((c1, c2) => c1.Priority.CompareTo(c2.Priority));
    }

    public bool TryGetResourceDrawProvider([MaybeNullWhen(false)] out IResourceDrawProvider provider) {
        provider = default;
        if (Main.ResourceSetsManager.ActiveSet == null) {
            return false;
        }
        return SupportedHBs.TryGetValue(Main.ResourceSetsManager.ActiveSet.GetType(), out provider);
    }

    protected override bool DrawSelf() {
        if (!TryGetResourceDrawProvider(out var resourceDrawProvider)) {
            return true;
        }

        WantedResource = ForcedVisible;

        if (WantedResource == null) {
            Player player = Main.LocalPlayer;
            var resources = player.GetModPlayer<ResourcesPlayer>();
            Item heldItem = player.HeldItem;

            //for (int i = 0; i < _resources.Count; i++) {
            for (int i = _resources.Count - 1; i > 0; i--) {
                PlayerResource r = _resources[i];
                if (r.IsActive(player, resources, heldItem)) {
                    WantedResource = r;
                    break;
                }
            }
        }

        // Default to the mana resource if somehow null.
        WantedResource ??= Mana;

        float time = AequusUI.Elapsed;

        WantedResource!.Animation += time * 3.5f;
        if (WantedResource.Animation > 1f) {
            WantedResource.Animation = 1f;
        }

        var mainInfo = resourceDrawProvider.GetInfo(WantedResource);
        WantedResource.Update(mainInfo);
        resourceDrawProvider.Draw(WantedResource, Main.spriteBatch, mainInfo);

        int activeResources = 1;
        for (int i = 0; i < _resources.Count; i++) {
            PlayerResource r = _resources[i];
            if (r == WantedResource || r.Animation <= 0f) {
                continue;
            }

            r.Animation -= time * 3.5f * activeResources;
            if (r.Animation <= 0f) {
                r.Animation = 0f;
                continue;
            }

            activeResources++;
            resourceDrawProvider.Draw(r, Main.spriteBatch, resourceDrawProvider.GetInfo(r));
        }

        return true;
    }

    public CustomResourceUI() : base("PlayerResources", AequusUI.InterfaceLayers.ResourceBars_26, InterfaceScaleType.UI) { }
}
#endif