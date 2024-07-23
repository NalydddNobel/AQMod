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
            //.AddCustomValue<GoldenFeather>(Commons.Cost.NPCSkyMerchantCustomPrice, Commons.Conditions.BetweenDays(DayOfWeek.Monday, DayOfWeek.Tuesday))
            //.AddCustomValue(celestialMagnetAltId, ItemDefaults.NPCSkyMerchantCustomPrice, Commons.Conditions.BetweenDays(DayOfWeek.Tuesday, DayOfWeek.Wednesday))
            //.AddCustomValue<StunGun>(Commons.Cost.NPCSkyMerchantCustomPrice, Commons.Conditions.BetweenDays(DayOfWeek.Wednesday, DayOfWeek.Thursday))
            //.AddCustomValue<WeightedHorseshoe>(Commons.Cost.NPCSkyMerchantCustomPrice, Commons.Conditions.BetweenDays(DayOfWeek.Thursday, DayOfWeek.Friday))
            //.AddCustomValue<Furystar>(Commons.Cost.NPCSkyMerchantCustomPrice, Commons.Conditions.BetweenDays(DayOfWeek.Friday, DayOfWeek.Saturday))
            .AddWithCustomValue<Items.Accessories.FlashwayShield.FlashwayShield>(ItemDefaults.NPCSkyMerchantCustomPrice, TimeSystem.ConditionByDayOfWeek(DayOfWeek.Saturday))
            //.AddCustomValue(ModContent.GetInstance<HotAirBalloonMount>().MountItem.Type, Commons.Cost.NPCSkyMerchantCustomPrice * 7)
            .AddWithCustomValue<global::Aequus.Items.Weapons.Ranged.Bows.SkyHunterCrossbow.SkyHunterCrossbow>((int)(ItemDefaults.NPCSkyMerchantCustomPrice * 1.5))
            .AddWithCustomValue<global::Aequus.Items.Tools.Pumpinator>(ItemDefaults.NPCSkyMerchantCustomPrice)
            .AddWithCustomValue<global::Aequus.Items.Tools.Bellows>(ItemDefaults.NPCSkyMerchantCustomPrice)
            .Add<global::Aequus.Items.Potions.Healing.Restoration.LesserRestorationPotion>()
            //.Add<Calendar>()
            .Add<Systems.Renaming.NameTag>()
            .Register();
    }
}