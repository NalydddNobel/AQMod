using Aequus.Content.Tiles.Banners;
using Aequus.Core.ContentGeneration;
using System.Collections.Generic;
using System.Linq;

namespace Aequus.Core.Initialization;

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

    /// <summary>
    /// Adds a banner buff for this NPC to <typeparamref name="TBannerNPC"/>'s registered banner. This is used grant a buff against Chained Souls when near the Keeper's Banner.
    /// </summary>
    /// <typeparam name="TBannerNPC"></typeparam>
    /// <param name="npcWithoutBanner"></param>
    public static void AddBannerBuff<TBannerNPC>(ModNPC npcWithoutBanner) where TBannerNPC : ModNPC {
        foreach (var b in ModContent.GetInstance<TBannerNPC>().Mod.GetContent<InstancedBannerTile>().Where(b => b._modNPC is TBannerNPC)) {
            b.AddNPCBuff(npcWithoutBanner);
        }
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