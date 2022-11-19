namespace Aequus.Content.Necromancy.Renderer
{
    public class ColorTargetID
    {
        public const int Unknown = 0;
        public const int ZombieScepter = 1;
        public const int Revenant = 2;
        public const int Osiris = 3;
        public const int Insurgency = 4;
        public const int BloodRed = 5;
        public const int FriendshipMagick = 6;
        public const int DemonPurple = 7;
        public const int DungeonDarkBlue = 8;

        public const int TeamWhite = 9;
        public const int TeamRed = TeamWhite + 1;
        public const int TeamGreen = TeamWhite + 2;
        public const int TeamBlue = TeamWhite + 3;
        public const int TeamYellow = TeamWhite + 4;
        public const int TeamPurple = TeamWhite + 5;

        public const int NormalCount = TeamWhite + 6;
        public static int Count = NormalCount;
    }
}