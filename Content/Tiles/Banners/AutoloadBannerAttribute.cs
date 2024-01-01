using Aequus.Core.Initialization;
using Aequus.Tiles.Banners;
using System;

namespace Aequus.Content.Tiles.Banners;

[AttributeUsage(AttributeTargets.Class)]
internal class AutoloadBannerAttribute : AutoloadXAttribute {
    private readonly int _legacyId;

    public AutoloadBannerAttribute(int legacyId = -1) {
        _legacyId = legacyId;
    }

    internal override void Load(ModType modType) {
        if (modType is not ModNPC modNPC) {
            throw new Exception("AutoloadBannerAttribute can only be applied to ModNPCs.");
        }

        BannerLoader.RegisterBanner(modNPC, _legacyId);
    }
}
