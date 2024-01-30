using Aequus.Core.Initialization;
using System;

namespace Aequus.Content.Tiles.Banners;

[AttributeUsage(AttributeTargets.Class)]
internal class AutoloadBannerAttribute : AutoloadXAttribute {
    private readonly Int32 _legacyId;

    public AutoloadBannerAttribute(Int32 legacyId = -1) {
        _legacyId = legacyId;
    }

    internal override void Load(ModType modType) {
        if (modType is not ModNPC modNPC) {
            throw new Exception("AutoloadBannerAttribute can only be applied to ModNPCs.");
        }

        BannerLoader.RegisterBanner(modNPC, _legacyId);
    }
}
