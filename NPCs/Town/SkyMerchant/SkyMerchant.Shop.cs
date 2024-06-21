using System;

namespace Aequus.NPCs.Town.SkyMerchant;

public partial class SkyMerchant {
    public override void AddShops() {
        int celestialMagnetAltId = VanillaChangesConfig.Instance.MoveTreasureMagnet ? ItemID.TreasureMagnet : ItemID.CelestialMagnet;

        new NPCShop(Type, "Shop")
            .AddCustomValue<SkyHunterCrossbow>(Commons.Cost.NPCSkyMerchantCustomPrice * 1.5)
            .AddCustomValue<Bellows>(Commons.Cost.NPCSkyMerchantCustomPrice)
            .AddCustomValue(ModContent.GetInstance<HotAirBalloonMount>().MountItem.Type, Commons.Cost.NPCSkyMerchantCustomPrice * 7)
            .Add<NameTag>()
            .Add<Calendar>()
            .Add<LesserRestorationPotion>()
            .AddCustomValue<SlimyBlueBalloon>(Commons.Cost.NPCSkyMerchantCustomPrice, Commons.Conditions.BetweenDays(DayOfWeek.Sunday, DayOfWeek.Monday))
            .AddCustomValue<GoldenFeather>(Commons.Cost.NPCSkyMerchantCustomPrice, Commons.Conditions.BetweenDays(DayOfWeek.Monday, DayOfWeek.Tuesday))
            .AddCustomValue(celestialMagnetAltId, Commons.Cost.NPCSkyMerchantCustomPrice, Commons.Conditions.BetweenDays(DayOfWeek.Tuesday, DayOfWeek.Wednesday))
            .AddCustomValue<StunGun>(Commons.Cost.NPCSkyMerchantCustomPrice, Commons.Conditions.BetweenDays(DayOfWeek.Wednesday, DayOfWeek.Thursday))
            .AddCustomValue<WeightedHorseshoe>(Commons.Cost.NPCSkyMerchantCustomPrice, Commons.Conditions.BetweenDays(DayOfWeek.Thursday, DayOfWeek.Friday))
            .AddCustomValue<Furystar>(Commons.Cost.NPCSkyMerchantCustomPrice, Commons.Conditions.BetweenDays(DayOfWeek.Friday, DayOfWeek.Saturday))
            .AddCustomValue<FlashwayShield>(Commons.Cost.NPCSkyMerchantCustomPrice, Commons.Conditions.DayOfTheWeek(DayOfWeek.Saturday))
            .Register();
    }
}