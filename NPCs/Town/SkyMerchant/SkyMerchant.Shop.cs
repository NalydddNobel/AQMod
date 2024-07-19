using Aequus.Common.Items;
using Aequus.Common.Preferences;

namespace Aequus.NPCs.Town.SkyMerchant;

public partial class SkyMerchant {
    public override void AddShops() {
        int celestialMagnetAltId = GameplayConfig.Instance.MoveTreasureMagnet ? ItemID.TreasureMagnet : ItemID.CelestialMagnet;

        new NPCShop(Type, "Shop")
            //.AddCustomValue<SlimyBlueBalloon>(Commons.Cost.NPCSkyMerchantCustomPrice, Commons.Conditions.BetweenDays(DayOfWeek.Sunday, DayOfWeek.Monday))
            //.AddCustomValue<GoldenFeather>(Commons.Cost.NPCSkyMerchantCustomPrice, Commons.Conditions.BetweenDays(DayOfWeek.Monday, DayOfWeek.Tuesday))
            //.AddCustomValue(celestialMagnetAltId, ItemDefaults.NPCSkyMerchantCustomPrice, Commons.Conditions.BetweenDays(DayOfWeek.Tuesday, DayOfWeek.Wednesday))
            //.AddCustomValue<StunGun>(Commons.Cost.NPCSkyMerchantCustomPrice, Commons.Conditions.BetweenDays(DayOfWeek.Wednesday, DayOfWeek.Thursday))
            //.AddCustomValue<WeightedHorseshoe>(Commons.Cost.NPCSkyMerchantCustomPrice, Commons.Conditions.BetweenDays(DayOfWeek.Thursday, DayOfWeek.Friday))
            //.AddCustomValue<Furystar>(Commons.Cost.NPCSkyMerchantCustomPrice, Commons.Conditions.BetweenDays(DayOfWeek.Friday, DayOfWeek.Saturday))
            //.AddCustomValue<FlashwayShield>(Commons.Cost.NPCSkyMerchantCustomPrice, Commons.Conditions.DayOfTheWeek(DayOfWeek.Saturday))
            //.AddCustomValue(ModContent.GetInstance<HotAirBalloonMount>().MountItem.Type, Commons.Cost.NPCSkyMerchantCustomPrice * 7)
            .AddWithCustomValue<Items.Weapons.Ranged.Bows.SkyHunterCrossbow.SkyHunterCrossbow>((int)(ItemDefaults.NPCSkyMerchantCustomPrice * 1.5))
            .AddWithCustomValue<Items.Tools.Pumpinator>(ItemDefaults.NPCSkyMerchantCustomPrice)
            .AddWithCustomValue<Items.Tools.Bellows>(ItemDefaults.NPCSkyMerchantCustomPrice)
            .Add<Items.Potions.Healing.Restoration.LesserRestorationPotion>()
            //.Add<Calendar>()
            .Add<Systems.Renaming.NameTag>()
            .Register();
    }
}