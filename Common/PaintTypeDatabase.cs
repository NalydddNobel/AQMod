using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common
{
    public class PaintTypeDatabase : ILoadable
    {
        internal static Color Furniture => new Color(191, 142, 111);

        public const byte Team = 254;
        public const byte Rainbow = 255;

        public static byte[] PaintIDs { get; private set; }
        public static Dictionary<short, byte> DyeToPaint { get; private set; }
        public static Dictionary<byte, short> PaintToDye { get; private set; }
        public static Dictionary<byte, short> PaintToYoyoString { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            PaintIDs = new byte[]
            {
                PaintID.RedPaint,
                PaintID.OrangePaint,
                PaintID.YellowPaint,
                PaintID.LimePaint,
                PaintID.GreenPaint,
                PaintID.TealPaint,
                PaintID.CyanPaint,
                PaintID.SkyBluePaint,
                PaintID.BluePaint,
                PaintID.PurplePaint,
                PaintID.VioletPaint,
                PaintID.PinkPaint,
                PaintID.BlackPaint,
                PaintID.WhitePaint,
                PaintID.BrownPaint,
                Team, // Team
                Rainbow, // Rainbow
            };
            DyeToPaint = new Dictionary<short, byte>
            {
                [ItemID.RedDye] = PaintIDs[0],
                [ItemID.OrangeDye] = PaintIDs[1],
                [ItemID.YellowDye] = PaintIDs[2],
                [ItemID.LimeDye] = PaintIDs[3],
                [ItemID.GreenDye] = PaintIDs[4],
                [ItemID.TealDye] = PaintIDs[5],
                [ItemID.CyanDye] = PaintIDs[6],
                [ItemID.SkyBlueDye] = PaintIDs[7],
                [ItemID.BlueDye] = PaintIDs[8],
                [ItemID.PurpleDye] = PaintIDs[9],
                [ItemID.VioletDye] = PaintIDs[10],
                [ItemID.PinkDye] = PaintIDs[11],
                [ItemID.BlackDye] = PaintIDs[12],
                [ItemID.SilverDye] = PaintIDs[13],
                [ItemID.BrownDye] = PaintIDs[14],
                [ItemID.TeamDye] = PaintIDs[15],
                [ItemID.RainbowDye] = PaintIDs[16],
            };
            PaintToDye = new Dictionary<byte, short>();
            foreach (var pair in DyeToPaint)
            {
                PaintToDye.Add(pair.Value, pair.Key);
            }
            PaintToYoyoString = new Dictionary<byte, short>
            {
                [0] = ItemID.WhiteString,
                [PaintIDs[0]] = ItemID.RedString,
                [PaintIDs[1]] = ItemID.OrangeString,
                [PaintIDs[2]] = ItemID.YellowString,
                [PaintIDs[3]] = ItemID.LimeString,
                [PaintIDs[4]] = ItemID.GreenString,
                [PaintIDs[5]] = ItemID.TealString,
                [PaintIDs[6]] = ItemID.CyanString,
                [PaintIDs[7]] = ItemID.SkyBlueString,
                [PaintIDs[8]] = ItemID.BlueString,
                [PaintIDs[9]] = ItemID.PurpleString,
                [PaintIDs[10]] = ItemID.VioletString,
                [PaintIDs[11]] = ItemID.PinkString,
                [PaintIDs[12]] = ItemID.BlackString,
                [PaintIDs[13]] = ItemID.WhiteString,
                [PaintIDs[14]] = ItemID.BrownString,
                [PaintIDs[15]] = 0,
                [PaintIDs[16]] = ItemID.RainbowString,
            };
        }

        void ILoadable.Unload()
        {
            PaintIDs = null;
            DyeToPaint?.Clear();
            DyeToPaint = null;
            PaintToDye?.Clear();
            PaintToDye = null;
            PaintToYoyoString?.Clear();
            PaintToYoyoString = null;
        }
    }
}
