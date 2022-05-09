using Aequus.NPCs;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Necromancy
{
    public sealed class AggroForcer : ILoadable
    {
        public static IEnemyAggressor Eclipse { get; private set; }
        public static IEnemyAggressor MartianMadness { get; private set; }
        public static IEnemyAggressor PirateInvasion { get; private set; }
        public static IEnemyAggressor GoblinArmy { get; private set; }
        public static IEnemyAggressor DayTime { get; private set; }
        public static IEnemyAggressor NightTime { get; private set; }

        public interface IEnemyAggressor
        {
            public abstract void OnPreAI(NPC npc, NecromancyNPC necro);

            public abstract void OnPostAI(NPC npc, NecromancyNPC necro);
        }

        public struct NeedsEclipse : IEnemyAggressor
        {
            public NeedsEclipse()
            {
            }

            public void OnPreAI(NPC npc, NecromancyNPC necro)
            {
                AequusHelpers.Main_dayTime.StartCaching(true);
                AequusHelpers.Main_eclipse.StartCaching(true);
            }

            public void OnPostAI(NPC npc, NecromancyNPC necro)
            {
                AequusHelpers.Main_eclipse.EndCaching();
                AequusHelpers.Main_dayTime.EndCaching();
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
                AequusHelpers.Main_invasionType.StartCaching(InvasionType);
            }

            public void OnPostAI(NPC npc, NecromancyNPC necro)
            {
                AequusHelpers.Main_invasionType.EndCaching();
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
                AequusHelpers.Main_dayTime.StartCaching(TimeOfDay);
            }

            public void OnPostAI(NPC npc, NecromancyNPC necro)
            {
                AequusHelpers.Main_dayTime.EndCaching();
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

        internal static void LoadAggression()
        {
            Eclipse = new NeedsEclipse();
            MartianMadness = new NeedsInvasion(InvasionID.MartianMadness);
            PirateInvasion = new NeedsInvasion(InvasionID.PirateInvasion);
            GoblinArmy = new NeedsInvasion(InvasionID.GoblinArmy);
            DayTime = new NeedsTimeOfDay(true);
            NightTime = new NeedsTimeOfDay(false);
        }

        void ILoadable.Unload()
        {
            MartianMadness = null;
            PirateInvasion = null;
            GoblinArmy = null;
            DayTime = null;
            NightTime = null;
        }
    }
}