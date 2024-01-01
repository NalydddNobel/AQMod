using Aequus.Content.Tiles.Banners;
using Aequus.Core.Initialization;
using System.Collections.Generic;

namespace Aequus.Tiles.Banners;

public class BannerLoader : GlobalNPC {
    public static readonly Dictionary<int, ModItem> NPCToBannerItemId = new();

    public static void RegisterBanner(ModNPC modNPC, int legacyId = -1) {
        var tile = new InstancedBannerTile(modNPC);
        var item = new InstancedBannerItem(modNPC, tile);
        var mod = modNPC.Mod;
        mod.AddContent(tile);
        mod.AddContent(item);

        if (legacyId > -1) {
            MonsterBanners.StyleToNewBannerTileConversion.Add((byte)legacyId, tile);
        }

        LoadingSteps.EnqueuePostSetupContent(() => NPCToBannerItemId.Add(modNPC.Type, item));
    }

    public override void Unload() {
        NPCToBannerItemId.Clear();
    }

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
        return NPCToBannerItemId.ContainsKey(entity.type);
    }

    public override void SetDefaults(NPC entity) {
        if (NPCToBannerItemId.TryGetValue(entity.type, out var banner)) {
            entity.ModNPC.Banner = entity.ModNPC.Type;
            entity.ModNPC.BannerItem = banner.Type;
        }
    }
}