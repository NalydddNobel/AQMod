using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Aequus.Content.Fishing.CrabPots;

public class CrabPotLootTable : ILoadable {
    public static Dictionary<int, List<CrabPotCatchRule>> Table { get; private set; } = new();

    public void Load(Mod mod) {
        Add(WaterStyleID.Purity, new(ItemID.Bass, 1));
        Add(WaterStyleID.Purity, new(ItemID.Damselfish, 1, Condition: (x, y) => Helper.ZoneSkyHeight(y)));
        Add(WaterStyleID.Purity, new(ItemID.BlueJellyfish, 30, Condition: (x, y) => y > Main.worldSurface));
        Add(WaterStyleID.Purity, new(ItemID.PinkJellyfish, 30, Condition: WorldGen.oceanDepths));
        Add(WaterStyleID.Purity, new(ItemID.GreenJellyfish, 30, Condition: (x, y) => Main.hardMode && y > Main.worldSurface));
        Add(WaterStyleID.Purity, new(ItemID.RedSnapper, 2, Condition: WorldGen.oceanDepths));
        Add(WaterStyleID.Purity, new(ItemID.Shrimp, 2, Condition: WorldGen.oceanDepths));
        Add(WaterStyleID.Purity, new(ItemID.Trout, 2, Condition: WorldGen.oceanDepths));
        Add(WaterStyleID.Purity, new(ItemID.Tuna, 2, Condition: WorldGen.oceanDepths));

        Add(WaterStyleID.Cavern, new(ItemID.SpecularFish, 1));
        Add(WaterStyleID.Cavern, new(ItemID.ArmoredCavefish, 1));
        Add(WaterStyleID.Cavern, new(ItemID.Stinkfish, 10));

        Add(WaterStyleID.Desert, new(ItemID.Flounder, 1));
        Add(WaterStyleID.Desert, new(ItemID.RockLobster, 1));

        Add(WaterStyleID.Snow, new(ItemID.AtlanticCod, 1));
        Add(WaterStyleID.Snow, new(ItemID.FrostMinnow, 1));

        Add(WaterStyleID.Jungle, new(ItemID.DoubleCod, 1));
        Add(WaterStyleID.Jungle, new(ItemID.NeonTetra, 1));
        Add(WaterStyleID.Jungle, new(ItemID.VariegatedLardfish, 1));

        Add(WaterStyleID.Corrupt, new(ItemID.Ebonkoi, 1));

        Add(WaterStyleID.Crimson, new(ItemID.CrimsonTigerfish, 1));
        Add(WaterStyleID.Crimson, new(ItemID.Hemopiranha, 1));

        Add(WaterStyleID.Hallow, new(ItemID.ChaosFish, 1));
        Add(WaterStyleID.Hallow, new(ItemID.PrincessFish, 1));
        Add(WaterStyleID.Hallow, new(ItemID.Prismite, 1));

        Add(WaterStyleID.Lava, new(ItemID.FlarefinKoi, 1));
        Add(WaterStyleID.Lava, new(ItemID.Obsidifish, 1));

        Add(WaterStyleID.Honey, new(ItemID.Honeyfin, 1));
    }

    public static void Add(int waterStyle, CrabPotCatchRule rule) {
        (CollectionsMarshal.GetValueRefOrAddDefault(Table, waterStyle, out _) ??= new()).Add(rule);
    }

    public void Unload() {
        Table.Clear();
    }
}