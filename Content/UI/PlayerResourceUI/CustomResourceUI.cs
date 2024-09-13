#if CUSTOM_RESOURCE_UI
using Aequus.Common.UI;
using System;
using System.Collections.Generic;
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

    internal void RegisterNew(PlayerResource resource) {
        _registeredResources.Add(resource);
    }

    public override void PostSetupContent() {
        _resources.AddRange(_registeredResources);
        _resources.Sort((c1, c2) => c1.Priority.CompareTo(c2.Priority));
    }

    protected override bool DrawSelf() {
        if (Main.ResourceSetsManager.ActiveSet is not FancyClassicPlayerResourcesDisplaySet) {
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

        float time = GetElapsedTime();

        WantedResource!.Animation += time * 3.5f;
        if (WantedResource.Animation > 1f) {
            WantedResource.Animation = 1f;
        }

        Vector2 origin = new Vector2(Main.screenWidth - 40f, 24f);

        WantedResource.DrawFancy(Main.spriteBatch, GetDrawParams(WantedResource, origin));

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
            r.DrawFancy(Main.spriteBatch, GetDrawParams(r, origin));
        }

        return true;
    }

    internal ResourceDrawInfo GetDrawParams(PlayerResource resource, Vector2 origin) {
        float resourceVerticalDir = WantedResource == resource ? 1f : -1f;
        return new ResourceDrawInfo() {
            Position = origin + new Vector2(100f * (1f - resource.Animation), MathF.Sin((1f - resource.Animation) * MathHelper.Pi) * 50f * resourceVerticalDir),
            Opacity = MathF.Pow(resource.Animation, 1.5f),
            ElapsedTime = GetElapsedTime()
        };
    }

    internal float GetElapsedTime() {
        return (float)Math.Max(Main._drawInterfaceGameTime.ElapsedGameTime.TotalSeconds, float.Epsilon);
    }

    public CustomResourceUI() : base("PlayerResources", AequusUI.InterfaceLayers.ResourceBars_26, InterfaceScaleType.UI) { }
}
#endif