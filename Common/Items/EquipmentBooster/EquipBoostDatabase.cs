using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Aequus.Common.Items.EquipmentBooster;

public class EquipBoostDatabase : ModSystem {
    public static EquipBoostDatabase Instance { get; private set; }
    public static EquipBoostEntry NoEffect { get; private set; }

    private EquipBoostEntry[] _entries;
    public EquipBoostEntry[] Entries => _entries;
    public readonly Dictionary<int, Action<Item, Player, bool>> SpecialUpdate = new();
    public readonly Dictionary<int, Action<IEntitySource, Item, Projectile>> OnSpawnProjectile = new();

    public static LocalizedText VanillaItemTooltip(int itemId) {
        return Language.GetText($"Mods.Aequus.Items.BoostTooltips.{ItemID.Search.GetName(itemId)}");
    }
    public static string ModItemKey(ModItem modItem) {
        return modItem.GetLocalizationKey($"{(modItem.Mod is Aequus ? "" : "Aequus_")}BoostTooltip");
    }
    public static LocalizedText ModItemTooltip(ModItem modItem) {
        return Language.GetOrRegister(ModItemKey(modItem));
    }

    public bool HasEntry(int itemId) {
        if (TryGetEntry(itemId, out var entry)) {
            return !entry.Invalid;
        }
        return false;
    }

    public bool TryGetEntry(int itemId, out EquipBoostEntry entry) {
        entry = default;
        if (!_entries.IndexInRange(itemId)) {
            return false;
        }

        entry = _entries[itemId];
        return true;
    }

    public void SetEntry(int itemId, EquipBoostEntry entry) {
        if (Entries.Length <= itemId) {
            Array.Resize(ref _entries, itemId + 1);
        }

        Entries[itemId] = entry;
    }

    public void SetVanillaEntry(int itemId, EquipBoostEntry.CustomUpdateMethod customUpdateMethod) {
        SetEntry(itemId, new(VanillaItemTooltip(itemId), customUpdateMethod));
    }
    public void SetVanillaEntry(int itemId) {
        SetVanillaEntry(itemId, null);
    }

    public void SetEntry(ModItem modItem, EquipBoostEntry entry) {
        SetEntry(modItem.Type, entry);
    }
    public void SetEntry(ModItem modItem, LocalizedText text, EquipBoostEntry.CustomUpdateMethod customUpdateMethod = null) {
        SetEntry(modItem.Type, new(text, customUpdateMethod));
    }
    public void SetEntry(ModItem modItem, EquipBoostEntry.CustomUpdateMethod customUpdateMethod) {
        SetEntry(modItem.Type, new(ModItemTooltip(modItem), customUpdateMethod));
    }
    public void SetEntry(ModItem modItem) {
        SetEntry(modItem, customUpdateMethod: null);
    }

    public void SetNoEffect(int itemId) {
        SetEntry(itemId, NoEffect);
    }

    #region Loading
    private bool _autoloadedEntries;

    private static bool NoEffectBoost(Player player, Item item) {
        return false;
    }

    private void LoadNoBoostEntries() {
        SetNoEffect(ItemID.FairyBoots);
        SetNoEffect(ItemID.ObsidianSkullRose);
        SetNoEffect(ItemID.TitanGlove);
        SetNoEffect(ItemID.SandBoots);
        SetNoEffect(ItemID.TreasureMagnet);
        SetNoEffect(ItemID.ShinyRedBalloon);
        SetNoEffect(ItemID.BlizzardinaBalloon);
        SetNoEffect(ItemID.BlueHorseshoeBalloon);
        SetNoEffect(ItemID.BundleofBalloons);
        SetNoEffect(ItemID.HoneyBalloon);
        SetNoEffect(ItemID.CloudinaBalloon);
        SetNoEffect(ItemID.FartInABalloon);
        SetNoEffect(ItemID.WhiteHorseshoeBalloon);
        SetNoEffect(ItemID.YellowHorseshoeBalloon);
        SetNoEffect(ItemID.SharkronBalloon);
        SetNoEffect(ItemID.TsunamiInABottle);
        SetNoEffect(ItemID.BlizzardinaBottle);
        SetNoEffect(ItemID.CloudinaBottle);
        SetNoEffect(ItemID.SandstorminaBottle);
        SetNoEffect(ItemID.FartinaJar);
        SetNoEffect(ItemID.BalloonPufferfish);
        SetNoEffect(ItemID.HorseshoeBundle);
        SetNoEffect(ItemID.BalloonHorseshoeFart);
        SetNoEffect(ItemID.BalloonHorseshoeHoney);
        SetNoEffect(ItemID.BalloonHorseshoeSharkron);
        SetNoEffect(ItemID.ObsidianHorseshoe);
        SetNoEffect(ItemID.SandstorminaBalloon);

        SetNoEffect(ItemID.CelestialStone);
        SetNoEffect(ItemID.CelestialShell);
        SetNoEffect(ItemID.MoonShell);
        SetNoEffect(ItemID.NeptunesShell);
        SetNoEffect(ItemID.MoonCharm);
        SetNoEffect(ItemID.SunStone);
        SetNoEffect(ItemID.MoonStone);

        SetNoEffect(ItemID.HermesBoots);
        SetNoEffect(ItemID.RocketBoots);
        SetNoEffect(ItemID.IceSkates);
        SetNoEffect(ItemID.FloatingTube);
        SetNoEffect(ItemID.WaterWalkingBoots);
        SetNoEffect(ItemID.ObsidianWaterWalkingBoots);
        SetNoEffect(ItemID.Flipper);
        SetNoEffect(ItemID.FlyingCarpet);
        SetNoEffect(ItemID.PortableStool);
        SetNoEffect(ItemID.TigerClimbingGear);
        SetNoEffect(ItemID.CopperWatch);
        SetNoEffect(ItemID.TinWatch);
        SetNoEffect(ItemID.SilverWatch);
        SetNoEffect(ItemID.TungstenWatch);
        SetNoEffect(ItemID.GoldWatch);
        SetNoEffect(ItemID.PlatinumWatch);
        SetNoEffect(ItemID.DepthMeter);
        SetNoEffect(ItemID.Compass);
        SetNoEffect(ItemID.Radar);
        SetNoEffect(ItemID.LifeformAnalyzer);
        SetNoEffect(ItemID.TallyCounter);
        SetNoEffect(ItemID.MetalDetector);
        SetNoEffect(ItemID.Stopwatch);
        SetNoEffect(ItemID.DPSMeter);
        SetNoEffect(ItemID.FishermansGuide);
        SetNoEffect(ItemID.WeatherRadio);
        SetNoEffect(ItemID.Sextant);
        SetNoEffect(ItemID.GPS);
        SetNoEffect(ItemID.REK);
        SetNoEffect(ItemID.GoblinTech);
        SetNoEffect(ItemID.FishFinder);
        SetNoEffect(ItemID.PDA);
        SetNoEffect(ItemID.MechanicalLens);
        SetNoEffect(ItemID.LaserRuler);
        SetNoEffect(ItemID.StarVeil);
        SetNoEffect(ItemID.DiscountCard);
        SetNoEffect(ItemID.LuckyCoin);
        SetNoEffect(ItemID.CrossNecklace);
        SetNoEffect(ItemID.AdhesiveBandage);
        SetNoEffect(ItemID.Bezoar);
        SetNoEffect(ItemID.ArmorPolish);
        SetNoEffect(ItemID.Blindfold);
        SetNoEffect(ItemID.FastClock);
        SetNoEffect(ItemID.Megaphone);
        SetNoEffect(ItemID.Nazar);
        SetNoEffect(ItemID.Vitamins);
        SetNoEffect(ItemID.TrifoldMap);
        SetNoEffect(ItemID.ArmorBracing);
        SetNoEffect(ItemID.MedicatedBandage);
        SetNoEffect(ItemID.ThePlan);
        SetNoEffect(ItemID.CountercurseMantra);
        SetNoEffect(ItemID.PaladinsShield);
        SetNoEffect(ItemID.BlackBelt);
        SetNoEffect(ItemID.Tabi);
        SetNoEffect(ItemID.HoneyComb);
        SetNoEffect(ItemID.GravityGlobe);
        SetNoEffect(ItemID.MasterNinjaGear);
        SetNoEffect(ItemID.JellyfishNecklace);
        SetNoEffect(ItemID.RifleScope);
        SetNoEffect(ItemID.PanicNecklace);
        SetNoEffect(ItemID.FrozenTurtleShell);
        SetNoEffect(ItemID.ClothierVoodooDoll);
        SetNoEffect(ItemID.GuideVoodooDoll);
        SetNoEffect(ItemID.MagmaStone);
        SetNoEffect(ItemID.ObsidianRose);
        SetNoEffect(ItemID.SweetheartNecklace);
        SetNoEffect(ItemID.FlurryBoots);
        SetNoEffect(ItemID.SailfishBoots);
        SetNoEffect(ItemID.HandWarmer);
        SetNoEffect(ItemID.GoldRing);
        SetNoEffect(ItemID.GreedyRing);
        SetNoEffect(ItemID.CoinRing);
        SetNoEffect(ItemID.FlowerBoots);
        SetNoEffect(ItemID.CordageGuide);
        SetNoEffect(ItemID.YoyoBag);
        SetNoEffect(ItemID.YoYoGlove);
        SetNoEffect(ItemID.BlackString);
        SetNoEffect(ItemID.BlueString);
        SetNoEffect(ItemID.BrownString);
        SetNoEffect(ItemID.CyanString);
        SetNoEffect(ItemID.GreenString);
        SetNoEffect(ItemID.LimeString);
        SetNoEffect(ItemID.OrangeString);
        SetNoEffect(ItemID.PinkString);
        SetNoEffect(ItemID.PurpleString);
        SetNoEffect(ItemID.RainbowString);
        SetNoEffect(ItemID.RedString);
        SetNoEffect(ItemID.SkyBlueString);
        SetNoEffect(ItemID.TealString);
        SetNoEffect(ItemID.VioletString);
        SetNoEffect(ItemID.WhiteString);
        SetNoEffect(ItemID.YellowString);
        SetNoEffect(ItemID.BlackCounterweight);
        SetNoEffect(ItemID.BlueCounterweight);
        SetNoEffect(ItemID.GreenCounterweight);
        SetNoEffect(ItemID.PurpleCounterweight);
        SetNoEffect(ItemID.RedCounterweight);
        SetNoEffect(ItemID.YellowCounterweight);
        SetNoEffect(ItemID.PhilosophersStone);
        SetNoEffect(ItemID.SpectreBoots);
        SetNoEffect(ItemID.SpectreGoggles);
        SetNoEffect(ItemID.HellfireTreads);
        SetNoEffect(ItemID.LavaFishingHook);
        SetNoEffect(ItemID.StarCloak);
        SetNoEffect(ItemID.CobaltShield);
        SetNoEffect(ItemID.ObsidianShield);
        SetNoEffect(ItemID.ObsidianSkull);
        SetNoEffect(ItemID.ReflectiveShades);
        SetNoEffect(ItemID.ShimmerCloak);

        SetNoEffect(ItemID.FishingBobber);
        SetNoEffect(ItemID.FishingBobberGlowingArgon);
        SetNoEffect(ItemID.FishingBobberGlowingKrypton);
        SetNoEffect(ItemID.FishingBobberGlowingViolet);
        SetNoEffect(ItemID.FishingBobberGlowingXenon);
        SetNoEffect(ItemID.FishingBobberGlowingRainbow);
        SetNoEffect(ItemID.FishingBobberGlowingStar);
        SetNoEffect(ItemID.FishingBobberGlowingLava);
        SetNoEffect(ItemID.LavaproofTackleBag);
        SetNoEffect(ItemID.TackleBox);
        SetNoEffect(ItemID.AnglerTackleBag);
        SetNoEffect(ItemID.AnglerEarring);
        SetNoEffect(ItemID.HighTestFishingLine);
    }

    private void LoadMiscEntries() {
        //SpecialUpdate[ItemID.EoCShield] = SpecialUpdate_ShieldOfCthulhu;
        //SpecialUpdate[ItemID.HiveBackpack] = SpecialUpdate_HivePack;
        //SpecialUpdate[ItemID.BrainOfConfusion] = SpecialUpdate_BrainOfConfusion;
        //SpecialUpdate[ItemID.BoneHelm] = SpecialUpdate_BoneHelm;
        //SpecialUpdate[ItemID.RoyalGel] = SpecialUpdate_RoyalGel;
        //SpecialUpdate[ItemID.VolatileGelatin] = SpecialUpdate_RoyalGel;
        SetNoEffect(ItemID.RoyalGel);
        //OnSpawnProjectile[ItemID.BoneGlove] = OnSpawn_BoneGlove;
        //OnSpawnProjectile[ItemID.VolatileGelatin] = OnSpawn_VolatileGelatin;
        //OnSpawnProjectile[ItemID.BoneHelm] = OnSpawn_BoneHelm;
        //OnSpawnProjectile[ItemID.SporeSac] = OnSpawn_SporeSac;
        SetNoEffect(ItemID.ShinyStone);
        SetNoEffect(ItemID.LongRainbowTrailWings);
    }

    public override void Load() {
        _autoloadedEntries = false;
        Instance = this;
        _entries = new EquipBoostEntry[ItemLoader.ItemCount];
        NoEffect = new(Language.GetOrRegister("Mods.Aequus.Items.BoostTooltips.NoEffect"), NoEffectBoost);
        LoadNoBoostEntries();
        LoadMiscEntries();
    }

    public override void OnLocalizationsLoaded() {
        if (_autoloadedEntries) {
            return;
        }
        _autoloadedEntries = true;

        for (int i = 0; i < ItemID.Count; i++) {
            if (HasEntry(i)) {
                continue;
            }

            string itemName = ItemID.Search.GetName(i);
            if (TextHelper.TryGet("Mods.Aequus.Items.BoostTooltips." + itemName, out var localizedText)) {
                _entries[i] = new(localizedText);
            }
        }

        for (int i = ItemID.Count; i < ItemLoader.ItemCount; i++) {
            var item = ItemLoader.GetItem(i);
            if (HasEntry(item.Type)) {
                continue;
            }

            string itemName = item.Name;
            if (TextHelper.TryGet(ModItemKey(item), out var localizedText)) {
                SetEntry(item.Type, new(localizedText));
            }
        }
    }

    public override void Unload() {
        _entries = null;
        Instance = null;
    }
    #endregion
}