using AequusRemake.Core.Structures;

namespace AequusRemake.Core;

public partial class Commons {
    public class Refs {
        public static readonly RefCache<bool> BloodMoon = new RefCache<bool>(() => ref Main.bloodMoon);
        public static readonly RefCache<bool> Eclipse = new RefCache<bool>(() => ref Main.eclipse);
        public static readonly RefCache<bool> FrostMoon = new RefCache<bool>(() => ref Main.snowMoon);
        public static readonly RefCache<bool> PumpkinMoon = new RefCache<bool>(() => ref Main.pumpkinMoon);
        public static readonly RefCache<bool> DayTime = new RefCache<bool>(() => ref Main.dayTime);
        public static readonly RefCache<double> Time = new RefCache<double>(() => ref Main.time);
    }
}
