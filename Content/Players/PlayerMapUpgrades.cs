using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Content.Players
{
    public sealed class PlayerMapUpgrades : ModPlayer
    {
        public static class MapUpgrade
        {
            public const byte NotObtained = 0;
            public const byte Visible = 1;
            public const byte InactiveLayer = 2;
        }

        public byte CosmicTelescope;
        public byte VialOfBlood;
        public byte Cabbage;
        public byte BlightedSoul;
        public byte Beeswax;

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["CosmicTelescope"] = CosmicTelescope,
                ["VialOfBlood"] = VialOfBlood,
                ["Cabbage"] = Cabbage,
                ["BlightedSoul"] = BlightedSoul,
                ["Beeswax"] = Beeswax,
            };
        }

        public override void Load(TagCompound tag)
        {
            CosmicTelescope = tag.GetByte("CosmicTelescope");
            VialOfBlood = tag.GetByte("VialOfBlood");
            Cabbage = tag.GetByte("Cabbage");
            BlightedSoul = tag.GetByte("BlightedSoul");
            Beeswax = tag.GetByte("Beeswax");
        }

        public static bool HasMapUpgrade(byte upgrade)
        {
            return upgrade > MapUpgrade.NotObtained;
        }

        public static bool MapUpgradeVisible(byte upgrade)
        {
            return upgrade == MapUpgrade.Visible;
        }

        public static void Obtain(ref byte upgrade, bool obtained)
        {
            if (obtained)
            {
                if (!HasMapUpgrade(upgrade))
                {
                    upgrade = MapUpgrade.Visible;
                }
            }
            else
            {
                upgrade = MapUpgrade.NotObtained;
            }
        }

        /// <summary>
        /// Toggles the visibilty of this map upgrade enum
        /// </summary>
        /// <param name="upgrade"></param>
        /// <returns></returns>
        public static void Visibility(ref byte upgrade)
        {
            Visibility(ref upgrade, !MapUpgradeVisible(upgrade));
        }
        public static void Visibility(ref byte upgrade, bool visible)
        {
            if (upgrade == MapUpgrade.InactiveLayer)
            {
                upgrade = MapUpgrade.Visible;
            }
            else
            {
                upgrade = MapUpgrade.InactiveLayer;
            }
        }
    }
}