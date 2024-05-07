using Aequus.Core.Initialization;
using System;
using tModLoaderExtended.Terraria.ModLoader;

namespace Aequus.Core.ContentGeneration;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal sealed class AutoloadBannerAttribute : Attribute, IAttributeOnModLoad {
    private readonly int _legacyId;

    public AutoloadBannerAttribute(int legacyId = -1) {
        _legacyId = legacyId;
    }

    public void OnModLoad(Mod mod, ILoad Content) {
        if (Content is not ModNPC modNPC) {
            throw new Exception($"{nameof(AutoloadBannerAttribute)} can only be applied to ModNPCs.");
        }

        BannerLoader.RegisterBanner(modNPC, _legacyId);
    }
}
