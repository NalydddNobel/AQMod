using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod
{
    public class AQBuff : GlobalBuff
    {
        public static class Sets
        {
            public static bool[] CanBeRemovedByWhiteBloodCell { get; private set; }
            public static bool[] CanBeTurnedIntoMolite { get; private set; }
            public static bool[] IsFoodBuff { get; private set; }

            internal static void Setup()
            {
                IsFoodBuff = new bool[BuffLoader.BuffCount];
                IsFoodBuff[BuffID.WellFed] = true;
                IsFoodBuff[ModContent.BuffType<Buffs.Foods.GrapePhanta>()] = true;
                IsFoodBuff[ModContent.BuffType<Buffs.Foods.SpicyEel>()] = true;
                IsFoodBuff[ModContent.BuffType<Buffs.Foods.NeutronYogurt>()] = true;
                IsFoodBuff[ModContent.BuffType<Buffs.Foods.PeeledCarrot>()] = true;
                IsFoodBuff[ModContent.BuffType<Buffs.Foods.RedLicorice>()] = true;

                CanBeTurnedIntoMolite = new bool[BuffLoader.BuffCount];
                IsFoodBuff.CopyTo(CanBeTurnedIntoMolite, 0);
                CanBeTurnedIntoMolite[ModContent.BuffType<Buffs.Timers.UmystickDelay>()] = false;
                CanBeTurnedIntoMolite[ModContent.BuffType<Buffs.Vampire.Vampirism>()] = false;

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
                CanBeRemovedByWhiteBloodCell[ModContent.BuffType<Buffs.Debuffs.BlueFire>()] = true;
                CanBeRemovedByWhiteBloodCell[ModContent.BuffType<Buffs.Debuffs.PickBreak>()] = true;
                CanBeRemovedByWhiteBloodCell[BuffID.OgreSpit] = true;
                CanBeRemovedByWhiteBloodCell[BuffID.VortexDebuff] = true;
            }

            internal static void Unload()
            {
            }
        }
    }
}