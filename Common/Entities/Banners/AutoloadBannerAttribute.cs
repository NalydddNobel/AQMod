using Aequus.Common.Initialization;
using System;

namespace Aequus.Common.Entities.Banners;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal sealed class AutoloadBannerAttribute(int LegacyId = -1) : Attribute, IAttributeOnModLoad {
    public void OnModLoad(Mod mod, ILoadable Content) {
        if (Content is not ModNPC modNPC) {
            throw new Exception($"{nameof(AutoloadBannerAttribute)} can only be applied to ModNPCs.");
        }

        BannerLoader.RegisterBanner(modNPC, LegacyId);
    }
}
