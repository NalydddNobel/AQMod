using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Necromancy.Aggression
{
    public class AggressionType : ILoadable
    {
        public static Dictionary<string, IEnemyAggressor> AggressorFromName { get; private set; }
        public static IEnemyAggressor BloodMoon { get; private set; }
        public static IEnemyAggressor Eclipse { get; private set; }
        public static IEnemyAggressor MartianMadness { get; private set; }
        public static IEnemyAggressor PirateInvasion { get; private set; }
        public static IEnemyAggressor GoblinArmy { get; private set; }
        public static IEnemyAggressor DayTime { get; private set; }
        public static IEnemyAggressor NightTime { get; private set; }

        public struct NeedsBloodMoon : IEnemyAggressor
        {
            public NeedsBloodMoon()
            {
            }

            public void OnPreAI(NPC npc, NecromancyNPC necro)
            {
                AequusSystem.Main_dayTime.StartCaching(false);
                AequusSystem.Main_bloodMoon.StartCaching(true);
            }

            public void OnPostAI(NPC npc, NecromancyNPC necro)
            {
                AequusSystem.Main_bloodMoon.EndCaching();
                AequusSystem.Main_dayTime.EndCaching();
            }
        }

        public struct NeedsEclipse : IEnemyAggressor
        {
            public NeedsEclipse()
            {
            }

            public void OnPreAI(NPC npc, NecromancyNPC necro)
            {
                AequusSystem.Main_dayTime.StartCaching(true);
                AequusSystem.Main_eclipse.StartCaching(true);
            }

            public void OnPostAI(NPC npc, NecromancyNPC necro)
            {
                AequusSystem.Main_eclipse.EndCaching();
                AequusSystem.Main_dayTime.EndCaching();
            }
        }

        public struct NeedsInvasion : IEnemyAggressor
        {
            private readonly int InvasionType;

            public NeedsInvasion(int invasionType)
            {
                InvasionType = invasionType;
            }

            public void OnPreAI(NPC npc, NecromancyNPC necro)
            {
                AequusSystem.Main_invasionSize.StartCaching(100);
                AequusSystem.Main_invasionType.StartCaching(InvasionType);
            }

            public void OnPostAI(NPC npc, NecromancyNPC necro)
            {
                AequusSystem.Main_invasionSize.EndCaching();
                AequusSystem.Main_invasionType.EndCaching();
            }
        }

        public struct NeedsTimeOfDay : IEnemyAggressor
        {
            private readonly bool TimeOfDay;

            public NeedsTimeOfDay(bool timeOfDay)
            {
                TimeOfDay = timeOfDay;
            }

            public void OnPreAI(NPC npc, NecromancyNPC necro)
            {
                AequusSystem.Main_dayTime.StartCaching(TimeOfDay);
            }

            public void OnPostAI(NPC npc, NecromancyNPC necro)
            {
                AequusSystem.Main_dayTime.EndCaching();
            }
        }

        internal struct ModCallCustom : IEnemyAggressor
        {
            private readonly Action<NPC> OnPreAIAction;
            private readonly Action<NPC> OnPostAIAction;

            public ModCallCustom(Action<NPC> onPreAI, Action<NPC> onPostAI)
            {
                OnPreAIAction = onPreAI;
                OnPostAIAction = onPostAI;
            }

            public void OnPreAI(NPC npc, NecromancyNPC necro)
            {
                OnPreAIAction?.Invoke(npc);
            }

            public void OnPostAI(NPC npc, NecromancyNPC necro)
            {
                OnPostAIAction?.Invoke(npc);
            }
        }

        void ILoadable.Load(Mod mod)
        {
        }

        internal static void LoadAggressions()
        {
            BloodMoon = new NeedsBloodMoon();
            Eclipse = new NeedsEclipse();
            MartianMadness = new NeedsInvasion(InvasionID.MartianMadness);
            PirateInvasion = new NeedsInvasion(InvasionID.PirateInvasion);
            GoblinArmy = new NeedsInvasion(InvasionID.GoblinArmy);
            DayTime = new NeedsTimeOfDay(true);
            NightTime = new NeedsTimeOfDay(false);

            AggressorFromName = new Dictionary<string, IEnemyAggressor>
            {
                { "NightTime", NightTime },
                { "DayTime", DayTime },
                { "GoblinArmy", GoblinArmy },
                { "PirateInvasion", PirateInvasion },
                { "MartianMadness", MartianMadness },
                { "Eclipse", Eclipse },
                { "BloodMoon", BloodMoon }
            };
        }

        void ILoadable.Unload()
        {
            AggressorFromName?.Clear();
            AggressorFromName = null;
            BloodMoon = null;
            Eclipse = null;
            MartianMadness = null;
            PirateInvasion = null;
            GoblinArmy = null;
            DayTime = null;
            NightTime = null;
        }
    }
}