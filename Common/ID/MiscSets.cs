using AQMod.Buffs;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common.ID
{
    public sealed class MiscSets
    {
        internal List<Color> ItemColorBlacklist;
        public Dictionary<int, Color> BuffToColor { get; private set; }
        public Dictionary<int, Color> ItemToColor { get; private set; }

        public Dictionary<int, int> ConcoctionItemConversions { get; private set; }

        public MiscSets()
        {
            ItemColorBlacklist = new List<Color>()
            {
                new Color(0, 0, 0, 255),
                new Color(255, 255, 255, 255),
                new Color(49, 49, 59, 255),
                new Color(82, 83, 97, 255),
                new Color(121, 123, 142, 255),
                new Color(153, 156, 180, 255),
                new Color(186, 189, 218, 255),
                new Color(226, 229, 255, 255),
            };

            BuffToColor = new Dictionary<int, Color>()
            {
                [BuffID.ObsidianSkin] = new Color(160, 60, 240, 255),
                [BuffID.Regeneration] = new Color(255, 100, 160, 255),
                [BuffID.Swiftness] = new Color(160, 240, 100, 255),
                [BuffID.Gills] = new Color(100, 160, 230, 255),
                [BuffID.Ironskin] = new Color(235, 255, 100, 255),
                [BuffID.ManaRegeneration] = new Color(255, 80, 150, 255),
                [BuffID.MagicPower] = new Color(79, 16, 164, 255),
                [BuffID.Spelunker] = new Color(255, 245, 150, 255),
                [BuffID.Thorns] = new Color(165, 255, 80, 255),

                [BuffID.OnFire] = new Color(255, 180, 100, 255),
                [BuffID.Burning] = new Color(255, 180, 100, 255),
                [BuffID.Lovestruck] = new Color(255, 180, 200, 255),
                [BuffID.Midas] = new Color(255, 180, 90, 255),
                [BuffID.Slimed] = new Color(180, 180, 255, 255),
                [BuffID.Wet] = new Color(180, 180, 255, 255),
                [BuffID.Ichor] = new Color(255, 255, 160, 255),
                [BuffID.BetsysCurse] = new Color(255, 220, 20, 255),
                [BuffID.CursedInferno] = new Color(160, 255, 160, 255),
                [BuffID.ShadowFlame] = new Color(200, 60, 255, 255),

                [ModContent.BuffType<Bloodthirst>()] = new Color(200, 60, 255, 255),
            };

            ItemToColor = new Dictionary<int, Color>()
            {
                [ItemID.ObsidianSkinPotion] = BuffToColor[BuffID.ObsidianSkin],
                [ItemID.RegenerationPotion] = BuffToColor[BuffID.Regeneration],
                [ItemID.SwiftnessPotion] = BuffToColor[BuffID.Swiftness],
                [ItemID.GillsPotion] = BuffToColor[BuffID.Gills],
                [ItemID.IronskinPotion] = BuffToColor[BuffID.Ironskin],
                [ItemID.ManaRegenerationPotion] = BuffToColor[BuffID.ManaRegeneration],
                [ItemID.MagicPowerPotion] = BuffToColor[BuffID.MagicPower],
                [ItemID.SpelunkerPotion] = BuffToColor[BuffID.Spelunker],
                [ItemID.ThornsPotion] = BuffToColor[BuffID.Thorns],
            };
        }

        internal void SetupContent()
        {

        }
    }
}