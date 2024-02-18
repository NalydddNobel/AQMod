using Aequus.Content.Bosses;
using Aequus.Core.Assets;
using Aequus.Core.Initialization;
using System;

namespace Aequus.Core.ContentGeneration;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal sealed class AutoloadTrophiesAttribute : AutoloadXAttribute {
    private readonly int _legacyId;

    private Type _relicRendererType;

    public AutoloadTrophiesAttribute(int legacyId = -1, Type relicRenderer = null) {
        _legacyId = legacyId;
        _relicRendererType = relicRenderer;
    }

    internal override void Load(ModType modType) {
        if (modType is not ModNPC modNPC) {
            throw new Exception($"{nameof(AutoloadTrophiesAttribute)} can only be applied to ModNPCs.");
        }

        string texturePath = $"{modNPC.NamespaceFilePath()}/Items/{modNPC.Name}Relic";
        RequestCache<Texture2D> requestCache = new RequestCache<Texture2D>(texturePath);

        IRelicRenderer renderer;
        if (_relicRendererType != null ) {
            renderer = (IRelicRenderer)Activator.CreateInstance(_relicRendererType, new object[] { requestCache });
        }
        else {
            renderer = new BasicRelicRenderer(requestCache);
        }

        BossItemInstantiator.AddTrophies(modNPC, renderer, _legacyId);
    }

    // Adding relics & trophies to drop pools is handled in AutoNPCDefaults.ModifyNPCLoot
}
