using Aequus.Common.Particles;
using Aequus.Items.Accessories.CrownOfBlood.Projectiles;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Common.Items.EquipmentBooster; 

public class EquipBoostDatabase : ILoadable {
    public static EquipBoostDatabase Instance { get; private set; }
    public static EquipBoostEntry NoEffect { get; private set; }

    private EquipBoostEntry[] _entries;
    public EquipBoostEntry[] Entries => _entries;
    public readonly Dictionary<int, Action<Item, Player, bool>> SpecialUpdate = new();
    public readonly Dictionary<int, Action<IEntitySource, Item, Projectile>> OnSpawnProjectile = new();

    public static LocalizedText VanillaItemTooltip(int itemId) {
        return Language.GetText($"Mods.Aequus.Items.BoostTooltips.{ItemID.Search.GetName(itemId)}");
    }
    public static LocalizedText ModItemTooltip(ModItem modItem) {
        return Language.GetText($"Mods.{modItem.Mod.Name}.Items.{modItem.Name}.BoostTooltip.{(modItem.Mod is Aequus ? "" : "Aequus_")}BoostTooltip");
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
    public void SetEntry(ModItem modItem, EquipBoostEntry.CustomUpdateMethod customUpdateMethod) {
        SetEntry(modItem.Type, new(ModItemTooltip(modItem), customUpdateMethod));
    }
    public void SetEntry(ModItem modItem) {
        SetEntry(modItem, customUpdateMethod: null);
    }

    #region Special Methods
    public static void SpecialUpdate_ShieldOfCthulhu(Item item, Player player, bool hideVisual) {
        if (player.dashType != 2) {
            return;
        }
        if (player.timeSinceLastDashStarted == 1 && (player.controlLeft | player.controlRight)) {
            //SoundEngine.PlaySound(SoundID.ForceRoar with { Pitch = 0.5f, }, player.Center);
            player.velocity.X = Math.Max(Math.Abs(player.velocity.X), 19f) * player.direction;
        }
        if (player.dashDelay < 0) {
            int particleCount = Math.Clamp((int)(Math.Abs(player.velocity.X) / 6f), 1, 3);
            for (int i = 0; i < particleCount; i++) {
                ParticleSystem.New<MonoBloomParticle>(ParticleLayer.BehindPlayers)
                    .Setup(
                    Main.rand.NextFromRect(player.Hitbox),
                    -player.velocity * Main.rand.NextFloat(0.3f),
                    Color.Red with { A = 20 } * 0.6f,
                    Color.Red with { A = 0 } * 0.16f,
                    Main.rand.NextFloat(0.7f, 1.4f),
                    0.3f
                );
            }
        }
        if (player.dashDelay > 1) {
            player.dashDelay--;
        }
        //if (player.eocDash > 0 && Main.myPlayer == player.whoAmI && player.ownedProjectileCounts[ModContent.ProjectileType<ShieldOfCthulhuBoost>()] <= 0) {
        //    Projectile.NewProjectile(player.GetSource_Accessory(item), player.Center, new Vector2(player.direction, 0f), ModContent.ProjectileType<ShieldOfCthulhuBoost>(), player.GetWeaponDamage(item) * 2, 1f, player.whoAmI);
        //}
    }

    public static void SpecialUpdate_BrainOfConfusion(Item item, Player player, bool hideVisual) {
        player.Aequus().flatDamageReduction += 17;
    }

    public static void SpecialUpdate_HivePack(Item item, Player player, bool hideVisual) {
        player.Aequus().crownOfBloodBees++;
    }

    public static void SpecialUpdate_BoneHelm(Item item, Player player, bool hideVisual) {
        player.Aequus().crownOfBloodDeerclops++;
    }

    public static void SpecialUpdate_RoyalGel(Item item, Player player, bool hideVisual) {
        player.Aequus().crownOfBloodFriendlySlimes++;
    }

    public static void OnSpawn_BoneGlove(IEntitySource source, Item item, Projectile projectile) {
        if (projectile.type == ProjectileID.BoneGloveProj) {
            projectile.Aequus().transform = ModContent.ProjectileType<Bonesaw>();
            projectile.velocity *= 1.25f;
            projectile.damage = (int)(projectile.damage * 1.5f);
        }
    }

    public static void OnSpawn_VolatileGelatin(IEntitySource source, Item item, Projectile projectile) {
        if (projectile.type == ProjectileID.VolatileGelatinBall) {
            projectile.Aequus().transform = ModContent.ProjectileType<ThermiteGel>();
            projectile.velocity *= 1.25f;
            projectile.damage = (int)(projectile.damage * 1.5f);
        }
    }

    public static void OnSpawn_BoneHelm(IEntitySource source, Item item, Projectile projectile) {
        if (projectile.type == ProjectileID.InsanityShadowFriendly) {
            projectile.extraUpdates++;
            projectile.damage = (int)(projectile.damage * 1.5f);
        }
    }
    #endregion

    #region Loading
    private static bool NoEffectBoost(Player player, Item item) {
        return false;
    }

    private void LoadNoBoostEntries() {
        SetEntry(ItemID.ShinyRedBalloon, NoEffect);
        SetEntry(ItemID.BlizzardinaBalloon, NoEffect);
        SetEntry(ItemID.BlueHorseshoeBalloon, NoEffect);
        SetEntry(ItemID.BundleofBalloons, NoEffect);
        SetEntry(ItemID.HoneyBalloon, NoEffect);
        SetEntry(ItemID.CloudinaBalloon, NoEffect);
        SetEntry(ItemID.FartInABalloon, NoEffect);
        SetEntry(ItemID.WhiteHorseshoeBalloon, NoEffect);
        SetEntry(ItemID.YellowHorseshoeBalloon, NoEffect);
        SetEntry(ItemID.SharkronBalloon, NoEffect);
        SetEntry(ItemID.TsunamiInABottle, NoEffect);
        SetEntry(ItemID.BlizzardinaBottle, NoEffect);
        SetEntry(ItemID.CloudinaBottle, NoEffect);
        SetEntry(ItemID.SandstorminaBottle, NoEffect);
        SetEntry(ItemID.FartinaJar, NoEffect);
        SetEntry(ItemID.BalloonPufferfish, NoEffect);
        SetEntry(ItemID.HorseshoeBundle, NoEffect);
        SetEntry(ItemID.BalloonHorseshoeFart, NoEffect);
        SetEntry(ItemID.BalloonHorseshoeHoney, NoEffect);
        SetEntry(ItemID.BalloonHorseshoeSharkron, NoEffect);
        SetEntry(ItemID.LuckyHorseshoe, NoEffect);
        SetEntry(ItemID.ObsidianHorseshoe, NoEffect);
        SetEntry(ItemID.SandstorminaBalloon, NoEffect);

        SetEntry(ItemID.CelestialStone, NoEffect);
        SetEntry(ItemID.CelestialShell, NoEffect);
        SetEntry(ItemID.MoonShell, NoEffect);
        SetEntry(ItemID.NeptunesShell, NoEffect);
        SetEntry(ItemID.MoonCharm, NoEffect);
        SetEntry(ItemID.SunStone, NoEffect);
        SetEntry(ItemID.MoonStone, NoEffect);

        SetEntry(ItemID.HermesBoots, NoEffect);
        SetEntry(ItemID.RocketBoots, NoEffect);
        SetEntry(ItemID.IceSkates, NoEffect);
        SetEntry(ItemID.FloatingTube, NoEffect);
        SetEntry(ItemID.WaterWalkingBoots, NoEffect);
        SetEntry(ItemID.ObsidianWaterWalkingBoots, NoEffect);
        SetEntry(ItemID.Flipper, NoEffect);
        SetEntry(ItemID.FlyingCarpet, NoEffect);
        SetEntry(ItemID.PortableStool, NoEffect);
        SetEntry(ItemID.TigerClimbingGear, NoEffect);
        SetEntry(ItemID.CopperWatch, NoEffect);
        SetEntry(ItemID.TinWatch, NoEffect);
        SetEntry(ItemID.SilverWatch, NoEffect);
        SetEntry(ItemID.TungstenWatch, NoEffect);
        SetEntry(ItemID.GoldWatch, NoEffect);
        SetEntry(ItemID.PlatinumWatch, NoEffect);
        SetEntry(ItemID.DepthMeter, NoEffect);
        SetEntry(ItemID.Compass, NoEffect);
        SetEntry(ItemID.Radar, NoEffect);
        SetEntry(ItemID.LifeformAnalyzer, NoEffect);
        SetEntry(ItemID.TallyCounter, NoEffect);
        SetEntry(ItemID.MetalDetector, NoEffect);
        SetEntry(ItemID.Stopwatch, NoEffect);
        SetEntry(ItemID.DPSMeter, NoEffect);
        SetEntry(ItemID.FishermansGuide, NoEffect);
        SetEntry(ItemID.WeatherRadio, NoEffect);
        SetEntry(ItemID.Sextant, NoEffect);
        SetEntry(ItemID.GPS, NoEffect);
        SetEntry(ItemID.REK, NoEffect);
        SetEntry(ItemID.GoblinTech, NoEffect);
        SetEntry(ItemID.FishFinder, NoEffect);
        SetEntry(ItemID.PDA, NoEffect);
        SetEntry(ItemID.MechanicalLens, NoEffect);
        SetEntry(ItemID.LaserRuler, NoEffect);
        SetEntry(ItemID.StarVeil, NoEffect);
        SetEntry(ItemID.DiscountCard, NoEffect);
        SetEntry(ItemID.LuckyCoin, NoEffect);
        SetEntry(ItemID.CrossNecklace, NoEffect);
        SetEntry(ItemID.AdhesiveBandage, NoEffect);
        SetEntry(ItemID.Bezoar, NoEffect);
        SetEntry(ItemID.ArmorPolish, NoEffect);
        SetEntry(ItemID.Blindfold, NoEffect);
        SetEntry(ItemID.FastClock, NoEffect);
        SetEntry(ItemID.Megaphone, NoEffect);
        SetEntry(ItemID.Nazar, NoEffect);
        SetEntry(ItemID.Vitamins, NoEffect);
        SetEntry(ItemID.TrifoldMap, NoEffect);
        SetEntry(ItemID.ArmorBracing, NoEffect);
        SetEntry(ItemID.MedicatedBandage, NoEffect);
        SetEntry(ItemID.ThePlan, NoEffect);
        SetEntry(ItemID.CountercurseMantra, NoEffect);
        SetEntry(ItemID.PaladinsShield, NoEffect);
        SetEntry(ItemID.BlackBelt, NoEffect);
        SetEntry(ItemID.Tabi, NoEffect);
        SetEntry(ItemID.HoneyComb, NoEffect);
        SetEntry(ItemID.GravityGlobe, NoEffect);
        SetEntry(ItemID.MasterNinjaGear, NoEffect);
        SetEntry(ItemID.JellyfishNecklace, NoEffect);
        SetEntry(ItemID.RifleScope, NoEffect);
        SetEntry(ItemID.PanicNecklace, NoEffect);
        SetEntry(ItemID.FrozenTurtleShell, NoEffect);
        SetEntry(ItemID.ClothierVoodooDoll, NoEffect);
        SetEntry(ItemID.GuideVoodooDoll, NoEffect);
        SetEntry(ItemID.MagmaStone, NoEffect);
        SetEntry(ItemID.ObsidianRose, NoEffect);
        SetEntry(ItemID.SweetheartNecklace, NoEffect);
        SetEntry(ItemID.FlurryBoots, NoEffect);
        SetEntry(ItemID.SailfishBoots, NoEffect);
        SetEntry(ItemID.HandWarmer, NoEffect);
        SetEntry(ItemID.GoldRing, NoEffect);
        SetEntry(ItemID.GreedyRing, NoEffect);
        SetEntry(ItemID.CoinRing, NoEffect);
        SetEntry(ItemID.FlowerBoots, NoEffect);
        SetEntry(ItemID.CordageGuide, NoEffect);
        SetEntry(ItemID.YoyoBag, NoEffect);
        SetEntry(ItemID.YoYoGlove, NoEffect);
        SetEntry(ItemID.BlackString, NoEffect);
        SetEntry(ItemID.BlueString, NoEffect);
        SetEntry(ItemID.BrownString, NoEffect);
        SetEntry(ItemID.CyanString, NoEffect);
        SetEntry(ItemID.GreenString, NoEffect);
        SetEntry(ItemID.LimeString, NoEffect);
        SetEntry(ItemID.OrangeString, NoEffect);
        SetEntry(ItemID.PinkString, NoEffect);
        SetEntry(ItemID.PurpleString, NoEffect);
        SetEntry(ItemID.RainbowString, NoEffect);
        SetEntry(ItemID.RedString, NoEffect);
        SetEntry(ItemID.SkyBlueString, NoEffect);
        SetEntry(ItemID.TealString, NoEffect);
        SetEntry(ItemID.VioletString, NoEffect);
        SetEntry(ItemID.WhiteString, NoEffect);
        SetEntry(ItemID.YellowString, NoEffect);
        SetEntry(ItemID.BlackCounterweight, NoEffect);
        SetEntry(ItemID.BlueCounterweight, NoEffect);
        SetEntry(ItemID.GreenCounterweight, NoEffect);
        SetEntry(ItemID.PurpleCounterweight, NoEffect);
        SetEntry(ItemID.RedCounterweight, NoEffect);
        SetEntry(ItemID.YellowCounterweight, NoEffect);
        SetEntry(ItemID.PhilosophersStone, NoEffect);
        SetEntry(ItemID.SpectreBoots, NoEffect);
        SetEntry(ItemID.SpectreGoggles, NoEffect);
        SetEntry(ItemID.HellfireTreads, NoEffect);
        SetEntry(ItemID.LavaFishingHook, NoEffect);
        SetEntry(ItemID.StarCloak, NoEffect);
        SetEntry(ItemID.CobaltShield, NoEffect);
        SetEntry(ItemID.ObsidianShield, NoEffect);
        SetEntry(ItemID.ObsidianSkull, NoEffect);
        SetEntry(ItemID.ReflectiveShades, NoEffect);
        SetEntry(ItemID.ShimmerCloak, NoEffect);

        SetEntry(ItemID.FishingBobber, NoEffect);
        SetEntry(ItemID.FishingBobberGlowingArgon, NoEffect);
        SetEntry(ItemID.FishingBobberGlowingKrypton, NoEffect);
        SetEntry(ItemID.FishingBobberGlowingViolet, NoEffect);
        SetEntry(ItemID.FishingBobberGlowingXenon, NoEffect);
        SetEntry(ItemID.FishingBobberGlowingRainbow, NoEffect);
        SetEntry(ItemID.FishingBobberGlowingStar, NoEffect);
        SetEntry(ItemID.FishingBobberGlowingLava, NoEffect);
        SetEntry(ItemID.LavaproofTackleBag, NoEffect);
        SetEntry(ItemID.TackleBox, NoEffect);
        SetEntry(ItemID.AnglerTackleBag, NoEffect);
        SetEntry(ItemID.AnglerEarring, NoEffect);
        SetEntry(ItemID.HighTestFishingLine, NoEffect);
    }

    private void LoadMiscEntries() {
        SpecialUpdate[ItemID.EoCShield] = SpecialUpdate_ShieldOfCthulhu;
        SpecialUpdate[ItemID.HiveBackpack] = SpecialUpdate_HivePack;
        SpecialUpdate[ItemID.BrainOfConfusion] = SpecialUpdate_BrainOfConfusion;
        SpecialUpdate[ItemID.BoneHelm] = SpecialUpdate_BoneHelm;
        SpecialUpdate[ItemID.RoyalGel] = SpecialUpdate_RoyalGel;
        SpecialUpdate[ItemID.VolatileGelatin] = SpecialUpdate_RoyalGel;
        OnSpawnProjectile[ItemID.BoneGlove] = OnSpawn_BoneGlove;
        OnSpawnProjectile[ItemID.VolatileGelatin] = OnSpawn_VolatileGelatin;
        OnSpawnProjectile[ItemID.BoneHelm] = OnSpawn_VolatileGelatin;
    }

    public void Load(Mod mod) {
        Instance = this;
        _entries = new EquipBoostEntry[ItemLoader.ItemCount];
        NoEffect = new(Language.GetOrRegister("Mods.Aequus.Items.BoostTooltips.NoEffect"), NoEffectBoost);
        LoadNoBoostEntries();
        LoadMiscEntries();
    }

    public void Unload() {
        _entries = null;
        Instance = null;
    }
    #endregion
}