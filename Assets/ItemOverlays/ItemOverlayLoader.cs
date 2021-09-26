using AQMod.Common.Config;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Assets.ItemOverlays
{
    public static class ItemOverlayLoader
    {
        private static ItemOverlay[] _overlays;

        private static int getID(int id)
        {
            return id - Main.maxItemTypes;
        }

        public static void Register(ItemOverlay overlay, int id)
        {
            if (AQMod.Loading && ModContent.GetInstance<AQConfigClient>().LoadItemOverlays)
            {
                id = getID(id);
                if (_overlays == null)
                {
                    _overlays = new ItemOverlay[id + 1];
                    _overlays[id] = overlay;
                }
                else
                {
                    Array.Resize(ref _overlays, id + 1);
                    _overlays[id] = overlay;
                }
            }
        }

        public static ItemOverlay GetOverlay(int id)
        {
            return _overlays[getID(id)];
        }

        internal static void Finish()
        {
            Array.Resize(ref _overlays, getID(ItemLoader.ItemCount));
        }

        internal static void Unload()
        {
            _overlays = null;
        }
    }
}
