using AQMod.Buffs;
using AQMod.Buffs.Debuffs;
using AQMod.Common.ID;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod
{
    public class AQBuff : GlobalBuff
    {
        public static class Sets
        {
            public static bool[] CanBeRemovedByWhiteBloodCell { get; private set; }
            public static bool[] CantBeTurnedIntoMolite { get; private set; }
            public static bool[] IsFoodBuff { get; private set; }
            public static bool[] CantBeSpreadToOtherNPCs { get; private set; }

            internal static void InternalInitalize()
            {
                SetUtils.Length = BuffLoader.BuffCount;
                SetUtils.GetIDFromType = (m, n) => m.BuffType(n);

                CantBeSpreadToOtherNPCs = SetUtils.CreateFlagSet(BuffID.StardustMinionBleed, BuffID.DryadsWardDebuff, BuffID.Lovestruck, 
                    typeof(LovestruckAQ), BuffID.Stinky);

                IsFoodBuff = new bool[BuffLoader.BuffCount];
                IsFoodBuff[BuffID.WellFed] = true;
                IsFoodBuff[ModContent.BuffType<GrapePhantaBuff>()] = true;
                IsFoodBuff[ModContent.BuffType<SpeedBoostFood>()] = true;
                IsFoodBuff[ModContent.BuffType<NeutronYogurtBuff>()] = true;
                IsFoodBuff[ModContent.BuffType<DragonCarrotBuff>()] = true;
                IsFoodBuff[ModContent.BuffType<RedLicoriceBuff>()] = true;

                CantBeTurnedIntoMolite = new bool[BuffLoader.BuffCount];
                IsFoodBuff.CopyTo(CantBeTurnedIntoMolite, 0);
                CantBeTurnedIntoMolite[ModContent.BuffType<UmystickDelay>()] = true;
                CantBeTurnedIntoMolite[ModContent.BuffType<Buffs.Vampire.Vampirism>()] = true;

                CanBeRemovedByWhiteBloodCell = new bool[BuffLoader.BuffCount];
                CanBeRemovedByWhiteBloodCell[BuffID.OnFire] = true;
                CanBeRemovedByWhiteBloodCell[BuffID.Ichor] = true;
                CanBeRemovedByWhiteBloodCell[BuffID.CursedInferno] = true;
                CanBeRemovedByWhiteBloodCell[BuffID.Frostburn] = true;
                CanBeRemovedByWhiteBloodCell[BuffID.Chilled] = true;
                CanBeRemovedByWhiteBloodCell[BuffID.Bleeding] = true;
                CanBeRemovedByWhiteBloodCell[BuffID.Confused] = true;
                CanBeRemovedByWhiteBloodCell[BuffID.Poisoned] = true;
                CanBeRemovedByWhiteBloodCell[BuffID.Venom] = true;
                CanBeRemovedByWhiteBloodCell[BuffID.Darkness] = true;
                CanBeRemovedByWhiteBloodCell[BuffID.Blackout] = true;
                CanBeRemovedByWhiteBloodCell[BuffID.Silenced] = true;
                CanBeRemovedByWhiteBloodCell[BuffID.Cursed] = true;
                CanBeRemovedByWhiteBloodCell[BuffID.Slow] = true;
                CanBeRemovedByWhiteBloodCell[BuffID.Weak] = true;
                CanBeRemovedByWhiteBloodCell[BuffID.WitheredArmor] = true;
                CanBeRemovedByWhiteBloodCell[BuffID.Electrified] = true;
                CanBeRemovedByWhiteBloodCell[BuffID.Rabies] = true;
                CanBeRemovedByWhiteBloodCell[ModContent.BuffType<Buffs.Vampire.Vampirism>()] = true;
                CanBeRemovedByWhiteBloodCell[ModContent.BuffType<BlueFire>()] = true;
                CanBeRemovedByWhiteBloodCell[ModContent.BuffType<PickBreak>()] = true;
                CanBeRemovedByWhiteBloodCell[BuffID.OgreSpit] = true;
                CanBeRemovedByWhiteBloodCell[BuffID.VortexDebuff] = true;
            }

            internal static void Unload()
            {
            }
        }
    }
}