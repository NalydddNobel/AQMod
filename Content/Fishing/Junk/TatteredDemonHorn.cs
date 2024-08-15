using Aequus.Common.DataSets;
using System.Collections.Generic;
using Terraria.Utilities;

namespace Aequus.Content.Fishing.Junk;

public class TatteredDemonHorn : ModItem {
    public static readonly List<(int, float)> DropPool = [
        (ItemID.Lavafly, 0.5f),
        (ItemID.MagmaSnail, 0.25f),
        (ItemID.HellButterfly, 1f)
    ];

    public override void SetStaticDefaults() {
        ItemID.Sets.ExtractinatorMode[Type] = Type;
        ItemSets.FishingTrashForDevilsTounge.Add(Type);
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.FishingSeaweed);
        Item.width = 10;
        Item.height = 16;
    }

    public override void ExtractinatorUse(int extractinatorBlockType, ref int resultType, ref int resultStack) {
        WeightedRandom<int> pool = new(Main.rand);

        foreach (var item in DropPool) {
            pool.Add(item.Item1, item.Item2);
        }

        resultType = pool.Get();
    }
}
