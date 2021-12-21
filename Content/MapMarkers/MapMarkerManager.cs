using AQMod.Common;
using AQMod.Content.MapMarkers.Data;
using AQMod.Items.Tools.MapMarkers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.MapMarkers
{
    public class MapMarkerManager : IAutoloadType
    {
        public static MapMarkerManager Instance => ModContent.GetInstance<MapMarkerManager>();

        private readonly Dictionary<string, MapMarkerData> _mapMarkers;

        public MapMarkerManager()
        {
            _mapMarkers = new Dictionary<string, MapMarkerData>();
        }

        void IAutoloadType.OnLoad()
        {
            ContentInstance.Register(this);
            addMapMarker(new CosmicMarkerData("CosmicMarker", ModContent.ItemType<CosmicTelescope>()));
            addMapMarker(new DungeonMarkerData("DungeonMarker", ModContent.ItemType<DungeonMap>()));
            addMapMarker(new LihzahrdMarkerData("LihzahrdMarker", ModContent.ItemType<LihzahrdMap>()));
            addMapMarker(new RetroMarkerData("RetroMarker", ModContent.ItemType<RetroGoggles>()));
        }

        void IAutoloadType.Unload()
        {
        }

        /// <summary>
        /// Returns null if no marker item was found
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public (Item, MapMarkerData) FindMarker(Item[] array)
        {
            for (int i = 0; i < Main.maxInventory; i++)
            {
                if (array[i].type >= Main.maxItemTypes)
                {
                    foreach (var m in _mapMarkers)
                    {
                        if (array[i].type == m.Value.ItemTypeBind)
                        {
                            return (array[i], m.Value);
                        }
                    }
                }
            }
            return (null, null);
        }

        internal void addMapMarker(MapMarkerData data)
        {
            _mapMarkers.Add(data.Name, data);
        }

        public bool AddMapMarker(string name, MapMarkerData data)
        {
            if (AQMod.Loading && !_mapMarkers.ContainsKey(name))
            {
                _mapMarkers.Add(name, data);
            }
            return false;
        }

        public void NormalizeMarkerList(List<string> markers)
        {
            for (int i = 0; i < markers.Count; i++)
            {
                if (!_mapMarkers.ContainsKey(markers[i]))
                {
                    markers.RemoveAt(i);
                    i--;
                }
            }
        }

        public MapMarkerData GetMarker(string name)
        {
            return _mapMarkers[name];
        }

        public bool TryGetMarker(string name, out MapMarkerData value)
        {
            return _mapMarkers.TryGetValue(name, out value);
        }
    }
}