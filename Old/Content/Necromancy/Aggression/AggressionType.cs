using System;
using System.Collections.Generic;
using System.Reflection;

namespace Aequus.Old.Content.Necromancy.Aggression;

public class AggressionType : ILoadable {
    public static Dictionary<string, IEnemyAggressor> AggressorFromName { get; private set; }
    public static IEnemyAggressor BloodMoon { get; private set; }
    public static IEnemyAggressor Eclipse { get; private set; }
    public static IEnemyAggressor MartianMadness { get; private set; }
    public static IEnemyAggressor PirateInvasion { get; private set; }
    public static IEnemyAggressor GoblinArmy { get; private set; }
    public static IEnemyAggressor DayTime { get; private set; }
    public static IEnemyAggressor NightTime { get; private set; }

    private static bool dayTime;
    private static bool bloodMoon;
    private static bool eclipse;
    private static int invasionSize;
    private static int invasionType;

    public struct NeedsBloodMoon : IEnemyAggressor {
        public NeedsBloodMoon() {
        }

        public void OnPreAI(NPC npc, NecromancyNPC necro) {
            dayTime = Main.dayTime;
            bloodMoon = Main.bloodMoon;

            Main.dayTime = false;
            Main.bloodMoon = true;
        }

        public void OnPostAI(NPC npc, NecromancyNPC necro) {
            Main.dayTime = dayTime;
            Main.bloodMoon = bloodMoon;
        }
    }

    public struct NeedsEclipse : IEnemyAggressor {
        public NeedsEclipse() {
        }

        public void OnPreAI(NPC npc, NecromancyNPC necro) {
            dayTime = Main.dayTime;
            eclipse = Main.eclipse;

            Main.dayTime = false;
            Main.eclipse = true;
        }

        public void OnPostAI(NPC npc, NecromancyNPC necro) {
            Main.dayTime = dayTime;
            Main.eclipse = eclipse;
        }
    }

    public struct NeedsInvasion : IEnemyAggressor {
        private readonly int InvasionType;

        public NeedsInvasion(int invasionType) {
            InvasionType = invasionType;
        }

        public void OnPreAI(NPC npc, NecromancyNPC necro) {
            invasionSize = Main.invasionSize;
            invasionType = Main.invasionType;

            Main.invasionSize = 100;
            Main.invasionType = InvasionType;
        }

        public void OnPostAI(NPC npc, NecromancyNPC necro) {
            Main.invasionSize = invasionSize;
            Main.invasionType = invasionType;
        }
    }

    public struct NeedsTimeOfDay : IEnemyAggressor {
        private readonly bool TimeOfDay;

        public NeedsTimeOfDay(bool timeOfDay) {
            TimeOfDay = timeOfDay;
        }

        public void OnPreAI(NPC npc, NecromancyNPC necro) {
            dayTime = Main.dayTime;

            Main.dayTime = TimeOfDay;
        }

        public void OnPostAI(NPC npc, NecromancyNPC necro) {
            Main.dayTime = dayTime;
        }
    }

    void ILoadable.Load(Mod mod) {
    }

    internal static void LoadAggressions() {
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

    void ILoadable.Unload() {
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