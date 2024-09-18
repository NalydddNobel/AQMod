#if CUSTOM_RESOURCE_UI
using System;

namespace Aequus.Content.UI.PlayerResourceUI;

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