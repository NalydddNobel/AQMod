using Microsoft.Xna.Framework;
using Terraria.ID;

namespace AQMod.Common
{
    internal static class Constants // This class is free to steal, it's full of magic numbers vanilla uses
    {
        public static class ChatColors
        {         
            internal static Color BossMessage => new Color(175, 75, 255, 255);
            internal static Color EventMessage => new Color(50, 255, 130, 255);
        }

        public static class MoonPhases
        {
            public const int FullMoon = 0;
            public const int WaningGibbious = 1;
            public const int ThirdQuarter = 2;
            public const int WaningCrescent = 3;
            public const int NewMoon = 4;
            public const int WaxingCrescent = 5;
            public const int FirstQuarter = 6;
            public const int WaxingGibbious = 7;
        }

        public static class BossSpawnItemSortOrder
        {
            public const int None = -1;
            public const int SuspiciousLookingEye = 1;
            public const int SlimeCrown = 2;
            public const int WormFood = 3;
            public const int BloodySpine = 3;
            public const int GoblinBattleStandard = 4;
            public const int Abeemination = 5;
            public const int MechanicalEye = 6;
            public const int MechanicalWorm = 7;
            public const int MechanicalSkull = 8;
            public const int TreasureMap = 9; // YEA THEY MESSED UP; THIS SHOULD BE THE PIRATE MAP.
            public const int TruffleWorm = 10;
            public const int SnowGlobe = 11; // why is the frost legion sorted after duke? I don't know
            public const int PumpkinMoonMedallion = 12;
            public const int NaughtyPresent = 13;
            public const int LihzahrdPowerCell = 14;
            public const int SolarTablet = 15;
            public const int CelestialSigil = 16;
        }

        public static class Paint
        {
            public const byte None = 0;
            public const byte Red = 1;
            public const byte Orange = 2;
            public const byte Yellow = 3;
            public const byte Lime = 4;
            public const byte Green = 5;
            public const byte Teal = 6;
            public const byte Cyan = 7;
            public const byte SkyBlue = 8;
            public const byte Blue = 9;
            public const byte Purple = 10;
            public const byte Violet = 11;
            public const byte Pink = 12;
            public const byte DeepRed = 13;
            public const byte DeepOrange = 14;
            public const byte DeepYellow = 15;
            public const byte DeepLime = 16;
            public const byte DeepGreen = 17;
            public const byte DeepTeal = 18;
            public const byte DeepCyan = 19;
            public const byte DeepSkyBlue = 20;
            public const byte DeepBlue = 21;
            public const byte DeepPurple = 22;
            public const byte DeepViolet = 23;
            public const byte DeepPink = 24;
            public const byte Black = 25;
            public const byte White = 26;
            public const byte Gray = 27;
            public const byte Brown = 28;
            public const byte Shadow = 29;
            public const byte Negative = 30;

            /// <summary>
            /// Gives you a string representation of a certain color
            /// </summary>
            /// <param name="clr"></param>
            /// <returns></returns>
            public static string GetClrName(byte clr) // TODO: ADD THE DEEP COLORS
            {
                switch (clr)
                {
                    case Red:
                    return "Red";
                    case Orange:
                    return "Orange";
                    case Yellow:
                    return "Yellow";
                    case Lime:
                    return "Lime";
                    case Green:
                    return "Green";
                    case Teal:
                    return "Teal";
                    case Cyan:
                    return "Cyan";
                    case SkyBlue:
                    return "SkyBlue";
                    case Blue:
                    return "Blue";
                    case Purple:
                    return "Purple";
                    case Violet:
                    return "Violet";
                    case Pink:
                    return "Pink";
                    case White:
                    return "White";
                    case Gray:
                    return "Gray";
                    case Brown:
                    return "Brown";
                    case Shadow:
                    return "Shadow";
                    case Negative:
                    return "Negative";
                }
                return "Unknown";
            }
        }

        public static class HoldStyle
        {
            public const int Torch = 1;
            public const int Umbrella = 2;
            public const int Harp = 3;
        }

        public static class ShopIDs
        {
            public const int Merchant = 1;
            public const int ArmsDealer = 2;
            public const int Dryad = 3;
            public const int Demolitionist = 4;
            public const int Clothier = 5;
            public const int GoblinTinkerer = 6;
            public const int Wizard = 7;
            public const int Mechanic = 8;
            public const int SantaClaus = 9;
            public const int Truffle = 10;

            public static int GetShopFromNPCID(int id)
            {
                switch (id)
                {
                    case NPCID.Merchant:
                    return Merchant;
                    case NPCID.ArmsDealer:
                    return ArmsDealer;
                    case NPCID.Dryad:
                    return Dryad;
                    case NPCID.Demolitionist:
                    return Demolitionist;
                    case NPCID.Clothier:
                    return Clothier;
                    case NPCID.GoblinTinkerer:
                    return GoblinTinkerer;
                    case NPCID.Wizard:
                    return Wizard;
                    case NPCID.Mechanic:
                    return Mechanic;
                    case NPCID.SantaClaus:
                    return SantaClaus;
                    case NPCID.Truffle:
                    return Truffle;
                    case NPCID.Steampunker:
                    return 11;
                    case NPCID.DyeTrader:
                    return 12;
                    case NPCID.PartyGirl:
                    return 13;
                    case NPCID.Cyborg:
                    return 14;
                    case NPCID.Painter:
                    return 15;
                    case NPCID.WitchDoctor:
                    return 16;
                    case NPCID.Pirate:
                    return 17;
                    case NPCID.Stylist:
                    return 18;
                    case NPCID.TravellingMerchant:
                    return 19;
                    case NPCID.SkeletonMerchant:
                    return 20;
                    case NPCID.DD2Bartender:
                    return 21;
                }
                return -1;
            }
            public static int GetNPCIDFromShop(int id)
            {
                switch (id)
                {
                    case Merchant:
                    return NPCID.Merchant;
                    case ArmsDealer:
                    return NPCID.ArmsDealer;
                    case Dryad:
                    return NPCID.Dryad;
                    case Demolitionist:
                    return NPCID.Demolitionist;
                    case Clothier:
                    return NPCID.Clothier;
                    case GoblinTinkerer:
                    return NPCID.GoblinTinkerer;
                    case Wizard:
                    return NPCID.Wizard;
                    case Mechanic:
                    return NPCID.Mechanic;
                    case SantaClaus:
                    return NPCID.SantaClaus;
                    case Truffle:
                    return NPCID.Truffle;
                    case 11:
                    return NPCID.Steampunker;
                    case 12:
                    return NPCID.DyeTrader;
                    case 13:
                    return NPCID.PartyGirl;
                    case 14:
                    return NPCID.Cyborg;
                    case 15:
                    return NPCID.Painter;
                    case 16:
                    return NPCID.WitchDoctor;
                    case 17:
                    return NPCID.Pirate;
                    case 18:
                    return NPCID.Stylist;
                    case 19:
                    return NPCID.TravellingMerchant;
                    case 20:
                    return NPCID.SkeletonMerchant;
                    case 21:
                    return NPCID.DD2Bartender;
                }
                return -1;
            }
        }
    }
}