using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace AequusRemake.Content.Backpacks;

public class BackpackPlayer : ModPlayer {
    public BackpackData[] backpacks;
    private readonly List<(string, TagCompound)> _unloadedBackpacks = new();

    public override void Initialize() {
        BackpackLoader.InitializeBackpacks(ref backpacks);
        backpacks = new BackpackData[BackpackLoader.Count];
        for (int i = 0; i < backpacks.Length; i++) {
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
        itemConsumedCallback = (item, slot) => item.NetStateChanged();
        return BackpackLoader.GetExtraCraftingItems(Player, backpacks);
    }

    public override void SaveData(TagCompound tag) {
        BackpackLoader.SaveBackpacks(tag, backpacks, _unloadedBackpacks);
    }

    public override void LoadData(TagCompound tag) {
        _unloadedBackpacks.Clear();
        BackpackLoader.LoadBackpacks(tag, backpacks, _unloadedBackpacks);
    }

    #region Networking
    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) {
        if (newPlayer) {
            for (int i = 0; i < backpacks.Length; i++) {
                GetPacket<BackpackPlayerSyncPacket>().Send(Player, backpacks[i], toWho, fromWho);
            }
        }
    }

    public override void CopyClientState(ModPlayer targetCopy) {
        BackpackPlayer clone = (BackpackPlayer)targetCopy;

        for (int i = 0; i < backpacks.Length; i++) {
            backpacks[i].CopyClientState(Player, clone.backpacks[i]);
        }
    }

    public override void SendClientChanges(ModPlayer clientPlayer) {
        BackpackPlayer clone = (BackpackPlayer)clientPlayer;

        for (int i = 0; i < backpacks.Length; i++) {
            backpacks[i].SyncNetStates(Player, clone.backpacks[i]);
        }
    }
    #endregion
}
