using AQMod.Common;
using AQMod.Common.CrossMod.BossChecklist;
using AQMod.Content.WorldEvents.ProgressBars;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.WorldEvents
{
    public abstract class WorldEvent : ModWorld, ISetupContentType
    {
        internal virtual EventEntry? BossChecklistEntry => null;
        internal virtual EventProgressBar ProgressBar => null;

        void ISetupContentType.SetupContent()
        {
            var mod = AQMod.Instance;
            Setup(mod);
            try
            {
                var bossChecklist = ModLoader.GetMod("BossChecklist");
                if (bossChecklist == null)
                    return;
                var entry = BossChecklistEntry;
                if (entry != null)
                {
                    entry.Value.AddEntry(bossChecklist);
                }
            }
            catch (Exception e)
            {
                mod.Logger.Error("An error occured when setting up boss checklist entries.");
                mod.Logger.Error(e.Message);
                mod.Logger.Error(e.StackTrace);
            }
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