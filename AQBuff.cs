using AQMod.Buffs;
using AQMod.Buffs.Debuffs;
using AQMod.Buffs.Vampire;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod
{
    public class AQBuff : GlobalBuff
    {
        public sealed class Sets
        {
            public static Sets Instance;

            public HashSet<int> CanBeRemovedByWhiteBloodCell { get; private set; }
            public HashSet<int> NoStarbyteUpgrade { get; private set; }
            public HashSet<int> FoodBuff { get; private set; }
            public HashSet<int> NoSpread { get; private set; }

            public Sets()
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
        }

        public static void WellFedMajor(Player player)
        {
            WellFedMinor(player, 4, 0.1f, 4, 0.1f, 0.4f, 1f);
        }
        public static void WellFedMedium(Player player)
        {
            WellFedMinor(player, 3, 0.075f, 3, 0.075f, 0.3f, 0.75f);
        }
        public static void WellFedMinor(Player player, int defense = 2, float damage = 0.05f, int crit = 2, float meleeSpeed = 0.05f, float moveSpeed = 0.2f, float minionKB = 0.5f)
        {
            WellFed(player, defense, damage, crit, crit, crit, meleeSpeed, moveSpeed, minionKB);
        }
        public static void WellFed(Player player, int defense = 2, float damage = 0.05f, int meleeCrit = 2, int rangedCrit = 2, int magicCrit = 2, float meleeSpeed = 0.05f, float moveSpeed = 0.2f, float minionKB = 0.5f)
        {
            player.wellFed = true;

            player.statDefense += defense;

            player.meleeCrit += meleeCrit;
            player.magicCrit += rangedCrit;
            player.rangedCrit += magicCrit;
            player.thrownCrit += meleeCrit;

            player.allDamage += damage;
            player.meleeSpeed += meleeSpeed;
            player.minionKB += minionKB;
            player.moveSpeed += moveSpeed;
        }
    }
}