using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Assets.Graphics
{
    public sealed class DrawOverlayLoader<Overlay>
    {
        private Overlay[] _overlays;
        /// <summary>
        /// Example: <see cref="Main.maxItemTypes"/>
        /// </summary>
        public readonly int VanillaCap;
        /// <summary>
        /// Example: <see cref="ItemLoader.ItemCount"/>
        /// </summary>
        public readonly Func<int> CompleteCap;

        internal DrawOverlayLoader(int vanillaCap, Func<int> completeCap)
        {
            VanillaCap = vanillaCap;
            CompleteCap = completeCap;
        }

        public bool ValidID(int id)
        {
            return id > VanillaCap && id < CompleteCap();
        }

        public Overlay GetOverlay(int id)
        {
            return _overlays[GetID(id)];
        }

        /// <summary>
        /// Registers an overlay
        /// </summary>
        /// <param name="overlay"></param>
        /// <param name="id"></param>
        public void Register(Overlay overlay, int id)
        {
            if (AQMod.Loading && !Main.dedServ)
            {
                id = GetID(id);
                if (_overlays == null)
                {
                    _overlays = new Overlay[id + 1];
                    _overlays[id] = overlay;
                }
                else
                {
                    Array.Resize(ref _overlays, id + 1);
                    _overlays[id] = overlay;
                }
            }
        }

        /// <summary>
        /// Resizes the array to fit the complete cap
        /// </summary>
        public void Finish()
        {
            Array.Resize(ref _overlays, GetID(CompleteCap()));
            try
            {
                Main.spriteBatch.End();
            }
            catch
            {
            }
        }

        private int GetID(int id)
        {
            return id - VanillaCap;
        }
    }
}