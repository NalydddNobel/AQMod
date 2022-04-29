using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.Catalogues
{
    public sealed class PaintsCatalogue : ILoadable
    {
        internal static Color Furniture => new Color(191, 142, 111);

        public static byte[] Paints { get; private set; }
        public static Dictionary<short, byte> DyeToPaint { get; private set; }
        public static Dictionary<byte, short> PaintToDye { get; private set; }
        public static Dictionary<byte, short> PaintToYoyoString { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            Paints = new byte[]
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
                253, // Team
                255, // Rainbow
            };
            DyeToPaint = new Dictionary<short, byte>
            {
                [ItemID.RedDye] = Paints[0],
                [ItemID.OrangeDye] = Paints[1],
                [ItemID.YellowDye] = Paints[2],
                [ItemID.LimeDye] = Paints[3],
                [ItemID.GreenDye] = Paints[4],
                [ItemID.TealDye] = Paints[5],
                [ItemID.CyanDye] = Paints[6],
                [ItemID.SkyBlueDye] = Paints[7],
                [ItemID.BlueDye] = Paints[8],
                [ItemID.PurpleDye] = Paints[9],
                [ItemID.VioletDye] = Paints[10],
                [ItemID.PinkDye] = Paints[11],
                [ItemID.BlackDye] = Paints[12],
                [ItemID.SilverDye] = Paints[13],
                [ItemID.BrownDye] = Paints[14],
                [ItemID.TeamDye] = Paints[15],
                [ItemID.RainbowDye] = Paints[16],
            };
            PaintToDye = new Dictionary<byte, short>();
            foreach (var pair in DyeToPaint)
            {
                PaintToDye.Add(pair.Value, pair.Key);
            }
            PaintToYoyoString = new Dictionary<byte, short>
            {
                [0] = ItemID.WhiteString,
                [Paints[0]] = ItemID.RedString,
                [Paints[1]] = ItemID.OrangeString,
                [Paints[2]] = ItemID.YellowString,
                [Paints[3]] = ItemID.LimeString,
                [Paints[4]] = ItemID.GreenString,
                [Paints[5]] = ItemID.TealString,
                [Paints[6]] = ItemID.CyanString,
                [Paints[7]] = ItemID.SkyBlueString,
                [Paints[8]] = ItemID.BlueString,
                [Paints[9]] = ItemID.PurpleString,
                [Paints[10]] = ItemID.VioletString,
                [Paints[11]] = ItemID.PinkString,
                [Paints[12]] = ItemID.BlackString,
                [Paints[13]] = ItemID.WhiteString,
                [Paints[14]] = ItemID.BrownString,
                [Paints[15]] = 0,
                [Paints[16]] = ItemID.RainbowString,
            };
        }

        void ILoadable.Unload()
        {
            Paints = null;
            DyeToPaint?.Clear();
            DyeToPaint = null;
            PaintToDye?.Clear();
            PaintToDye = null;
            PaintToYoyoString?.Clear();
            PaintToYoyoString = null;
        }
    }
}
