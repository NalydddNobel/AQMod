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
            .AddWithCustomValue<Items.Weapons.Classless.StunGun.StunGun>(ItemDefaults.NPCSkyMerchantCustomPrice, TimeSystem.ConditionBetweenDaysOfWeek(DayOfWeek.Wednesday, DayOfWeek.Thursday))
            .AddWithCustomValue<Items.Accessories.WeightedHorseshoe.WeightedHorseshoe>(ItemDefaults.NPCSkyMerchantCustomPrice, TimeSystem.ConditionBetweenDaysOfWeek(DayOfWeek.Thursday, DayOfWeek.Friday))
            .AddWithCustomValue<Items.Weapons.Magic.Furystar.Furystar>(ItemDefaults.NPCSkyMerchantCustomPrice, TimeSystem.ConditionBetweenDaysOfWeek(DayOfWeek.Friday, DayOfWeek.Saturday))
            .AddWithCustomValue<Items.Accessories.FlashwayShield.FlashwayShield>(ItemDefaults.NPCSkyMerchantCustomPrice, TimeSystem.ConditionByDayOfWeek(DayOfWeek.Saturday))
            .AddWithCustomValue(ModContent.GetInstance<Mounts.HotAirBalloon.HotAirBalloonMount>().MountItem.Type, ItemDefaults.NPCSkyMerchantCustomPrice * 7)
            .AddWithCustomValue<Items.Weapons.Ranged.SkyHunterCrossbow.SkyHunterCrossbow>((int)(ItemDefaults.NPCSkyMerchantCustomPrice * 1.5))
            .AddWithCustomValue<global::Aequus.Items.Tools.Pumpinator>(ItemDefaults.NPCSkyMerchantCustomPrice)
            .AddWithCustomValue<global::Aequus.Items.Tools.Bellows>(ItemDefaults.NPCSkyMerchantCustomPrice)
            .AddWithCustomValue<global::Aequus.Items.Tools.FishingPoles.Nimrod>(ItemDefaults.NPCSkyMerchantCustomPrice)
            .Add<global::Aequus.Items.Potions.Healing.Restoration.LesserRestorationPotion>()
            .Add<Items.Accessories.Informational.Calendar.Calendar>()
            .Add<Systems.Renaming.NameTag>()
            .Register();
    }
}