using Aequus.Common.Items;
using Aequus.Common.Preferences;
using Aequus.Systems;
using System;

namespace Aequus.Content.Villagers.SkyMerchant;

public partial class SkyMerchant {
    public override void AddShops() {
        int celestialMagnetAltId = GameplayConfig.Instance.MoveTreasureMagnet ? ItemID.TreasureMagnet : ItemID.CelestialMagnet;

        new NPCShop(Type, "Shop")
            .AddWithCustomValue<Items.Accessories.Balloons.SlimyBlueBalloon>(ItemDefaults.NPCSkyMerchantCustomPrice, TimeSystem.ConditionBetweenDaysOfWeek(DayOfWeek.Sunday, DayOfWeek.Monday))
            .AddWithCustomValue<Items.Accessories.GoldenFeather.GoldenFeather>(ItemDefaults.NPCSkyMerchantCustomPrice, TimeSystem.ConditionBetweenDaysOfWeek(DayOfWeek.Monday, DayOfWeek.Tuesday))
            .AddWithCustomValue(celestialMagnetAltId, ItemDefaults.NPCSkyMerchantCustomPrice, TimeSystem.ConditionBetweenDaysOfWeek(DayOfWeek.Tuesday, DayOfWeek.Wednesday))
            //.AddWithCustomValue<StunGun>(ItemDefaults.NPCSkyMerchantCustomPrice, TimeSystem.ConditionBetweenDaysOfWeek(DayOfWeek.Wednesday, DayOfWeek.Thursday))
            //.AddWithCustomValue<WeightedHorseshoe>(ItemDefaults.NPCSkyMerchantCustomPrice, TimeSystem.ConditionBetweenDaysOfWeek(DayOfWeek.Thursday, DayOfWeek.Friday))
            //.AddWithCustomValue<Furystar>(ItemDefaults.NPCSkyMerchantCustomPrice, TimeSystem.ConditionBetweenDaysOfWeek(DayOfWeek.Friday, DayOfWeek.Saturday))
            .AddWithCustomValue<Items.Accessories.FlashwayShield.FlashwayShield>(ItemDefaults.NPCSkyMerchantCustomPrice, TimeSystem.ConditionByDayOfWeek(DayOfWeek.Saturday))
            //.AddWithCustomValue(ModContent.GetInstance<HotAirBalloonMount>().MountItem.Type, ItemDefaults.NPCSkyMerchantCustomPrice * 7)
            .AddWithCustomValue<global::Aequus.Items.Weapons.Ranged.Bows.SkyHunterCrossbow.SkyHunterCrossbow>((int)(ItemDefaults.NPCSkyMerchantCustomPrice * 1.5))
            .AddWithCustomValue<global::Aequus.Items.Tools.Pumpinator>(ItemDefaults.NPCSkyMerchantCustomPrice)
            .AddWithCustomValue<global::Aequus.Items.Tools.Bellows>(ItemDefaults.NPCSkyMerchantCustomPrice)
            .Add<global::Aequus.Items.Potions.Healing.Restoration.LesserRestorationPotion>()
            //.Add<Calendar>()
            .Add<Systems.Renaming.NameTag>()
            .Register();
    }
}