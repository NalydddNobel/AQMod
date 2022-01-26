using AQMod.Common;
using AQMod.Common.CrossMod;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.World.Events
{
    public abstract class WorldEvent : ModWorld, ISetupContentType
    {
        internal virtual EventProgressBar ProgressBar => null;

        void ISetupContentType.SetupContent()
        {
            var mod = AQMod.GetInstance();
            Setup(mod);
            if (!Main.dedServ)
            {
                var bar = ProgressBar;
                if (bar != null)
                    EventProgressBarLoader.AddBar(bar);
            }
            PostSetup(mod);
        }

        protected virtual void Setup(AQMod mod) { }
        protected virtual void PostSetup(AQMod mod) { }
    }
}