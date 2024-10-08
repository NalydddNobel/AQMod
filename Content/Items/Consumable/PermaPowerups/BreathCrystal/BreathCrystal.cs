using Aequus.Common.Utilities.Helpers;
using System;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Items.Consumable.PermaPowerups.BreathCrystal;

public class BreathCrystal : ModItem {
    public override void SetStaticDefaults() {
        Item.CloneResearchUnlockCount(ItemID.LifeCrystal);
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.LifeCrystal);
    }

    public override bool? UseItem(Player player) {
        if (!player.TryGetModPlayer(out BreathCrystalPlayer crystalPlayer) || crystalPlayer.numCrystalsConsumed >= BreathCrystalPlayer.MaxCrystals) {
            return false;
        }

        crystalPlayer.numCrystalsConsumed++;
        player.breath += 20;
        CombatText.NewText(player.Hitbox, CommonColor.CombatText_RestoreBreath, 1);

        return true;
    }
}

public class BreathCrystalPlayer : ModPlayer {
    public static readonly int MaxCrystals = 10;
    public int numCrystalsConsumed;

    int _modBreathMax;

    public int IncreasedBreathStat => 20 * numCrystalsConsumed;

    const string TAG = "bCrystals";

#if !AQUA_CRYSTAL
    public override bool IsLoadingEnabled(Mod mod) {
        return false;
    }
#endif

    public override void PostUpdateMiscEffects() {
        // Breath max is not reset by vanilla, so we need to keep track of our modifications.

        // This implementation tracks how much we modify the breath stat,
        // instead of simply setting breath max to 200. Since that would destroy modifiers from other mods.
        Player.breathMax -= _modBreathMax;

        // Increase the breath stat.
        int increase = IncreasedBreathStat;
        Player.breathMax += increase;

        // Note down how much we modified the max breath stat,
        // so we can subtract it properly later.
        _modBreathMax = increase;
    }

    public override void SaveData(TagCompound tag) {
        if (numCrystalsConsumed > 0) {
            tag[TAG] = numCrystalsConsumed;
        }
    }

    public override void LoadData(TagCompound tag) {
        if (tag.TryGet(TAG, out int numCrystals)) {
            numCrystalsConsumed = Math.Min(numCrystalsConsumed, MaxCrystals);
        }
    }
}