using Aequus.Common.Items.Components;
using Aequus.Content.Fishing;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using Terraria.DataStructures;

namespace Aequus.Old.Content.Fishing.GimmickBait;

public class Omnibait : ModBait, IModifyFishAttempt {
    private static BitsByte[] _vanillaZoneFlags = new BitsByte[5];
    private static BitArray _moddedZoneFlags;

    private static FieldInfo FieldInfo_Player_modBiomeFlags;

    public override void Load() {
        FieldInfo_Player_modBiomeFlags = typeof(Player).GetField("modBiomeFlags", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (FieldInfo_Player_modBiomeFlags == null) {
            Mod.Logger.Error("Omnibait Reflection: \"Player.modBiomeFlags\" not found!");
        }
        else if (FieldInfo_Player_modBiomeFlags.FieldType != typeof(BitArray)) {
            Mod.Logger.Error($"Omnibait Reflection: \"Player.modBiomeFlags\" is not of type \"{typeof(BitArray).FullName}\"!");
        }
    }

    public override void SetDefaults() {
        Item.width = 6;
        Item.height = 6;
        Item.bait = 30;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.value = Item.sellPrice(silver: 20);
        Item.rare = ItemRarityID.LightPurple;
    }

    public bool PreCatchFish(Projectile bobber, ref FishingAttempt fisher) {
        Player player = Main.player[bobber.owner];
        // Shuffle the static biome flags
        ShuffleBiomeFlags(player);
        // Swap player flags with the shuffled static flags
        SwapBiomeFlags(player);

        ScrewFishingValues(ref fisher);

        LimitRandomness(player);

        return true;
    }

    private static void ScrewFishingValues(ref FishingAttempt fisher) {
        // Randomize Height.
        if (Main.rand.NextBool(4)) {
            fisher.heightLevel = Main.rand.Next(5);
        }

        // Force Lava fish pool.
        if (Main.rand.NextBool(8)) {
            //Need lava fishing for this now, for balancing
            //and also cause it grants lava fishing achievement on accident.
            //fisher.CanFishInLava = true;
            fisher.inLava = true;
        }
        // Force Honey fish pool.
        else if (Main.rand.NextBool(8)) {
            fisher.inLava = false;
            fisher.inHoney = true;
        }
        // Force Water fish pool.
        else if (Main.rand.NextBool(8)) {
            fisher.inLava = false;
            fisher.inHoney = false;
        }
    }

    private static void LimitRandomness(Player player) {
        // Disable dungeon loot pool unless Skeletron has been defeated.
        if (!NPC.downedBoss3) {
            player.ZoneDungeon = false;
        }

        // Limit Hallow/Lihzahrd Temple loot pool unless WoF has been defeated.
        if (!Main.hardMode) {
            player.ZoneLihzhardTemple = false;
            player.ZoneHallow = false;
        }

        // Limit Lihzahrd Temple loot pool unless Plantera has been defeated.
        if (!NPC.downedPlantBoss) {
            player.ZoneLihzhardTemple = false;
        }
    }

    public void PostCatchFish(Player player, FishingAttempt attempt, ref int itemDrop, ref int enemySpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition) {
        // Swap again, fixing the biome flags.
        SwapBiomeFlags(player);
    }

    private static void ShuffleBiomeFlags(Player player) {
        for (int i = 0; i < _vanillaZoneFlags.Length; i++) {
            _vanillaZoneFlags[i] = (BitsByte)Main.rand.Next(byte.MaxValue + 1);
        }

        if (TryGetModdedBiomeBitArray(player, out BitArray modBiomes)) {
            spitOutBitArray("Default", modBiomes);
            spitOutBitArray("Shuffled", _moddedZoneFlags);

            if (_moddedZoneFlags == null || _moddedZoneFlags.Length != modBiomes.Length) {
                _moddedZoneFlags = new BitArray(modBiomes.Length);
            }

            for (int i = 0; i < _moddedZoneFlags.Length; i++) {
                _moddedZoneFlags[i] = Main.rand.NextBool();
            }
        }
    }

    private static void SwapBiomeFlags(Player player) {
        Utils.Swap(ref player.zone1, ref _vanillaZoneFlags[0]);
        Utils.Swap(ref player.zone2, ref _vanillaZoneFlags[1]);
        Utils.Swap(ref player.zone3, ref _vanillaZoneFlags[2]);
        Utils.Swap(ref player.zone4, ref _vanillaZoneFlags[3]);
        Utils.Swap(ref player.zone5, ref _vanillaZoneFlags[4]);

        if (TryGetModdedBiomeBitArray(player, out BitArray modBiomes)) {
            FieldInfo_Player_modBiomeFlags.SetValue(player, _moddedZoneFlags);
            _moddedZoneFlags = modBiomes;
        }
    }

    private static bool TryGetModdedBiomeBitArray(Player player, out BitArray modBiomes) {
        if (FieldInfo_Player_modBiomeFlags == null || FieldInfo_Player_modBiomeFlags.FieldType != typeof(BitArray)) {
            modBiomes = default;
            return false;
        }

        modBiomes = (BitArray)FieldInfo_Player_modBiomeFlags.GetValue(player);
        return true;
    }

    [Conditional("DEBUG")]
    private static void spitOutBitArray(string arrName, BitArray bitArray) {
        string bitArrText = "";
        for (int i = 0; i < bitArray.Length; i++) {
            bitArrText += bitArray[i] ? "1" : "0";
        }
        Main.NewText($"{arrName} arr: {bitArrText}");
    }
}