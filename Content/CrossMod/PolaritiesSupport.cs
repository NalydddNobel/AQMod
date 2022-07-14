using Aequus.Common;
using Aequus.Common.Utilities;
using Aequus.Content.Necromancy;
using Aequus.Content.Necromancy.Aggression;
using System;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    internal class PolaritiesSupport : IAddRecipes, ILoadBefore
    {
        private static Mod polarities;
        public static Mod Polarities => polarities;

        Type ILoadBefore.LoadBefore => typeof(NecromancyDatabase);

        void ILoadable.Load(Mod mod)
        {
        }

        void IAddRecipes.AddRecipes(Aequus aequus)
        {
            if (ModLoader.TryGetMod("Polarities", out polarities))
            {
                new DatabaseBuilder<GhostInfo>(NecromancyDatabase.NPCs, polarities, NPCID.Search)
                    .TryAddModEntry("BloodBat", GhostInfo.One.WithAggro(AggressionType.NightTime))
                    .TryAddModEntry("BatSlime", GhostInfo.Two)
                    .TryAddModEntry("ConeShell", GhostInfo.Two)
                    .TryAddModEntry("GreatStellatedSlime", GhostInfo.Three)
                    .TryAddModEntry("Alkalabomination", GhostInfo.Three)
                    .TryAddModEntry("AlkaliSpirit", GhostInfo.One)
                    .TryAddModEntry("FlowWorm", GhostInfo.One)
                    .TryAddModEntry("Limeshell", GhostInfo.Three)
                    .TryAddModEntry("Slimey", GhostInfo.Three)
                    .TryAddModEntry("StalagBeetle", GhostInfo.One)
                    .TryAddModEntry("NestGuardian", GhostInfo.One)
                    .TryAddModEntry("Rattler", GhostInfo.One)
                    .TryAddModEntry("BrineDweller", GhostInfo.Three)
                    .TryAddModEntry("Mussel", GhostInfo.Three);
                return;
            }
            polarities = null;
        }

        void ILoadable.Unload()
        {
            polarities = null;
        }
    }
}