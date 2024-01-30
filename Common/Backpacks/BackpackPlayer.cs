using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Backpacks;

public class BackpackPlayer : ModPlayer {
    public BackpackData[] backpacks;
    private readonly List<(System.String, TagCompound)> _unloadedBackpacks = new();

    public override void Initialize() {
        BackpackLoader.InitializeBackpacks(ref backpacks);
        backpacks = new BackpackData[BackpackLoader.Count];
        for (System.Int32 i = 0; i < backpacks.Length; i++) {
            backpacks[i] = BackpackLoader.Backpacks[i].CreateInstance();
            backpacks[i].Type = i;
            backpacks[i].Inventory = new Item[backpacks[i].Capacity];
        }
    }

    public override void PreUpdate() {
        BackpackLoader.UpdateBackpacks(Player, backpacks);
    }

    public override void PostUpdateEquips() {
        BackpackLoader.UpdateInfoAccessories(Player, backpacks);
    }

    public override void PostUpdateBuffs() {
        BackpackLoader.ResetEffects(Player, backpacks);
    }

    public override IEnumerable<Item> AddMaterialsForCrafting(out ItemConsumedCallback itemConsumedCallback) {
        itemConsumedCallback = null;
        return BackpackLoader.GetExtraCraftingItems(Player, backpacks);
    }

    public override void SaveData(TagCompound tag) {
        BackpackLoader.SaveBackpacks(tag, backpacks, _unloadedBackpacks);
    }

    public override void LoadData(TagCompound tag) {
        _unloadedBackpacks.Clear();
        BackpackLoader.LoadBackpacks(tag, backpacks, _unloadedBackpacks);
    }
}
