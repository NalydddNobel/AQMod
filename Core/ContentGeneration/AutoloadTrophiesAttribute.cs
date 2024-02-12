using Aequus.Content.Bosses;
using Aequus.Core.Assets;
using Aequus.Core.Initialization;
using System;

namespace Aequus.Core.ContentGeneration;

[AttributeUsage(AttributeTargets.Class)]
internal class AutoloadTrophiesAttribute : AutoloadXAttribute {
    private readonly int _legacyId;

    public AutoloadTrophiesAttribute(int legacyId = -1) {
        _legacyId = legacyId;
    }

    internal override void Load(ModType modType) {
        if (modType is not ModNPC modNPC) {
            throw new Exception($"{nameof(AutoloadTrophiesAttribute)} can only be applied to ModNPCs.");
        }

        string texturePath = $"{modNPC.NamespaceFilePath()}/Items/{modNPC.Name}Relic";
        IRelicRenderer renderer = new BasicRelicRenderer(new RequestCache<Texture2D>(texturePath));

        BossItemInstantiator.AddTrophies(modNPC, renderer, _legacyId);
    }
}
