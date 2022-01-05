using AQMod.Common;
using AQMod.Common.CrossMod.BossChecklist;
using System;
using Terraria.ModLoader;

namespace AQMod.NPCs.Boss
{
    public abstract class AQBoss : ModNPC, ISetupContentType
    {
        public virtual BossEntry? BossChecklistEntry => null;

        void ISetupContentType.SetupContent()
        {
            var mod = AQMod.GetInstance();
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
            PostSetupContent(mod);
        }

        protected virtual void PostSetupContent(Mod mod)
        {

        }
    }
}