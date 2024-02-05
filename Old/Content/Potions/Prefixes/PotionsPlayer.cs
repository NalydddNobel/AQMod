using Aequus.Core.DataSets;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader.IO;

namespace Aequus.Old.Content.Potions.Prefixes;

public class PotionsPlayer : ModPlayer {
    public readonly List<int> BoundedPotionIds = new();

    public override void UpdateDead() {
        foreach (var buff in BoundedPotionIds) {
            // Set all bounded buffs to be persistent
            // Since if they were already persistant, you wouldn't be able to make the potion anyway.
            Main.persistentBuff[buff] = true;
        }
    }

    public override void ResetEffects() {
        foreach (var buff in BoundedPotionIds) {
            // Set all bounded buffs to not be persistent
            Main.persistentBuff[buff] = false;
        }
    }

    #region Save & Load
    const string TAG_BOUNDED = "Bounded";

    public override void SaveData(TagCompound tag) {
        if (BoundedPotionIds.Count > 0) {
            tag[TAG_BOUNDED] = BoundedPotionIds.Select(IDCommons<BuffID>.GetStringIdentifier).ToList();
        }
    }

    public override void LoadData(TagCompound tag) {
        if (tag.TryGet(TAG_BOUNDED, out List<string> boundedPotions)) {
            BoundedPotionIds.Clear();

            foreach (string b in boundedPotions) {
                if (BuffID.Search.TryGetId(b, out int id)) {
                    BoundedPotionIds.Add(id);
                }
            }
        }
    }
    #endregion

    #region Networking
    public override void CopyClientState(ModPlayer targetCopy) {
        PotionsPlayer copy = (PotionsPlayer)targetCopy;

        copy.BoundedPotionIds.Clear();
        copy.BoundedPotionIds.AddRange(BoundedPotionIds);
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) {
        Aequus.GetPacket<PotionsPlayerSyncPacket>().Send(Player, this, toWho, fromWho);
    }

    public override void SendClientChanges(ModPlayer clientPlayer) {
        PotionsPlayer clone = (PotionsPlayer)clientPlayer;

        if (!BoundPotionsMatch(clone)) {
            SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
        }
    }

    private bool BoundPotionsMatch(PotionsPlayer clone) {
        if (clone.BoundedPotionIds.Count != BoundedPotionIds.Count) {
            return false; // Length mismatch
        }

        for (int i = 0; i < clone.BoundedPotionIds.Count; i++) {
            if (clone.BoundedPotionIds[i] != BoundedPotionIds[i]) {
                return false; // Type mismatch
            }
        }

        return true;
    }
    #endregion
}
