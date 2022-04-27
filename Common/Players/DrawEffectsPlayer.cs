using Aequus.Common.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Players
{
    public sealed class DrawEffectsPlayer : ModPlayer
    {
        public const int ArmorFrameHeight = 56;
        public const int ArmorFrameCount = 20;

        public const int Draw_ArachnotronArmorOldPositionLength = 8;
        public const float Draw_CelesteZMultiplier = 0.0157f;

        public const string Path_Masks = "AQMod/Content/Players/Masks/Mask_";
        public const string Path_HeadAccs = "AQMod/Content/Players/HeadAccs/HeadAcc_";

        public static int MyOldPositionsLengthCache;
        public static Vector2[] ClientOldPositionsCache;
        public static bool MyArachnotronHeadTrail;
        public static bool MyArachnotronBodyTrail;

        public Vector3[] accOmegaStaritePos;
        public float accOmegaStariteScale;
        internal static Color MothmanMaskEyeColorDefault = new Color(50, 155, 255, 0);
        internal static Color MothmanMaskEyeColorShadowScale => new Color(90 + (int)(Math.Cos(Main.GlobalTimeWrappedHourly * 10f) * 30), 25, 140 - (int)(Math.Sin(Main.GlobalTimeWrappedHourly * 10f) * 30), 0);
        internal static Color MothmanMaskEyeColorMolten = new Color(50, 155, 255, 0);
        public Color MothmanMaskEyeColor;

        public byte headAcc = 0;
        public byte mask = 0;
        public int cHeadAcc;
        public int cMask;
        public int cCelesteTorus;

        private bool initUpdate;
        public static IColorGradient NalydGradient { get; private set; }
        public IColorGradient NalydGradientPersonal { get; private set; }

        private void InitUpdate(string name = null)
        {
            NalydGradient = new Gradients.ColorWaveGradient(10f, Color.Violet, Color.MediumPurple);
            NalydGradientPersonal = NalydGradient;
        }
        public override void Initialize()
        {
            InitUpdate(Player.name);
            initUpdate = true;
            ResetDrawingInfo();
        }

        public override void ResetEffects()
        {
            if (initUpdate)
            {
                InitUpdate(Player.name);
                initUpdate = false;
            }
            ResetDrawingInfo();
        }

        private void ResetDrawingInfo()
        {
            headAcc = 0;
            mask = 0;
            cHeadAcc = 0;
            cMask = 0;
            cCelesteTorus = 0;
            MothmanMaskEyeColor = MothmanMaskEyeColorDefault;
        }

        public static bool ShouldDrawOldPos(Player player)
        {
            if (player.mount.Active || player.frozen || player.stoned || player.GetModPlayer<DrawEffectsPlayer>().mask > 0)
                return false;
            return true;
        }

        public static int GetOldPosCountMaxed(int maxCount)
        {
            int count = 0;
            for (; count < maxCount; count++)
            {
                if (ClientOldPositionsCache[count] == default(Vector2))
                    break;
            }
            return count;
        }
    }
}