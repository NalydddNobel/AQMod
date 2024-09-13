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

public interface IResourceDrawProvider {
    ResourceDrawInfo GetInfo(PlayerResource resource);
    void Draw(PlayerResource resource, SpriteBatch sb, ResourceDrawInfo info);
}

public readonly struct ClassicFancyResourceDrawProvider : IResourceDrawProvider {
    void IResourceDrawProvider.Draw(PlayerResource resource, SpriteBatch sb, ResourceDrawInfo info) {
        resource.DrawFancy(sb, info);
    }

    ResourceDrawInfo IResourceDrawProvider.GetInfo(PlayerResource resource) {
        float resourceVerticalDir = Instance<CustomResourceUI>().WantedResource == resource ? 1f : -1f;
        Vector2 origin = new Vector2(Main.screenWidth - 40f, 24f);
        Vector2 offset = new Vector2(100f * (1f - resource.Animation), MathF.Sin((1f - resource.Animation) * MathHelper.Pi) * 50f * resourceVerticalDir);
        return new ResourceDrawInfo() {
            Position = origin + offset,
            Origin = origin,
            Offset = offset,
            Opacity = MathF.Pow(resource.Animation, 1.5f),
        };
    }
}

public readonly struct ClassicResourceDrawProvider : IResourceDrawProvider {
    void IResourceDrawProvider.Draw(PlayerResource resource, SpriteBatch sb, ResourceDrawInfo info) {
        resource.DrawClassic(sb, info);
    }

    ResourceDrawInfo IResourceDrawProvider.GetInfo(PlayerResource resource) {
        float resourceVerticalDir = Instance<CustomResourceUI>().WantedResource == resource ? 1f : -1f;
        Vector2 origin = new Vector2(Main.screenWidth - 40f, 24f);
        Vector2 offset = new Vector2(100f * (1f - resource.Animation), MathF.Sin((1f - resource.Animation) * MathHelper.Pi) * 50f * resourceVerticalDir);
        return new ResourceDrawInfo() {
            Position = origin + offset,
            Origin = origin,
            Offset = offset,
            Opacity = MathF.Pow(resource.Animation, 1.5f),
        };
    }
}

public readonly struct BarResourceDrawProvider : IResourceDrawProvider {
    void IResourceDrawProvider.Draw(PlayerResource resource, SpriteBatch sb, ResourceDrawInfo info) {
        resource.DrawBar(sb, info);
    }

    ResourceDrawInfo IResourceDrawProvider.GetInfo(PlayerResource resource) {
        float resourceVerticalDir = Instance<CustomResourceUI>().WantedResource == resource ? 1f : -1f;
        Vector2 origin = new Vector2(Main.screenWidth - 40f, 24f);
        Vector2 offset = new Vector2(100f * (1f - resource.Animation), MathF.Sin((1f - resource.Animation) * MathHelper.Pi) * 50f * resourceVerticalDir);
        return new ResourceDrawInfo() {
            Position = origin + offset,
            Origin = origin,
            Offset = offset,
            Opacity = Instance<CustomResourceUI>().WantedResource == resource ? 1f : 0f,
        };
    }
}
#endif