using Terraria.ID;

namespace AQMod.Common.ID
{
    internal static class ShopID
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
        public const int Steampunker = 11;
        public const int DyeTrader = 12;
        public const int PartyGirl = 13;
        public const int Cyborg = 14;
        public const int Painter = 15;
        public const int WitchDoctor = 16;
        public const int Pirate = 17;
        public const int Stylist = 18;
        public const int TravellingMerchant = 19;
        public const int SkeletonMerchant = 20;
        public const int DD2Bartender = 21;

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
                    return Steampunker;
                case NPCID.DyeTrader:
                    return DyeTrader;
                case NPCID.PartyGirl:
                    return PartyGirl;
                case NPCID.Cyborg:
                    return Cyborg;
                case NPCID.Painter:
                    return Painter;
                case NPCID.WitchDoctor:
                    return WitchDoctor;
                case NPCID.Pirate:
                    return Pirate;
                case NPCID.Stylist:
                    return Stylist;
                case NPCID.TravellingMerchant:
                    return TravellingMerchant;
                case NPCID.SkeletonMerchant:
                    return SkeletonMerchant;
                case NPCID.DD2Bartender:
                    return DD2Bartender;
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
                case Steampunker:
                    return NPCID.Steampunker;
                case DyeTrader:
                    return NPCID.DyeTrader;
                case PartyGirl:
                    return NPCID.PartyGirl;
                case Cyborg:
                    return NPCID.Cyborg;
                case Painter:
                    return NPCID.Painter;
                case WitchDoctor:
                    return NPCID.WitchDoctor;
                case Pirate:
                    return NPCID.Pirate;
                case Stylist:
                    return NPCID.Stylist;
                case TravellingMerchant:
                    return NPCID.TravellingMerchant;
                case SkeletonMerchant:
                    return NPCID.SkeletonMerchant;
                case DD2Bartender:
                    return NPCID.DD2Bartender;
            }
            return -1;
        }
    }
}