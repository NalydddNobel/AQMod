using AQMod.Items.Misc.Markers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Content
{
    public sealed class MapMarkerPlayer : ModPlayer
    {
        public static class ID
        {
            public const int CosmicMarker = 0;
            public const int DungeonMarker = 1;
            public const int LihzahrdMarker = 2;
            public const int RetroMarker = 3;
        }

        public bool[] MarkersObtained { get; private set; }
        public bool[] MarkersHidden { get; private set; }

        public static MapMarker GetMarker(int type)
        {
            return _markers[type];
        }

        private static List<MapMarker> _markers;
        public static int MapMarkerCount { get; private set; }

        internal string ApplyMarkers(AQPlayer aQPlayer, string mouseText)
        {
            for (int i = 0; i < MapMarkerCount; i++)
            {
                if (MarkersObtained[i] && !MarkersHidden[i])
                    mouseText = _markers[i].Apply(player, aQPlayer, mouseText, this);
            }
            return mouseText;
        }

        internal static int AddMarker<T>() where T : MapMarker
        {
            if (AQMod.Loading)
            {
                int type = ModContent.ItemType<T>();
                if (type >= Main.maxItemTypes)
                {
                    var item = new Item();
                    item.SetDefaults(type, true);
                    _markers.Add((T)item.modItem);
                    MapMarkerCount++;
                    return MapMarkerCount - 1;
                }
            }
            return -1;
        }

        internal static void Setup()
        {
            MapMarkerCount = 0;
            _markers = new List<MapMarker>();
            AddMarker<CosmicTelescope>();
            AddMarker<DungeonMap>();
            AddMarker<LihzahrdMap>();
            AddMarker<RetroGoggles>();
        }

        internal static void Unload()
        {
            _markers = null;
        }

        public override void Initialize()
        {
            MarkersObtained = new bool[MapMarkerCount];
            MarkersHidden = new bool[MapMarkerCount];
        }

        public override TagCompound Save()
        {
            var tag = new TagCompound();
            tag["MarkerCount"] = MapMarkerCount;
            for (int i = 0; i < MapMarkerCount; i++)
            {
                tag["Obtained_" + i] = MarkersObtained[i];
                tag["Hidden_" + i] = MarkersHidden[i];
            }
            return tag;
        }

        public override void Load(TagCompound tag)
        {
            if (!tag.ContainsKey("MarkerCount"))
            {
                return;
            }
            int count = tag.GetInt("MarkerCount");
            for (int i = 0; i < count; i++)
            {
                MarkersObtained[i] = tag.GetBool("Obtained_" + i);
                MarkersHidden[i] = tag.GetBool("Hidden_" + i);
            }
        }
    }
}