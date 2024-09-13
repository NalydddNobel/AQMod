#if CUSTOM_RESOURCE_UI
using Aequus.Common.UI;
using System;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.UI;

namespace Aequus.Content.UI.PlayerResourceUI;

public class CustomResourceUI : NewUILayer {
    private readonly List<PlayerResource> _registeredResources = [];
    private readonly List<PlayerResource> _resources = [];

    public PlayerResource? Mana { get; internal set; }
    public PlayerResource? Minion { get; internal set; }

    public PlayerResource? ForcedVisible;

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

        PlayerResource? wantedResource = ForcedVisible;

        if (wantedResource == null) {
            Player player = Main.LocalPlayer;
            var resources = player.GetModPlayer<ResourcesPlayer>();
            Item heldItem = player.HeldItem;

            //for (int i = 0; i < _resources.Count; i++) {
            for (int i = _resources.Count - 1; i > 0; i--) {
                PlayerResource r = _resources[i];
                if (r.IsActive(player, resources, heldItem)) {
                    wantedResource = r;
                    break;
                }
            }
        }

        // Default to the mana resource if somehow null.
        wantedResource ??= Mana;

        float time = (float)Math.Max(Main._drawInterfaceGameTime.ElapsedGameTime.TotalSeconds * 3.5f, float.Epsilon);

        wantedResource!.Opacity += time;
        if (wantedResource.Opacity > 1f) {
            wantedResource.Opacity = 1f;
        }

        Vector2 origin = new Vector2(Main.screenWidth - 40f, 24f);

        wantedResource.DrawFancy(Main.spriteBatch, origin + new Vector2(100f * (1f - wantedResource.Opacity), MathF.Sin((1f - wantedResource.Opacity) * MathHelper.Pi) * 50f));
        int activeResources = 1;
        for (int i = 0; i < _resources.Count; i++) {
            PlayerResource r = _resources[i];
            if (r == wantedResource || r.Opacity <= 0f) {
                continue;
            }

            r.Opacity -= time * activeResources;
            if (r.Opacity <= 0f) {
                r.Opacity = 0f;
                continue;
            }

            activeResources++;
            r.DrawFancy(Main.spriteBatch, origin + new Vector2(100f * (1f - r.Opacity), -MathF.Sin((1f - r.Opacity) * MathHelper.Pi) * 50f));
        }

        return true;
    }

    public CustomResourceUI() : base("PlayerResources", AequusUI.InterfaceLayers.ResourceBars_26, InterfaceScaleType.UI) { }
}

public class MinionSlotResource : PlayerResource {
    public override void DrawFancy(SpriteBatch sb, Vector2 origin) {
        for (int i = 0; i < Main.LocalPlayer.maxMinions; i++) {
            Color color = ResourceColor;
            if (i >= Main.LocalPlayer.numMinions) {
                color *= 0.5f;
                color.A = 255;
            }
            sb.DrawAlign(TextureAssets.MagicPixel.Value, origin + new Vector2(16f, 16f + 32f * i), new Rectangle(0, 0, 32, 32), color * Opacity, MathHelper.PiOver4, 1f, SpriteEffects.None);
        }
    }

    public override void Load() {
        Instance<CustomResourceUI>().Minion = this;
        Priority = 1f;
        ResourceColor = new Color(220, 195, 90);
    }

    protected override bool Active(Player player, ResourcesPlayer resources, Item heldItem) {
        return heldItem.DamageType.CountsAsClass(DamageClass.Summon);
    }
}

public class ManaResource : PlayerResource {
    // Do nothing for drawing, vanilla draws this bar.
    public override void DrawFancy(SpriteBatch sb, Vector2 origin) {
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

public abstract class PlayerResource : ModType {
    public float Opacity { get; internal set; } = 1f;
    public float Priority { get; protected set; } = 0f;
    public int Height => 224;
    public Color ResourceColor { get; protected set; } = Color.White;

    public bool IsActive(Player player, ResourcesPlayer resources, Item heldItem) {
        return Active(player, resources, heldItem);
    }

    protected abstract bool Active(Player player, ResourcesPlayer resources, Item heldItem);

    public abstract void DrawFancy(SpriteBatch sb, Vector2 origin);

    protected sealed override void Register() {
        Instance<CustomResourceUI>().RegisterNew(this);
    }

    public sealed override void SetupContent() {
        SetStaticDefaults();
    }
}

public class ResourcesPlayer : ModPlayer {
}

public class ManaResourceOverride : ModResourceOverlay {
    public override bool PreDrawResource(ResourceOverlayDrawContext context) {
        CustomResourceUI resourceUI = Instance<CustomResourceUI>();
        resourceUI.Activate();
        if (resourceUI.Mana!.Opacity < 1f) {
            if (context.texture.Name.Contains("Star")) {
                context.color *= MathF.Pow(resourceUI.Mana.Opacity, 1.5f);
                context.position += new Vector2(100f * (1f - resourceUI.Mana.Opacity), MathF.Sin((1f - resourceUI.Mana.Opacity) * MathHelper.Pi) * (resourceUI.Minion.IsActive(Main.LocalPlayer, null, Main.LocalPlayer.HeldItem) ? -1 : 1) * 50f);
                context.Draw();
                return false;
            }
        }

        return true;
    }
}
#endif