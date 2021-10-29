using AQMod.Content.MapMarkers.Data;
using AQMod.Tiles.TileEntities;
using System.Collections.Generic;

namespace AQMod.Content.MapMarkers
{
    public class MapMarkerManager
    {
        private readonly Dictionary<string, MapMarkerData> _mapMarkers;
        public static TEGlobe LocalGlobe { get; private set; }

        internal MapMarkerManager()
        {
            _mapMarkers = new Dictionary<string, MapMarkerData>();
        }

        internal void Setup(bool setupStatics = false)
        {
            addMapMarker("CosmicMarker", new CosmicMarkerData());
        }

        internal void addMapMarker(string name, MapMarkerData data)
        {
            _mapMarkers.Add(name, data);
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
    }
}