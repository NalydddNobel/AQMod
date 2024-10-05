using Aequus.Common.Items;
using Aequus.Common.Preferences;
using Aequus.Content.Systems.Seasons;
using System;

namespace Aequus.Content.Villagers.SkyMerchant;

public partial class SkyMerchant {
    public override void AddShops() {
        int celestialMagnetAltId = GameplayConfig.Instance.MoveTreasureMagnet ? ItemID.TreasureMagnet : ItemID.CelestialMagnet;

        new NPCShop(Type, "Shop")
            .AddWithCustomValue<Items.Accessories.Balloons.SlimyBlueBalloon>(ItemDefaults.NPCSkyMerchantCustomPrice, TimeCondition.BetweenWeekdays(DayOfWeek.Sunday, DayOfWeek.Monday))
            .AddWithCustomValue<Items.Accessories.RespawnFeather.GoldenFeather>(ItemDefaults.NPCSkyMerchantCustomPrice, TimeCondition.BetweenWeekdays(DayOfWeek.Monday, DayOfWeek.Tuesday))
            .AddWithCustomValue(celestialMagnetAltId, ItemDefaults.NPCSkyMerchantCustomPrice, TimeCondition.BetweenWeekdays(DayOfWeek.Tuesday, DayOfWeek.Wednesday))
            .AddWithCustomValue<Items.Weapons.Classless.StunGun.StunGun>(ItemDefaults.NPCSkyMerchantCustomPrice, TimeCondition.BetweenWeekdays(DayOfWeek.Wednesday, DayOfWeek.Thursday))
            .AddWithCustomValue<Items.Accessories.FallSpeedHorseshoe.WeightedHorseshoe>(ItemDefaults.NPCSkyMerchantCustomPrice, TimeCondition.BetweenWeekdays(DayOfWeek.Thursday, DayOfWeek.Friday))
            .AddWithCustomValue<Items.Weapons.MagicStaffs.Furystar.Furystar>(ItemDefaults.NPCSkyMerchantCustomPrice, TimeCondition.BetweenWeekdays(DayOfWeek.Friday, DayOfWeek.Saturday))
            .AddWithCustomValue<Items.Accessories.FlashwayShield.FlashwayShield>(ItemDefaults.NPCSkyMerchantCustomPrice, TimeCondition.Weekday(DayOfWeek.Saturday))
            .AddWithCustomValue(ModContent.GetInstance<Mounts.HotAirBalloon.HotAirBalloonMount>().MountItem.Type, ItemDefaults.NPCSkyMerchantCustomPrice * 7)
            .AddWithCustomValue<Items.Weapons.RangedBows.SkyHunterCrossbow.SkyHunterCrossbow>((int)(ItemDefaults.NPCSkyMerchantCustomPrice * 1.5))
            .AddWithCustomValue<global::Aequus.Items.Tools.Pumpinator>(ItemDefaults.NPCSkyMerchantCustomPrice)
            .AddWithCustomValue<global::Aequus.Items.Tools.Bellows>(ItemDefaults.NPCSkyMerchantCustomPrice)
            .AddWithCustomValue<global::Aequus.Items.Tools.FishingPoles.Nimrod>(ItemDefaults.NPCSkyMerchantCustomPrice)
            .AddWithCustomValue<Items.Accessories.ValentinesRing.ValentineRing>(ItemDefaults.NPCSkyMerchantCustomPrice)
            .Add<global::Aequus.Items.Potions.Healing.Restoration.LesserRestorationPotion>()
            .Add<Items.Accessories.Informational.Calendar.Calendar>()
            .AddWithCustomValue(ItemID.WormholePotion, Item.buyPrice(silver: 8), Condition.Multiplayer)
            .Add<global::Aequus.Systems.Renaming.NameTag>()

            .Add(Instance<Items.Vanity.SetSkyMerchant.SkyMerchantVanity>().Hat!.Type, Condition.Halloween)

            .Register();
    }
}