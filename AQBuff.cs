using AQMod.Buffs;
using AQMod.Buffs.Debuffs;
using AQMod.Buffs.Vampire;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod
{
    public class AQBuff : GlobalBuff
    {
        public static class Sets
        {
            public static HashSet<int> CanBeRemovedByWhiteBloodCell { get; private set; }
            public static HashSet<int> NoStarbyteUpgrade { get; private set; }
            public static HashSet<int> FoodBuff { get; private set; }
            public static HashSet<int> NoSpread { get; private set; }

            internal static void Setup()
            {
                NoSpread = new HashSet<int>()
                {
                    BuffID.StardustMinionBleed,
                    BuffID.DryadsWardDebuff,
                    BuffID.Lovestruck,
                    BuffID.Stinky,
                    ModContent.BuffType<LovestruckAQ>(),
                };

                FoodBuff = new HashSet<int>()
                {
                    BuffID.WellFed,
                    ModContent.BuffType<GrapePhantaBuff>(),
                    ModContent.BuffType<SpeedBoostFood>(),
                    ModContent.BuffType<NeutronYogurtBuff>(),
                    ModContent.BuffType<DragonCarrotBuff>(),
                    ModContent.BuffType<RedLicoriceBuff>(),
                    ModContent.BuffType<BaguetteBuff>(),
                };

                NoStarbyteUpgrade = new HashSet<int>(FoodBuff)
                {
                    BuffID.Tipsy,
                    BuffID.Honey,
                    BuffID.Lifeforce,
                    ModContent.BuffType<Bossrush>(),
                    ModContent.BuffType<Spoiled>(),
                    ModContent.BuffType<UmystickDelay>(),
                    ModContent.BuffType<Vampirism>(),
                };

                CanBeRemovedByWhiteBloodCell = new HashSet<int>()
                {
                    BuffID.OnFire,
                    BuffID.Ichor,
                    BuffID.CursedInferno,
                    BuffID.Frostburn,
                    BuffID.Chilled,
                    BuffID.Bleeding,
                    BuffID.Confused,
                    BuffID.Poisoned,
                    BuffID.Venom,
                    BuffID.Darkness,
                    BuffID.Blackout,
                    BuffID.Silenced,
                    BuffID.Cursed,
                    BuffID.Slow,
                    BuffID.Weak,
                    BuffID.WitheredArmor,
                    BuffID.Electrified,
                    BuffID.Rabies,
                    BuffID.OgreSpit,
                    BuffID.VortexDebuff,
                    ModContent.BuffType<Vampirism>(),
                    ModContent.BuffType<BlueFire>(),
                    ModContent.BuffType<PickBreak>(),
                };
            }

            internal static void Unload()
            {
                NoSpread?.Clear();
                NoSpread = null;
            }
        }
    }
}